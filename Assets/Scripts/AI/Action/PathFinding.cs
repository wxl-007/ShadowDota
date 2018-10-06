using UnityEngine;
using System.Collections;
using AW.War;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using AW.Data;
using System.Collections.Generic;
using Pathfinding;

namespace AW.AI
{
	[TaskDescription("move toward the target")]
	[TaskCategory("Hero")]
	public class PathFinding : Action
    {
		//寻路的目标点
        public List<ServerLifeNpc> targets;
        private List<ServerLifeNpc> tempList;
        private SortedList<int, ServerLifeNpc> sortList;

		//当前寻路的目标索引
		public SharedInt index;

		//寻路脚本
        private AIPath pathFind;
		//npc脚本
        private ServerLifeNpc npc;
		private WarMsgParam param;

		private bool bInitFinish;

		private Transform mTrans;
		private Transform mTargetTrans;

        private IpcNpcAnimMsg animMsg;

		public override void OnAwake()
		{
            npc = GetComponent<ServerLifeNpc>();
            pathFind = npc.pathFinding;
            param = new WarMsgParam ();
            tempList = new List<ServerLifeNpc>();
            sortList = new SortedList<int, ServerLifeNpc>();
            targets = new List<ServerLifeNpc> ();
			mTrans = npc.transform;
            animMsg = new IpcNpcAnimMsg();
		}

		public override void OnStart()
		{
			InitData ();
		}

		void InitData()
		{
            if (!WarServerManager.Instance.bInit)
				return;

			CAMP camp = CAMP.Neutral;
			if (npc.Camp == CAMP.Enemy)
				camp = CAMP.Player;
			else if (npc.Camp == CAMP.Player)
				camp = CAMP.Enemy;

			//得到敌方阵营的所有目标点
			if (camp != CAMP.Neutral)
			{
				tempList.Clear ();
				sortList.Clear ();
				targets.Clear ();

                tempList = WarServerManager.Instance.npcMgr.GetBuildByWay (camp, npc.data.btData.way);
				for (int i = 0; i < tempList.Count; i++)
				{
					sortList.Add (tempList [i].dataInScene.index, tempList [i]);
				}

                foreach (KeyValuePair<int, ServerLifeNpc> itor in sortList)
				{
					targets.Add (itor.Value);
				}
					
                NeHeQiaoNpcMgr npcMgr = WarServerManager.Instance.npcMgr as NeHeQiaoNpcMgr;
				if(camp == CAMP.Player)
					targets.Add (npcMgr.SelfMilitaryBase);
				else if(camp == CAMP.Enemy)
					targets.Add (npcMgr.EnemyMilitaryBase);
			}
				
            if (targets != null && targets.Count > 0 && index.Value < targets.Count)
				mTargetTrans = targets [index.Value].transform;

			bInitFinish = true;
		}

		public override TaskStatus OnUpdate()
		{
			if (!bInitFinish)
			{
				InitData ();
				return TaskStatus.Running;
			}

			if(targets == null || targets.Count == 0)
			{
				if (npc.outLog)
				{
					ConsoleEx.DebugError ("all the emermy is dead");
				}
				return TaskStatus.Failure;
			}
				
			npc.data.btData.atkerID = 0;
			npc.data.btData.IsInBattle = false;

			//如果跑到最后一个目标点，赢了
			if (index.Value >= targets.Count)
			{
//				param.cmdType = WarMsg_Type.Idle;
//				param.Sender = npc.UniqueID;
//				param.Receiver = npc.UniqueID;
//				npc.SendMsg (npc.UniqueID, param);

                npc.SendAnimMsg(WarMsg_Type.Stand);

				return TaskStatus.Success;
			}

			pathFind.speed = npc.data.configData.speed;

            if (npc.mAnimState.canMove)
			{
				if (!pathFind.enabled)
					pathFind.enabled = true;
                if(pathFind != null && mTargetTrans != null)
                 pathFind.destination = mTargetTrans.position;
			}

			//如果跑到了当前位置，或者当前目标死亡，切换下一个目标，返回true
			if (AITools.IsInRange (mTrans.position, 2, mTargetTrans.position) || !targets[index.Value].IsAlive)
			{
				index.Value++;
			}
				
            if (npc.mAnimState.STATE != NpcAnimState.Run)
			{
                npc.SendAnimMsg(WarMsg_Type.Running);
			}

			return TaskStatus.Running;
		}
	}
}