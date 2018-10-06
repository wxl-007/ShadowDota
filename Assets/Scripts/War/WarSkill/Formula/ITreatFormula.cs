using System;
using AW.Data;
using AW.Framework;

namespace AW.War {
	/// <summary>
	/// 回复（治疗）
	/// </summary>
	public interface ITreatFormula {
		Treat toTargetTreat (NPCData self, NPCData target, EffectConfigData cfg);
	}
}

