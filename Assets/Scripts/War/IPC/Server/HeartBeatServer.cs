using System;
using System.Threading;
using NetMQ.Sockets;
using NetMQ;
using t_Timer = System.Threading.Timer;

namespace AW.War {

	/// <summary>
	/// 心跳包的服务器
	/// </summary>
	public class HeartBeatServer : BaseSsock  {
		/// <summary>
		/// 当网络连接断开的时候，UI的处理逻辑
		/// </summary>
		public Action OnDisconnected;

		private ResponseSocket respSocket = null;
		private string BindingAddress;
		private Poller poller;
		private NetMQMessage Msg;
		private MonitorClient monitor;
		///
		/// 目前timer没有启用，忽略
		///
		private const int TimeoutPeriod = 12000;
		private t_Timer TimeOut = null;

		public HeartBeatServer(WarInfo war, MonitorClient climonitor) : base(war) {
			this.poller = new Poller();
			monitor = climonitor;

			Msg = new NetMQMessage();
			Msg.Append(1);
			establish();
			//TimeOut = new t_Timer(new TimerCallback(TTimeOut), null, Timeout.Infinite, Timeout.Infinite);
		}

		void establish() {

			ConsoleEx.DebugLog("HeartBeat Server socket is binding...", ConsoleEx.RED);

			var context = Core.ZeroMQ;
			respSocket = context.CreateResponseSocket();
			respSocket.Options.SendHighWatermark = EngCfg.HighWatermark;
			respSocket.Options.ReceiveHighWatermark = EngCfg.HighWatermark;
			//生成通讯地址和协议方式
			BindingAddress = BindAddr(typeof(HeartBeatServer));
			respSocket.Bind(BindingAddress);

			///
			/// 绑定完成，等待Request的请求
			///
			respSocket.ReceiveReady += Server_ReceiveReady;
			poller.AddSocket(respSocket);

			ThreadPool.QueueUserWorkItem( (ob) => { poller.PollTillCancelled(); });

		}

		void Server_ReceiveReady(object sender, NetMQSocketEventArgs e) {
			/// 
			/// 每一个Recev后能接一个Send
			/// 
			var message     = e.Socket.ReceiveMessage();
			int heart       = message[0].ConvertToInt32();
			string clientid = message[1].ConvertToString();

			ConsoleEx.DebugLog("Hearbeat from client : " + heart.ToString());
			monitor.OnHeartBeat(clientid);

			/// 是否为心跳包
			if(heart == 0) {
				///
				/// 目前timer没有启用，忽略
				///
				/// TimeOut.Change(TimeoutPeriod, Timeout.Infinite);
				e.Socket.SendMessage(Msg);
			}
		}

		/// <summary>
		/// 目前服务器端不存在断线的情况，忽略
		/// </summary>
		/// <param name="obj">Object.</param>
		[Obsolete]
		void TTimeOut(Object obj) {
			ConsoleEx.DebugLog("HeartBeat Server Sock is timeout.", ConsoleEx.RED);

			if(TimeOut != null)
				TimeOut.Change(Timeout.Infinite, Timeout.Infinite);

			if(OnDisconnected != null) 
				OnDisconnected();

			if(poller != null ) {
				if(poller.IsStarted)
					poller.CancelAndJoin();
				poller.Dispose();
			}

			if(respSocket != null) { 
				respSocket.Unbind(BindingAddress);
				respSocket.Close();
			}

			ConsoleEx.DebugLog("HeartBeat Server socket is Connecting...", ConsoleEx.RED);

			var context = Core.ZeroMQ;
			respSocket = context.CreateResponseSocket();
			respSocket.Options.SendHighWatermark = EngCfg.HighWatermark;
			respSocket.Options.ReceiveHighWatermark = EngCfg.HighWatermark;

			//生成通讯地址和协议方式
			BindingAddress = BindAddr(typeof(HeartBeatServer));
			respSocket.Bind(BindingAddress);

			respSocket.ReceiveReady += Server_ReceiveReady;
			poller.AddSocket(respSocket);

			ThreadPool.QueueUserWorkItem( (ob) => { poller.PollTillCancelled(); });
		}

		public void Quit() {
			///
			/// 目前timer没有启用，忽略
			///
			if(TimeOut != null) {
				TimeOut.Change(Timeout.Infinite, Timeout.Infinite);
				TimeOut.Dispose();
			}

			if(poller != null) {
				if(poller.IsStarted)
					poller.CancelAndJoin();
				poller.Dispose();
			} 

			if(respSocket != null) {
				respSocket.Unbind(BindingAddress);
				respSocket.Close();
			}
		}

	}
}