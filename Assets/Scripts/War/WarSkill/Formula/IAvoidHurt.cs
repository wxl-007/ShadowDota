using AW.Data;

namespace AW.War {

	/// 
	/// 最最核心的免伤计算规则
	/// 
	public interface IAvoidHurt {
		/// <summary>
		/// 物理免伤百分比
		/// </summary>
		/// <returns>The avoid.</returns>
		/// <param name="npc">Npc.</param>
		float Physical_Avoid(NPCData self, NPCData enemy);

		/// <summary>
		/// 魔法免伤百分比
		/// </summary>
		/// <returns>The avoid.</returns>
		/// <param name="npc">Npc.</param>
		float Magical_Avoid(NPCData self, NPCData enemy);
	}
}

