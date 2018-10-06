using System;
using System.Collections.Generic;
using AW.Data;

namespace AW.War {
	/// <summary>
	/// 根据技能的优先级来选择目标
	/// </summary>
	public interface IPrioritySelect {
		/// <summary>
		/// 根据优先级挑选出排序后的npc列表
		/// </summary>
		/// <param name="unPriority">未排序的数据.</param>
		/// <param name="priority">Priority 规则</param>
		/// <param name="HasPriority">已排序的数据.</param>
		void SortByPriority(IEnumerable<ServerNPC> unPriority, WarServerNpcMgr npcMgr, List<List<ServerNPC>> HasPriority);
	}
}