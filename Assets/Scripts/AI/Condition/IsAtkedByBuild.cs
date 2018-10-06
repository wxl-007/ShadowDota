using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.AI;
using AW.Data;

namespace AW.AI
{
	[TaskDescription("自己是否是对方建筑的攻击目标")]
	[TaskCategory("Hero")]
	public class IsAtkedByBuild : Conditional
	{
		public SharedLifeNPC target;
        private ServerLifeNpc myHero;
		public override void OnStart()
		{
            myHero = GetComponent<ServerLifeNpc>();
		}

		public override TaskStatus OnUpdate()
		{
			//如果目标不是建筑，直接返回失败
			if(target.Value.WhatTypeOf != LifeNPCType.Build)
				return TaskStatus.Failure;

			//得到攻击目标
            SharedLifeNPC atkTarget = (SharedLifeNPC)target.Value.AutoAiTree.GetVariable ("attackTarget");
			//如果对方的攻击目标是自己，返回成功
			if (atkTarget != null && atkTarget.Value != null && atkTarget.Value.UniqueID == myHero.UniqueID)
			{
				return TaskStatus.Success;
			}

			return TaskStatus.Failure;
		}

		public override void OnEnd()
		{

		}
	}
}