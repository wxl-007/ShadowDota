using UnityEngine;
using System.Collections;
using AW.War;
using AW.Data;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("move toward the target")]
	[TaskCategory("Hero")]
	public class Seek : Action
	{
		public SharedLifeNPC target;

        private AIPath pathFind;
        private ServerLifeNpc npc;
		WarMsgParam param;
		private Transform mTrans;
		private Transform mTargetTrans;


		public override void OnStart()
		{
            npc = GetComponent<ServerLifeNpc>();
			if (npc == null)
				Debug.LogWarning ("npc is null");

			if (npc.data == null)
				Debug.LogWarning ("npc.data is null");

			mTrans = npc.transform;
			mTargetTrans = target.Value.transform;
            pathFind = npc.pathFinding;

			param = new WarMsgParam ();

			if (npc.data.configData.moveable == Moveable.Movable)
			{
				pathFind.speed = npc.data.configData.speed;
				pathFind.enabled = true;
                pathFind.destination = target.Value.transform.position;
			}
		}

		public override TaskStatus OnUpdate()
		{
			//如果建筑不能移动，
			if(npc.data.configData.moveable == Moveable.BeStatic)
				return TaskStatus.Success;

			//如果对方死了，返回失败
			if (!target.Value.IsAlive)
				return TaskStatus.Failure;

			//如果进入攻击范围，返回true
			if (AITools.IsInRange(mTrans.position, npc.ATKRange + target.Value.data.configData.radius, mTargetTrans.position))
			{
				pathFind.enabled = false;
				return TaskStatus.Success;
			}

			if (!pathFind.enabled)
				pathFind.enabled = true;

            if(npc.mAnimState.canMove)
                pathFind.destination = target.Value.transform.position;

            if (npc.mAnimState.STATE != NpcAnimState.Run)
			{
//				param.cmdType = WarMsg_Type.Running;
//				param.Sender = npc.UniqueID;
//				param.Receiver = npc.UniqueID;
//				npc.SendMsg (npc.UniqueID, param);
                npc.SendAnimMsg(WarMsg_Type.Running);
			}

			return TaskStatus.Running;
		}


	}
}
