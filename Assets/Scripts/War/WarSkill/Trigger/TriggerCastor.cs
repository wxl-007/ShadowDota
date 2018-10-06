using System;
using System.Collections.Generic;
using AW.Data;
using AW.Message;
using AW.Framework;
using System.Linq;

namespace AW.War {
	/// <summary>
	/// 触发器用来释放技能
	/// </summary>
	public class TriggerCastor {
		/// <summary>
		/// 消息容器
		/// </summary>
		private List<MsgParam> container;
		/// <summary>
		/// 效果释放器
		/// </summary>
		private EffectCastor EfCastor;

		private WarServerNpcMgr msgMgr = null;

		private TriggerCastor() {
			container = new List<MsgParam>();
		}

		public static TriggerCastor instance {
			get { return GenericSingleton<TriggerCastor>.Instance; }
		}

		public void init(WarServerNpcMgr npcMgr) {
			msgMgr = npcMgr;
			EfCastor = new EffectCastor(msgMgr);
		}

		public void cast(ServerNPC castor, IEnumerable<ServerNPC> targets, TriggerConfigData triCfg) {
			#if DEBUG
			Utils.Assert(castor == null, "TriggerCastor can't cask if castor is null.");
			Utils.Assert(targets == null, "TriggerCastor can't cask if target is null.");
			Utils.Assert(triCfg == null, "TriggerCastor can't cask if TriggerConfigData is null.");
			#endif

			RtSkData skill = new RtSkData(triCfg.SkillID, -1);
			targets = filtor(targets, triCfg);
			container.Clear();
			EfCastor.Cast(castor, targets, skill, container, false);
			dispatchMsg(container, skill);
		}

		/// <summary>
		/// 筛选，将死亡的，概率上不通过的都去除
		/// </summary>
		/// <param name="targets">Targets.</param>
		/// <param name="triCfg">Tri cfg.</param>
		IEnumerable<ServerNPC> filtor(IEnumerable<ServerNPC> targets, TriggerConfigData triCfg) {
			List<ServerNPC> valid = new List<ServerNPC>();
			foreach(ServerNPC bnpc in targets) {
				if(bnpc.data.rtData.curHp > 0) {
					bool happend = PseudoRandom.getInstance().happen(triCfg.Prob);
					if(happend) {
						valid.Add(bnpc);
					}
				}
			}
			return valid.AsEnumerable<ServerNPC>();
		}

		/// <summary>
		/// 派发出去消息
		/// </summary>
		/// <param name="outMsg">Out message.</param>
		void dispatchMsg(List<MsgParam> outMsg, RtSkData skill) {

			if(outMsg != null && outMsg.Count > 0) {
				int count = outMsg.Count;

				for(int i = 0; i < count; ++ i) {
					MsgParam msg = outMsg[i];
					WarAnimParam warMsg = msg as WarAnimParam;
					warMsg.cmdType = WarMsg_Type.UseTrigger;
					warMsg.SkillId = skill.Num;

					if(warMsg != null && warMsg.described != null ) {
						SelfDescribed des = warMsg.described;
						#if DEBUG
						ConsoleEx.DebugLog("Trigger Msg is going out : " + fastJSON.JSON.Instance.ToJSON(des), ConsoleEx.YELLOW);
						#endif
						msgMgr.SendMessageAsync(des.src, des.target, warMsg);
					}
				}

			}
		}
	}
}
