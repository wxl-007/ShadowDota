using System;
using AW.Data;
using System.Collections.Generic;

namespace AW.War {

	/// <summary>
	/// 效果的范围选择器, 同时可以选择阵营
	/// 返回结果不为空
	/// </summary>
	public interface IEffectAreaSelector {
		List<ServerNPC> Select(ServerNPC caster, List<ServerNPC> targets, EffectConfigData efCfg, WarServerNpcMgr npcMgr);
	}
}

