using System;


namespace AW.War {

	/// <summary>
	/// 位于服务器端的客户端的代理对象
	/// </summary>
	public class ProxyClient : IClient {
		public readonly int SignedId = 0;
		//消息发布者
		private PubServer publisher;

		public ProxyClient(int signedId, PubServer existOne) {
			SignedId = signedId;
			publisher = existOne;
		}

		#region 发送给真实客户端
		/// <summary>
		/// 创建地形
		/// </summary>
		/// <param name="msg">Message.</param>
		public void CtorEnv(MapInfo Map) {
			IpcCreateMapMsg msg = new IpcCreateMapMsg();
			msg.MapId   = Map.ID;
			msg.MapType = (int) Map.type;
			publisher.send(msg);
		}

		/// <summary>
		/// 创建Npc
		/// </summary>
		public void CtorNpc(IpcCreateNpcMsg msg) {
			publisher.send(msg);
		}

        /// <summary>
        /// 创建英雄
        /// </summary>
        public void CtorHero(IpcCreateHeroMsg msg) {
            publisher.send(msg);
        }

        //npc位移消息
        public void NPCMove(IpcNpcMoveMsg msg)
        {
            publisher.send(msg);
        }

        //npc血量变化
        public void NPChp(IpcNpcHpMsg msg)
        {
            publisher.send(msg);
        }

        //npc动画
        public void NPCAnim(IpcNpcAnimMsg msg)
        {
            publisher.send(msg);
        }

        //npc状态
        public void NPCStatus(IpcNpcStatusMsg msg)
        {
            publisher.send(msg);
        }

		///
		/// 给客户端一条战斗消息
		/// 
		public void Deliver(IpcMsg msg) {
			publisher.send(msg);
		}

        public void NpcDestroy(IpcDestroyNpcMsg msg) {
            publisher.send(msg);
        }

        public void NpcSkillCD(IpcSkillMsg msg)
        {
            publisher.send(msg);
        }

		/// <summary>
		/// 发送给客户端的同步数据
		/// </summary>
		/// <param name="msg">Message.</param>
		public void SyncClient(IpcSyncClientMsg msg) {
			publisher.send(msg);
		}

		/// <summary>
		/// 服务器准备好了，可以连接到服务器
		/// 参数是服务器信息， 参数没用
		/// </summary>
		/// <param name="msg">Message.</param>
		public void ServerReady(ServerInfo Server) {
			IpcServerReadyMsg ServerReady = new IpcServerReadyMsg();
			ServerReady.IpAddr  = Server.IpAddr;
			ServerReady.PubPort = Server.PubPort;
			ServerReady.PairPort= Server.PairPort;
			ServerReady.HeartBeatPort = Server.HeartBeatPort;
			ServerReady.ServerName = Server.ServerName;
			ServerReady.ServerID= Server.ServerID;

			publisher.send(ServerReady);
		}

		/// <summary>
		/// 调用完毕后，要记得关闭RealServer
		/// </summary>
		/// <param name="serverId">Server identifier.</param>
		public void ServerQuit(string serverId) {
			IpcServerQuitMsg quit = new IpcServerQuitMsg();
			quit.ServerID = serverId;

			publisher.send(quit);
		}

		/// <summary>
		/// Enters the war.
		/// </summary>
		public void EnterWar(ServerInfo Server, MapInfo map) {
			IpcEnterWar enter = new IpcEnterWar();
			enter.ServerName  = Server.ServerName;
			enter.ServerID    = Server.ServerID;
			enter.MapId       = map.ID;
			enter.MapType     = (int) map.type;
			publisher.send(enter);
		}

		#endregion
	}

}