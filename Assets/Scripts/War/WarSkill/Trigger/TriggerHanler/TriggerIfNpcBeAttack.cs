using System;
using System.Collections.Generic;
using AW.Message;
using AW.Data;

namespace AW.War {

	/// <summary>
	/// 挨打时候触发的触发器
	/// </summary>

	[Trigger(Cmd = WarMsg_Type.BeAttacked)]
	public class TriggerIfNpcBeAttack : Trigger, ITriggerItem {
		#region ITriggerItem implementation

		public int GetID () {
			return TriggerId;
		}

		public void OnHappen (MsgParam msg, WarServerNpcMgr npcMgr) {

			WarAnimParam warParam = msg as WarAnimParam;
			if(warParam != null && warParam.described != null) {

				//转发消息
				ServerNPC tag = npcMgr.TagNpc("Trigger");

				SelfDescribed described = warParam.described;
				if(described.srcEnd != null) {
					warParam.cmdType = WarMsg_Type.CounterAttack;
					npcMgr.SendMessageAsync(tag.UniqueID, tag.UniqueID, warParam);
				}
			
			}

		}

		public void OnRest () {
			///
			/// 没有特殊的值需要变量需要清空
			/// 
		}

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
