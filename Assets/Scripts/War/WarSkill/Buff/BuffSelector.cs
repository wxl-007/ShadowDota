using System;
using System.Collections.Generic;
using AW.Data;
using AW.Framework;
using BufSel = AW.Data.BuffChosen;

namespace AW.War {

	/// <summary>
	/// Buff的目标选择器，这个目标选择比较简单
	/// 1. 如果Buff属于OriginOfBuff.Alone则施法者，依附者都是同一个npc（施法者）
	/// 2. 如果buff属于OriginOfBuff.BornWithSkill的，则需要判定施法源头在哪里，判定施法的目标在哪里
	/// </summary>
	public class BuffSelector {

		private WarServerNpcMgr npcMgr;
		public BuffSelector(WarServerNpcMgr NpcMgr) {
			npcMgr = NpcMgr;
		}

		/// <summary>
		/// 获取挂载BUff的NPC
		/// </summary>
		/// <returns>The hang up.</returns>
		public ServerNPC getHangUp (RtBufData rtBf) {
			#if DEBUG
			Utils.Assert(rtBf == null, "RtBufData is null when get Hang Up.");
			#endif

			ServerNPC hangUp = npcMgr.GetNPCByUniqueID(rtBf.HangUpNpcID);
			return hangUp;
		}

		public ServerNPC locateCastor(RtBufData rtBf) {
			#if DEBUG
			Utils.Assert(rtBf == null, "RtBufData is null when relocate buff origin.");
			#endif

			ServerNPC castor = null;

			if(rtBf.origin == OriginOfBuff.Alone) {
				castor = npcMgr.GetNPCByUniqueID(rtBf.HangUpNpcID);
			} else {
				switch(rtBf.BuffCfg.Caster) {
				case BufSel.Add_To_Adhere:
					castor = npcMgr.GetNPCByUniqueID(rtBf.HangUpNpcID);
					break;
				case BufSel.Add_To_Caster:
					castor = npcMgr.GetNPCByUniqueID(rtBf.CastorNpcID);
					break;
				}
			}
			return castor;
		}

		/// <summary>
		/// buff属于OriginOfBuff.BornWithSkill这个方法才有效
		/// 选择出Buff的目标
		/// </summary>
		/// <param name="adhere">Adhere.</param>
		/// <param name="bufCfg">Buffer cfg.</param>
		public ServerNPC locateTarget(RtBufData rtBf) {
			#if DEBUG
			Utils.Assert(rtBf == null, "RtBufData is null when relocate buff target.");
			#endif

			ServerNPC target = null;

			if(rtBf.origin == OriginOfBuff.Alone) {
				target = npcMgr.GetNPCByUniqueID(rtBf.HangUpNpcID);
			} else {
				switch(rtBf.BuffCfg.Target) {
				case BufSel.Add_To_Adhere:
					target = npcMgr.GetNPCByUniqueID(rtBf.HangUpNpcID);
					break;
				case BufSel.Add_To_Caster:
					target = npcMgr.GetNPCByUniqueID(rtBf.CastorNpcID);
					break;
				}
			}

			return target;
		}
	}
}


