using System;
using System.Collections.Generic;
using AW.Data;
using AW.Framework;

namespace AW.War {

	public class SelfAndTargetSelector : IEffectAreaSelector {
		#region IEffectAreaSelector implementation

		/// <summary>
		/// EffectConfigData 和 WarServerNpcMgr 为空
		/// </summary>
		/// <param name="caster">Caster.</param>
		/// <param name="targets">Targets.</param>
		/// <param name="efCfg">Ef cfg.</param>
		/// <param name="npcMgr">Npc mgr.</param>
		public List<ServerNPC> Select (ServerNPC caster, List<ServerNPC> targets, EffectConfigData efCfg, WarServerNpcMgr npcMgr) {
			#if DEBUG
			Utils.Assert(caster  == null, "Can't find target unless caster isn't null.");
			Utils.Assert(targets == null, "Can't find target unless targets isn't null.");
			#endif

			List<ServerNPC> reTarget = new List<ServerNPC>();
			reTarget.AddRange(targets);
			reTarget.Add(caster);

			return reTarget;
		}

		#endregion


	}
}
