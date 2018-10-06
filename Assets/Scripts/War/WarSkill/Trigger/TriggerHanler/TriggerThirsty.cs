using System;
using AW.Data;
using AW.Message;

namespace AW.War {
	/// <summary>
	/// 杀死别人，自己身上的Buff和Trigger消失的逻辑
	/// 属于子Trigger类型
	/// </summary>
	[Trigger(Cmd = WarMsg_Type.RmBufIfKilling)]
	public class TriggerThirsty : Trigger, ITriggerItem {
		#region ITriggerItem implementation

		public int GetID() {
			return TriggerId;
		}

		public void OnHappen (MsgParam msg, WarServerNpcMgr npcMgr) {
			//TODO : do something
			WarAnimParam warParam = msg as WarAnimParam;
			if(warParam != null && warParam.described != null) {
				//杀死人，described只有一个元素
				SelfDescribed described = warParam.described;
				if(described.srcEnd != null) {

					///
					/// 杀死别人，自己身上的Buff和Trigger消失
					///
					int casterId = described.srcEnd.param1;
					//int sufferId = described.srcEnd.param2;
					if(casterId == HangUpNpcId) {

						///
						/// 概率上的检测
						///

						bool happed = PseudoRandom.getInstance().happen(cfg.Prob);
						if(happed) OnEnd();
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