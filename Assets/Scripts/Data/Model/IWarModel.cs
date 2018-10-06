using System;

namespace AW.War {
	/// <summary>
	/// 战斗依赖的数据模型
	/// </summary>
	public interface IWarModel {
		MapInfo getMap();
		RoomCharactor getCharactor (WarCamp camp);
		Team getTeam(WarCamp camp);
	}
}