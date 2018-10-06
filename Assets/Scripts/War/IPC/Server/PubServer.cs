using System;
using System.Collections.Generic;
using System.Threading;

namespace AW.War {

	using NetMQ;
	using NetMQ.Sockets;
	using ProtoBuf;

	/// <summary>
	/// 服务器端发布者-- 不能是单例
	/// </summary>
	public class PubServer : BaseSsock {
		private PublisherSocket pubSocket = null;
		private string BindAddres;
		private bool isBinded = false;

		public PubServer (WarInfo war, Action binded) : base(war) {
			isBinded = false;
			establish(binded);
		}

		void establish(Action binded) {

			ConsoleEx.DebugLog("Publisher socket binding...", ConsoleEx.RED);

			var context = Core.ZeroMQ;
			pubSocket = context.CreatePublisherSocket();
			pubSocket.Options.SendHighWatermark = EngCfg.HighWatermark;

			//生成通讯地址和协议方式
			BindAddres = BindAddr(typeof(PublisherSocket));
			pubSocket.Bind(BindAddres);
			ConsoleEx.DebugLog("Pub socket has binded to " + BindAddres, ConsoleEx.YELLOW);

			isBinded = true;
			if(binded != null) binded();

		}

		public void send(IpcMsg msg) {
			if(msg != null) {
				#if DEBUG
				if(msg.op != OP.NpcMove) {
					string plain = fastJSON.JSON.Instance.ToJSON(msg);
					ConsoleEx.DebugLog("Sending message : " + msg.op.ToString() + "  " + plain, ConsoleEx.YELLOW);
				}
				#endif

				if(isBinded) {
					byte[] outBytes = ProtoLoader.serializeProtoObject<IpcMsg>(msg);
					pubSocket.SendMore(msg.op.ToString()).Send(outBytes);
				}
			}
		}

		public void Quit() {
			isBinded = false;
			if(pubSocket != null) {
				pubSocket.Unbind(BindAddres);
				pubSocket.Close();
				pubSocket = null;
			}

		}

	}


}

