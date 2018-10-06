using System;
using AW.Message;
using AW.Data;

namespace AW.War {
	/// <summary>
	/// 监视使用技能的情况
	/// </summary>
	[Trigger(Cmd = WarMsg_Type.UseSkill)]
	public class TriggerIfNpcCastSkill : Trigger, ITriggerItem {
		#region ITriggerItem implementation

		public int GetID () {
			return TriggerId;
		}

		public void OnHappen (MsgParam msg, WarServerNpcMgr npcMgr) {

		}

		public void OnRest () { }

		#endregion

		/// <summary>
		/// 不需要被触发后，还每一帧继续触发。也就是说这个触发器执行一次
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public override bool TickPerFrame () {
			return false;
		}
	}
}
