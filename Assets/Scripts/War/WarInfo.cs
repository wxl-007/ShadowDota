using System;
using AW.Data;

namespace AW.War {

	/// <summary>
	/// 战斗模式
	/// </summary>
	public enum WarMode : byte {
		// 本地端战斗
		NativeWar,
		//局域网道具
		InLanWar,
	}

	/// <summary>
	/// 是否是主机
	/// </summary>
	public enum WarSide : byte {
		ServerAndClient,
		OnlyClient,
	}

	/// <summary>
	/// 进入战斗前，需要设置的值(服务器端信息）
	/// </summary>
	public class WarInfo {
		/// 
		///  ------------ Following Data is for Server & Client -----------
		/// 
		public WarMode warMo;
		//这个仅仅表示主机和客户端 的端信息
		public WarSide Side;

		/// <summary>
		///  ------------ Following Data is for Server  -----------
		/// </summary>
		//进入战斗需要的客户端数量
		public int RequiredClientCount;
		//地图信息
		public MapInfo Map;
		/// 
		///  ------------ Following Data is for Client -----------
		/// 

		//客户端的角色信息
		public RoomCharactor Charactor;

		//服务端IP
		public string ServerIp;
	}

	/// <summary>
	/// 玩家是哪一边的，进入战斗后（会转换为Camp)
	/// </summary>
	public enum WarCamp : byte {
		AutoCamp,     //等待服务器端分配，是哪个阵营
		FirstCamp,    //第一阵营, 对应为 Camp的Player
		SecondCamp,   //第二阵营，对应为 Camp的Enemy
		ThirdCamp,    //观看者，对应为 Camp的None
	}

	public static class WarCamp2Camp {

		public static CAMP toCamp(WarCamp wc) {
			CAMP camp = CAMP.Player;
			switch(wc) {
			case WarCamp.FirstCamp:
				camp = CAMP.Player;
				break;
			case WarCamp.SecondCamp:
				camp = CAMP.Enemy;
				break;
			case WarCamp.ThirdCamp:
				camp = CAMP.None;
				break;
			}

			return camp;
		}

		public static WarCamp toWarCamp(CAMP cp) {
			WarCamp wcamp = WarCamp.FirstCamp;

			switch(cp) {
			case CAMP.Player:
				wcamp = WarCamp.FirstCamp;
				break;
			case CAMP.Enemy:
				wcamp = WarCamp.SecondCamp;
				break;
			case CAMP.None:
				wcamp = WarCamp.ThirdCamp;
				break;
			}
			return wcamp;
		}

	}




}
