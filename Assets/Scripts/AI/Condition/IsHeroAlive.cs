using UnityEngine;
using AW.War;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("npc附近没有敌方英雄")]
	[TaskCategory("Hero")]
	public class IsHeroAlive : Conditional
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
			if (npc.IsAlive)
				return TaskStatus.Success;
			return TaskStatus.Failure;
		}

		public override void OnEnd()
		{

		}
	}
}
