using System;
using AW.Data;
using System.Collections.Generic;

namespace AW.War {

	/// <summary>
	/// 技能激活的条件的判定。
	/// 判定链 --- 从前往后判定，如果预见条件判定成功，执行逻辑后，退出条件判定链。
	/// </summary>
	public interface ICondition {
		/// <summary>
		/// 检测是否符合逻辑
		/// </summary>
		/// <param name="sk">技能</param>
		/// <param name="sk">条件配置技能</param>
		/// <param name="caster">施法者</param>
		/// <param name="targets">目标者们</param>
		bool check(RtSkData sk, ConditionConfigure cfg, ServerNPC caster, IEnumerable<ServerNPC> targets);
	}
}
