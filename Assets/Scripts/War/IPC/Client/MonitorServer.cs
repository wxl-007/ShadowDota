using System;
using System.Diagnostics;

namespace AW.War {

	/// <summary>
	/// 当前服务前的状态
	/// </summary>
	public enum ServerStatus {
		None     = 0x0,
		Ready    = 0x1,
		EnterWar = 0x2,
		Quit     = 0x4,
		Timeout  = 0x8,
	}

	public class VirtualServer {
		//服务器端ID
		public string ServerID;
		//服务器端的状态
		public ServerStatus curStatus;
		//心跳的间隔时间
		public long HeartTick;
	}

	/// <summary>
	/// 监控服务器端的情况
	/// </summary>
	public class MonitorServer {
		public readonly VirtualServer Server;
		private Stopwatch stopwatch;

		//心跳客户端
		private HeartBeatClient hbCli;

		private Action mHeartBeatDisConn;
		public MonitorServer() {
			Server = new VirtualServer();
			stopwatch = Stopwatch.StartNew();
		}

		public void startMonitor(WarInfo war, Action HeartBeatDisConn) {
			if(war.warMo == WarMode.InLanWar) {
				mHeartBeatDisConn = HeartBeatDisConn;
				hbCli = new HeartBeatClient(war, ServerTimeOut);
			}
		}

		public void ServerReady(ServerInfo server) {
			Server.ServerID = server.ServerID;
			Server.curStatus = ServerStatus.Ready;
		}

		public void ServerQuit(string ID) {
			if(Server.ServerID == ID) {
				Server.curStatus = ServerStatus.Quit;
			}
		}

		public void ServerTimeOut() {
			Server.curStatus = ServerStatus.Timeout;
			if(mHeartBeatDisConn != null) mHeartBeatDisConn();
		}

		public void EnterWar(string ID) {
			if(Server.ServerID == ID) {
				Server.curStatus = ServerStatus.EnterWar;
			}
		}

		public void Quit() {
			stopwatch.Stop();
			if(hbCli != null) hbCli.Quit();
		}

	}

}
