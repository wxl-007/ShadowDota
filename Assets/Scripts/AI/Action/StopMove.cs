using AW.War;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AW.Message;
using AW.Data;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("停止移动")]
	[TaskCategory("Hero")]
	public class StopMove : Action
	{
        private ServerLifeNpc myHero;

		public override void OnAwake()
		{
            myHero = GetComponent<ServerLifeNpc>();
		}

		public override TaskStatus OnUpdate()
		{
			if (myHero.pathFinding != null && myHero.pathFinding.enabled)
				myHero.pathFinding.enabled = false;
			
			return TaskStatus.Success;
		}
			
		public override void OnEnd()
		{

		}
	}
}
