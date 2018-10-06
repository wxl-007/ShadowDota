using System;
using AW.Data;
using AW.Framework;
using System.Collections.Generic;

namespace AW.War {

	public class Operator {
		#region 免伤的实现
		private const int FACTOR = 30;
		///
		///〖物理免伤〗_自  =〖物理防御〗_自/(〖物理防御〗_自+F(lv) ) -〖 物理穿透〗_敌
		///
		///F(lv) = 英雄系数 * ( lv + 30 )

		public float Physical_Avoid (NPCData self, NPCData enemy) {
			#if DEBUG
			Utils.Assert(self == null, "Src npc  is null when doing Physical Avoid.");
			Utils.Assert(enemy == null, "Enemy npc is null when doing Physical Avoid.");
			#endif

			float bPhysicalArmor = self.rtData.armorclass * 1.0F;
			float flv = self.rtData.factor * (self.rtData.lv + FACTOR);
			float ratio = bPhysicalArmor / (bPhysicalArmor + flv) - self.rtData.armorpenetration + enemy.rtData.attackdamagereduction;

			ratio =  ratio <= 0f ? 0f : ratio ;

			return ratio;
		}
		///
		///〖法术免伤〗_自  =〖法术防御〗_自/(〖法术防御〗_自+F(lv)) - 〖法术穿透〗_敌
		/// 
		public float Magical_Avoid (NPCData self, NPCData enemy) {
			#if DEBUG
			Utils.Assert(self == null, "Src npc  is null when doing Physical Avoid.");
			Utils.Assert(enemy == null, "Enemy npc is null when doing Physical Avoid.");
			#endif

			float bMagicalArmor = self.rtData.spellresistance * 1.0F;
			float flv = self.rtData.factor * (self.rtData.lv + FACTOR);
			float ratio = bMagicalArmor / (bMagicalArmor + flv) - self.rtData.spellpenetration + enemy.rtData.spelldamagereduction;

			ratio =  ratio <= 0f ? 0f : ratio ;

			return ratio;
		}

		#endregion

		#region 暴击的实现
		private const float CRITICAL_RATIO = 1.0F;
		public float CriticalRatio (NPCData self, NPCData enemy, EffectConfigData cfg) {
			//TODO: 断言
			#if DEBUG
			Utils.Assert(self == null, "Src npc  is null when doing Critical Ratio.");
			Utils.Assert(enemy == null, "Enemy npc is null when doing Critical Ratio.");
			Utils.Assert(cfg == null, "EffectConfigData is null when doing Critial Ratio.");
			#endif

			float ratio = self.rtData.crit - enemy.rtData.critresistance;
			ratio = ratio <= 0f ? 0f : ratio;

			if(cfg != null && cfg.EffectType == EffectOp.Injury) {
				ratio = ratio + cfg.Param5 * Consts.OneThousand;
			}

			return ratio;
		}

		///	暴击伤害加成 = 150% +人物暴击伤害加成 + 技能暴击加成
		public float AdditionRatio (NPCData self, EffectConfigData cfg) {
			//TODO: 断言
			#if DEBUG
			Utils.Assert(self == null, "Src npc is null when doing Critial Addition Ratio.");
			Utils.Assert(cfg == null, "EffectConfigData is null when doing Critial Addition Ratio.");
			#endif

			float ratio = CRITICAL_RATIO + self.rtData.additionalcrit;
			if(cfg != null && cfg.EffectType == EffectOp.Injury) {
				ratio += cfg.Param6 * Consts.OneThousand;
			}

			return ratio;
		}
		#endregion

	}

	public class OperatorMgr : IocMgr {
		private Dictionary<EffectOp, Type> IOpType = null;

		/// <summary>
		/// 把所有的创建出来的实例都缓存起来
		/// </summary>
		private Dictionary<EffectOp, Object> ImpleOp = null;

		private OperatorMgr() : base() {
			IOpType = new Dictionary<EffectOp, Type>();
			ImpleOp = new Dictionary<EffectOp, Object>();

			ScanOperatorClasses(IOpType, typeof(Operator));
		}

		public static OperatorMgr instance {
			get { return GenericSingleton<OperatorMgr>.Instance; }
		}

		public T getImplement<T>(EffectOp op) {
			Object imp = null;
			Type type = null;

			if(ImpleOp.TryGetValue(op, out imp)) {
				return (T)imp;
			} else {
				if(IOpType.TryGetValue(op, out type)) {
					imp = Activator.CreateInstance(type, true);
					ImpleOp[op] = imp;
					return (T)imp;
				} else {
					ConsoleEx.DebugLog("[OperatorMgr] Op = " + op.ToString() + ". isn't finished yet.");
					return default(T);
				}
			}
		}

	}

}
