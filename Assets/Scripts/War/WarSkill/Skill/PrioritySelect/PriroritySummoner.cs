using System;
using System.Collections.Generic;
using AW.Data;

namespace AW.War {

	/// <summary>
	/// 英雄英雄攻击的目标 > 英雄 > 召唤兽 > 小兵 > 防御塔 > 建筑   （召唤兽使用）
	/// </summary>
	[SkPriority(priority = SkTargetPriority.PriorityForSummoner)]
	public class PriroritySummoner : BasePriority, IPrioritySelect {

		enum ExistFlag : byte {
			None     = 0x0,
			HeroTar  = 0x1,
			Hero     = 0x2,
			Summoner = 0x4,
			minion   = 0x8,
			Tower    = 0x10,
			Building = 0x20,
		}


		protected override int getPos (byte enumFlag) {
			int i = 0;

			ExistFlag flag = (ExistFlag) Enum.ToObject(typeof(ExistFlag), enumFlag);
			switch(flag) {
			case ExistFlag.HeroTar:
				i = 0;
				break;
			case ExistFlag.Hero:
				i = 1;
				break;
			case ExistFlag.Summoner:
				i = 2;
				break;
			case ExistFlag.minion:
				i = 3;
				break;
			case ExistFlag.Tower:
				i = 4;
				break;
			case ExistFlag.Building:
				i = 5;
				break;
			}

			return i;
		}

		public void SortByPriority(IEnumerable<ServerNPC> unPriority, WarServerNpcMgr npcMgr, List<List<ServerNPC>> HasPriority) {
			//已经排好序的
			if(HasPriority == null) 
				HasPriority = new List<List<ServerNPC>>();
			else 
				HasPriority.Clear();

			for(int i = 0; i < 6; ++ i) {
				HasPriority.Add(null);
			}

			foreach(ServerNPC npc in unPriority) {
				ExistFlag toTest = toExistType(npc, npcMgr);
				base.addNpcByPriority(npc, (byte)toTest, HasPriority);
			}

		}

		#region 获取一个NPC属于何种优先级
		ExistFlag toExistType(ServerNPC npc, WarServerNpcMgr npcMgr) {
			NPCConfigData cfg = npc.data.configData;
			ExistFlag flag    = LifeNpcType2ExistFlag(cfg.type, cfg.bldType);
			if(flag == ExistFlag.Hero) {

				if(npc.TargetID != -1) {

					ServerNPC npcTar = npcMgr.GetNPCByUniqueID(npc.TargetID);
					if(npcTar != null) {
						flag = ExistFlag.HeroTar;
					}

				}

			}

			return flag;
		}
		ExistFlag LifeNpcType2ExistFlag(LifeNPCType type, BuildNPCType bldType) {
			ExistFlag flag = ExistFlag.None;

			switch(type) {
			case LifeNPCType.Hero:
				flag = ExistFlag.Hero;
				break;

			case LifeNPCType.Build:
				switch(bldType) {
				case BuildNPCType.Tower:
					flag = ExistFlag.Tower;
					break;

				case BuildNPCType.Barrank:
				case BuildNPCType.Base:
					flag = ExistFlag.Building;
					break;

				}
				break;
			case LifeNPCType.Soldier:
				flag = ExistFlag.minion;
				break;
			case LifeNPCType.Summon:
				flag = ExistFlag.Summoner;
				break;
			}

			return flag;
		}
		#endregion

	}
}
