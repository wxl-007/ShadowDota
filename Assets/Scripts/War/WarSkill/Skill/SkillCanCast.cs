using System;
using AW.Data;
using AW.Framework;
using System.Collections.Generic;
using UVec3 = UnityEngine.Vector3;
using System.Linq;

namespace AW.War {

	/// <summary>
	/// 判定能否释放技能的先决判定，提供给AI使用
	/// </summary>
	public class SkillCanCast {

		private WarServerNpcMgr NpcMgr;
		public SkillCanCast(WarServerNpcMgr npcMgr) {
			NpcMgr = npcMgr;
		}

		/// <summary>
		/// 检测伤害技能能否释放, 
		/// 回血，恢复之类的技能不做检测
		/// 
		/// 判定顺序是： 
		/// 0. 判定技能的类型
		/// 1. 判定是否是相同的阵营
		/// 2. 判定施法者的状态
		/// 3. 判定目标是什么类型的NPC（比如建筑物，英雄。。。）
		/// 3. 判定施法的距离
		/// 5. 目标的状态
		/// 
		/// </summary>
		/// <returns><c>true</c>, if cast was caned, <c>false</c> otherwise.</returns>
		public bool canCast(ServerNPC caster, ServerNPC target, RtSkData rtOne, bool ignoreDis = false) {
			bool can = true;

			#if DEBUG
			Utils.Assert(caster == null, "Can't know whether can cast skill unless caster isn't null.");
			Utils.Assert(target == null, "Can't know whether can cast skill unless target isn't null.");
			Utils.Assert(rtOne == null,  "Can't know whether can cast skill unless runtime skill isn't null");
			#endif

			SkillConfigData skillCfg = rtOne.skillCfg;
			/// 0. 判定技能的类型
			can = skillCfg.DamageType == 0;
			if(can == false) return can;

			/// 1.判定是否是相同的阵营
			can = caster.Camp != target.Camp;
			if(can == false) return can;

			/// 2. 判定施法者的状态

			//只有LifeNPC才检查施法者的状态
			ServerLifeNpc castlife = caster as ServerLifeNpc;
			if(castlife != null) {
				if(skillCfg.CasterStatusReject.AnySame(castlife.curStatus)) {
					//不可施法
					can = false;
				}
			}
			if(can == false) return can;

			/// 3. 判定目标是什么类型的NPC
			LifeNPCType type = skillCfg.TargetType.toPositive();
			can = type.check(target.data.configData.type);

			if(can == false) return can;

			/// 4. 判定施法的距离
			if(ignoreDis == false) {
				can = AITools.IsInRange(caster.transform.position, skillCfg.Distance, target.transform);
				if(can == false) return can;
			}

			/// 5. 目标的状态

			ServerLifeNpc targetLife = target as ServerLifeNpc;
			if(targetLife != null) {
				if(targetLife.curStatus.AnySame(skillCfg.TargetStatusReject)) {
					//不可施法
					can = false;
				}
			}

			return can;
		}

		/// <summary>
		/// 检测回复技能
		/// 
		/// 检测逻辑和上面的并不太一致
		/// 判定顺序是： 
		/// 0. 判定技能的类型
		/// 1. 判定施法者的状态
		/// 2. 选在施法的距离（特定范围）内血最少的 英雄 NPC
		/// </summary>
		/// <returns><c>true</c>, if cast was caned, <c>false</c> otherwise.</returns>
		/// <param name="caster">Caster.</param>
		public bool canCast(ServerNPC caster, RtSkData rtOne) {
			bool can = true;

			#if DEBUG
			Utils.Assert(caster == null, "Can't know whether can cast skill unless caster isn't null.");
			Utils.Assert(rtOne == null,  "Can't know whether can cast skill unless runtime skill isn't null");
			#endif

			SkillConfigData skillCfg = rtOne.skillCfg;
			/// 0. 判定技能的类型
			can = skillCfg.DamageType == 1;
			if(can == false) return can;

			/// 1.判定施法者的状态

			//只有LifeNPC才检查施法者的状态
			ServerLifeNpc castlife = caster as ServerLifeNpc;
			if(castlife != null) {
				if(skillCfg.CasterStatusReject.AnySame(castlife.curStatus)) {
					//不可施法
					can = false;
				}
			}
			if(can == false) return can;

			/// 2. 选在施法的距离（特定范围）内血最少的 英雄 NPC
			List<ServerNPC> friendlyArmy = SelectorTools.GetNpcWithInRange(caster, skillCfg.Distance, NpcMgr, caster.Camp, KindOfNPC.Life, LifeNPCType.Hero);
			if(friendlyArmy != null && friendlyArmy.Count > 0) {
				//选择血最少的
				ServerNPC friend = friendlyArmy.OrderBy( npc => (npc.data.rtData.curHp * 1.0f / npc.data.rtData.totalHp) ).FirstOrDefault();
				if(friend != null) {
					NPCRuntimeData npcRtdt = friend.data.rtData;
					float factor = npcRtdt.curHp * 1.0f / npcRtdt.totalHp;
					//低于20%的
					if(factor > 0.2f) {
						can = false;
					}
				}

			}

			return can;
		}


	}
}
