using System;
using AW.Data;
using System.Collections.Generic;
using AW.Framework;
using UVec3 = UnityEngine.Vector3;
using System.Linq;

namespace AW.War {
	/// <summary>
	/// 技能选择目标的接口
	/// </summary>
	public class SkillSelector {

		private SkPriorityMgr priorityMgr;
		private WarServerNpcMgr npcMgr;
		public SkillSelector(WarServerNpcMgr NpcMgr) {
			npcMgr = NpcMgr;
			priorityMgr = SkPriorityMgr.instance;
		}

		/// <summary>
		/// 选择的规则是： 
		///  多目标判定规则
		/// 1. 判定施法者的状态
		/// 2. 判定施法者的上次目标，如果目标在攻击范围内，则选择为最优先目标
		/// 3. 判定施法的范围类型
		/// 4. 目标的状态
		/// 5. 根据施法的距离选择出目标
		/// 6. 根据血量选择出目标
		/// 7. 目标的优先级
		/// 8. 单体攻击敌方，并且有最高优先级的目标
		/// 
		/// 
		/// 单目标判定规则
		/// 
		/// 
		/// 
		/// </summary>
		/// <returns><c>true</c>, if cast was caned, <c>false</c> otherwise.</returns>
		public IEnumerable<ServerNPC> Select(ServerNPC caster, RtSkData rtOne, Sight sight) {
			//枚举器
			IEnumerable<ServerNPC> itor = null, itor1 = null;

			#if DEBUG
			Utils.Assert(caster == null, "Can't find target unless caster isn't null");
			Utils.Assert(rtOne == null,  "Can't find target unless runtime skill isn't null");
			#endif


			SkillConfigData skillCfg = rtOne.skillCfg;

			/// 视野范围 -- 决定索敌范围
			float CheckDistance = 0F;
			if(sight == Sight.NearSight) {
				CheckDistance = skillCfg.Distance;
			} else {
				CheckDistance = caster.data.rtData.seekRange;
			}

			bool castcan = true;
			//只有LifeNPC才检查施法者的状态
			//1. 判定施法者的状态
			ServerLifeNpc castlife = caster as ServerLifeNpc;
			if(castlife != null) {
				if(castlife.curStatus.AnySame(skillCfg.CasterStatusReject)) {
					//不可施法
					castcan = false;
				}
			}

			if(castcan == false) return new List<ServerNPC>().AsEnumerable<ServerNPC>();
			//能进入CD
			rtOne.EnterCD();

			///
			/// 3. 判定施法的范围类型 . 
			/// 单体和AOE都需要在技能配置里选择出目标 ， 再次到Effect里面选择目标。 -- 叫做前置判定
			/// 方向性则不需要选择目标，该类型的技能可对空地释放，再次到EffectTarget的目标是skill的目标 -- 叫做后置判定
			///

			TargetClass rtTargetCls = skillCfg.SkillTarget;

			if(skillCfg.RangeType == RangeClass.Direction) {
				//后置判定
				return new List<ServerNPC>().AsEnumerable<ServerNPC>();
			} 

			bool isMultiTarget = skillCfg.RangeType != RangeClass.SingleTarget;
			bool isNoPriority  = skillCfg.TargetPriority == SkTargetPriority.NonePriority;
			bool isTargetSelf  = rtTargetCls.AnySame(TargetClass.Self);
			//单目标
			ServerNPC singlePriority = null;

			LifeNPCType type = skillCfg.TargetType.toPositive();
			///
			/// 是否为多目标的战斗, 或者是没有优先级的单目标
			///
			if(isMultiTarget || isNoPriority || isTargetSelf) {

				///
				/// 4. 目标的状态
				/// 
				if(rtTargetCls.AnySame(TargetClass.Friendly)) {
					//友方
					itor1 = SelectorTools.GetNPCValideStatus(caster, npcMgr, KindOfNPC.Life, rtTargetCls, NpcStatus.None, type);
				} else if(rtTargetCls.AnySame(TargetClass.Hostile)) {
					//敌方
					itor1 = SelectorTools.GetNPCValideStatus(caster, npcMgr, KindOfNPC.Life, rtTargetCls, skillCfg.TargetStatusReject, type);
				} else if(rtTargetCls.AnySame(TargetClass.Self)) {
					//自己
					List<ServerNPC> targets = new List<ServerNPC>();
					targets.Add(caster);
					itor = targets.AsEnumerable<ServerNPC>();
					return targets;
				} else {
					itor1 = SelectorTools.GetNPCValideStatus(caster, npcMgr, KindOfNPC.Life, rtTargetCls, skillCfg.TargetStatusReject, type);
					ConsoleEx.DebugLog("Warning : We should have a Camp. But sometimes, it's Ok.", ConsoleEx.RED);
				}

				///
				/// 5. 根据施法的距离选择出目标
				/// 

				itor1 = SelectorTools.GetNPCInRadius(caster, CheckDistance, rtTargetCls, itor1);

				///
				/// 6. 根据血量选择出目标
				/// 
				itor1 = SelectorTools.GetNPCByHp(caster, rtTargetCls, itor1);

				if(isNoPriority) 
					singlePriority = itor1.FirstOrDefault();

			} else {

				///
				/// 获取单目标
				///

				singlePriority = SelectorTools.GetPrioritiedNpc(caster, rtTargetCls, npcMgr, CheckDistance, skillCfg.TargetStatusReject, skillCfg.TargetPriority, priorityMgr, type);

				//单目标的友方，有可能选择到自己（但并不属于 TargetClass 64.就针对自己）
				if(!rtTargetCls.AnySame(TargetClass.Hostile)) {
					List<ServerNPC> broker = new List<ServerNPC>();
					if(singlePriority != null) {
						broker.Add(singlePriority);
					}  
					itor1 = broker.AsEnumerable<ServerNPC>();
				}

			}

			if(isMultiTarget == false) {

				///
				/// 9. 单体攻击敌方，只有打敌方单体的时候，TargetID才会被重新设定
				///
				if(rtTargetCls.AnySame(TargetClass.Hostile)) {

					ServerLifeNpc HighestPriority = null;
					///
					/// 检测嘲讽
					///
					int tarId = caster.getHighestHatred;
					if(tarId != -1) {
						//有被嘲讽的目标
						HighestPriority = npcMgr.GetNPCByUniqueID(tarId) as ServerLifeNpc;
					} else {
						//没有被嘲讽的目标

						///
						///  判定施法者的上次目标，则选择为最优先目标
						///  此逻辑，能保证追击残血逃跑的
						/// 

						if(caster.TargetID != -1) {
							HighestPriority = npcMgr.GetNPCByUniqueID(caster.TargetID) as ServerLifeNpc;
							if(HighestPriority != null) {

								///
								/// 如果单一目标的是英雄，
								///
								if(singlePriority != null && singlePriority.data.configData.type == LifeNPCType.Hero
									&& HighestPriority.data.configData.type != LifeNPCType.Hero) {

									HighestPriority = null;
									caster.TargetID = -1;

								} else {

									bool validate1 = true, validate2 = true, validate3 = true;
									/// 生命判定
									if(HighestPriority.data.rtData.curHp <= 0) 
										validate1 = false;

									/// 距离判定
									float distance = SelectorTools.GetDistance(HighestPriority.transform.position, caster.transform.position);
									distance = distance - HighestPriority.data.configData.radius - caster.data.configData.radius;

									/// 视野范围
									if(distance > CheckDistance) 
										validate2 = false;

									///如果有相同的状态，则不让释放技能
									validate3 = !HighestPriority.curStatus.AnySame(skillCfg.TargetStatusReject);

									/// 失败的判定
									bool validate = validate1 & validate2 & validate3;
									if(!validate) {
										HighestPriority = null;
										caster.TargetID = -1;
									} else {
										caster.RstTimeout();
									}

								}

							} else {
								caster.TargetID = -1;
							}
						}


					}

					//找到普通选择出的目标
					if(HighestPriority == null) {
						ServerNPC target = singlePriority;
						caster.TargetID = target != null ? target.UniqueID : -1;
						HighestPriority = target as ServerLifeNpc;
					}

					List<ServerNPC> high = new List<ServerNPC>();
					if(HighestPriority != null) {
						high.Add(HighestPriority);
					}  
					itor1 = high.AsEnumerable<ServerNPC>();

				} else {
					
					///
					/// 10.单体友方
					///
					if(itor1.Any())
						itor1 = itor1.Take(1);
				}

			}


			itor = itor1;
			return itor;
		}



	}
}
