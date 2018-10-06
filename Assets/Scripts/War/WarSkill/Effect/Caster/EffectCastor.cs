using System;
using System.Collections.Generic;
using AW.Data;
using AW.Message;
using AW.Framework;
using System.Linq;

namespace AW.War {

	/// <summary>
	/// 效果的释放器
	/// </summary>
	public class EffectCastor {
		private EffectCastMgr effMgr;
		//Effect选择器
		private EffectSelector selector;
		//激活判定器
		private ConditionCastor conDicder;

		public EffectCastor(WarServerNpcMgr NpcMgr) {
			effMgr    = EffectCastMgr.Instance;
			selector  = new EffectSelector(NpcMgr);
			conDicder = ConditionCastor.instance;
		}

		/// <summary>
		/// Cast the specified caster, chosen, rtSk and MsgContainer.
		/// 这个是给普通攻击使用的，其实就是立即有结果的模式
		/// 没有激活判定器
		/// </summary>
		/// <param name="caster">Caster.</param>
		/// <param name="chosen">Chosen.可以为空</param>
		/// <param name="rtSk">Rt sk.</param>
		/// <param name="MsgContainer">Message container.</param>
		public void Cast(ServerNPC caster, IEnumerable<ServerNPC> chosen, RtSkData rtSk, List<MsgParam> MsgContainer) {
			bool skDirectlyHurt = true;
			Cast(caster, chosen, rtSk, MsgContainer, skDirectlyHurt);
		}

		public void Cast(ServerNPC caster, IEnumerable<ServerNPC> chosen, RtSkData rtSk, List<MsgParam> MsgContainer, bool skDirecHurt) {
			#if DEBUG
			Utils.Assert(caster == null, "Effect Castor");
			Utils.Assert(rtSk == null, "Effect Castor");
			#endif
			List<EffectConfigData> effCfg = new List<EffectConfigData>();
			foreach(EffectConfigData cfg in rtSk.effectCfgDic.Values) {
				effCfg.Add(cfg);
			}

			ICastEffect[] castor = effMgr.getImplements(effCfg.ToArray(), rtSk.skillCfg);
			if(castor != null) {
				int len = castor.Length;
				for(int i = 0; i < len; ++ i) {
					IEnumerable<ServerNPC> reTarget = selector.Select(caster, chosen, effCfg[i]);
					castor[i].Cast(caster, chosen, reTarget, skDirecHurt, MsgContainer);
					effMgr.Recycle(effCfg[i], castor[i]);
				}
			}
			castor = null;
		}

		/// <summary>
		/// 这个是给技能使用的，可能不会立即有结果
		/// </summary>
		/// <param name="caster">Caster.</param>
		/// <param name="chosen">Chosen.</param>
		/// <param name="rtSk">技能</param>
		/// <param name="pos">技能的索引</param>
		/// <param name="Report">Report.</param>
		public void Cast(ServerNPC Caster, IEnumerable<ServerNPC> Chosen, RtSkData RtSk, Action<MsgParam> ReportFunc) {
			#if DEBUG
			Utils.Assert(Caster == null, "Effect Castor");
			Utils.Assert(RtSk == null, "Effect Castor");
			#endif

			List<EffectConfigData> effCfg = new List<EffectConfigData>();
			foreach(EffectConfigData cfg in RtSk.effectCfgDic.Values) {
				effCfg.Add(cfg);
			}
			//虚假的Buff桩对象
			EffectConfigData stub = ctorStubEffCfg(RtSk);
			if(stub != null) effCfg.Add(stub);

			ICastEffect[] castor = effMgr.getImplements(effCfg.ToArray(), RtSk.skillCfg);
			if(castor != null) {
				int len = castor.Length;
				for(int i = 0; i < len; ++ i) {

					if(castor[i] == null) {
						ConsoleEx.DebugLog("ICastEffect Not Implement. ID = " + effCfg[i].ID, ConsoleEx.YELLOW);
					} else {
						float Delay = RtSk.skillCfg.EffectDelayTime[i];
						if(Delay > 0) {
							DelayedSkData data = new DelayedSkData() {
								caster = Caster,
								chosen = Chosen,
								castor = castor[i],
								cfg    = effCfg[i],
								Report = ReportFunc,
								rtsk   = RtSk,
							};
							SkAsyncRunner.AysncRun(DelayCast, Delay, data);
						} else {
							DelayCast(Caster, Chosen, castor[i], effCfg[i], RtSk, ReportFunc);
						}
					}
				}
			}
			castor = null;
		}

