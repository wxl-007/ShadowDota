using System;
using AW.Data;
using System.Collections.Generic;
using AW.Framework;

namespace AW.War {

	/// <summary>
	/// 激活的判定器
	/// </summary>
	public class ConditionCastor {
		private ConditionMgr Mgr;
		private SkConditionModel ConModel;

		private ConditionCastor() {
			Mgr = ConditionMgr.instance;
			ConModel = Core.Data.getIModelConfig<SkConditionModel>();
		}

		public static ConditionCastor instance {
			get { return GenericSingleton<ConditionCastor>.Instance; }
		}


		bool CheckIncite(RtSkData sk, EffectConfigData efCfg) {
			#if DEBUG
			Utils.Assert(sk == null, "RtSkData is null in CheckIncite.");
			Utils.Assert(efCfg == null, "EffectConfigData is null in CheckIncite.");
			#endif

			return sk.skillCfg.InciteEffectID == efCfg.ID;
		}

		/// <summary>
		/// 检测有没有斩首的逻辑，如果有伤害逻辑，就得要延迟执行
		/// </summary>
		/// <returns><c>true</c>, if be head was checked, <c>false</c> otherwise.</returns>
		/// <param name="sk">Sk.</param>
		bool CheckBeHead(RtSkData sk) {
			bool hasInjury = false;

			ConditionConfigure ConCfg = null;

			//获取激活的判定规则ID列表
			int[] IncideCon = sk.skillCfg.Incite;
			if(IncideCon != null && IncideCon.Length > 0) {

				int len = IncideCon.Length;
				for(int i = 0; i < len; ++ i) {
					//获取激活的判定规则ID
					int CondiId = IncideCon[i];
					if(CondiId > 0) {
						ConCfg = ConModel.get(CondiId);
						if(ConCfg.ConditionType == SkConditionType.BeHead || ConCfg.ConditionType == SkConditionType.BeHead2
							|| ConCfg.ConditionType == SkConditionType.BeHeadReset || ConCfg.ConditionType == SkConditionType.BeHead2Reset) {

							hasInjury = true;
							break;
						}
					}
				}
			}

			return hasInjury;
		}

		/// <summary>
		/// 挑出TimeOut的判定
		/// </summary>
		/// <returns>The up.</returns>
		/// <param name="sk">Sk.</param>
		ConditionConfigure pickUp(RtSkData sk) {

			ConditionConfigure ConCfg = null;

			//获取激活的判定规则ID列表
			int[] IncideCon = sk.skillCfg.Incite;
			if(IncideCon != null && IncideCon.Length > 0) {

				int len = IncideCon.Length;
				for(int i = 0; i < len; ++ i) {
					//获取激活的判定规则ID
					int CondiId = IncideCon[i];
					if(CondiId > 0) {
						ConCfg = ConModel.get(CondiId);
						if(ConCfg.ConditionType == SkConditionType.TimeOut) {
							break;
						}
					}
				}
			}

			return ConCfg;
		}

		public void EnterIncite (RtSkData sk, EffectConfigData efCfg, ServerNPC caster, IEnumerable<ServerNPC> targets) {
			bool canEnter = CheckIncite(sk, efCfg);
			if(canEnter) {
				bool injured = CheckBeHead(sk);
				if(injured) {
					float Delay = 0.6F;
					///
					/// 检测伤害类的还需要至少延迟了两个FixedUpdate，因为承受伤害的逻辑，还没开始执行
					/// 所以，目标血量的判定还不能开始
					///
					DelayedSkData data = new DelayedSkData() {
						caster = caster,
						chosen = targets,
						rtsk   = sk,
					};
					SkAsyncRunner.AysncRun(EnterIncite, Delay, data);
				} else {
					EnterIncite(sk, caster, targets);
				}
			}
		}

		/// <summary>
		/// 倒计时调用的检查逻辑
		/// </summary>
		/// <param name="fakeSk">Fake sk.</param>
		public void EnterIncite (RtFakeSkData fakeSk) {
			ConditionConfigure timeOut = pickUp(fakeSk);
			Utils.Assert(timeOut == null, "Time out must exist.");
			float max = timeOut.Param1 * Consts.OneThousand;
			//超时
			if(fakeSk.aliveDur >= max) {
				ServerLifeNpc life = WarServerManager.Instance.npcMgr.GetNPCByUniqueID(fakeSk.lifeNpcId) as ServerLifeNpc;
				life.runSkMd.switchToSkill(fakeSk.pos, timeOut.TargetSkID, false);
			}
		}

		#region 真正的检测逻辑

		void EnterIncite(DelayedSkData delayed) {
			EnterIncite(delayed.rtsk, delayed.caster, delayed.chosen);
		}

		void EnterIncite (RtSkData sk, ServerNPC caster, IEnumerable<ServerNPC> targets) {
			ConditionConfigure ConCfg = null;
			//获取激活的判定规则ID列表
			int[] IncideCon = sk.skillCfg.Incite;
			if(IncideCon != null && IncideCon.Length > 0) {

				int len = IncideCon.Length;
				for(int i = 0; i < len; ++ i) {
					//获取激活的判定规则ID
					int CondiId = IncideCon[i];
					if(CondiId > 0) {
						ConCfg = ConModel.get(CondiId);

						Utils.Assert(ConCfg == null, "Can't find Condition Configure. Condition ID = " + ConCfg);
						//判定器--- 如果成功就跳出
						ICondition decider = Mgr.getImplement(ConCfg.ConditionType);
						bool suc = decider.check(sk, ConCfg, caster, targets);
						if(suc) {
							ServerLifeNpc life = caster as ServerLifeNpc;
							bool isReset = ConCfg.ConditionClass == SkConditionClass.ResetSkill;
							life.runSkMd.switchToSkill(sk.pos, ConCfg.TargetSkID, isReset);
							break;
						}
					}
				}
			}
		}
		#endregion

	}

}
