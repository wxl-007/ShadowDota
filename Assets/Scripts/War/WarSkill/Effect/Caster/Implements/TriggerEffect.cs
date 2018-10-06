using System;
using AW.Data;
using System.Collections.Generic;
using AW.Message;
using AW.Framework;
using System.Linq;

namespace AW.War {

	/// <summary>
	/// 用来触发“触发器”的Effect
	/// </summary>
	[Effect(OP=EffectOp.Trigger)]
	public class TriggerEffect : BaseEffect, ICastEffect {

		public TriggerEffect() : base() { }

		#region ICastEffect implementation
		public void Init(EffectConfigData config, SkillConfigData skillcfg) {
			base.cfg   = config;
			base.skCfg = skillcfg;
		}

		public void Cast (ServerNPC src, IEnumerable<ServerNPC> skTarget, IEnumerable<ServerNPC> target, bool skDirectHurt, List<MsgParam> container) {

			#if DEBUG
			Utils.Assert(src == null, "Treat Effect can't find attacker.");
			#endif

			Verb verb = Verb.Punch;
			if(cfg.Param1 > 0) 
				verb = (Verb)Enum.ToObject(typeof(Verb), cfg.Param1 + 0xA5);
			if(cfg.Param2 > 0)
				verb = (Verb)Enum.ToObject(typeof(Verb), cfg.Param2 + 0xC5);

			List<ServerNPC> effTarget = target.ToList();
			int count = effTarget.Count;
			if(count > 0) {
				for(int i = 0; i < count; ++ i) {
					WarTarAnimParam param = new WarTarAnimParam();

					ServerNPC tar = effTarget[i];

					SelfDescribed des = new SelfDescribed() {
						src = src.UniqueID,
						target = tar.UniqueID,
						act = verb,
						srcEnd = new EndResult(){
							param1 = cfg.Param4,
							param2 = cfg.Param3,
							param3 = cfg.Param1 > 0 ? 0 : 1,
						},
						targetEnd = null,
					};

					param.OP        = EffectOp.Trigger;
					param.SkillId   = skCfg.ID;
					param.described = des;

					container.Add(param);
				}
			}


		}
		#endregion
	}
}


