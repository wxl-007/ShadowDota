using System;
using System.Net;
using fastJSON;
using xClient.Action;
using System.Collections.Generic;
using SuperSocket.ClientEngine;
using ActProtocol = xClient.Action.Protocol;
using AW.Framework;
using AW.FSM;

public class SocketEngine : ISystemHandler {
	//Queue的容量大概在10以内
	private const int QUEUE_CAPACITY = 10;
	//如果socket 没有起来，则添加任务到这个队列里面
	private ThreadSafeQueue<SocketTask> workQueue;
	//Key is Act. If the same act ID, we just replace it.我把发送的Task记录在这里
	private Dictionary<int, SocketTask> TaskQueue;
	//是否和服务器连上的
	private bool isConnected {
		get  { return curConnectType == isConnectType.isConnected;}
	}

	public enum isConnectType
	{
		isConnected,
		isConnecting,
		isDisconnect
	}

	public isConnectType curConnectType;

	private NonBlockingConnection Conn = null;
	//IP and Port
	public DnsEndPoint endPoint = null;

	//--- call back routine
	public Action<SocketTask> Socket_OnReceive;
	public Action<SocketTask> Socket_OnException;
	public Action<string> Socket_CommException;

	private static SocketEngine _engine;
	private SocketEngine () {
		curConnectType = isConnectType.isDisconnect;
		workQueue = new ThreadSafeQueue<SocketTask>(QUEUE_CAPACITY);
		TaskQueue = new Dictionary<int, SocketTask>();
	}

	public static SocketEngine getInstance() {
		return _engine ?? (_engine = new SocketEngine());
	}

	#region Socket 各种操作

	public void sendSocketTask( SocketTask task ) {
		if(task.request.type == BaseSocketRequestType.Internal_Control) {
			HandleInternalTask(task);
		} else {
			if (isConnected) {
				HandleTask (task);

			} else  {
				//添加到队列里面来，
				workQueue.Enqueue (task);
				if (curConnectType != isConnectType.isConnecting) {
					curConnectType = isConnectType.isConnecting;
					Conn.Connect ();
				}
					
			}
		}
	}

    /// <summary>
    /// 强制重连
    /// </summary>
    public void ReConnect() {
        if (curConnectType != isConnectType.isConnecting) {
            curConnectType = isConnectType.isConnecting;
            Conn.Connect ();
        }
    }

	/// <summary>
	/// Register Task for UI layer.
	/// </summary>
	/// <param name="task">Task.</param>
	public void RegisterSocketTask(SocketTask task) {
		if(task != null) {
			SocketRequest sockReq =  task.request as SocketRequest;

			if( sockReq != null && !TaskQueue.ContainsKey(sockReq.Act))
				TaskQueue[sockReq.Act] = task;
		}
	}

	#endregion

	private void HandleTask () {
		while(workQueue.Count > 0) {
			SocketTask task = null;
			if(workQueue.TryDequeue(out task)) {
				HandleTask(task);
			}
		}
		
	}

	private void HandleTask ( SocketTask task ) {
		if(task != null && task.request != null) {
			HandldCommonTask(task);
			HandleInternalTask(task);
		}
	}

	private void HandldCommonTask ( SocketTask task ) {
		if(task.request.type == BaseSocketRequestType.Common_Socket_Request) {
			SocketRequest sockReq = task.request as SocketRequest;

			//记录下已经发送数据的 Task
			TaskQueue[sockReq.Act] = task;
			//send to server
			if (task.respType != TaskResponse.Donot_Send) {
				ConsoleEx.DebugLog ("Socket to be sent : " + sockReq.toJson());
				Conn.Write(sockReq.toJson());
			}
				
		} 
	}

