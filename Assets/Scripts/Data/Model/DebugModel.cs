using AW.War;
using System;
using System.Collections.Generic;

namespace AW.Data {
	/// <summary>
	/// 仅仅是开发时期使用，
	/// </summary>
	public class DebugModel : IWarModel {

		public MapInfo getMap() {
			return new MapInfo() {
				ID = 1,
				type = ConfigType.PVPBattle,
			};
		}

		public RoomCharactor getCharactor (WarCamp camp) {
			Team clientTeam = getTeam(camp);

			RoomCharactor charactor = new RoomCharactor() {
				Name = "Allen",
				UID  = DeviceInfo.GUID,
				Lv   = 3,
				camp = camp,
				team = clientTeam,
			};

			return charactor;
		}

		public Team getTeam(WarCamp camp) {
			Team team = new Team() {
				team = new List<RoomNpc>(),
			};
			RoomNpc npc = null;

			if(camp == WarCamp.FirstCamp) {

				npc = new RoomNpc(){
					NpcNum  = 1004, 
					NpcStar = 1,
				};
				team.team.Add(npc);

				npc = new RoomNpc(){
					NpcNum  = 1002, 
					NpcStar = 1,
				};
				team.team.Add(npc);

				npc = new RoomNpc(){
					NpcNum  = 1003, 
					NpcStar = 1,
				};
				team.team.Add(npc);


			} else if(camp == WarCamp.SecondCamp) {

				npc = new RoomNpc(){
					NpcNum  = 1001, 
					NpcStar = 1,
				};
				team.team.Add(npc);

				npc = new RoomNpc(){
					NpcNum  = 1002, 
					NpcStar = 1,
				};
				team.team.Add(npc);

				npc = new RoomNpc(){
					NpcNum  = 1003, 
					NpcStar = 1,
				};
				team.team.Add(npc);
			}
			return team;
		}

	}
}