using UnityEngine;
using System.Collections;

namespace AW.War {
	/// <summary>
	/// Npc mgr factory.
	/// </summary>
	public class ServerNpcMgrFactory {
		public WarServerNpcMgr getNpcMgr () {
			return new NeHeQiaoNpcMgr();
		}
	}

}
