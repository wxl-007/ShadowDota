using AW.War;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AW.Message;
using AW.Data;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("战斗")]
	[TaskCategory("Hero")]
	public class EmptyAttack : Action
	{
        private ServerLifeNpc myHero;

		public override void OnAwake()
		{
            myHero = GetComponent<ServerLifeNpc>();
		}

		public override TaskStatus OnUpdate()
		{
			if (myHero.valid)
			{
				myHero.Attack ();

                if (myHero.outLog)
                {
                    ConsoleEx.DebugWarning (myHero.name + " ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ empty attck enemy ");
                }
					
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
			
		public override void OnEnd()
		{

		}
	}
}
