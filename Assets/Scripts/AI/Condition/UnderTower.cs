using UnityEngine;
using System.Collections.Generic;
using AW.Data;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;

namespace AW.AI
{
	[TaskDescription("npc是否在塔下")]
	[TaskCategory("Hero")]
	public class UnderTower : Conditional
	{
		public ServerLifeNpc npc;
		private Transform mTransform;
        private WarServerNpcMgr npcMgr;
        private float distance;

		public override void OnStart()
		{
			mTransform = npc.transform;
            npcMgr = WarServerManager.Instance.npcMgr;
            distance = npc.data.configData.radius + 3.0f;
		}

		public override TaskStatus OnUpdate()
		{

            List<ServerLifeNpc> tower = npcMgr.GetBuildByType(npc.Camp, BuildNPCType.Tower);
            if (tower == null || tower.Count == 0)
				return TaskStatus.Success;
               
			// 范围内有人，返回失败
            for (int i = 0; i < tower.Count; i++)
			{
                if (tower[i].IsAlive && AITools.IsInRange (mTransform.position, distance + tower[i].data.configData.radius, tower[i].transform))
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

