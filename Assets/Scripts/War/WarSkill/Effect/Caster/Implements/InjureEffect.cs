using System;
using AW.Data;
using System.Collections.Generic;
using AW.Message;
using AW.Framework;
using System.Linq;

namespace AW.War {
	[Effect(OP=EffectOp.Injury)]
	public class InjureEffect : BaseEffect, ICastEffect {

		public InjureEffect() : base() { }

		#region ICastEffect implementation
		public void Init(EffectConfigData config, SkillConfigData skillcfg) {
			cfg = config;
			skCfg = skillcfg;
		}

		public void Cast (ServerNPC src,IEnumerable<ServerNPC> skTarget, IEnumerable<ServerNPC> target, bool skDirectHurt, List<MsgParam> container) {

			#if DEBUG
			Utils.Assert(src == null, "Injury Effect can't find attacker.");
			Utils.Assert(skCfg == null, "Skill Configure is null in Injury Effect .");
			#endif

			//发起者的信息
			WarSrcAnimParam SrcParam = new WarSrcAnimParam() {
				OP = EffectOp.Injury,
				ShootAction = skCfg.ShootAction,
				ShootTime   = skCfg.ShootTime,
                ShootEventTime = skCfg.ShootEventTime,
				//Debug信息
				SkillId   = skCfg == null ? -1 : skCfg.ID,
			};

			SelfDescribed SrcDes = new SelfDescribed() {
				src = src.UniqueID,
				target = src.UniqueID,
				act = Verb.Punch,
				srcEnd = null,
				targetEnd = new EndResult() {
					param1 = 0,
					param2 = 0,
					param3 = 0,
					param4 = 0,
					param8 = skCfg.MissileSpeed,
					param9 = skCfg.Distance,
				},
			};

			SrcParam.described = SrcDes;

			List<ServerNPC> effTarget = target.ToList();
			int count = effTarget.Count;
			//如果没有目标的时候
			if(count == 0)  {
				SrcParam.InjureTar = null;
				//新建一个目标为空的序列，主要告知skCfg.MissileSpeed 和 skCfg.Distance
				//该技能没有目标去命中
				container.Add(SrcParam);
				return; 
			}
			//如果存在目标的时候
			InjuryOp cal = OpMgr.getImplement<InjuryOp>(EffectOp.Injury);

			List<ServerNPC> skillTarget = skTarget.ToList();

			///
			/// ---- 获取主目标
			///
			int MainTargetID = 0;
			int skcount = skillTarget.Count;
			if(skcount > 0) {
				ServerNPC npc = skillTarget[0];
				MainTargetID = npc.UniqueID;
			}

			//count 必定 > 0
			//如果是技能直接的伤害
			if(skDirectHurt) {
				WarTarAnimParam[] TarList = new WarTarAnimParam[count];

				for(int i = 0; i < count; ++ i) {
					WarTarAnimParam TarParam = new WarTarAnimParam() {
						OP = EffectOp.Injury,
						HitAction = cfg.HitAction,
						SkillId   = skCfg.ID,
					};

					ServerNPC tar = effTarget[i];
					Dmg dmg = cal.toTargetDmg(src.data, tar.data, cfg);
					SelfDescribed des = new SelfDescribed() {
						src = src.UniqueID,
						target = tar.UniqueID,
						act = Verb.Punch,
						srcEnd = null,
						targetEnd = new EndResult() {
							param1 = (int)dmg.dmgValue,
							param2 = dmg.isCritical ? 1 : 0,
							param3 = (int)dmg.dmgType,
							param4 = (int)dmg.hitCls,
							param5 = MainTargetID,
							param8 = skCfg.MissileSpeed,
							param9 = skCfg.Distance,
						},
					};
					TarParam.described = des;

					TarList[i] = TarParam;
				}

				SrcParam.InjureTar = TarList;
				container.Add(SrcParam);
			} else {
				container.Add(SrcParam);
				//如果不是技能的伤害，而是buff或者Trigger的伤害
				for(int i = 0; i < count; ++ i) {

					WarTarAnimParam TarParam = new WarTarAnimParam() {
						OP = EffectOp.Injury,
						HitAction = cfg.HitAction,
						SkillId   = skCfg.ID,
					};

					ServerNPC tar = effTarget[i];
					Dmg dmg = cal.toTargetDmg(src.data, tar.data, cfg);
					SelfDescribed des = new SelfDescribed() {
						src = src.UniqueID,
						target = tar.UniqueID,
						act = Verb.Punch,
						srcEnd = null,
						targetEnd = new EndResult() {
							param1 = (int)dmg.dmgValue,
							param2 = dmg.isCritical ? 1 : 0,
							param3 = (int)dmg.dmgType,
							param4 = (int)dmg.hitCls,
							param5 = MainTargetID,
							param8 = skCfg.MissileSpeed,
							param9 = skCfg.Distance,
						},
					};

					TarParam.described = des;

					container.Add(TarParam);
				}
			}


		}

		#endregion
	}

}
