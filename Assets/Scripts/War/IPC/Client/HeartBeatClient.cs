using System;
using System.Threading;

namespace AW.War {
	using NetMQ;
	using NetMQ.Sockets;
	using t_Timer = System.Threading.Timer;

	/// <summary>
	/// 心跳的客户端
	/// </summary>
	public class HeartBeatClient : BaseSsock {
		/// <summary>
		/// 当网络连接断开的时候，UI的处理逻辑
		/// </summary>
		private Action OnDisconnected;

		private RequestSocket reqSock;
		private string ConnectingAddress;
		public bool isConnected {
			private set;
			get;
		}
		private NetMQMessage msg;

		//5s超时
		private const int TimeoutPeriod = 5000;
		//每10s发送一次心跳请求
		private const int IntervalPeriod = 10000;

		//超时次数
		private int CurTimeoutCnt;

		//DueTime to TimeOut.Infinite to prevent the timer from starting
		//Period Specify Timeout.Infinite to disable periodic signaling, callback routine run once.
		private t_Timer RepeatTimer = null;
		private t_Timer TimeOut = null;

		public HeartBeatClient(WarInfo war, Action disCon) : base(war) {
			isConnected   = false;
			OnDisconnected= disCon;
			CurTimeoutCnt = 0;
			RepeatTimer   = new t_Timer(new TimerCallback(Beat), null, Timeout.Infinite, Timeout.Infinite);
			TimeOut       = new t_Timer(new TimerCallback(TTimeOut), null, Timeout.Infinite, Timeout.Infinite);

			msg = new NetMQMessage();
			msg.Append(0);
			msg.Append(DeviceInfo.GUID);

			establish();
		}

		void establish() {

			ThreadPool.QueueUserWorkItem( (o) => {

				ConsoleEx.DebugLog("HeartBeat socket is Connecting...", ConsoleEx.RED);
				Thread.Sleep(2000);

				var context = Core.ZeroMQ;
				reqSock = context.CreateRequestSocket();
				reqSock.Options.SendHighWatermark = EngCfg.HighWatermark;
				reqSock.Options.ReceiveHighWatermark = EngCfg.HighWatermark;

				//生成通讯地址和协议方式
				ConnectingAddress = ConnectAddr(typeof(HeartBeatClient));
				reqSock.Connect(ConnectingAddress);

				isConnected = true;

				Beat(null);

			});

		}

		void TTimeOut(Object obj) {
			ConsoleEx.DebugLog("HeartBeat Client Sock is timeout.", ConsoleEx.RED);

			CurTimeoutCnt ++;
			if(OnDisconnected != null) 
				OnDisconnected();

			ReconnectIfDis();
		}

		void Beat(Object state) {
			//启动超时判定
			TimeOut.Change(TimeoutPeriod, Timeout.Infinite);
			///
			/// 发送请求
			/// 
			reqSock.SendMessage(msg);

			///
			/// 接收回来的消息
			///
			var repMsg = reqSock.ReceiveMessage();

			if(repMsg != null && repMsg.FrameCount > 0) {
				TimeOut.Change(Timeout.Infinite, Timeout.Infinite);

				int heart  = repMsg[0].ConvertToInt32();
				ConsoleEx.DebugLog("Heart from Server : " + heart.ToString());
				//开始下一个时间任务
				if(heart == 1) 
					RepeatTimer.Change(IntervalPeriod, Timeout.Infinite);
			}

		}

		/// <summary>
		/// 断掉连接，重新连接
		/// </summary>
		void ReconnectIfDis() {
			///
			/// 超时处理
			///
			isConnected = false;
			if(reqSock != null) { 
				reqSock.Disconnect(ConnectingAddress);
				reqSock.Close();
			}

			ConsoleEx.DebugLog("HeartBeat socket is Connecting...", ConsoleEx.RED);

			var context = Core.ZeroMQ;
			reqSock = context.CreateRequestSocket();
			reqSock.Options.SendHighWatermark = EngCfg.HighWatermark;
			reqSock.Options.ReceiveHighWatermark = EngCfg.HighWatermark;

			//生成通讯地址和协议方式
			ConnectingAddress = ConnectAddr(typeof(HeartBeatClient));
			reqSock.Connect(ConnectingAddress);

			isConnected = true;

			RepeatTimer.Change(IntervalPeriod, Timeout.Infinite);
		}

		public void Quit() {
			if(reqSock != null) {
				reqSock.Disconnect(ConnectingAddress);
				reqSock.Close();
			}

			if(RepeatTimer != null) {
				RepeatTimer.Dispose();
			}

			if(TimeOut != null) {
				TimeOut.Dispose();
			}
		}
	}

}