using System;
using System.Collections.Generic;
using AW.Data;
using AW.Framework;
using UVec3 = UnityEngine.Vector3;

namespace AW.War {
	/// <summary>
	/// 目标的圆形区域
	/// </summary>
	public class TargetRadiusSelector : IEffectAreaSelector {
		#region IEffectAreaSelector implementation

		/// <summary>
		/// 这个Targets不能为空
		/// </summary>
		/// <param name="caster">Caster.</param>
		/// <param name="targets">Targets.</param>
		/// <param name="efCfg">Ef cfg.</param>
		/// <param name="npcMgr">Npc mgr.</param>
		public List<ServerNPC> Select (ServerNPC caster, List<ServerNPC> targets, EffectConfigData efCfg, WarServerNpcMgr npcMgr) {
			#if DEBUG
			Utils.Assert(efCfg == null, "Can't find target unless EffectConfigData isn't null.");
			Utils.Assert(npcMgr == null, "Can't find target unless WarServerNpcMgr isn't null.");
			Utils.Assert(targets == null, "Can't find target unless targets isn't null.");
			#endif

			List<ServerNPC> reTarget = new List<ServerNPC>();
			//转换为Camp类型，还有一部分的检查不在这里
			//剔除自己的是在Factory里面（即EffectAreaSector）
			CAMP camp = efCfg.Flags.switchTo(caster.Camp);

			int count = targets.Count;
			if(count > 0) {
				for(int i = 0; i < count; ++ i) {
					UVec3 anchor = targets[i].transform.position;
					reTarget.AddRange( SelectorTools.GetNPCRadius(anchor, caster.data.configData.radius + efCfg.eParam1, camp, npcMgr) );
				}
			}

			return reTarget;
		}

		#endregion
	}

}
