using System;
using System.Threading;
using AW.FSM;

namespace AW.War {
	using NetMQ;
	using NetMQ.Sockets;
	using ProtoBuf;

	public class SubClient : BaseSsock {

		//All message is concern
		private const string topic = "";
		//绑定地址
		private string ConnAddres;
		private SubscriberSocket subSocket;
		/// <summary>
		/// 数据接收器
		/// </summary>
		private Poller poller;

		private MsgPool<IpcMsg> ClientPool;


		public SubClient (MsgPool<IpcMsg> pool, WarInfo war) : base(war) {
			ClientPool = pool;
			poller = new Poller();

			establish();
		}

		void establish() {

			subSocket = Core.ZeroMQ.CreateSubscriberSocket();
			subSocket.Options.ReceiveHighWatermark = EngCfg.HighWatermark;
			ConnAddres = ConnectAddr(typeof(SubscriberSocket));
			subSocket.Connect(ConnAddres);
			ConsoleEx.DebugLog("Sub socket has connected to " + ConnAddres, ConsoleEx.YELLOW);
			subSocket.Subscribe(topic);

			subSocket.ReceiveReady += Client_ReceiveReady;

			AddPoller();
		}

		void AddPoller() {
			poller.AddSocket(subSocket);
			ThreadPool.QueueUserWorkItem( (o) => { poller.PollTillCancelled(); } );
		}

		public void ReceiveTimeout () {
			ConsoleEx.DebugLog("Subscribe is timeout.", ConsoleEx.RED);

			if(poller != null) {
				if(poller.IsStarted) {
					poller.CancelAndJoin();
				}
				poller.Dispose();
			}

			if(subSocket != null) {
				if(warInfo.warMo == WarMode.InLanWar)
					subSocket.Disconnect(ConnAddres);
				subSocket.Close();
				subSocket = null;
			}

			poller = new Poller();
			establish();
		}

		/// <summary>
		/// 一直不停的接收信息, 在移动设备上一直不退出，但是在Editor上要退出
		/// </summary>
		void Client_ReceiveReady(object sender, NetMQSocketEventArgs e) {

			string msgTopicReceived = e.Socket.ReceiveString();
			byte[] msgReceived      = e.Socket.Receive();

			OP op = (OP)Enum.Parse(typeof(OP), msgTopicReceived);
			IpcMsg msg = ProtoLoader.deserializeProtoObj(msgReceived, IpcMsg.Table[op]);
			ClientPool.OnReceive(msg);

		}

		public void Quit() {
			if(subSocket != null) {
				if(warInfo.warMo == WarMode.InLanWar)
					subSocket.Disconnect(ConnAddres);
				subSocket.Close();
				subSocket = null;
			}

			if(poller != null) {
				if(poller.IsStarted)
					poller.CancelAndJoin();
				poller.Dispose();
				poller = null;
			}
		}
	}
}
