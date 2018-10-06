using System;
using AW.Data;
using AW.Message;
using System.Collections.Generic;
using AW.Framework;
using System.Linq;

namespace AW.War {

	/// <summary>
	/// 这个效果是一个纯动画的效果
	/// </summary>
	[Effect(OP=EffectOp.MoveTarget)]
	public class TargetMoveEffect : BaseEffect, ICastEffect {

		public TargetMoveEffect() : base() { }

		#region ICastEffect implementation
		public void Init(EffectConfigData config, SkillConfigData skillcfg) {
			cfg   = config;
			skCfg = skillcfg;
		}


		/// <summary>
		/// 释放技能效果
		/// </summary>
		/// <param name="src">效果发起者</param>
		/// <param name="skTarget">技能选择的目标</param>
		/// <param name="target">技能先选择目标后，再次Effect选择后的目标</param>
		/// <param name="skDirectHurt">是否是技能的直接伤害</param>
		/// <param name="container">所有数据展现的容器，本次Effect施法的效果会依次加入</param>
		public void Cast (ServerNPC src, IEnumerable<ServerNPC> skTarget, IEnumerable<ServerNPC> target, bool skDirectHurt, List<MsgParam> container) {
			#if DEBUG
			Utils.Assert(cfg == null, "Effect Configure is null in SwitchNpcEffect.");
			#endif
			//有且只有一个
			//Utils.Assert(target.Count() != 1, "HookNpcEffect is DisWhenFinalTar type, so only one target is allowed.");
            int targetId = 0;

            if (target != null && target.Count() > 0)
            {
                targetId = target.First().UniqueID;
            }

			WarSrcAnimParam SrcParam = new WarSrcAnimParam() {
				OP          = EffectOp.MoveTarget,
				ShootAction = skCfg.ShootAction,
				ShootTime   = skCfg.ShootTime,
                ShootEventTime = skCfg.ShootEventTime,
				SkillId     = skCfg.ID,
			};

			SelfDescribed des = new SelfDescribed() {
				src    = src.UniqueID,
				target = src.UniqueID,
				act    = Verb.Move,
				srcEnd = null,
				targetEnd = new EndResult() {
					param1 = cfg.ID,
					param2 = src.UniqueID,
					param3 = targetId,
				}
			};

			SrcParam.described = des;
			container.Add(SrcParam);
		}

		#endregion
	}

}
