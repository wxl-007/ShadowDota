using System;
using System.Collections.Generic;
using AW.Data;
using AW.Message;

namespace AW.War {

	/// <summary>
	/// 挨打之后反击敌人 ----  属于子Trigger类型
	/// </summary>
	[Trigger(Cmd = WarMsg_Type.CounterAttack)]
	public class TriggerCounterAtk : Trigger, ITriggerItem {
		#region ITriggerItem implementation

		public int GetID() {
			return TriggerId;
		}

		public void OnHappen (MsgParam msg, WarServerNpcMgr npcMgr) {
			WarAnimParam warParam = msg as WarAnimParam;
			if(warParam != null && warParam.described != null) {

				SelfDescribed described = warParam.described;
				if(described.srcEnd != null) {
					///
					/// 每次被攻击几率对周围造成伤害
					///
					int casterId = described.srcEnd.param1;
					int sufferId = described.srcEnd.param2;

					//不能自己打自己也反弹
					if(sufferId == casterId) return;

					if(sufferId == HangUpNpcId) {

						//选择自己和目标
						//目标也是自己
						ServerNPC castor = npcMgr.GetNPCByUniqueID(sufferId);
						List<ServerNPC> targets = new List<ServerNPC>();
						targets.Add(castor);

						warMgr.triMgr.trigCastor.cast(castor, targets, cfg);

					}
				}

			}
		}

		public void OnRest () {
			cfg = null;
		}

		#endregion

		/// <summary>
		/// 只执行一次
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public override bool TickPerFrame () {
			return false;
		}
	}
}
