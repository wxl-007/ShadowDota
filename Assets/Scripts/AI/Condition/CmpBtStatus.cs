using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.AI;
using AW.Data;

namespace AW.AI
{
	[TaskDescription("比较战斗状态")]
	[TaskCategory("Hero")]
	public class CmpBtStatus : Conditional
	{
		public SharedLifeNPC hero;
		public NPCBattle_Status curStatus;
		public bool self;
        private ServerLifeNpc npc;

		public override void OnAwake()
		{
			if (self)
                npc = GetComponent<ServerLifeNpc> ();
			else
				npc = hero.Value;
		}

		public override TaskStatus OnUpdate()
		{
			//如果npc不是英雄，返回失败
			if(npc.WhatTypeOf != LifeNPCType.Hero)
				return TaskStatus.Failure;

			if (curStatus == npc.data.btData.btStatus)
				return TaskStatus.Success;

			return TaskStatus.Failure;
		}
	}
}