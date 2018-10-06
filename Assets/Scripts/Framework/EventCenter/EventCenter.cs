using System;
using AW.Entity;
using System.Collections.Generic;

/// <summary>
/// Message type. 这个类型实际上是处理默认的处理网络请求的类型。
/// </summary>
public enum MsgType {
	HTTP_EXCEPTION,
	SOCK_EXCEPTION,
    SOCK_COMMONE_EXCEPTION,
}

namespace AW.Event {

	/// 
	/// 消息处理中心，此版本的消息处理模式
	/// 所有经过UI的Controller发送的消息，都必须经过EventCenter
	/// UI <--> Controller <--> EventCenter <--> HttpQueue
	/// 但是如果是使用HttpTask直接发出来的则：发送的时候依然不经过EventCenter

	public class EventCenter : IDispatch {

		/// <summary>
		/// 存储默认的网络UI处理层次
		/// </summary>
		private Dictionary<MsgType, Action<BaseTaskAbstract>> exchange = null;
		private HttpThread httpEngine;
		private SocketEngine sockEngine;
		private EntityManager entityMgr;

		/// <summary>
		/// 依赖的网络层
		/// </summary>
		public EventCenter(HttpThread httpEngine, SocketEngine sockEngine, EntityManager entityMgr) { 
			exchange = new Dictionary<MsgType, Action<BaseTaskAbstract>>();

			this.httpEngine = httpEngine;
			this.sockEngine = sockEngine;
			this.entityMgr  = entityMgr;

			if(httpEngine != null) {
				httpEngine.Http_OnException = Http_OnException;
				httpEngine.Http_OnReceive = Http_OnReceive;
			}

			if(sockEngine != null) {
				sockEngine.Socket_OnException = Sock_OnException;
				sockEngine.Socket_OnReceive = Sock_OnReceive;
				sockEngine.Socket_CommException = Sock_OnCommonExcption;
			}
		}

		#region 默认的网络处理者
		/// <summary>
		/// Registers the default handler. 实际上是处理默认的处理网络请求的类型
		/// </summary>
		/// <param name="kind">Kind.</param>
		/// <param name="method">Method.</param>
		public void RegisterDefaultHandler (MsgType kind, Action<BaseTaskAbstract> method) {
			if(method != null) {
				if (exchange.ContainsKey (kind) && exchange.ContainsValue (method)) {
					return;
				} else
					exchange.Add (kind, method);

			}
		}

		Action<BaseTaskAbstract> DefaultHttpExce {
			get {
				Action<BaseTaskAbstract> task = null;
				if(!exchange.TryGetValue(MsgType.HTTP_EXCEPTION, out task)){
					task = null;
				}

				return task;
			}
		}

		Action<BaseTaskAbstract> DefaultSockExce {
			get {
				Action<BaseTaskAbstract> task = null;
				if(!exchange.TryGetValue(MsgType.SOCK_EXCEPTION, out task)){
					task = null;
				}

				return task;
			}
		}

		Action<BaseTaskAbstract> CommonSockExce {
			get {
				Action<BaseTaskAbstract> task = null;
				if(!exchange.TryGetValue(MsgType.SOCK_COMMONE_EXCEPTION, out task)){
					task = null;
				}

				return task;
			}
		}
		#endregion

		#region 网络消息发送入口

		public void Dispatch(BaseTaskAbstract task) {
			if(task != null) {

				if(task.type == TaskType.HttpTask) {
					HttpTask ht = task as HttpTask;
					httpEngine.sendHttpTask(ht);
				} else if(task.type == TaskType.HttpTaskEx) {
					HttpTaskEx htEx = task as HttpTaskEx;
					httpEngine.sendHttpTask(htEx);
				} else if(task.type == TaskType.SocketTask) {

				}

			} else {
				ConsoleEx.DebugLog("Event Center try to dispatch a empty task.", ConsoleEx.YELLOW);
			}
		}

		#endregion


