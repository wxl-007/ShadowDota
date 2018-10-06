using System;
using AW.Message;
using AW.Data;
using System.Collections.Generic;

namespace AW.War {

	/// <summary>
	/// 承受Injure伤害的逻辑
	/// </summary>
	[Effect(OP = EffectOp.Injury)]
	public class SufferInjureEffect : ISufferEffect {

		//处理后的伤害
		private Dmg handled;
		public Dmg getHandled {
			get {
				return handled;
			}
		}

		//处理后的受到伤害
		private Suf suf;
		public Suf getHandledSuf {
			get { return suf; }
		}

		#region ISufferEffect implementation
		/// <summary>
		/// Suffer the specified caster, target and damage.
		/// 收到伤害的时候，没有返回值.
		/// </summary>
		/// <param name="caster">释放者</param>
		/// <param name="target">目标者</param>
		/// <param name="damage">伤害值</param>
		public void Suffer (ServerNPC caster, ServerNPC sufferer, SelfDescribed des, WarServerNpcMgr npcMgr) {
			//拿到算子
			InjuryOp sufOp = OperatorMgr.instance.getImplement<InjuryOp>(EffectOp.Injury);

			handled = new Dmg {
				dmgValue = des.targetEnd.param1,
				dmgType = (SkillTypeClass)des.targetEnd.param3,
				isCritical = des.targetEnd.param2 == 1,
				hitCls   = (HurtHitClass) Enum.ToObject(typeof(HurtHitClass), des.targetEnd.param4),
			};

			//TODO: find buff 
			EffectConfigData[] help = null;
			suf = sufOp.toSuffer(ref handled, sufferer.data, caster.data, help);
			///
			///更多的事情发生了，比如护盾，吸血，反弹
			/// 
			report(caster.UniqueID, sufferer.UniqueID, suf, npcMgr);

			///
			/// --------- 最终结算 --------
			///
			sufferer.data.rtData.curHp -= (int)handled.dmgValue;


			///
			/// --------- 通知Trigger，OnKilled ----------
			///
			toTriggerMsg(caster, sufferer, npcMgr);
		}


		void report (int CasterId, int SufferId, Suf suf, WarServerNpcMgr npcMgr) {


			//检测护盾
			if(suf.resDmgType.check(ResistanteClass.Protection)) {
				if(suf.protectVal != null) {

					//统计数据
					SelfDescribed des = new SelfDescribed() {
						src    = CasterId,
						target = SufferId, //SufferId 应该变为真实的护盾的NPC
						act    = Verb.Punch,
						srcEnd = null,
					};

					EndResult tarEnd = new EndResult();

					Dmg protectDmg = suf.protectVal.Value;
					tarEnd.param1 = (int)protectDmg.dmgValue;
					tarEnd.param2 = protectDmg.isCritical ? 1 : 0;
					tarEnd.param3 = (int)protectDmg.dmgType;
					tarEnd.param4 = (int)HurtHitClass.None;

					des.targetEnd = tarEnd;

					WarTarAnimParam param = new WarTarAnimParam() {
						OP = EffectOp.Injury,
						OringinOP = EffectOp.Injury,
						described = des,
					};

					npcMgr.SendMessageAsync(des.src, des.target, param);
				}
			}

			if(suf.resDmgType.check(ResistanteClass.Bloody) || suf.resDmgType.check(ResistanteClass.Rebound)) {

				//反弹不为空
				if(suf.rebValue != null) {
					//统计数据
					SelfDescribed des = new SelfDescribed() {
						src    = SufferId,
						target = CasterId,
						act    = Verb.Punch,
						srcEnd = null,
					};

					EndResult tarEnd = new EndResult();

					Dmg rebDmg = suf.rebValue.Value;
					tarEnd.param1 = (int)rebDmg.dmgValue;
					tarEnd.param2 = rebDmg.isCritical ? 1 : 0;
					tarEnd.param3 = (int)rebDmg.dmgType;
					tarEnd.param4 = (int)HurtHitClass.None;

					des.targetEnd = tarEnd;

					WarTarAnimParam param = new WarTarAnimParam() {
						OP = EffectOp.Injury,
						OringinOP = EffectOp.Injury,
						described = des,
					};

					npcMgr.SendMessageAsync(des.src, des.target, param);
				}

				//吸血不为空
				if(suf.bdyValue != null) {

					//统计数据
					SelfDescribed des = new SelfDescribed() {
						src    = SufferId,
						target = CasterId,
						act    = Verb.Recover,
						srcEnd = null,
					};

					EndResult tarEnd = new EndResult();

					Dmg bdyDmg = suf.bdyValue.Value;
					tarEnd.param1 = (int)bdyDmg.dmgValue;
					tarEnd.param2 = bdyDmg.isCritical ? 1 : 0;
					tarEnd.param3 = (int)bdyDmg.dmgType;
					tarEnd.param4 = (int)HurtHitClass.None;

					des.targetEnd = tarEnd;

					WarTarAnimParam param = new WarTarAnimParam() {
						OP = EffectOp.Treat,
						OringinOP = EffectOp.Treat,
						described = des,
					};

					npcMgr.SendMessageAsync(des.src, des.target, param);
				}

			}
		}

		//发送给Trigger的数据
		void toTriggerMsg(ServerNPC caster, ServerNPC sufferer, WarServerNpcMgr npcMgr) {
			ServerNPC tri = npcMgr.TagNpc("Trigger");

			NPCData sfData = sufferer.data;
			NPCRuntimeData sfRt = sfData.rtData;
			if(sfRt.curHp > 0) {
				/// TODO: add OnAttack，在释放出添加, 不在这里

				/// TODO: add BeAttack
				SelfDescribed des = new SelfDescribed() {
					srcEnd = new EndResult () {
						param1 = caster.UniqueID,
						param2 = sufferer.UniqueID,
					},
					targetEnd = null,
				};
				WarAnimParam param = new WarAnimParam() {
					cmdType = WarMsg_Type.BeAttacked,
					described = des,
				};

				npcMgr.SendMessageAsync(sufferer.UniqueID, tri.UniqueID, param);
			}

			if(sfRt.curHp <= 0) {
				/// TODO: add OnKilled
				SelfDescribed des = new SelfDescribed() {
					srcEnd = new EndResult () {
						param1 = caster.UniqueID,
						param2 = sufferer.UniqueID,
					},
					targetEnd = null,
				};
				WarAnimParam param = new WarAnimParam() {
					cmdType = WarMsg_Type.OnKilled,
					described = des,
				};

				npcMgr.SendMessageAsync(sufferer.UniqueID, tri.UniqueID, param);

				/// TODO: add BeKilled
				SelfDescribed des2 = new SelfDescribed() {
					srcEnd = new EndResult () {
						param1 = caster.UniqueID,
						param2 = sufferer.UniqueID,
					},
					targetEnd = null,
				};
				WarAnimParam param2 = new WarAnimParam() {
					cmdType = WarMsg_Type.BeKilled,
					described = des2,
				};

				npcMgr.SendMessageAsync(sufferer.UniqueID, tri.UniqueID, param2);
			}

		}

		#endregion
	}
}
