using UnityEngine;
using AW.War;
using System.Collections.Generic;
using AW.Data;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("npc附近是否有自己的小兵")]
	[TaskCategory("Hero")]
	public class SelfSoldierNearNPC : Conditional
	{
		private ServerLifeNpc npc;
		private Transform mTransform;
        private float seekRange;

        public override void OnStart()
		{
            npc = GetComponent<ServerLifeNpc>();
            mTransform = npc.transform;
            seekRange = npc.data.configData.seekRange;
		}

		public override TaskStatus OnUpdate()
		{
            List<ServerLifeNpc> soldiers = WarServerManager.Instance.npcMgr.GetLifeNPCByType(LifeNPCType.Soldier, npc.Camp);
            if (soldiers == null || soldiers.Count == 0)
                return TaskStatus.Failure;
                
			// 范围内有人，返回失败
            for (int i = 0; i < soldiers.Count; i++)
			{
                if (soldiers[i].IsAlive &&  AITools.IsInRange (mTransform.position, seekRange, soldiers[
					i].transform))
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

