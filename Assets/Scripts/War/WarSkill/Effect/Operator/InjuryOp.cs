using System;
using AW.Data;
using AW.Framework;

namespace AW.War {
	/// <summary>
	/// 伤害的结构
	/// </summary>
	public struct Dmg {
		//伤害值
		public FloatFog dmgValue;
		//伤害类型
		public SkillTypeClass dmgType;
		//是否暴击
		public bool isCritical;
		//避免死亡，不计算减伤，不计算护盾
		public HurtHitClass hitCls;
	}

	/// <summary>
	/// 治疗的结构
	/// </summary>
	public struct Treat {
		//血量值
		public FloatFog treatValue;
		//治疗类型
		public SkillTypeClass treatType;
		//是否暴击
		public bool isCritical;
	}

	/// <summary>
	/// 受到伤害的结构
	/// </summary>
	public struct Suf {
		//最终的伤害值
		public Dmg sufValue;
		//反弹的伤害值
		public Dmg? rebValue;
		//吸血的伤害值
		public Dmg? bdyValue;
		//对护盾的伤害值
		public Dmg? protectVal;
		//抵消伤害类型
		public ResistanteClass resDmgType;
	}

	/// <summary>
	/// 受到治疗时的结构
	/// </summary>
	public struct SufTreat {
		//血量值
		public FloatFog treatValue;
		//治疗类型
		public SkillTypeClass treatType;
		//是否暴击
		public bool isCritical;
		//是否治疗护盾
		public bool isCureProtectioin;
	}


	/// 
	/// Effect为Injury和Treat的核心出入口
	/// 
	[Effect(OP = EffectOp.Injury)]
	public class InjuryOp : Operator, IHitFormula, IAvoidHurt, ICriticalHit, IHurtFormula, ITreatFormula {

		private InjuryOp() { }

		#region 攻击力的实现

		public float Physical_Hit (NPCData self, EffectConfigData cfg) {
			#if DEBUG
			Utils.Assert(self == null, "npc is null.");
			Utils.Assert(cfg == null, "EffectConfigData is null.");
			#endif

			float bPhyHit = self.rtData.attackpower * cfg.Param1 * Consts.OneThousand;

			return bPhyHit;
		}

		public float Magical_Hit (NPCData self, EffectConfigData cfg) {
			#if DEBUG
			Utils.Assert(self == null, "npc is null.");
			Utils.Assert(cfg == null, "EffectConfigData is null.");
			#endif

			float bMagicalHit = self.rtData.spellpower * cfg.Param2 * Consts.OneThousand;

			return bMagicalHit;
		}

		#endregion

		#region 技能伤害力的实现,

		/// 
		/// 带AI的伤害技能的实现
		///      但是要不要减伤
		/// 
		/// 	〖物理伤害〗_敌=〖物理攻击力〗_敌×(1-〖物理免伤〗_自  )
		/// 	〖法术伤害〗_敌=〖法术攻击力〗_敌×(1-〖法术免伤〗_自  )
		/// 〖伤害〗_敌=〖物理伤害〗_敌+〖法术伤害〗_敌
		/// 
		/// Self 对 target的技能伤害
		/// 只考虑了输出值，并不考虑后续的反弹，护盾等情况
		/// 
		public Dmg toTargetDmg (NPCData self, NPCData target, EffectConfigData cfg) {
			///-------- 物理伤害强度------
			///先计算自己的物理伤害，
			float bphyselfhit = Physical_Hit(self, cfg);

			///-------- 魔法伤害强度 -------
			/// 先计算自己的魔法伤害，
			float bMagselfhit = Magical_Hit(self, cfg);

			///-------- NPC的总攻击强度，是cfg.EffectClass 类型-------
			float hit = bphyselfhit + bMagselfhit + cfg.Param3;

			HurtHitClass hitType = (HurtHitClass)Enum.ToObject(typeof(HurtHitClass), cfg.Param7);

			///
			/// --------- 检测 不计算减伤 -----------
			///
			bool forbidArmer = hitType.check(HurtHitClass.Forbid_Armer);

			float dmg = 0f;

			if(forbidArmer) {
				dmg = hit;
			} else {
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
			}

			dmg = dmg * (1 + cfg.Param4 * Consts.OneThousand);

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
				hitCls     = hitType,
			};

			return final;
		}

		#endregion

		#region 最终挨打的实现

