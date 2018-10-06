using System;
using System.Collections.Generic;
using AW.Data;

namespace AW.War {
	/// <summary>
	/// 3.攻击我方英雄的敌方英雄 > 攻击我方英雄的敌方召唤兽 > 攻击我方英雄的敌方小兵 > 小兵 > 召唤兽 > 防御塔 > 英雄 > 建筑  （小兵使用）
	/// </summary>
	public class PriorityMinion : BasePriority, IPrioritySelect {
		[Flags]
		enum ExistFlag : byte {
			None = 0x0,
			Hostile_Hero_ATK_Self_Hero     = 0x1,
			Hostile_Summoner_ATK_Self_Hero = 0x2,
			Hostile_Minion_ATK_Self_Hero   = 0x4,
			Hostile_Minion                 = 0x8,
			Hostile_Summoner               = 0x10,
			Hostile_Tower                  = 0x20,
			Hostile_Hero                   = 0x40,
			Hostile_Building               = 0x80,
		}

		protected override int getPos (byte enumFlag) {
			int index = 0;

			ExistFlag flag = (ExistFlag)Enum.ToObject(typeof(ExistFlag), enumFlag);
			switch(flag) {
			case ExistFlag.Hostile_Hero_ATK_Self_Hero :
				index = 0;
				break;
			case ExistFlag.Hostile_Summoner_ATK_Self_Hero :
				index = 1;
				break;
			case ExistFlag.Hostile_Minion_ATK_Self_Hero :
				index = 2;
				break;
			case ExistFlag.Hostile_Minion :
				index = 3;
				break;
			case ExistFlag.Hostile_Summoner :
				index = 4;
				break;
			case ExistFlag.Hostile_Tower :
				index = 5;
				break;
			case ExistFlag.Hostile_Hero :
				index = 6;
				break;
			case ExistFlag.Hostile_Building :
				index = 7;
				break;
			}

			return index;
		}

		public void SortByPriority(IEnumerable<ServerNPC> unPriority, WarServerNpcMgr npcMgr, List<List<ServerNPC>> HasPriority) {
			//已经排好序的
			if(HasPriority == null) 
				HasPriority = new List<List<ServerNPC>>();
			else 
				HasPriority.Clear();

			for(int i = 0; i < 8; ++ i) {
				HasPriority.Add(null);
			}

			foreach(ServerNPC npc in unPriority) {
				ExistFlag toTest = ExistFlag.None;
				///
				/// 挑选出关心的 敌方英雄，敌方召唤物， 敌方小兵
				///
				NPCConfigData cfg = npc.data.configData;
				bool careHero     = cfg.type.check(LifeNPCType.Hero);
				bool careSummoner = cfg.type.check(LifeNPCType.Summon);
				bool careMinion   = cfg.type.check(LifeNPCType.Soldier);
				bool careBuilding = cfg.type.check(LifeNPCType.Build);


				bool care = careHero || careSummoner || careMinion || careBuilding;
				if(care == false) 
					continue;

				if(careHero)
					toTest = NPCToFlag(npc, npcMgr, LifeNPCType.Hero);
				if(careSummoner)
					toTest = NPCToFlag(npc, npcMgr, LifeNPCType.Summon);
				if(careMinion)
					toTest = NPCToFlag(npc, npcMgr, LifeNPCType.Soldier);
				if(careBuilding)
					toTest = NPCToFlag(npc, npcMgr, LifeNPCType.Build);

				base.addNpcByPriority(npc, (byte)toTest, HasPriority);
			}

		}

		#region 获取一个NPC属于何种优先级
		ExistFlag NPCToFlag(ServerNPC npc, WarServerNpcMgr npcMgr, LifeNPCType type) {
			ExistFlag tar1 = ExistFlag.None;
			ExistFlag tar2 = ExistFlag.None;

			switch(type) {
			case LifeNPCType.Hero:
				tar1 = ExistFlag.Hostile_Hero;
				tar2 = ExistFlag.Hostile_Hero_ATK_Self_Hero;
				break;
			case LifeNPCType.Summon:
				tar1 = ExistFlag.Hostile_Summoner;
				tar2 = ExistFlag.Hostile_Summoner_ATK_Self_Hero;
				break;
			case LifeNPCType.Soldier:
				tar1 = ExistFlag.Hostile_Minion;
				tar2 = ExistFlag.Hostile_Minion_ATK_Self_Hero;
				break;
			}


			ExistFlag toTest = ExistFlag.None;

			if(type == LifeNPCType.Build) {
				NPCConfigData cfg = npc.data.configData;
				if(cfg.bldType.check(BuildNPCType.Tower))
					toTest = ExistFlag.Hostile_Tower;
				else if(cfg.bldType.check(BuildNPCType.Barrank) || cfg.bldType.check(BuildNPCType.Base))
					toTest = ExistFlag.Hostile_Building;
			} else {
				//没有攻击过的目标
				if(npc.TargetID == -1) {
					toTest = tar1;
				} else {
					ServerNPC targetNpc = npcMgr.GetNPCByUniqueID(npc.TargetID);
					if(targetNpc != null) {
						///
						/// 发现目标是Hero，则会判定为攻击了英雄
						///
						NPCConfigData tarCfg = targetNpc.data.configData;
						if( tarCfg.type.check(LifeNPCType.Hero) ) {
							toTest = tar2;
						} else {
							toTest = tar1;
						}

					} else {
						toTest = tar1;
					}

				}
			}

			return toTest;
		}

		#endregion


	}
}
