using System;
using AW.Message;
using AW.Framework;
using AW.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace AW.War {

	/// 
	/// 战斗中技能的释放的口
	/// 
	public class SkillCastor {

		/// <summary>
		/// 技能选择器
		/// </summary>
		private SkillSelector SelectS;
		/// <summary>
		/// 技能能否释放
		/// </summary>
		private SkillCanCast CanCast;
		/// <summary>
		/// 效果释放器
		/// </summary>
		private EffectCastor EfCastor;

		private List<MsgParam> container;

		private SkillCastor () {
			container = new List<MsgParam>();
		}

		public void init(WarServerNpcMgr npcMgr) {
			SelectS = new SkillSelector(npcMgr);
			CanCast = new SkillCanCast(npcMgr);
			EfCastor = new EffectCastor(npcMgr);
		}

		public static SkillCastor Instance {
			get {
				return GenericSingleton<SkillCastor>.Instance;
			}
		}

		#region 施法技能
		/// <summary>
		/// 返回可以释放的技能, 这个要一直检测
		/// 这个只检查4个技能，不检查普通攻击
		/// </summary>
		/// <returns>The cast.</returns>
		/// <param name="caster">Caster.</param>
		/// <param name="target">Target.</param>
		public List<short> canCast(ServerNPC caster, ServerNPC target) {
			List<short> skNumList = new List<short>();

			#if DEBUG 
			Utils.Assert(caster == null, "Caster is null when check whether can cast skill.");
			Utils.Assert(target == null, "Target is null when check whether can cast skill.");
			#endif

			ServerLifeNpc castLife = caster as ServerLifeNpc;
			if(castLife.IsAlive == false) return skNumList;

			short pos = 0;
			for( ; pos < Consts.MAX_SKILL_COUNT; ++ pos) {
				RtSkData rtsk = castLife.runSkMd.getRuntimeSkill(pos);
				if(rtsk != null) {
					bool can = false;
					///
					/// 冷却时间好了吗
					/// 
					can = rtsk.canCast;

					SkillConfigData skCfg = rtsk.skillCfg;
					if(skCfg.DamageType == (short)1) {
						can = can & CanCast.canCast(caster, rtsk);
					} else if (skCfg.DamageType == (short)0) {
						can = can & CanCast.canCast(caster, target, rtsk);
					}

					//如果可以则加入可释放列表
					if(can) {
						skNumList.Add(pos);
					}
				}
			}

			return skNumList;
		}
			
		/// <summary>
		/// // 判断skData能否对target使用
		/// </summary>
		/// <returns><c>true</c>, if cast was caned, <c>false</c> otherwise.</returns>
		/// <param name="caster">施法者.</param>
		/// <param name="target">技能使用目标.</param>
		/// <param name="skData">使用的技能.</param>
		public bool canCast(ServerNPC caster, ServerNPC target, RtSkData rtsk)
		{
			if (rtsk != null)
			{
				bool can = false;
				///
				/// 冷却时间好了吗
				/// 
				can = rtsk.canCast;

				SkillConfigData skCfg = rtsk.skillCfg;
				if (skCfg.DamageType == (short)1)
				{
					can = can & CanCast.canCast (caster, rtsk);
				}
				else if (skCfg.DamageType == (short)0)
				{
					can = can & CanCast.canCast (caster, target, rtsk);
				}

				return can;
			}
			return false;
		}


		//不检查CD，不检查距离
		public bool isVaild(ServerNPC caster, ServerNPC target, short pos) {
			bool isValid = false;
			ServerLifeNpc castLife = caster as ServerLifeNpc;
			if(castLife.IsAlive == false) return isValid;

			RtSkData rtsk = castLife.runSkMd.getRuntimeSkill(pos);
			if(rtsk != null) {
				isValid = CanCast.canCast(caster, target, rtsk, true);
			}

			return isValid;
		}

		//判断target是否是skData的一个有效技能使用目标（不包括cd和距离，值判断能否对target使用）
		public bool IsValidTarget(ServerNPC caster, ServerNPC target, RtSkData rtsk) {
			bool isValid = false;
			ServerLifeNpc castLife = caster as ServerLifeNpc;
			if(castLife.IsAlive == false) return isValid;

			if(rtsk != null) {
				isValid = CanCast.canCast(caster, target, rtsk, true);
			}

			return isValid;
		}

		/// 
		/// 释放技能, pos的第几个技能
		/// 
		public void Cast(ServerNPC caster, short pos, Action<MsgParam> Report) {

			#if DEBUG 
			Utils.Assert(caster == null, "Caster is null when cast skill.");
			#endif

			ServerLifeNpc src = caster as ServerLifeNpc;
			if(src.IsAlive == false) return;

			RtSkData rtSk = src.runSkMd.getRuntimeSkill(pos);
			///
			/// 冷却时间好了吗, 死亡了吗
			/// 
			if(rtSk != null && rtSk.canCast) {
				//进入Skill释放的状态
				src.curStatus = src.curStatus.set(NpcStatus.InSkill);

				IEnumerable<ServerNPC> targets = SelectS.Select(caster, rtSk, Sight.NearSight);
				EfCastor.Cast(caster, targets, rtSk, Report);
			}

		}

		/// <summary>
		/// 直接释放技能，技能框架里面直接调用
		/// </summary>
		/// <param name="caster">Caster.</param>
		/// <param name="rtSk">Rt sk.</param>
		/// <param name="Report">Report.</param>
		public void Cast(ServerNPC caster, RtSkData rtSk, Action<MsgParam> Report) {
			#if DEBUG 
			Utils.Assert(caster == null, "Caster is null when cast skill.");
			#endif

			ServerLifeNpc src = caster as ServerLifeNpc;
			if(src.IsAlive == false) return;

			///
			/// 冷却时间好了吗, 死亡了吗
			/// 
			if(rtSk != null && rtSk.canCast) {
				//进入Skill释放的状态
				src.curStatus = src.curStatus.set(NpcStatus.InSkill);

				IEnumerable<ServerNPC> targets = SelectS.Select(caster, rtSk, Sight.NearSight);
				EfCastor.Cast(caster, targets, rtSk, Report);
			}
		}
		#endregion

		#region 普通攻击

		/// <summary>
		/// pos 普通攻击的第几步
		/// </summary>
		public List<MsgParam> NormalAttack(ServerNPC caster, short pos) {
			container.Clear();

			#if DEBUG 
			Utils.Assert(caster == null, "Caster is null when cast skill.");
			#endif

			ServerLifeNpc src = caster as ServerLifeNpc;
			if(src.IsAlive == false) return container;

			RtSkData rtSk = src.runSkMd.getAttack(pos);
			if(rtSk != null) {
				IEnumerable<ServerNPC> targets = SelectS.Select(caster, rtSk, Sight.NearSight);
				EfCastor.Cast(caster, targets, rtSk, container);
			}

			int cnt = container.Count;
			if(cnt > 0) {
				for(int i = 0; i < cnt; ++ i) {
					((WarMsgParam)container[i]).cmdType = WarMsg_Type.Attack;
				}
			}

			return container;
		}

		/// <summary>
		/// 搜索攻击目标
		/// </summary>
		/// <returns>The atk targets.</returns>
		public IEnumerable<ServerNPC> FindAtkTargets(ServerNPC caster, short pos, Sight sight) {
			#if DEBUG 
			Utils.Assert(caster == null, "Caster is null when cast skill.");
			#endif

			ServerLifeNpc src = caster as ServerLifeNpc;
			if(src.IsAlive == false) return null;

			RtSkData rtSk = src.runSkMd.getAttack(pos);
			if(rtSk != null) {
				IEnumerable<ServerNPC> targets = SelectS.Select(caster, rtSk, sight);
				return targets;
			} else {
				return null;
			}
		}

		#endregion

	}

}
