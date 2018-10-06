using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.AI;
using AW.Data;

namespace AW.AI
{
	[TaskDescription("英雄是否可以移动")]
	[TaskCategory("Hero")]
	public class CanMove : Conditional
	{
		public SharedLifeNPC hero;
		public bool self;
        private ServerLifeNpc npc;

		public override void OnStart()
		{
			if (self)
                npc = GetComponent<ServerLifeNpc> ();
			else
				npc = hero.Value;
		}

		public override TaskStatus OnUpdate()
		{
			if (npc.curStatus.AnySame (NpcStatus.ForbidMove))
			{
				return TaskStatus.Failure;
			}

			return TaskStatus.Success;
		}
	}
}