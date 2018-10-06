using AW.War;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AW.Message;
using BehaviorDesigner.Runtime.Tasks;
using AW.Data;

namespace AW.AI
{
	[TaskDescription("当自己单独以少敌多的时候，逃跑去防守")]
	[TaskCategory("Hero")]
	public class EscapeToDefence : Action
	{
        public ServerNPC target;
        private ServerLifeNpc myHero;
        private AIPath pathFind;
        private WarMsgParam param;
		private Transform mTrans;
		private Transform mTargetTrans; 
        private float distance;

        public Vector3 targetPos;

		public override void OnAwake()
		{
            myHero = GetComponent<ServerLifeNpc>();
            pathFind = myHero.pathFinding;
            mTrans = myHero.transform;
            param = new WarMsgParam();
		}

		public override void OnStart()
		{
			SelTarget ();
		}

		private void SelTarget()
		{
            WarServerNpcMgr npcMgr = WarServerManager.Instance.npcMgr;
		
            List<ServerLifeNpc> buildList = npcMgr.GetBuildByType(myHero.Camp, BuildNPCType.Tower);

            if (buildList == null || buildList.Count == 0)
                ConsoleEx.DebugError("no tower find ");

            int len = buildList.Count;

            float minDis = Mathf.Infinity;
            for (int i = 0; i < len; i++)
            {
                if (buildList[i].IsAlive)
                {
                    float dis = AITools.GetSqrDis(mTrans.position, buildList[i].transform.position);
                    if (dis < minDis)
                    {
                        minDis = dis;
                        target = buildList[i];
                    }
                }
            }

            mTargetTrans = target.transform;
            targetPos = target.transform.forward * target.data.configData.seekRange * -1;

            distance = myHero.data.configData.radius + target.data.configData.radius + 2.0f;
		}

		public override TaskStatus OnUpdate()
		{
            if (target == null)
            {
                SelTarget();
                return TaskStatus.Running;
            }

            //跑到塔下了
            if (AITools.IsInRange (mTrans.position, distance , targetPos))
			{
                myHero.SendAnimMsg(WarMsg_Type.Stand);

                myHero.data.btData.btStatus = NPCBattle_Status.None;
				pathFind.enabled = false;

				return TaskStatus.Success;
			}

			if (pathFind != null && !pathFind.enabled)
				pathFind.enabled = true;

            if(myHero.mAnimState.STATE != NpcAnimState.Run)
                myHero.SendAnimMsg(WarMsg_Type.Running);

            if (myHero.mAnimState.canMove)
			{
                pathFind.destination = targetPos;
			}
				
			return TaskStatus.Running;
		}

		public override void OnEnd()
		{

		}
	}
}
