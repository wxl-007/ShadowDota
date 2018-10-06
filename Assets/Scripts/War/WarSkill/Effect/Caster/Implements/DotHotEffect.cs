using System;
using AW.Data;
using AW.Message;
using System.Collections.Generic;
using AW.Framework;

namespace AW.War {
	[Effect(OP = EffectOp.DotHot)]
	public class DotHotEffect : BaseEffect, ICastEffect {

		public DotHotEffect() : base() { }

		#region ICastEffect implementation

		public void Init (EffectConfigData config, SkillConfigData skillcfg) {
			base.cfg   = config;
			base.skCfg = skillcfg;
		}

		/// <summary>
		/// 释放技能效果
		/// </summary>
		/// <param name="src">效果发起者</param>
		/// <param name="skTarget">技能选择的目标，本参数在这里无意义</param>
		/// <param name="target">技能先选择目标后，再次Effect选择后的目标。本参数数量 >= 0 </param></param>
		/// <param name="skDirectHurt">是否是技能的直接伤害, 这个没有伤害</param>
		/// <param name="container">所有数据展现的容器，本次Effect施法的效果会依次加入</param>
		public void Cast (ServerNPC src, IEnumerable<ServerNPC> skTarget, IEnumerable<ServerNPC> target, bool skDirectHurt, List<MsgParam> container) {
			#if DEBUG
			Utils.Assert(cfg == null, "Effect configure is null in DotHotEffect.");
			Utils.Assert(skCfg == null, "skill configure is null in DotHotEffect.");
			#endif

			foreach(ServerNPC bnpc in target) {
				WarTarAnimParam param = new WarTarAnimParam() {
					OP = EffectOp.DotHot,
					HitAction = cfg.HitAction,
				};
				
				BuffMgr bufMgr = WarServerManager.Instance.bufMgr;

				BuffCtorParamEx ctor = new BuffCtorParamEx() {
					fromNpcId    = src.UniqueID,
					toNpcId      = bnpc.UniqueID,
					cfg          = base.cfg,
				};
				//创建BUff
				RtBufData maybe = bufMgr.createBuff(ctor);
				//statistics
				SelfDescribed des  = statistics(maybe, src, bnpc);
				param.described = des;
				container.Add(param);
			}

			//通知给技能释放者的目标
			WarSrcAnimParam SrcParam = new WarSrcAnimParam() {
				OP  = EffectOp.DotHot,
				ShootAction = skCfg.ShootAction,
				ShootTime = skCfg.ShootTime,
                ShootEventTime = skCfg.ShootEventTime,
				described = toSelf(src.UniqueID),
			};
			container.Add(SrcParam);
		}

		#endregion

		SelfDescribed toSelf(int casterID) {
			SelfDescribed described  = new SelfDescribed() {
				src    = casterID,
				target = casterID,
				act    = Verb.DotHot,
				srcEnd = null,
				targetEnd = null,
			};

			return described;
		}


		///分析BuffAction的信息
		SelfDescribed statistics(RtBufData maybeCreated, ServerNPC src, ServerNPC target) {
			SelfDescribed described = null;
			int action = maybeCreated.BuffCfg.BuffAction;
			if(action > 0) {
				described = new SelfDescribed();
				described.src    = src.UniqueID;
				described.target = target.UniqueID;
				described.act    = Verb.BuffEffect;
				described.targetEnd = null;
				described.srcEnd = new EndResult() {
					param2 = action,
					param3 = 0,
				};
			}
			return described;
		}

	}

}
