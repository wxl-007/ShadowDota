using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.AI;
using AW.Data;

namespace AW.AI
{
	[TaskDescription("npc是否按照组刷新")]
	[TaskCategory("Hero")]
	public class IsFreshByGroup : IsFreshType
	{
		public override TaskStatus OnUpdate()
		{
			if (myHero.dataInScene.refreshRules == NPCRefreshRules.RefreshByGroup)
			{
				InitData ();
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}