using UnityEngine;
using System.Collections;
using AW.War;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("follow the leader")]
	[TaskCategory("Hero")]
	public class Follow : Action
	{
		[Tooltip("移动速度")]
		public float speed;
		[Tooltip("移动角速度（转身速度）")]
		public float angularSpeed;
		[Tooltip("Agents less than this distance apart are neighbors")]
		public SharedFloat neighborDistance = 10;
		[Tooltip("队长身后多远的距离需要队员去跟随")]
		public SharedFloat leaderBehindDistance = 2;
		[Tooltip("队员之间的间距")]
		public SharedFloat separationDistance = 2;
		[Tooltip("The agent is getting too close to the front of the leader if they are within the aheadDistance")]
		public SharedFloat aheadDistance = 2;
		[Tooltip("队长")]
        public AIPath leader = null;
		[Tooltip("跟随队长的队员")]
        public AIPath[] agents = null;


        private WarServerCharactor chaPool;
		private WarMsgParam param;

        private ServerLifeNpc[] agentsHero;

		// The transform of the leader
		private Transform leaderTransform;
		// The corresponding transforms of the agents
		private Transform[] agentTransforms;

        private ServerLifeNpc myHero;

		public override void OnAwake()
		{
			param = new WarMsgParam ();
            myHero = GetComponent<ServerLifeNpc>();
		}

		public override void OnStart()
		{
            chaPool = WarServerManager.Instance.realServer.monitor.CharactorPool;
			InitData ();
		}

		void InitData()
		{
            if (!WarServerManager.Instance.bInit)
				return;

			//给leader赋值
//            leader = WarServerManager.Instance.npcMgr.ActiveHero.pathFinding;
			if (leader == null)
			{
				Debug.LogWarning ("leader is null");
				return;
			}
//			leaderTransform = WarClientManager.Instance.npcMgr.ActiveHero.transform;

			//给队员赋值
            agents = new AIPath[2];
            agentsHero = new ServerLifeNpc[2];
            ServerLifeNpc[] teams = chaPool.GetHeroList(AW.Data.CAMP.Player).ToArray();

			int index = 0;
			for (int i = 0; i < teams.Length; i++)
			{
//                if(teams[i] != WarServerManager.Instance.npcMgr.ActiveHero)
				{
					agents [index] = teams [i].pathFinding;
					agentsHero [index] = teams [i];
					index++;
				}
			}

			agentTransforms = new Transform[agents.Length];
			// Cache the transform of the agents
			for (int i = 0; i < agents.Length; ++i) 
			{
				agentTransforms[i] = agents[i].transform;
				agents[i].enabled = true;
				agents[i].speed = speed;
			}
		}

		// The agents will always be following the leader so always return running
		public override TaskStatus OnUpdate()
		{
			if (leader == null)
				InitData ();

			if (leader == null)
				return TaskStatus.Running;
				
			var behindPosition = LeaderBehindPosition();

			// Determine a destination for each agent

			bool arrive = true;
			for (int i = 0; i < agents.Length; ++i) 
			{
				if (Vector3.SqrMagnitude (agentTransforms [i].position - leaderTransform.position) < leaderBehindDistance.Value)
				{
					if (agents [i].enabled)
					{
						agents [i].enabled = false;
					}

                    if (agentsHero [i].mAnimState.STATE != NpcAnimState.Stand)
					{
//						param.cmdType = WarMsg_Type.Stand;
//						param.Sender = agentsHero [i].UniqueID;
//						param.Receiver = agentsHero [i].UniqueID;
//						agentsHero [i].SendMsg (agentsHero [i].UniqueID, param);
                        agentsHero[i].SendAnimMsg(WarMsg_Type.Stand);
					}
					continue;
				}

				arrive = false;
				if (!agents [i].enabled)
				{
					agents [i].enabled = true;
				}

				// Get out of the way of the leader if the leader is currently looking at the agent and is getting close
				if (LeaderLookingAtAgent(i) && Vector3.SqrMagnitude(leaderTransform.position - agentTransforms[i].position) < aheadDistance.Value) 
				{
                    if(myHero.mAnimState.canMove)
                    agents[i].destination = agentTransforms[i].position + (agentTransforms[i].position - leaderTransform.position).normalized * aheadDistance.Value;
				} 
				else
				{
					// The destination is the behind position added to the separation vector
                    if(myHero.mAnimState.canMove)
						agents[i].destination = behindPosition + DetermineSeparation(i);
				}

                if (myHero.mAnimState.STATE != NpcAnimState.Run)
				{
//					param.cmdType = WarMsg_Type.Running;
//					param.Sender = myHero.UniqueID;
//					param.Receiver = myHero.UniqueID;
//					myHero.SendMsg (myHero.UniqueID, param);
                    myHero.SendAnimMsg(WarMsg_Type.Running);
				}
			}

			if (arrive)
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Running;
		}

		public override void OnEnd()
		{
			// Disable the nav mesh
			for (int i = 0; i < agents.Length; ++i) {
				if (agents[i] != null)
					agents[i].enabled = false;
			}
		}

		private Vector3 LeaderBehindPosition()
		{
			// The behind position is the normalized inverse of the leader's velocity multiplied by the leaderBehindDistance
            return leaderTransform.position + (-leaderTransform.forward).normalized * leaderBehindDistance.Value;
		}

		// Determine the separation between the current agent and all of the other agents also following the leader
		private Vector3 DetermineSeparation(int agentIndex)
		{
			var separation = Vector3.zero;
			int neighborCount = 0;
			var agentTransform = agentTransforms[agentIndex];
			// Loop through each agent to determine the separation
			for (int i = 0; i < agents.Length; ++i) {
				// The agent can't compare against itself
				if (agentIndex != i) {
					// Only determine the parameters if the other agent is its neighbor
					if (Vector3.SqrMagnitude(agentTransforms[i].position - agentTransform.position) < neighborDistance.Value) {
						// This agent is the neighbor of the original agent so add the separation
						separation += agentTransforms[i].position - agentTransform.position;
						neighborCount++;
					}
				}
			}

			// Don't move if there are no neighbors
			if (neighborCount == 0) {
				return Vector3.zero;
			}
			// Normalize the value
			return ((separation / neighborCount) * -1).normalized * separationDistance.Value;
		}

		// Use the dot product to determine if the leader is looking at the current agent
		public bool LeaderLookingAtAgent(int agentIndex)
		{
			return Vector3.Dot(leaderTransform.forward, agentTransforms[agentIndex].forward) < -0.5f;
		}

		// Reset the public variables
		public override void OnReset()
		{
			speed = 10;
			angularSpeed = 10;
			neighborDistance = 10;
			leaderBehindDistance = 2;
			separationDistance = 2;
			aheadDistance = 2;
			leader = null;
			agents = null;
		}
	}
}
