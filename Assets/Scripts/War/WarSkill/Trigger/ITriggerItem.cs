using System;
using AW.Message;
using System.Collections;

namespace AW.War {

	public interface ITriggerItem {
		/// <summary>
		/// 返回唯一ID
		/// </summary>
		/// <returns>The I.</returns>
		int GetID();

		/// <summary>
		/// 触发条件满足，或者说感兴趣的事情发生了。
		/// 但是是不是要真的处理什么逻辑，还不一定
		/// </summary>
		/// <param name="msg">Message.</param>
		void OnHappen(MsgParam msg, WarServerNpcMgr npcMgr);

		/// <summary>
		/// 重置触发器-强制全部重置
		/// </summary>
		void OnRest();
	}
}

