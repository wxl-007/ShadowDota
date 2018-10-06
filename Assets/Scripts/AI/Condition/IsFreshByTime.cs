using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.AI;
using AW.Data;
using System.Collections.Generic;

namespace AW.AI
{
	[TaskDescription("npc是否按照时间刷新")]
	[TaskCategory("Hero")]
	public class IsFreshByTime : IsFreshType
	{
		public override TaskStatus OnUpdate()
		{
			if (myHero.dataInScene.refreshRules == NPCRefreshRules.RefreshByTime)
			{
				InitData ();
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}