using AW.Data;

namespace AW.War {
	/// 
	/// 吸血的规则
	/// 
	public interface ISuckBloodFormula {

		/// <summary>
		/// 吸血量=〖物理伤害〗×物理吸血+〖法术伤害〗×法术吸血
		/// </summary>
		float Suck(NPCData npc, float PhysicalHurt, float MagicalHurt);
	}
}


