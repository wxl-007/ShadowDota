using UnityEngine;
using System.Collections;
using AW.War;
using AW.Data;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("move toward the target")]
	[TaskCategory("Hero")]
	public class SkillSeek : Action
	{
		public SharedLifeNPC target;			//当前攻击目标
		public SharedRtSkillData curSkill;		//当前使用技能

		private float seekRange;
        private AIPath pathFind;
        private ServerLifeNpc npc;
		WarMsgParam param;

		private Transform mTrans;
		private Transform mTargetTrans;

		public override void OnAwake()
		{
            npc = GetComponent<ServerLifeNpc>();
			param = new WarMsgParam ();

            if (npc.data.configData.moveable == Moveable.Movable)
                pathFind = npc.pathFinding;
		}


		public override void OnStart()
		{
			if (npc.data.configData.moveable == Moveable.Movable)
			{
				pathFind.speed = npc.data.configData.speed;
				pathFind.enabled = true;
                if(npc.mAnimState.canMove)
                    pathFind.destination = target.Value.transform.position;
			}

			seekRange = npc.data.configData.radius;
			if(curSkill.Value != null && target.Value != null)
				seekRange += curSkill.Value.skillCfg.Distance + target.Value.data.configData.radius;

			mTrans = npc.transform;
			mTargetTrans = target.Value.transform;
		}

		public override TaskStatus OnUpdate()
		{
			//如果建筑不能移动，
			if(npc.data.configData.moveable == Moveable.BeStatic)
				return TaskStatus.Success;

			//如果对方死了，返回失败
			if (!target.Value.IsAlive)
				return TaskStatus.Failure;

			//如果进入技能范围，返回true
			if (AITools.IsInRange(mTrans.position, seekRange,  mTargetTrans.position))
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
