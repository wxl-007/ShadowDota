using UnityEngine;
using AW.War;
using System.Collections.Generic;
using AW.Data;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("npc附近是否有敌方英雄")]
	[TaskCategory("Hero")]
	public class NoEnemyNearNPC : Conditional
	{
		public SharedLifeNPC target;
        private Transform mTargetTrans;
        public List<ServerLifeNpc> enemy;
        private WarServerCharactor chaPool;
        public float seekRange;

        public override void OnAwake()
        {
            chaPool = WarServerManager.Instance.realServer.monitor.CharactorPool;
        }

        public override void OnStart()
		{
            mTargetTrans = target.Value.transform;
            CAMP enemyCamp = CAMP.All;

            enemy = chaPool.GetHeroList(target.Value.Camp);
            seekRange = target.Value.data.configData.seekRange + target.Value.data.configData.radius;
        }

		public override TaskStatus OnUpdate()
		{
			if (enemy == null || enemy.Count == 0)
				return TaskStatus.Success;

			
			// 范围内有人，返回失败
			for (int i = 0; i < enemy.Count; i++)
			{
                if (enemy[i].IsAlive &&  AITools.IsInRange (mTargetTrans.position, seekRange + enemy[i].data.configData.radius, enemy[i].transform))
				{
					return TaskStatus.Failure;
				}
			}

			return TaskStatus.Success;
		}

		public override void OnEnd()
		{

		}
	}
}

