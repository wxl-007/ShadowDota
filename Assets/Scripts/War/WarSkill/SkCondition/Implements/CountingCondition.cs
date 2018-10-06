using System;
using AW.Data;
using AW.Framework;
using System.Collections.Generic;

namespace AW.War {
	[Condition(Con = SkConditionType.Counting)]
	public class CountingCondition : ICondition {
		#region ICondition implementation
		/// <summary>
		/// 检测是否符合逻辑
		/// </summary>
		/// <param name="sk">技能</param>
		/// <param name="cfg">条件配置技能, 忽略</param>
		/// <param name="caster">施法者, 忽略</param>
		/// <param name="targets">目标者们, 忽略</param>
		public bool check (RtSkData sk, ConditionConfigure cfg, ServerNPC caster, IEnumerable<ServerNPC> targets) {
			#if DEBUG
			Utils.Assert(sk == null, "Skill is null in CountingCondition.");
			Utils.Assert(cfg == null, "ConditionConfigure is null in CountingCondition.");
			#endif

			bool Condi = false;

			///
			/// 1. 判定是否符合关心的Counting类型
			///
			var self = this.GetType();
			var classAttribute = (ConditionAttribute)Attribute.GetCustomAttribute(self, typeof(ConditionAttribute));
			if(cfg.ConditionType == classAttribute.Con) {

				///
				/// 2. 只有RtFakeSkData才有计算功能
				///
				RtFakeSkData fakeSk = sk as RtFakeSkData;
				if(fakeSk != null) {

					///
					/// 3. 判定计数
					///
					if(fakeSk.curCounting >= cfg.Param1) {
						Condi = true;
					}

					///4.判定概率
					if(Condi) {
						Condi = PseudoRandom.getInstance().happen(cfg.Prop);
					}
				}

			}

			return Condi;
		}

		#endregion


	}

}
