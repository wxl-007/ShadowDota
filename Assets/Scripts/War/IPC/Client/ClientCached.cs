using System;
using AW.Data;

namespace AW.War {
	/// <summary>
	/// 客户端的一些数据缓存
	/// </summary>
	public class ClientCached {
		public static ClientCached Instance {
			get {
				return GenericSingleton<ClientCached>.Instance;
			}
		}

		public MapInfo map = null;

		public ServerInfo curServer = null;

		//当前端的信息
		public RoomCharactor Charactor;

		//当前地图信息
		public ChapterConfigData ChapConfig;

		/// <summary>
		/// 每次进入战斗前都需要清理数据
		/// </summary>
		public void clear() {
			map = null;
			curServer = null;
		}

	}
}
