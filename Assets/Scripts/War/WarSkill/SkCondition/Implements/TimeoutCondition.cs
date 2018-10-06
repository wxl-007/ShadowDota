using System;
using System.Collections.Generic;
using AW.Data;
using AW.Framework;

namespace AW.War {
	[Condition(Con = SkConditionType.TimeOut)]
	public class TimeoutCondition : ICondition {
		#region ICondition implementation
		/// <summary>
		/// 检测是否符合逻辑
		/// </summary>
		/// <param name="sk">技能</param>
		/// <param name="cfg">条件配置技能</param>
		/// <param name="caster">施法者忽略</param>
		/// <param name="targets">目标者们忽略</param>
		public bool check (RtSkData sk, ConditionConfigure cfg, ServerNPC caster, IEnumerable<ServerNPC> targets) {
			#if DEBUG
			Utils.Assert(sk == null, "Skill is null in TimeoutCondition.");
			Utils.Assert(cfg == null, "ConditionConfigure is null in TimeoutCondition.");
			#endif

			bool Condi = false;

			///
			/// 1. 判定是否符合关心的BeHead类型
			///
			var self = this.GetType();
			var classAttribute = (ConditionAttribute)Attribute.GetCustomAttribute(self, typeof(ConditionAttribute));
			if(cfg.ConditionType == classAttribute.Con) {
				RtFakeSkData fakeSk = sk as RtFakeSkData;
				if(fakeSk != null) {
					float TimeOut = cfg.Param1 * Consts.OneThousand;
					///
					///2.判定是否超时
					///
					Condi = fakeSk.aliveDur >= TimeOut;

					///3.判定概率
					if(Condi) Condi = PseudoRandom.getInstance().happen(cfg.Prop);
				}
			}

			return Condi;
		}

		#endregion


	}
}
