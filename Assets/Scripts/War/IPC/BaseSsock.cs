using System;
using System.Text;
using NetMQ.Sockets;

namespace AW.War {

	/// <summary>
	/// 所有通讯的基类
	/// </summary>
	public class BaseSsock {
		//一场战斗开始的依赖情况
		protected WarInfo warInfo;
		//Socket配置信息
		protected EngineCfg EngCfg;

		public BaseSsock(WarInfo war) {
			warInfo = war;
			EngCfg  = Core.EngCfg ;
		}

		/// <summary>
		/// 是否为进程内通讯
		/// </summary>
		/// <value><c>true</c> if is inproc; otherwise, <c>false</c>.</value>
		protected bool isInproc {
			get {
				bool inproc = false;
				if(warInfo != null) {
					if(warInfo.warMo == WarMode.NativeWar) {
						inproc = true;
					}
				}
				return inproc;
			}
		}

		protected string Protocol {
			get {
				string protocol = "tcp://";
				if(warInfo != null) {
					if(warInfo.warMo == WarMode.NativeWar) {
						protocol = "inproc://";
					}
				}
				return protocol;
			}
		}

		/// <summary>
		/// 绑定地址，只有服务器端有效
		/// </summary>
		/// <value>The bind address.</value>
		protected string BindAddr(Type sockType) {
			StringBuilder sb = new StringBuilder();
			sb.Append(Protocol);
			if(isInproc) {
				sb.Append("Inproc_");

				if(sockType == typeof(ResponseSocket))
					sb.Append(EngCfg.PairPort.ToString());
				else if(sockType == typeof(PublisherSocket))
					sb.Append(EngCfg.PubPort.ToString());
				else if(sockType == typeof(HeartBeatServer)) 
					sb.Append(EngCfg.HeartBeatPort.ToString());

			} else {

				if(sockType == typeof(ResponseSocket))
					sb.Append("*:").Append(EngCfg.PairPort.ToString());
				else if(sockType == typeof(PublisherSocket))
					sb.Append("*:").Append(EngCfg.PubPort.ToString());
				else if(sockType == typeof(HeartBeatServer))
					sb.Append("*:").Append(EngCfg.HeartBeatPort.ToString());

			}
			return sb.ToString();
		}

		/// <summary>
		/// 链接地址，只对客户端有用
		/// </summary>
		/// <returns>The address.</returns>
		/// <param name="sockType">Sock type.</param>
		protected string ConnectAddr(Type sockType, string ipAdd = null) {
			StringBuilder sb = new StringBuilder();
			sb.Append(Protocol);
			if(isInproc) {
				sb.Append("Inproc_");

				if(sockType == typeof(RequestSocket))
					sb.Append(EngCfg.PairPort.ToString());
				else if(sockType == typeof(SubscriberSocket))
					sb.Append(EngCfg.PubPort.ToString());
				else if(sockType == typeof(HeartBeatClient))
					sb.Append(EngCfg.HeartBeatPort.ToString());

			} else {

				string ip = warInfo.ServerIp + ":";

				if(sockType == typeof(RequestSocket))
					sb.Append(ip).Append(EngCfg.PairPort.ToString());
				else if(sockType == typeof(SubscriberSocket))
					sb.Append(ip).Append(EngCfg.PubPort.ToString());
				else if(sockType == typeof(HeartBeatClient))
					sb.Append(ip).Append(EngCfg.HeartBeatPort.ToString());

			}
			return sb.ToString();
		}

	}
}
