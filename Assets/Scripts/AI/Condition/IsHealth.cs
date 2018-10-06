using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.AI;
using AW.Data;

namespace AW.AI
{
	[TaskDescription("英雄是否健康")]
	[TaskCategory("Hero")]
	public class IsHealth : Conditional
	{
		public SharedLifeNPC hero;
		public SharedFloat percent;
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
			//如果npc不是英雄，一律认为他是health
			if(npc.WhatTypeOf != LifeNPCType.Hero)
				return TaskStatus.Success;

			if (npc.data.rtData.curHp >= npc.data.rtData.totalHp * percent.Value)
				return TaskStatus.Success;

			return TaskStatus.Failure;
		}
	}
}