		// ********************************  HTTP CALLBACK ****************************
		/*
		 * Running in the background thread
		 * 如果errorInfo为空，则Response一定有
		 */ 
		void Http_OnReceive(HttpTask task)
		{
			if (task != null && string.IsNullOrEmpty(task.errorInfo) )
			{
				///
				/// ----------- 分开try catch的原因是：有可能处理HttpResponse时报出异常 ， 异常之后无法正确的处理回调，-----------
				///
				try {
					//所有内部的处理，包括转换为合理的格式
					Core.Data.fullfillByNetwork(task);
				} catch(Exception ex) {
					ConsoleEx.DebugLog(ex.ToString());
				} 


				///
				/// ------------- try catch保证：回调异常了之后，不影响Http线程 ------
				///
				try {
					if(task.threadType == ThreadType.MainThread) {
						AsyncTask.QueueOnMainThread( () => { task.handleMainThreadCompleted(); } );
					} else {
						task.handleBackGroundCompleted();
					}
				} catch(Exception ex) {
					ConsoleEx.DebugLog(ex.ToString(), ConsoleEx.RED);
				}

			} else {
				//Json error.
				Http_OnException(task);
			}
		}

		/*
		 * Running in the main thread
		 */
		void Http_OnException(HttpTask task) {
			if(task != null) {
				if(!string.IsNullOrEmpty(task.errorInfo))
					ConsoleEx.DebugLog(" --> Event Center Recevied Http Exception." + task.errorInfo);
				///
				/// ----------- 分开try catch的原因是：有可能处理HttpResponse时报出异常 ， 异常之后无法正确的处理回调，-----------
				///

				//we should send following code to the main thread
				try {
					AsyncTask.QueueOnMainThread( () => { task.handleErrorOcurr(); } );
				} catch(Exception ex) {
					ConsoleEx.DebugLog(ex.ToString(), ConsoleEx.RED);
				}

				//Default handler if ErrorOccured is null
				if(DefaultHttpExce != null && task.ErrorOccured == null) {
					try {
						AsyncTask.QueueOnMainThread( () => { DefaultHttpExce(task);} );
					} catch(Exception ex) {
						ConsoleEx.DebugLog(ex.ToString(), ConsoleEx.RED);
					}
				}
					
			}

		}


		// ******************************** SOCKET CALLBACK ****************************
		// ***************************************************************************** 
		void Sock_OnReceive(SocketTask task) {
			if (task != null && string.IsNullOrEmpty(task.errorInfo) )
			{
				///
				/// ----------- 分开try catch的原因是：有可能处理HttpResponse时报出异常 ， 异常之后无法正确的处理回调，-----------
				///
				try {
					//所有内部的处理，包括转换为合理的格式
					task.handleCompletedInternal(ActionOnReceiveSockResponse.getAction(task));
				} catch(Exception ex) {
					ConsoleEx.DebugLog(ex.ToString());
				}

				///
				/// ------------- try catch保证：回调异常了之后，不影响Http线程 ------
				///
				try {

					if(task.threadType == ThreadType.MainThread) {
						AsyncTask.QueueOnMainThread( () => { task.handleMainThreadCompleted(); } );
					} else {
						task.handleBackGroundCompleted();
					}
				} catch (Exception ex) {
					ConsoleEx.DebugLog(ex.ToString());
				}

			} else {
				//Json error.
				Sock_OnException(task);
			}
		}

		void Sock_OnException(SocketTask task) {
			ConsoleEx.DebugLog(" --> Event Center Recevied Socket Exception");

			if(task != null) {
				//we should send following code to the main thread
				AsyncTask.QueueOnMainThread( () => { task.handleErrorOcurr(); } );

				//Default handler if ErrorOccured is null
				if(DefaultSockExce != null && task.ErrorOccured == null)
					AsyncTask.QueueOnMainThread( () => { DefaultSockExce(task);} );
			}

		}

		void Sock_OnCommonExcption(string error) {
			ConsoleEx.DebugLog(" --> Event Center Recevied Common Socket Exception. " + error);
			//Default handler if ErrorOccured is null
			if(DefaultSockExce != null) {
				SocketTask sTask = new SocketTask(ThreadType.MainThread);
				sTask.errorInfo = error;
				AsyncTask.QueueOnMainThread( () => { DefaultSockExce(sTask);} );
			}

			if(CommonSockExce != null) {
				SocketTask sTask = new SocketTask(ThreadType.MainThread);
				sTask.errorInfo = error;
				AsyncTask.QueueOnMainThread( () => { CommonSockExce(sTask);} );
			}

		}

		// ******************************** TIMER CALLBACK ****************************
	}
}