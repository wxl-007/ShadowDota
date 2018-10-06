using System;
using AW.Data;
using AW.Message;
using AW.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AW.War {
    [Effect(OP = EffectOp.HookNpc)]
	public class HookNpcEffect : BaseEffect, ICastEffect {

		public HookNpcEffect() : base() { }

		#region ICastEffect implementation

		public void Init (EffectConfigData config, SkillConfigData skillcfg) {
			base.cfg   = config;
			base.skCfg = skillcfg;
		}

		/// <summary>
		/// 释放技能效果
		/// </summary>
		/// <param name="src">效果发起者</param>
		/// <param name="target">目标参数无效</param>
		/// <param name="skDirectHurt">是否是技能的直接伤害, 这个没有伤害</param>
		/// <param name="container">所有数据展现的容器，本次Effect施法的效果会依次加入</param>
		/// <param name="skTarget">Sk target.</param>
		public void Cast (ServerNPC src, IEnumerable<ServerNPC> skTarget, IEnumerable<ServerNPC> target, bool skDirectHurt, List<MsgParam> container) {
			#if DEBUG
			Utils.Assert(skCfg == null, "Skill Configure is null in BulletNpcEffect routine.");
			#endif

			int finalTarget = 0;
			HookNpcDisappearType disappearType = (HookNpcDisappearType) Enum.ToObject(typeof(HookNpcDisappearType), cfg.Param2);
			if(disappearType == HookNpcDisappearType.DisWhenFinalTar) {
				Utils.Assert(target.Count() != 1, "HookNpcEffect is DisWhenFinalTar type, so only one target is allowed.");
				finalTarget = target.First().UniqueID;
			}

			WarSrcAnimParam SrcParam = new WarSrcAnimParam() {
				OP          = EffectOp.Bullet_NPC,
				ShootAction = skCfg.ShootAction,
				ShootTime   = skCfg.ShootTime,
				SkillId     = skCfg.ID,
                ShootEventTime = skCfg.ShootEventTime,
			};

			SelfDescribed des = new SelfDescribed() {
				src    = src.UniqueID,
				target = src.UniqueID,
				act    = Verb.Creature,
				srcEnd = null,
				targetEnd = new EndResult() {
					param1 = src.UniqueID,
					param2 = cfg.ID,
					param3 = finalTarget,
				}
			};

			SrcParam.described = des;
			container.Add(SrcParam);
		}

		#endregion
	}
}