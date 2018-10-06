using AW.Data;

namespace AW.War {

	/// 
	/// 最最核心的攻击力的计算公式
	/// 
	/// 攻击力的计算分为两种：
	/// 1. 有AI的，NPC的攻击力（比如hero，召唤的地狱火）
	/// 2. 没有AI的攻击力（比如射出去的弓箭）
	/// 
	/// 但是所有创建出来的NPC，都会出现在NPC表里。
	/// 也就是说所有的技能攻击力运算都是符合一个流程的。
	/// 什么流程：1. 攻击力
	///          2. 减伤
	///          3. 暴击
	/// 
	public interface IHitFormula {

		/// <summary>
		/// 物理攻击力
		/// </summary>
		/// <returns>The hit.</returns>
		/// <param name="src">Source.</param>
		float Physical_Hit(NPCData npc, EffectConfigData cfg);

		/// <summary>
		/// 魔法攻击力
		/// </summary>
		/// <returns>The hit.</returns>
		/// <param name="src">Source.</param>
		float Magical_Hit(NPCData npc, EffectConfigData cfg);
	}

}

