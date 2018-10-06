using AW.Data;

namespace AW.War {

	/// <summary>
	/// 只删除Trigger，不删除Buff
	/// </summary>
	public class RmTrigger : ITriggerEnd {
		#region ITriggerEnd implementation

		public void IfEnd (RtBufData buf, Trigger trigger) {

		}

		#endregion
	}

}
