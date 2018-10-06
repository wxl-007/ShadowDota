using System;
using System.Collections.Generic;
using AW.Data;
using AW.Framework;

namespace AW.War {

	[Condition(Con = SkConditionType.Pro)]
	public class PropCondition : ICondition {
		#region ICondition implementation
		/// <summary>
		/// 检测是否符合逻辑
		/// </summary>
		/// <param name="sk">技能忽略</param>
		/// <param name="cfg">条件配置技能忽略</param>
		/// <param name="caster">施法者忽略</param>
		/// <param name="targets">目标者们忽略</param>
		public bool check (RtSkData sk, ConditionConfigure cfg, ServerNPC caster, IEnumerable<ServerNPC> targets) {
			#if DEBUG
			Utils.Assert(cfg == null, "ConditionConfigure is null in PropCondition.");
			#endif

			///1.判定概率
			bool Condi = PseudoRandom.getInstance().happen(cfg.Prop);

			return Condi;
		}
		#endregion
	
	}
}
