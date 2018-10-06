using System;
using AW.Data;
using AW.Message;
using System.Collections.Generic;
using AW.Framework;

namespace AW.War {

	/// <summary>
	/// 在当前位置创建一个ID为<Param1>的NPC，NPC携带ID为<Param2>的buff，
	/// 并以<param3>的速度方向位移<Param4>距离后以<Param5>的方式消失.
	/// 对碰到的敌方单位造成的伤害为<Param6>物理攻击强度+<Param7>法术攻击强度，
	/// 伤害的方式为<param8>，范围为<Param9>
	/// 
	/// 后置判定的效果
	/// </summary>

	[Effect(OP = EffectOp.Bullet_NPC)]
	public class BulletNpcEffect : BaseEffect, ICastEffect {

		public BulletNpcEffect() : base() { }

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

			WarSrcAnimParam SrcParam = new WarSrcAnimParam() {
				OP = EffectOp.Bullet_NPC,
				ShootAction = skCfg.ShootAction,
				ShootTime   = skCfg.ShootTime,
                ShootEventTime = skCfg.ShootEventTime,
			};
			///
			/// 有延迟的伤害类技能，伤害的结算都推后
			///
			SelfDescribed des = new SelfDescribed() {
				src    = src.UniqueID,
				target = src.UniqueID,
				act    = Verb.Creature,
				srcEnd = null,
				targetEnd = new EndResult() {
					param1 = src.UniqueID,
					param2 = cfg.ID,
                    param3 = cfg.Param1,
				}
			};

			SrcParam.described = des;
			container.Add(SrcParam);
		}

		#endregion
	}

}
