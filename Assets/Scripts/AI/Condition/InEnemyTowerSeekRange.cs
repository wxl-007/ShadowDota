using UnityEngine;
using System.Collections.Generic;
using AW.Data;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;

namespace AW.AI
{
    [TaskDescription("自己是否在敌方塔下")]
	[TaskCategory("Hero")]
    public class InEnemyTowerSeekRange : Conditional
	{
        public SharedLifeNPC targetTower;
        private ServerLifeNpc npc;
		private Transform mTransform;
        private WarServerNpcMgr npcMgr;
        private List<ServerLifeNpc> tower;

		public override void OnStart()
		{
            npc = GetComponent<ServerLifeNpc>();
			mTransform = npc.transform;
            npcMgr = WarServerManager.Instance.npcMgr;
            CAMP camp = CAMP.Player;
            if (npc.Camp == CAMP.Enemy)
            {
                camp = CAMP.Player;
            }
            else if (npc.Camp == CAMP.Player)
            {
                camp = CAMP.Enemy;
            }
            tower = npcMgr.GetBuildByType(camp, BuildNPCType.Tower);
		}

		public override TaskStatus OnUpdate()
		{
            if (tower == null || tower.Count == 0)
                return TaskStatus.Failure;

            //在塔下。返回成功
            for (int i = 0; i < tower.Count; i++)
			{
                float seekRange = tower[i].ATKRange + tower[i].data.configData.radius + npc.data.configData.radius + 2;

                if (tower[i].IsAlive && AITools.IsInRange (mTransform.position, seekRange , tower[i].transform))
				{
                    targetTower.Value = tower[i];
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

