using System;
using AW.Data;

namespace AW.War {
	public class RmBuffWithTrigger : ITriggerEnd {
		#region ITriggerEnd implementation

		public void IfEnd (RtBufData buf, Trigger trigger) {
			var info = trigger.GetType();
			var classAttribute = (TriggerAttribute)Attribute.GetCustomAttribute(info, typeof(TriggerAttribute));

			WarServerManager warMgr = WarServerManager.Instance;
			warMgr.bufMgr.rmBuff(buf.ID, buf.HangUpNpcID);
			warMgr.triMgr.RemoveTrigger(classAttribute.Cmd, trigger.TriggerId, trigger.HangUpNpcId);
		}

		#endregion
	}
}
