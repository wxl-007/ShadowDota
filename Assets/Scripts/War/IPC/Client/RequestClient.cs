using System;
using System.Threading;

namespace AW.War {

	using NetMQ;
	using NetMQ.Sockets;

	/// <summary>
	/// NetMQ的pattern之Req和Rep是： 1 对 N 
	/// 1是Rep，N是Req
	/// 
	/// RequestClient 维持和Server的通讯
	/// </summary>
	public class RequestClient : BaseSsock {

		private Action<NetMQMessage> handler = null;
		private RequestSocket reqSock;

		private MsgPool<NetMQMessage> Pool = null;

		//断线重连后的回调
		private Action Reconnected = null;
		//是否已经连接上了
		private bool connected = false;
		// 如果超过1分钟，则考虑断线重连
		private const int IntervalPeriod = 60000;

		private string ConnectingAddress;

		public RequestClient(WarInfo war, Action<NetMQMessage> real, Action Connected) : base(war) {
			handler = real;
			connected = false;

			establish(Connected);
			Pool = new MsgPool<NetMQMessage>(sendAndRecv);
		}

		void establish(Action Connected) {

			ThreadPool.QueueUserWorkItem( (o) => {
				ConsoleEx.DebugLog("Request socket is Connecting...", ConsoleEx.RED);

				var context = Core.ZeroMQ;
				reqSock = context.CreateRequestSocket();
				reqSock.Options.SendHighWatermark = EngCfg.HighWatermark;
				reqSock.Options.ReceiveHighWatermark = EngCfg.HighWatermark;

				//生成通讯地址和协议方式
				ConnectingAddress = ConnectAddr(typeof(RequestSocket));
				reqSock.Connect(ConnectingAddress);

				connected = true;

				//稍微等待一下
				Thread.Sleep(100);
				if(Connected != null) Connected();
			});

		}

		public void send(NetMQMessage msg) {
			if(connected == false) return;
			Pool.OnReceive(msg);
		}

		//如果连接尚未建立，则会丢弃消息
		void sendAndRecv(NetMQMessage msg) {

			if(connected == false) return;

			///
			/// 发送请求
			/// 
			reqSock.SendMessage(msg);


			if(connected == false) return;
			///
			/// 接收回来的消息
			///
			var repMsg = reqSock.ReceiveMessage();

			//处理消息
			if(repMsg != null && repMsg != null) {
				handler(repMsg);
			}

		}

		/// <summary>
		/// 断掉连接，重新连接
		/// </summary>
		public void ReceiveTimeout() {
			ConsoleEx.DebugLog("Req Sock is timeout.", ConsoleEx.RED);
			///
			/// 超时处理
			///
			connected = false;
			if(reqSock != null) { 
				reqSock.Disconnect(ConnectingAddress);
				reqSock.Close();
			}
			establish(Reconnected);
		}

		public void Quit() {
			connected = false;
			if(reqSock != null) {
				reqSock.Disconnect(ConnectingAddress);
				reqSock.Close();
			}
			if(Pool != null) {
				Pool.QuitMsgPool();
				Pool = null;
			}
		}

	}

}