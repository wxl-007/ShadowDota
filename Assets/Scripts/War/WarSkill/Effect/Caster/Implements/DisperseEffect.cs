using System;
using AW.Data;
using AW.Message;
using System.Collections.Generic;
using AW.Framework;
using System.Linq;

namespace AW.War {
	/// <summary>
	/// 驱散的逻辑
	/// 驱散满足指定集合的某一个buff指定层数, <Param2>驱散集合（目前不用），<Param4>和<Param5><Param6> 是驱散指定Buff的ID
	/// </summary>
	[Effect(OP = EffectOp.DriveAway)]
	public class DisperseEffect : BaseEffect, ICastEffect {

		public DisperseEffect() : base() { }

		#region ICastEffect implementation

		public void Init (EffectConfigData config, SkillConfigData skillcfg) {
			base.cfg   = config;
			base.skCfg = skillcfg;
		}

		/// <summary>
		/// 释放技能效果
		/// </summary>
		/// <param name="src">效果发起者</param>
		/// <param name="skTarget">技能选择的目标，忽略</param>
		/// <param name="target">技能先选择目标后，再次Effect选择后的目标</param>
		/// <param name="skDirectHurt">是否是技能的直接伤害</param>
		/// <param name="container">所有数据展现的容器，本次Effect施法的效果会依次加入</param>
		public void Cast (ServerNPC src, IEnumerable<ServerNPC> skTarget, IEnumerable<ServerNPC> target, bool skDirectHurt, List<MsgParam> container) {

			///
			/// 实际上该效果分为两部分的逻辑
			/// 1. 消除某些类型的buff
			/// 2. 消除特定ID的buff
			///

			#if DEBUG 
			Utils.Assert( src == null, "DisperseEffect can't find src.");
			Utils.Assert( target == null, "DisperseEffect can't find target.");
			#endif

			BuffMgr bufMgr = WarServerManager.Instance.bufMgr;

			//TODO: 1. 消除某些类型的buff


			/// 2. 消除特定ID的buff
			List<int> bufIds = new List<int>();
			if(cfg.Param4 > 0) bufIds.Add(cfg.Param4);
			if(cfg.Param5 > 0) bufIds.Add(cfg.Param5);
			if(cfg.Param6 > 0) bufIds.Add(cfg.Param6);

			int count = bufIds.Count;
			if(count > 0) {
				if(target.Any()) {

					foreach(ServerNPC npc in target) {

						for(int i = 0; i < count; ++ i) {
							List<RtBufData> bufList = bufMgr.findBuff(bufIds[i], npc.UniqueID);

							int foundBufCnt = bufList.Count;
							if(foundBufCnt > 0) {
								for(int j = 0; j < foundBufCnt; ++ j) {
									bufMgr.rmBuff(bufList[j].ID, npc.UniqueID);
								}
							}

						}

					}

				}
			}

		}

		#endregion
	}
}