using System;
using AW.Message;
using AW.Data;
using System.Collections.Generic;

namespace AW.War {
	[Effect(OP = EffectOp.Treat)]
	public class SufferTreatEffect : ISufferEffect {

		private Treat handled;
		public Treat getHandled {
			get {
				return handled;
			}
		}

		#region ISufferEffect implementation

		public void Suffer (ServerNPC caster, ServerNPC sufferer, SelfDescribed des, WarServerNpcMgr npcMgr) {
			//拿到算子
			InjuryOp CoreS = OperatorMgr.instance.getImplement<InjuryOp>(EffectOp.Injury);

			//TODO: find buff
			EffectConfigData[] help = null;

			handled = new Treat {
				treatValue = des.targetEnd.param1,
				treatType = (SkillTypeClass)des.targetEnd.param3,
				isCritical = des.targetEnd.param2 == 1,
			};

			SufTreat res = CoreS.toSufferTreat(ref handled, sufferer.data, caster.data, help);
			if(res.isCureProtectioin) {
				//TODO : add Info here.
			}

			///
			/// 最终结果的计算
			/// 
			NPCRuntimeData rtdata = sufferer.data.rtData;
			rtdata.curHp += (int)handled.treatValue;
			rtdata.curHp = rtdata.curHp > rtdata.totalHp ? rtdata.totalHp : rtdata.curHp;
		}

		#endregion

	}


}
