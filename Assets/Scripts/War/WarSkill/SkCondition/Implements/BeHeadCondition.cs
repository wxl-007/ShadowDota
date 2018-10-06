using System;
using AW.Data;
using AW.Framework;
using System.Collections.Generic;

namespace AW.War { 

	/// <summary>
	/// 斩首-- 
	/// </summary>
	[Condition( Con = SkConditionType.BeHead)]
	public class BeHeadCondition : ICondition {

		#region ICondition implementation
		/// <summary>
		/// 检测是否符合逻辑
		/// </summary>
		/// <param name="sk">技能，忽略</param>
		/// <param name="caster">施法者，忽略</param>
		/// <param name="targets">目标者们</param>
		public bool check (RtSkData sk, ConditionConfigure cfg, ServerNPC caster, IEnumerable<ServerNPC> targets) {

			#if DEBUG
			Utils.Assert(cfg == null,     "ConditionConfigure is null in BeHeadCondition.");
			Utils.Assert(targets == null, "Targets are null in BeHeadCondition.");
			#endif

			bool Condi = false;

			///
			/// 1. 判定是否符合关心的BeHead类型
			///
			var self = this.GetType();
			var classAttribute = (ConditionAttribute)Attribute.GetCustomAttribute(self, typeof(ConditionAttribute));
			if(cfg.ConditionType == classAttribute.Con) {

				//血线
				int hpLine = cfg.Param1;
				//满足的个数
				int Count  = cfg.Param2;

				int curCnt = 0;

				foreach(ServerNPC npc in targets) {
					if(npc.data.rtData.curHp <= hpLine) {
						curCnt ++;
					}
				}
				///2. 判定血量
				if(curCnt >= Count) Condi = true;

				///3.判定概率
				if(Condi) {
					Condi = PseudoRandom.getInstance().happen(cfg.Prop);
				}

			}

			return Condi;
		}


		#endregion
	}

	/// <summary>
	/// 斩首-- 重置技能 , 检测逻辑一模一样
	/// </summary>
	[Condition(Con = SkConditionType.BeHeadReset)]
	public class BeHeadResetCondition : BeHeadCondition {

	}

}
