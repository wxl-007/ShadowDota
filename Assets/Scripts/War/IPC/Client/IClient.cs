using System;
using System.Collections.Generic;

namespace AW.War {
	/// <summary>
	/// 客户端 <----> 服务器， 通讯接口
	/// </summary>
	public interface IClient {

		/// <summary>
		/// 服务器准备好了，可以连接到服务器
		/// 参数是服务器信息
		/// </summary>
		/// <param name="msg">Message.</param>
		void ServerReady(ServerInfo Server);
		/// <summary>
		/// 创建地形
		/// </summary>
		/// <param name="msg">Message.</param>
		void CtorEnv(MapInfo Map);
		/// <summary>
		/// 创建Npc
		/// </summary>
		void CtorNpc(IpcCreateNpcMsg msg);
		///
		/// 给客户端一条战斗消息
		/// 
		void Deliver(IpcMsg msg);

	}
}