	private void HandleInternalTask ( SocketTask task ) {
		if(task.request.type == BaseSocketRequestType.Internal_Control) {
			SockInternalRequest inSockReq = task.request as SockInternalRequest;

			switch(inSockReq.CommondType) {
			case InternalRequestType.SHUT_DOWN:
				if(Conn != null) Conn.Disconnect();
				workQueue.Clear();
				TaskQueue.Clear();
				break;
			case InternalRequestType.RESUME:
    			if(isConnected == false) {
					Conn.Connect();
					workQueue.Clear();
					TaskQueue.Clear();
				}
				break;
			case InternalRequestType.RESET:
				workQueue.Clear();
				TaskQueue.Clear();
				break;
			case InternalRequestType.HOLDING_ON:
				//	curConnectType = isConnectType.isConnecting;
				break;
			}

		}
	}

	private int getAct (string str) {
		int act = 0;

		if(string.IsNullOrEmpty(str)) 
			return act;

		int pos = str.IndexOf(SocketRequest.ACTION_R);   ///接受时候 Action 截取  act  不带引号
		int startPos = 0, endPos = 0;
		if( pos >= 0 ) {
			//one for """    one for ":"
			startPos = pos + SocketRequest.ACTION_R.Length + 2;

			int count = str.Length;
			for(int i = startPos; i < count; ++ i) {
				if(str[i] == ',') {
					endPos = i;
					break;
				}
			}
			try {
				act = Convert.ToInt32(str.Substring(startPos, endPos - startPos));
			} catch (Exception ex) {
				act = 0;
				ConsoleEx.DebugLog(ex.Message);
			}

		}
		return act;
	}


	#region Handler Routine

	public bool OnConnect (NonBlockingConnection conn)
	{
		ConsoleEx.DebugLog("Connect to the Server.");
		curConnectType = isConnectType.isConnected;
		HandleTask();
		return true;
	}

	public bool OnData (NonBlockingConnection conn) {
		IList<string> json = conn.ReadStrings();
		if((json == null) || (json.Count == 0)) {
			return true;
		}

		foreach(string s in json) {
			string acknowledge = s.Trim();
			int Act = getAct(acknowledge);
			SocketTask task = null;
			if(TaskQueue.TryGetValue(Act, out task)) {

				if(task.respType == TaskResponse.Default_Response || task.respType == TaskResponse.Donot_Send) {
					if( Utils.checkJsonFormat(acknowledge) ) {
						SocketResponseFactory.createResponse(task, acknowledge);
						Socket_OnReceive(task);
					} else {
						ExceptionResponse exResp = new ExceptionResponse();
						exResp.HttpError = acknowledge;
						task.response = exResp;
						Socket_OnException(task);
					}
				}

			} else {
				Socket_CommException(acknowledge);
			}
		}

		return true;
	}

	public bool OnDisconnect (NonBlockingConnection conn)
	{
		Socket_CommException("Disconnect from the Server.");
		curConnectType = isConnectType.isDisconnect;
		return true;
	}

	public bool OnException (NonBlockingConnection conn, Exception e)
	{
		Socket_CommException(e.ToString());
		curConnectType = isConnectType.isDisconnect;
		return true;
	}

	#endregion

	#region ICore implementation

	public void Dispose ()
	{
		SocketTask task = new SocketTask(ThreadType.BackGround);
		task.AppendCmdParam(InternalRequestType.SHUT_DOWN);
		HandleInternalTask(task);
	}

	public void OnPause() {
		SocketTask task = new SocketTask(ThreadType.BackGround);
		task.AppendCmdParam(InternalRequestType.HOLDING_ON);
		HandleInternalTask(task);
	}

	public void Reset ()
	{
		throw new NotImplementedException ();
	}

	public void OnLogin (object ServerEndPoint)
	{
		endPoint = ServerEndPoint as DnsEndPoint;
		Conn = new NonBlockingConnection( endPoint.Host, endPoint.Port, this);

		if (curConnectType == isConnectType.isConnecting)
			curConnectType = isConnectType.isDisconnect;
	}

	#endregion


}