		public Suf toSuffer (ref Dmg damage, NPCData suffer, NPCData from, EffectConfigData[] resis) {
			#if DEBUG
			Utils.Assert(suffer == null, "NPCData suffer can't be null.");
			Utils.Assert(suffer == null, "NPCData attacker can't be null.");
			#endif

			///
			/// --------------- Suffer 获取Dot/Hot -------------
			///

			bool forbid_die = damage.hitCls.check(HurtHitClass.Forbid_Die);
			if(forbid_die) {
				if(damage.dmgValue >= suffer.rtData.curHp) {
					damage.dmgValue = suffer.rtData.curHp - 1;
				} 
			}

			//TODO : 错误逻辑，仅仅是测试使用
			bool forbid_shield = damage.hitCls.check(HurtHitClass.Forbid_ProtectionShield);
			if(!forbid_shield) { //不无视护盾

			}

			//要计算反弹吗？
			bool rebound = false;
			switch(damage.dmgType) {
			case SkillTypeClass.Rebound_Hol:
			case SkillTypeClass.Rebound_Mag:
			case SkillTypeClass.Rebound_Phy:
				rebound = false;
				break;

			case SkillTypeClass.Holly:
			case SkillTypeClass.Magical:
			case SkillTypeClass.Physical:
				rebound = true;
				break;
			}

			if(rebound) { //计算反弹

			}


			///
			/// ---- 计算吸血 ------
			///

			Suf suf = new Suf() {
				sufValue = damage,
				rebValue = null,
				bdyValue = null,
				protectVal  = null,
				resDmgType  = ResistanteClass.None,
			};
			return suf;
		}

		#endregion

		#region 治疗的实现

		/// 治疗值 = (p(1)*物理攻击系数+p(2)*法术攻击系数) /1000 + p(3) +p(4) * TargetAttr(MaxHP) / 1000
		///
		/// 疗效 = TargetAttr(BeHeal)
		///	治疗值 = 治疗值 * (1+疗效) * (1+p(5) / 1000)
		/// 
		///	是否可暴击按照p(6)处理
		///	技能暴击率 = 暴击率 + p(7) / 1000
		///	技能治疗暴击提升比=治疗暴击提升比+P(8) / 1000
		///
		///	if 暴击 then
		///		真实治疗值 = 治疗值 * 技能治疗暴击提升比
		///		end if
		public Treat toTargetTreat (NPCData self, NPCData target, EffectConfigData cfg){
			///-------- 物理伤害强度------
			///先计算自己的物理伤害，
			float bphyselfhit = Physical_Hit(self, cfg);

			///-------- 魔法伤害强度 -------
			/// 先计算自己的魔法伤害，
			float bMagselfhit = Magical_Hit(self, cfg);

			///-------- NPC的总攻击强度，是cfg.EffectClass 类型-------
			float hit = bphyselfhit + bMagselfhit + cfg.Param3 + cfg.Param4 * self.rtData.totalHp * Consts.OneThousand;

			///
			/// ------------ 分为不可暴击 和 不计算疗效 ------------
			/// 
			TreatClass treatCls = (TreatClass)Enum.ToObject(typeof(TreatClass), cfg.Param6);

			bool forbid_critical = treatCls == TreatClass.Forbid_Critical;
			bool forbid_beheal   = treatCls == TreatClass.Forbid_BeHeal;

			/// 治疗值
			float treat = hit * (1 + cfg.Param7 * Consts.OneThousand);
			if(!forbid_beheal) treat = treat * (1 + self.rtData.BeHeal);

			/// --------- 暴击伤害 ----------
			/// 
			bool isCri = false;
			if(!forbid_critical) {
				float critialRatio = CriticalRatio(self, target, cfg);
				isCri = PseudoRandom.getInstance().happen(critialRatio);
				if(isCri) {
					float addtion = AdditionRatio(self, cfg);
					treat = treat * addtion;
				}
			} 

			Treat val = new Treat() {
				treatValue   = (int)treat,
				treatType    = cfg.EffectClass,
				isCritical = isCri,
			};

			return val;
		}
		#endregion

		#region 治疗加在BPC身上的时候

		public SufTreat toSufferTreat (ref Treat treat, NPCData suffer, NPCData from, EffectConfigData[] resis) {
			#if DEBUG
			Utils.Assert(suffer == null, "suffer argument can't be null when suffering treatment.");
			Utils.Assert(from == null, "from argument can't be null when suffering treatment.");
			#endif

			SufTreat res = new SufTreat() {
				treatType = treat.treatType,
				treatValue= treat.treatValue,
				isCritical= treat.isCritical,
			};

			return res;
		}

		#endregion
	}
}
