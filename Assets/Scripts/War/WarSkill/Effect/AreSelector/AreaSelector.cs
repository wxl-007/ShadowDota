using System;
using System.Collections.Generic;
using AW.Data;
using System.Linq;

namespace AW.War {

	/// <summary>
	/// Effect的区域和阵营的选择器
	/// </summary>

	public class EffectAreaSelector {
		private WarServerNpcMgr npcMgr;
		//缓存
		private SelfFrontFanSelector fan;
		private SelfFrontRectangleSelector rectangle;
		private TargetRadiusSelector radius;
		private SelfAndTargetSelector both;

		public EffectAreaSelector(WarServerNpcMgr NpcMgr) {
			npcMgr = NpcMgr;
		}

		public List<ServerNPC> SelectArea (ServerNPC caster, IEnumerable<ServerNPC> targets, EffectConfigData efCfg) {

			///1. 判定EffectTarget(范围）, 不区分敌友
			IEffectAreaSelector areaSelector = null;
			List<ServerNPC> list = null;
			switch(efCfg.EffectTarget) {
			case EffectTargetClass.Self_Front_Fan:
				areaSelector = fan ?? (fan = new SelfFrontFanSelector());
				break;
			case EffectTargetClass.Self_Front_Rectangle:
				areaSelector = rectangle ?? (rectangle = new SelfFrontRectangleSelector());
				break;
			case EffectTargetClass.SkillTarget_Radius:
				areaSelector = radius ?? (radius = new TargetRadiusSelector());
				list = targets.ToList();
				break;
			case EffectTargetClass.SkillTarget:
				IEnumerable<ServerNPC> itor = SelectCamp(caster, targets, efCfg);
				list = itor.ToList();
				break;
			case EffectTargetClass.Self_And_Target:
				areaSelector = both ?? (both = new SelfAndTargetSelector());
				break;
			}
			///
			///安装Camp和区域选择目标
			List<ServerNPC> reTarget = areaSelector != null ? areaSelector.Select(caster, list, efCfg, npcMgr) : list;

			//是否要过滤掉自己
			bool ck = efCfg.Flags.check(EffectFlag.Forbid_Self);
			if(ck) {
				int fCount = reTarget.Count;
				if(fCount > 0) {
					int found = -1;
					for(int i = 0; i < fCount; ++ i) {
						if(reTarget[i].UniqueID == caster.UniqueID) {
							found = i;
							break;
						}
					}

					if(found >= 0) {
						reTarget.RemoveAt(found);
					}
				}
			}

			return reTarget;
		}

		//挑出阵营,但是不应该把施法者过滤掉
		//后续如果有要求，才会过滤施法者
		IEnumerable<ServerNPC> SelectCamp(ServerNPC caster, IEnumerable<ServerNPC> targets, EffectConfigData efCfg) {
			CAMP camp = efCfg.Flags.switchTo(caster.Camp);
			IEnumerable<ServerNPC> itor = targets.Where(n => camp.check(n.Camp) || caster.UniqueID == n.UniqueID);
			return itor;
		}

	}
}

