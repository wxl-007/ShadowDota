using AW.War;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AW.Message;
using BehaviorDesigner.Runtime.Tasks;
using AW.Data;

namespace AW.AI
{
	[TaskDescription("逃跑去补血")]
	[TaskCategory("Hero")]
	public class Escape : Action
	{
        public SharedNPC target;
        private ServerLifeNpc myHero;
        private AIPath pathFind;
		private WarMsgParam param;
        private List<ServerNPC> npcList;

		private Transform mTrans;
		private Transform mTargetTrans; 

		public override void OnAwake()
		{
            myHero = GetComponent<ServerLifeNpc>();
            pathFind = myHero.pathFinding;
            npcList = new List<ServerNPC> ();
			param = new WarMsgParam ();
		}

		public override void OnStart()
		{
			SelTarget ();
		}

		private void SelTarget()
		{
            NeHeQiaoNpcMgr npcMgr = WarServerManager.Instance.npcMgr as NeHeQiaoNpcMgr;
		
			//自家泉水
            if(myHero.Camp == CAMP.Player && npcMgr.SelfSpring != null)
				npcList.Add (npcMgr.SelfSpring);
            else if(myHero.Camp == CAMP.Enemy && npcMgr.EnemySpring != null)
                npcList.Add (npcMgr.EnemySpring);

			//公共泉水
			if (npcMgr.NeutralSpring != null)
				npcList.Add (npcMgr.NeutralSpring);


			//所有道具
            List<ServerNPC> allProp = WarServerManager.Instance.npcMgr.GetNPCByType (LifeNPCType.Prop, CAMP.None);
			if (allProp != null && allProp.Count > 0)
			{
				npcList.AddRange (allProp.ToArray ());
			}

			//得到距离自己最近的
            ServerNPC npcTarget = null;
			if (npcList.Count > 0)
			{
				float minDis = Mathf.Infinity;

				for (int i = 0; i < npcList.Count; i++)
				{
					float distance = AITools.GetSqrDis (this.transform.position, npcList [i].transform.position);
					if (distance < minDis)
					{
						minDis = distance;
						npcTarget = npcList [i];
					}
				}
			}

            target.Value = npcTarget;
			mTrans = myHero.transform;
            mTargetTrans = target.Value.transform;
		}

		public override TaskStatus OnUpdate()
		{
            if (myHero.mAnimState.canMove)
            {
                pathFind.destination = mTargetTrans.position;
            }

            if (pathFind != null && !pathFind.enabled)
                pathFind.enabled = true;

            if (myHero.mAnimState.STATE != NpcAnimState.Run)
            {
                myHero.SendAnimMsg(WarMsg_Type.Running);
            }

			if (AITools.IsInRange (mTrans.position, 1, mTargetTrans.position))
			{
                myHero.SendAnimMsg(WarMsg_Type.Stand);

				pathFind.enabled = false;

				myHero.data.btData.atkerID = 0;
				myHero.data.btData.IsInBattle = false;

                myHero.data.btData.btStatus = NPCBattle_Status.RestForBlood;

				return TaskStatus.Success;
			}

			return TaskStatus.Running;
		}

		public override void OnEnd()
		{

		}
	}
}