		#region 延迟施法

		void DelayCast(DelayedSkData delaySkData) {
			bool skDirecHurt = true;

			List<MsgParam> MsgContainer = new List<MsgParam>();

			IEnumerable<ServerNPC> reTarget = selector.Select(delaySkData.caster, delaySkData.chosen, delaySkData.cfg);
			delaySkData.castor.Cast(delaySkData.caster, delaySkData.chosen, reTarget, skDirecHurt, MsgContainer);
			effMgr.Recycle(delaySkData.cfg, delaySkData.castor); 

			///
			/// ----  进入激活判定器 ----
			///
			conDicder.EnterIncite(delaySkData.rtsk, delaySkData.cfg, delaySkData.caster, delaySkData.chosen);

			//通知UI的信息
			short pos = delaySkData.rtsk.pos;
			Action<MsgParam> Report = delaySkData.Report;
			///
			///  回报数据，区分一条数据和多条数据
			///
			HowToReport(MsgContainer, pos, Report);
		}

		void DelayCast(ServerNPC caster, IEnumerable<ServerNPC> chosen, ICastEffect castor, EffectConfigData cfg, RtSkData RtSk, Action<MsgParam> Report) {
			bool skDirecHurt = true;
			List<MsgParam> MsgContainer = new List<MsgParam>();

			IEnumerable<ServerNPC> reTarget = selector.Select(caster, chosen, cfg);
			castor.Cast(caster, chosen, reTarget, skDirecHurt, MsgContainer);
			effMgr.Recycle(cfg, castor); 

			///
			/// ----  进入激活判定器 ----
			///
			conDicder.EnterIncite(RtSk, cfg, caster, chosen);

			short pos = RtSk.pos;
			///
			///  回报数据，区分一条数据和多条数据
			///
			HowToReport(MsgContainer, pos, Report);
		}


		void HowToReport(List<MsgParam> MsgContainer, short pos, Action<MsgParam> Report) {
			//一个Effect产生了多条数据
			if(MsgContainer.Count > 0) {
				int count = MsgContainer.Count;
				for(int i = 0; i < count; ++ i) {
					WarMsgParam warParam = (WarMsgParam)MsgContainer[i];
					//arg1代表第几个技能
					warParam.arg1 = pos;
					warParam.cmdType = WarMsg_Type.UseSkill;
					Report(warParam);
				}
			} 
		}

		#endregion

		#region 引导buff
		///
		/// 引导Buff是可以被打断的
		/// 创建假的Effect的配置(桩对象）
		///
		EffectConfigData ctorStubEffCfg (RtSkData RtSk) {
			EffectConfigData stub = null;
			if(RtSk.ChannelBuff != null) {

				stub = new EffectConfigData() {
					ID    = -1,
					Flags = EffectFlag.None,
					EffectClass  = SkillTypeClass.Magical,
					EffectTarget = EffectTargetClass.SkillTarget,
					EffectTargetType = TargetSubClass.AllTarget,
					EffectTargetStatusReject = NpcStatus.None,
					EffectType = EffectOp.DotHot,
					Param1     = RtSk.ChannelBuff.ID,
					Prob       = 1000,
					EffectLimit= -1,
				};
			}
			return stub;
		}

		#endregion
	}

}
