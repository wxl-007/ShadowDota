using System;
using AW.Data;
using AW.Message;
using System.Collections.Generic;

namespace AW.War {
	//战斗的数据统计
	public class StatictisInSkill {

		class Pair {
			public int CastorNum;
			public int KilledNum;
		}

		private List<Pair> killingInfo = null;

		private StatictisInSkill() {
			killingInfo = new List<Pair>();
		}

		public static StatictisInSkill instance {
			get { 
				return GenericSingleton<StatictisInSkill>.Instance;
			}
		}

		public void analyze(MsgParam msg, WarServerNpcMgr npcMgr) {
			WarAnimParam warParam = msg as WarAnimParam;
			if(warParam != null && warParam.described != null) {
				SelfDescribed des = warParam.described;

				int castor   = des.srcEnd.param1;
				int sufferer = des.srcEnd.param2;

				ServerNPC bCast   = npcMgr.GetNPCByUniqueID(castor);
				ServerNPC bSuf    = npcMgr.GetNPCByUniqueID(sufferer);

				if(bCast != null && bSuf != null) {
					Pair pair = new Pair() {
						CastorNum = bCast.data.num,
						KilledNum = bCast.data.num,
					};
					killingInfo.Add(pair);
				}

			}
		}

	}
}