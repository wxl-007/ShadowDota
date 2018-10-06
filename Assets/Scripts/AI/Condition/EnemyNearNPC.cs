using UnityEngine;
using AW.War;
using System.Collections.Generic;
using AW.Data;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("npc附近是否有敌方英雄")]
	[TaskCategory("Hero")]
	public class EnemyNearNPC : Conditional
	{
		public SharedLifeNPC target;
        public List<ServerLifeNpc> enemy;

		private Transform mTransform;
		public float seekRange;
        private ServerLifeNpc myHero;
	
		public override void OnAwake()
		{
            myHero = GetComponent<ServerLifeNpc>();
		}

		public override void OnStart()
		{
			mTransform = target.Value.transform;
		 	seekRange = target.Value.data.configData.seekRange;
		}

		public override TaskStatus OnUpdate()
		{

            CAMP enemyCamp = CAMP.None;
            if (myHero.Camp == CAMP.Enemy)
                enemyCamp = CAMP.Player;
            else if (myHero.Camp == CAMP.Player)
                enemyCamp = CAMP.Enemy;

            enemy = WarServerManager.Instance.realServer.monitor.CharactorPool.GetHeroList(enemyCamp);
				
			if (enemy == null || enemy.Count == 0)
				return TaskStatus.Failure;


			// 范围内有人，返回失败
			for (int i = 0; i < enemy.Count; i++)
			{
				if (AITools.IsInRange (mTransform.position, seekRange, enemy[i].transform))
				{
					return TaskStatus.Success;
				}
			}

			return TaskStatus.Failure;
		}

		public override void OnEnd()
		{

		}
	}
}

