using System;
using System.Collections.Generic;
using AW.Data;

namespace AW.War {

	/// <summary>
	/// 给英雄使用的优先级列表
	/// </summary>
	[SkPriority(priority = SkTargetPriority.PriorityForHero)]
	public class PriorityHero : BasePriority, IPrioritySelect {

		enum ExistFlag : byte {
			None     = 0x0,
			Hero     = 0x1,
			Summoner = 0x2,
			minion   = 0x4,
			Tower    = 0x8,
			Building = 0x10,
		}

		/// 1.英雄 > 召唤兽 > 小兵 > 防御塔 > 建筑  （英雄使用）
		public void SortByPriority(IEnumerable<ServerNPC> unPriority, WarServerNpcMgr npcMgr, List<List<ServerNPC>> HasPriority) {
			//已经排好序的
			if(HasPriority == null) 
				HasPriority = new List<List<ServerNPC>>();
			else 
				HasPriority.Clear();

			for(int i = 0; i < 5; ++ i) {
				HasPriority.Add(null);
			}

			foreach(ServerNPC npc in unPriority) {

				NPCConfigData cfg = npc.data.configData;
				ExistFlag totest = LifeNpcType2ExistFlag(cfg.type, cfg.bldType);
				///
				/// 如果属于不关心的类型，则丢弃
				///
				if(totest == ExistFlag.None) continue;

				base.addNpcByPriority(npc, (byte)totest, HasPriority);
			}

		}


		protected override int getPos(byte enumFlag) {
			int i = 0;

			ExistFlag flag = (ExistFlag) Enum.ToObject(typeof(ExistFlag), enumFlag);
			switch(flag) {
			case ExistFlag.Hero:
				i = 0;
				break;
			case ExistFlag.Summoner:
				i = 1;
				break;
			case ExistFlag.minion:
				i = 2;
				break;
			case ExistFlag.Tower:
				i = 3;
				break;
			case ExistFlag.Building:
				i = 4;
				break;
			}

			return i;
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

	}

}