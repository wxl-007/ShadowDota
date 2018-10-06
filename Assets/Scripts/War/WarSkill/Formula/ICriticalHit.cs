using AW.Data;

namespace AW.War {

	/// 
	/// 暴击的计算
	/// 
	public interface ICriticalHit {

		/// <summary>
		/// 基础暴击率的计算
		/// </summary>
		/// <returns>The ratio.</returns>
		/// <param name="npc">Npc.</param>
		/// 真实暴击率 = 暴击率 - 免暴率
		/// cfg是特定的Injure的技能效果能有暴击的加成
		float CriticalRatio(NPCData self, NPCData enemy, EffectConfigData cfg);

		/// 暴击伤害的加成率 暴击伤害加成=150%+暴击伤害加成
		float AdditionRatio(NPCData npc, EffectConfigData cfg);

	}
}

