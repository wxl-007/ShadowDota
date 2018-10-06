using AW.Data;
namespace AW.War {

	/// 
	/// 受到的伤害计算规则，这个是挨打人员的受伤害的结算
	/// 
	/// 可能挨打身上有护盾，有反弹
	/// 
	public interface IHurtFormula {
		/// <summary>
		/// 挨打时候
		/// </summary>
		/// <returns>The suffer.</returns>
		/// <param name="damage">这个参数会被修改.</param>
		/// <param name="suffer">挨打者.</param>
		/// <param name="from">打人者</param>
		/// <param name="resis">抵抗效果</param>
		Suf toSuffer(ref Dmg damage, NPCData suffer, NPCData from, EffectConfigData[] resis);
	}
}
