using System;
using AW.Data;
using AW.Framework;
using System.Collections.Generic;

namespace AW.War {
	/// <summary>
	/// 血量的百分比
	/// </summary>
	[Condition(Con = SkConditionType.BeHead2)]
	public class BeHead2Condition : ICondition {
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
				float hpLineFactor = cfg.Param1 * Consts.OneThousand;
				//满足的个数
				int Count  = cfg.Param2;

				int curCnt = 0;

				foreach(ServerNPC npc in targets) {
					float hpLine = (int)hpLineFactor * npc.data.rtData.totalHp;
					if(npc.data.rtData.curHp <= hpLine) {
						curCnt ++;
					}
				}

				if(curCnt >= Count) Condi = true;

				//判定概率
				if(Condi) {
					Condi = PseudoRandom.getInstance().happen(cfg.Prop);
				}
			}

			return Condi;
		}
		#endregion
	}

	/// <summary>
	/// 血量的百分比
	/// </summary>
	[Condition(Con = SkConditionType.BeHead2Reset)]
	public class BeHead2RestCondition : BeHead2Condition {

	}
}
