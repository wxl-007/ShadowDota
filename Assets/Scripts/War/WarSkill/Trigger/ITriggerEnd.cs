using System;
using AW.Data;

namespace AW.War {
	/// <summary>
	/// 用来决定触发器如何结束工作
	/// 
	/// 0.删除源Buff和Trigger
	/// 1.不删除源Buff但删除Trigger
	/// 2.重置计数器
	/// 3.删除1层源Buff(如果源Buff的stacks>1,则减1层),且重置计数器
	/// </summary>
	public interface ITriggerEnd {
		void IfEnd(RtBufData buf, Trigger trigger);
	}
}
