using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.AI;
using AW.Data;
using System.Collections.Generic;

namespace AW.AI
{
	[TaskDescription("英雄是否在兵线后面")]
	[TaskCategory("Hero")]
	public class BehindSelfSoldier : Conditional
	{
        public SharedLifeNPC target;
        private ServerLifeNpc npc;
        private WarServerNpcMgr npcMgr;

        private Transform mTrans;
        private Transform mTargetTrans;

        public override void OnAwake()
        {
            npc = GetComponent<ServerLifeNpc> ();
            npcMgr = WarServerManager.Instance.npcMgr;
            mTrans = npc.transform;
        }

        public override void OnStart()
        {
            mTargetTrans = target.Value.transform;
        }
            
		public override TaskStatus OnUpdate()
		{
            //自己阵营所有的npc
            List<ServerLifeNpc> selfNpcs = npcMgr.GetLifeNPCListByCamp(npc.Camp);

            //自己身后六米的地方
            Vector3 dir = Vector3.Normalize(mTargetTrans.position - mTrans.position) * 10; 
            Vector3 fromPos = mTrans.position + dir;

            List<ServerLifeNpc> nearNpcs = AITools.GetAllNPCInRange(fromPos, 4, selfNpcs.ToArray()); 
            if (nearNpcs != null && nearNpcs.Count > 0)
            {
                return TaskStatus.Success;
            }
			return TaskStatus.Failure;
		}
	}
}