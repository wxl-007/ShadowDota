using AW.War;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AW.Message;
using BehaviorDesigner.Runtime.Tasks;
using AW.Data;

namespace AW.AI
{
    [TaskDescription("后退到兵线后面")]
	[TaskCategory("Hero")]
	public class Flee : Action
	{
		public SharedLifeNPC fleeTarget;
        public BNPC moveTarget;
        private ServerLifeNpc myHero;
        private AIPath pathFind;
		private WarMsgParam param;

        private Transform mTargetTran;
        private Transform mTran;

		public override void OnAwake()
		{
            myHero = GetComponent<ServerLifeNpc> ();
            mTran = myHero.transform;
            pathFind = myHero.pathFinding;
            param = new WarMsgParam ();
		}

		public override void OnStart()
		{
			SelTarget ();
            mTargetTran = moveTarget.transform;  
		}

		private void SelTarget()
		{
            //自家塔
            List<ServerLifeNpc> mySpring = WarServerManager.Instance.npcMgr.GetBuildByType (myHero.Camp, BuildNPCType.Tower);
            if (mySpring != null && mySpring.Count > 0)
            {
                moveTarget = AITools.GetNeareastNPC(mTran.position, mySpring.ToArray());
            }
		}


		//往家里跑
		public override TaskStatus OnUpdate()
		{
			myHero.data.btData.btStatus = NPCBattle_Status.Fleeing;

            //如果跑到了，返回成功
            if (AITools.IsInRange(mTran.position, 3, mTargetTran.position))
            {
                if (pathFind != null && pathFind.enabled)
                    pathFind.enabled = false;

                myHero.data.btData.btStatus = NPCBattle_Status.None;
                fleeTarget.Value = null;
                myHero.data.btData.atkerID = 0;
                myHero.data.btData.IsInBattle = false;

                return TaskStatus.Success;
            }

			if (pathFind != null && !pathFind.enabled)
				pathFind.enabled = true;
                
            if (myHero.mAnimState.canMove)
                pathFind.destination = mTargetTran.position;
                
            if (myHero.mAnimState.STATE != NpcAnimState.Run)
			{
                myHero.SendAnimMsg(WarMsg_Type.Running);
			}

			return TaskStatus.Running;
		}

		public override void OnEnd()
		{

		}
	}
}
