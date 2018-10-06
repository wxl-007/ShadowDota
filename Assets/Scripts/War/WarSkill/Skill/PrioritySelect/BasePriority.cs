using System;
using System.Collections.Generic;

namespace AW.War {
	//技能目标优先级的基类
	public abstract class BasePriority {
		/// <summary>
		/// 子类实现
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="enumFlag">Enum flag.</param>
		protected abstract int getPos(byte enumFlag);

		/// <summary>
		/// 根据优先级分类，加入数据池
		/// </summary>
		/// <param name="npc">Npc.</param>
		/// <param name="flag">Flag.</param>
		/// <param name="totest">Totest.</param>
		/// <param name="HasPriority">Has priority.</param>
		protected bool addNpcByPriority(ServerNPC npc, byte totest, List<List<ServerNPC>> HasPriority) {
			List<ServerNPC> container = HasPriority[getPos(totest)];

			bool exist = false;
			if(container == null) {
				container = new List<ServerNPC>();
			} else {
				exist = true;
			}

			container.Add(npc);

			if(!exist) {
				HasPriority[getPos(totest)] = container;
			}

			return !exist;
		}

	}
}
