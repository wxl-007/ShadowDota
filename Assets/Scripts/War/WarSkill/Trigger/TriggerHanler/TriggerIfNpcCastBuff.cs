using System;
using AW.Message;

namespace AW.War {
	[Trigger(Cmd = WarMsg_Type.UseBuff)]
	public class TriggerIfNpcCastBuff : Trigger, ITriggerItem {
		#region ITriggerItem implementation

		public int GetID () {
			return TriggerId;
		}

		public void OnHappen (MsgParam msg, WarServerNpcMgr npcMgr) {
			WarAnimParam warParam = msg as WarAnimParam;
			if(warParam != null && warParam.described != null) {

				SelfDescribed described = warParam.described;

				if(described.srcEnd != null && described.srcEnd.param1 > 0) {
					//转发消息
					ServerNPC tag = npcMgr.TagNpc("Trigger");

					switch(described.act) {
					case Verb.LineEnemy:
						warParam.cmdType = WarMsg_Type.LineEnemy;
						//创建一个触发器
						warMgr.triMgr.CreateTrigger(described.src, described.srcEnd.param1, null);
						npcMgr.SendMessageAsync(tag.UniqueID, tag.UniqueID, msg);
						break;
					}
				}

			}
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
