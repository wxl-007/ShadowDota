using System;
using AW.Data;
using AW.Message;

namespace AW.War {
	/// <summary>
	/// Trigger if npc is on killed.
	/// 主动杀死NPC的触发器
	/// </summary>
	[Trigger(Cmd = WarMsg_Type.OnKilled)]
	public class TriggerIfNpcOnKilled : Trigger, ITriggerItem {
		#region ITriggerItem implementation

		public int GetID() {
			return TriggerId;
		}

		public void OnHappen (MsgParam msg, WarServerNpcMgr npcMgr) {
			//TODO : do something
			WarAnimParam warParam = msg as WarAnimParam;
			if(warParam != null && warParam.described != null) {
				SelfDescribed described = warParam.described;
				if(described.srcEnd != null) {
					//转发消息
					ServerNPC tag = npcMgr.TagNpc("Trigger");
					warParam.cmdType = WarMsg_Type.RmBufIfKilling;
					npcMgr.SendMessageAsync(tag.UniqueID, tag.UniqueID, warParam);
				}
			}
		}

		public void OnRest () {

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
