using System;
using AW.Data;
using System.Collections.Generic;
using AW.Message;
using AW.Framework;
using System.Linq;

namespace AW.War {
	/// <summary>
	/// 治疗的效果逻辑
	/// </summary>

	[Effect(OP=EffectOp.Treat)]
	public class TreatEffect : BaseEffect, ICastEffect {

		public TreatEffect() : base() { }

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
			InjuryOp cal = OpMgr.getImplement<InjuryOp>(EffectOp.Injury);

			#if DEBUG
			Utils.Assert(src == null, "Treat Effect can't find attacker.");
			#endif

			List<ServerNPC> effTarget = target.ToList();
			int count = effTarget.Count;
			if(count > 0) {
				for(int i = 0; i < count; ++ i) {
					WarTarAnimParam param = new WarTarAnimParam();

					ServerNPC tar = effTarget[i];
					Treat dmg = cal.toTargetTreat(src.data, tar.data, cfg);
					SelfDescribed des = new SelfDescribed() {
						src = src.UniqueID,
						target = tar.UniqueID,
						act = Verb.Recover,
						srcEnd = null,
						targetEnd = new EndResult() {
							param1 = (int)dmg.treatValue,
							param2 = dmg.isCritical ? 1 : 0,
							param3 = (int)dmg.treatType,
						},
					};

					param.OP        = EffectOp.Treat;
					param.described = des;

					container.Add(param);
				}
			}

		
		}
		#endregion
	}
}
