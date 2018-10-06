using System;
using System.Collections.Generic;
using System.Threading;
using AW.Framework;
using AW.IO;
using AW.FSM;

public class HttpThread : IGameState
{
	public static string UNABLE_GET_RESPONSE = "Http request is timeout or can't get response.";

	private readonly object _locker = new object ();
	private Queue<HttpTask> sendingQueue;
	private Thread httpThread;
	//if user wants to stop Network Engine , Loop should be false
	private bool Loop = true;
	private HttpClient httpClient;
	//keep singleton
	private static HttpThread _engine;
    //--- call back routine
    public Action<HttpTask> Http_OnReceive;
    public Action<HttpTask> Http_OnException;

    //This should be set after one server is selected
    private HttpData_Completeness httpDataCom;

    private static readonly RequestType[] NonHttpDataCompleteness = {
        RequestType.GET_PARTITION_SERVER,
		RequestType.THIRD_GET_SERVER,
    };

  
	/*	
	 * Initialization Section
	 */
	private HttpThread ()
	{
		_locker = new object ();
		sendingQueue = new Queue<HttpTask> ();
		Loop = true;
		httpClient = HttpClient.getInstance ();

		httpThread = new Thread (new ThreadStart (Run));
		httpThread.Start ();
	}

	public static HttpThread getInstance ()
	{

		return _engine ?? ( _engine = new HttpThread() );
	}

	public void sendHttpTask (HttpTask task)
	{
		lock (_locker) {
			sendingQueue.Enqueue (task);          // We must pulse because we're
			Monitor.Pulse (_locker);              // changing a blocking condition.
		}
	}

	private HttpTask getHttpTask ()
	{

		lock (_locker) {
			while (sendingQueue.Count == 0)
				Monitor.Wait (_locker);
			return sendingQueue.Dequeue ();    // This signals our exit.
		}
	}

	private void ResetRequet() {
		lock (_locker) {
			sendingQueue.Clear();
		}
	}

	void http_onReceieve (HttpTask task, string acknowlege)
	{
		if(task.request is HttpRequest) {
			if (Utils.checkJsonFormat (acknowlege)){
				HttpResponseFactory.createResponse(task, acknowlege);
				// ... TO DO.  notify event center
				if(Http_OnReceive != null) 
					Http_OnReceive(task);

				HttpRequest curRequest = task.request as HttpRequest;
				if (!Utils.inArray<RequestType>(curRequest.Type, NonHttpDataCompleteness))
				{
					if (httpDataCom != null)
						httpDataCom.incHttpRequestNo(curRequest);
				}
			}
			else 
				http_onException (task, acknowlege);

		} else if(task.request is ThirdPartyHttpRequest) {
			task.response = new ThirdPartyResponse(acknowlege);

			if(Http_OnReceive != null) 
				Http_OnReceive(task);

		} else if(task.request is HttpDownloadRequest) {
			task.response = new HttpDownloadResponse();

			if(Http_OnReceive != null)
				Http_OnReceive(task);
		}

	}


	// All http error will go through this routine.
	// Handle exception
	void http_onException (HttpTask task, string message)
	{
		// ... Todo. notify event center
        ExceptionResponse exResp = new ExceptionResponse();
		exResp.HttpError = message;
        task.response = exResp;
		task.errorInfo = message;

        if (Http_OnException != null) Http_OnException(task);
	}

	private void Run ()
	{
		// Keep consuming until told otherwise.
		while (Loop) {
            HttpTask task = getHttpTask();
			if(task != null && task.request != null) {
				if(task.request is HttpRequest) 
					RunCommon(task);
				else if(task.request is InternalRequest) 
					RunInternal(task);
				else if(task.request is ThirdPartyHttpRequest)
					RunThirdParty(task);
				else if(task.request is HttpDownloadRequest)
					RunDownloadTask(task);
			}

		}

		Dispose();
		ConsoleEx.DebugLog("Http Thread is Exited.");
	}

	//立刻执行某系命令 -- 会block当前的thread until it finished.
	public void RunImmediatly(HttpTask task) {
		if(task != null && task.request != null) {
			if(task.request is HttpRequest) 
				RunCommon(task);
			else if(task.request is InternalRequest) 
				RunInternal(task);
			else if(task.request is ThirdPartyHttpRequest)
				RunThirdParty(task);
			else if(task.request is HttpDownloadRequest)
				RunDownloadTask(task);
		}
	}

	//网络下载任务
	private void RunDownloadTask(HttpTask task) {
		bool failure = httpClient.doDownload(task as HttpDownloadTask);
		if(!failure)
			http_onReceieve(task, null);
		else
			http_onException(task, UNABLE_GET_RESPONSE);
	}

	private void RunCommon(HttpTask task) {
		HttpRequest request = task.request as HttpRequest;
		//send http request
        string acknowlege = string.Empty;

        if(task.OutFilter.check()) {
            acknowlege = task.OutFilter.handleLocalTask(task);
        } else {
            acknowlege = httpClient.doRequest (request, httpDataCom);
        }

        //Check if we need response or not.
        if (task.respType != TaskResponse.Igonre_Response) {
            if (!string.IsNullOrEmpty(acknowlege)) {
                acknowlege = acknowlege.Trim();
                http_onReceieve(task, acknowlege);
            } else {
                // error ocurr
                http_onException(task, UNABLE_GET_RESPONSE);
            }
        }
	}

	//运行内部控制命令
	private void RunInternal(HttpTask task) {
		InternalRequest request = task.request as InternalRequest;

		switch(request.CommondType) {
		case InternalRequestType.RESET:
			ResetRequet();
			break;
		case InternalRequestType.SHUT_DOWN:
			Loop = false;
			break;
		case InternalRequestType.RESUME:
			break;
		case InternalRequestType.HOLDING_ON:
			break;
		default:
			//Do nothing...
			break;
		}
	}
	//运行第三方的网络需求
	private void RunThirdParty(HttpTask task) {
		ThirdPartyHttpRequest request = task.request as ThirdPartyHttpRequest;
		//send http request
		string acknowlege = httpClient.doRequest (request.RequestUrl);
		//Check if we need response or not.
		if (task.respType != TaskResponse.Igonre_Response) {
			if (!string.IsNullOrEmpty(acknowlege)) {
				http_onReceieve(task, acknowlege.Trim());
			} else {
				// error ocurr
				http_onException(task, UNABLE_GET_RESPONSE);
			}
		}
	}

	void Dispose() {
		httpClient = null;
		httpDataCom.OnUnregister(null);
		httpDataCom = null;
		_engine = null;
	}

	/****************************************************  状态的改变  ***************************************************************/

	public void OnLogin(StateParam<GameState> obj) {
		if(obj != null) {
			LoginInfo Log = obj.obj as LoginInfo;
			httpDataCom = new HttpData_Completeness(Log.LocalIOMgr);
		} else {
			throw new DragonException("HttpThread OnLogin Param must be LocalIOManager");
		}
    }

	/// <summary>
	/// 清空所有的Http的任务，删除DataComplete
	/// </summary>
	public void OnUnregister(StateParam<GameState> obj) {
		ResetRequet();
		httpDataCom = null;
	}

	public void OnDayChanged(StateParam<GameState> obj) { }

	public void OnLevelChanged(StateParam<GameState> obj) { }
}