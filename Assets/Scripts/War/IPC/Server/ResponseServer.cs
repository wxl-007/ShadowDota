using System;
using System.Threading;

namespace AW.War {

	using NetMQ;
	using NetMQ.Sockets;

	/// <summary>
	/// 服务器端的一对一通讯者
	/// 走协议数据（也就是NetMQ Message格式）
	/// </summary>
	public class ResponseServer : BaseSsock {

		/// <summary>
		/// 
		/// NetMQ的pattern之Req和Rep是： 1 对 N 
		/// 1是Rep，N是Req
		/// 
		/// ResponseServer会处理所有client端发送过来的数据
		/// 
		/// </summary>
		private Func<NetMQMessage, NetMQMessage> handler = null;
		private ResponseSocket respSocket = null;
		private Poller poller;
		private string BindingAddress;

		public ResponseServer(WarInfo war, Func<NetMQMessage, NetMQMessage> real, Action BindEd) : base(war) {
			handler = real;
			poller  = new Poller();
			establish(BindEd);
		}

		void establish(Action BindEd) {

			ThreadPool.QueueUserWorkItem( (o) => {
				ConsoleEx.DebugLog("Respose socket is binding...", ConsoleEx.RED);

				var context = Core.ZeroMQ;
				respSocket = context.CreateResponseSocket();
				respSocket.Options.SendHighWatermark = EngCfg.HighWatermark;
				respSocket.Options.ReceiveHighWatermark = EngCfg.HighWatermark;

				//生成通讯地址和协议方式
				BindingAddress = BindAddr(typeof(ResponseSocket));
				respSocket.Bind(BindingAddress);

				///
				/// 绑定完成，等待Request的请求
				///
				respSocket.ReceiveReady += Server_ReceiveReady;
				poller.AddSocket(respSocket);
				ThreadPool.QueueUserWorkItem( (ob) => { poller.PollTillCancelled(); } );

				if(BindEd != null) { BindEd (); }
			});

		}

		void Server_ReceiveReady(object sender, NetMQSocketEventArgs e) {
			/// 每一个Recev后能接一个Send
			///
			var message = e.Socket.ReceiveMessage();
			// 处理后需要发送的Msg
			var RespMsg = handler(message);
			///
			/// 返回信息给发送者
			///
			if(RespMsg != null)
				e.Socket.SendMessage(RespMsg);
		}

		public void Quit() {
			if(poller != null) {
				bool started = poller.IsStarted;
				if(started)
					poller.CancelAndJoin();
				poller.Dispose();
			} 

			if(respSocket != null) {
				respSocket.Unbind(BindingAddress);
				respSocket.Close();
				respSocket = null;
			}

		}

	}
}
