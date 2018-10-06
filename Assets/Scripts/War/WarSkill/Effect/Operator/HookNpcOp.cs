using System;
using AW.Data;
using AW.Framework;

namespace AW.War {
	/// <summary>
	/// 钩子NPC的伤害和子弹型NPC的伤害是一模一样的
	/// </summary>
	[Effect(OP = EffectOp.HookNpc)]
	public class HookNpcOp : Operator {
		#region 攻击力的实现

		public float Physical_Hit (NPCData self, EffectConfigData cfg) {
			#if DEBUG
			Utils.Assert(self == null, "npc is null.");
			Utils.Assert(cfg == null, "EffectConfigData is null.");
			#endif

			float bPhyHit = self.rtData.attackpower * cfg.Param6 * Consts.OneThousand;

			return bPhyHit;
		}

		public float Magical_Hit (NPCData self, EffectConfigData cfg) {
			#if DEBUG
			Utils.Assert(self == null, "npc is null.");
			Utils.Assert(cfg == null, "EffectConfigData is null.");
			#endif

			float bMagicalHit = self.rtData.spellpower * cfg.Param7 * Consts.OneThousand;

			return bMagicalHit;
		}

		#endregion

		public Dmg toTargetDmg (NPCData self, NPCData target, EffectConfigData cfg) {
			///-------- 物理伤害强度------
			///先计算自己的物理伤害，
			float bphyselfhit = Physical_Hit(self, cfg);

			///-------- 魔法伤害强度 -------
			/// 先计算自己的魔法伤害，
			float bMagselfhit = Magical_Hit(self, cfg);

			///-------- NPC的总攻击强度，是cfg.EffectClass 类型-------
			float hit = bphyselfhit + bMagselfhit;

			//---------- 开始计算伤害 ---------
			float dmg = 0f;
			switch(cfg.EffectClass) {
			case SkillTypeClass.Holly:
				//神圣伤害不减少伤害
				dmg = hit;
				break;
			case SkillTypeClass.Magical:
				float magenemyavoid = Magical_Avoid(target, self);
				dmg = hit * (1.0f - magenemyavoid);
				break;
			case SkillTypeClass.Physical:
				float avdenemyratio = Physical_Avoid(target, self);
				dmg = hit * (1.0f - avdenemyratio);
				break;
			}

			/// --------- 暴击伤害 ----------
			/// 
			float critialRatio = CriticalRatio(self, target, cfg);
			bool isCri = PseudoRandom.getInstance().happen(critialRatio);
			if(isCri) {
				float addtion = AdditionRatio(self, cfg);
				dmg = dmg * addtion;
			}

			Dmg final = new Dmg() {
				dmgValue   = dmg,
				dmgType    = cfg.EffectClass,
				isCritical = isCri,
				hitCls     = HurtHitClass.None,
			};

			return final;
		}
	}
}
