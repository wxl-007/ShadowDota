using System;
using AW.Data;
using System.Collections.Generic;
using System.Linq;
using AW.Framework;
using UVec3 = UnityEngine.Vector3;

namespace AW.War {
	/// <summary>
	/// 技能效果 选择目标的接口
	/// 技能效果的初始选择的目标从技能选择器里选择出来的
	/// </summary>
	public class EffectSelector {
		private EffectAreaSelector areaSelector;
		private WarServerNpcMgr npcMgr;
		private PseudoRandom random;

		public EffectSelector(WarServerNpcMgr NpcMgr) {
			npcMgr = NpcMgr;
			areaSelector = new EffectAreaSelector(npcMgr);
			random = PseudoRandom.getInstance();
		}

		/// <summary>
		/// 选择的规则是：
		/// 1. 判定EffectTarget(范围）
		/// 2. 判定flag-剔除友军，敌军等概念, 进入战斗状态的则再最后判定(状态可能目前并不会去设定）
		/// 3. 判定EffectTargetType-剔除
		/// 4. 每个NPC的概率
		/// 5. 判定EffectTargetStatusReject-剔除
		/// 6. 选择目标的数量不能超过上限
		/// 
		/// </summary>
		/// <param name="caster">Caster.</param>
		/// <param name="targets">Targets.</param>
		/// <param name="efCfg">Ef cfg.</param>
		public IEnumerable<ServerNPC> Select(ServerNPC caster, IEnumerable<ServerNPC> targets, EffectConfigData efCfg) {
			#if DEBUG
			Utils.Assert(efCfg == null, "Effect Selector can't find target unless EffectConfigData isn't null");
			#endif

			List<ServerNPC> reTarget = null;
			IEnumerable<ServerNPC> itor = null;

			/// 1. 判定EffectTarget(范围）2. 判定flag-剔除友军，敌军等概念
			reTarget = areaSelector.SelectArea(caster, targets, efCfg);

			/// 3. 判定EffectTargetType-剔除 建筑物
			LifeNPCType lifeType = efCfg.EffectTargetType.toPositive();
			itor = reTarget.Where( n => lifeType.check(n.data.configData.type) );

			/// 4. 每个NPC的概率   5.判定EffectTargetStatusReject
			itor = itor.Where( n => {
				ServerLifeNpc lifeTar = n as ServerLifeNpc; 
				bool ok = random.happen(efCfg.Prob);
				if(lifeTar != null) {
					if(ok)
						return !lifeTar.curStatus.AnySame(efCfg.EffectTargetStatusReject);
					else 
						return ok;
				} else {
					return ok;
				}}
			);

			///6. 选择的目标数量不能超过上限
			if(efCfg.EffectLimit > 0) {
				itor = itor.Take(efCfg.EffectLimit);
			} else if(efCfg.EffectLimit == 0) {
				itor = new List<ServerNPC>().AsEnumerable<ServerNPC>();
			}

			return itor;
		}
	}
}
