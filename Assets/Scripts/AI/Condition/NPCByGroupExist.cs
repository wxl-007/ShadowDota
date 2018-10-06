using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.AI;
using AW.Data;
using System.Collections.Generic;

namespace AW.AI
{
	[TaskDescription("npc是否还存在by group")]
	[TaskCategory("Hero")]
	public class NPCByGroupExist : Conditional
	{
		public SharedInt npcGroup;
		private BNPC npc;
		public override void OnStart()
		{
			npc = GetComponent<BNPC>();
		}

		public override TaskStatus OnUpdate()
		{
            List<ServerNPC> npcList = WarServerManager.Instance.npcMgr.GetNPCList (npc.Camp, npcGroup.Value);
			if (npcList != null && npcList.Count > 0)
			{
				return TaskStatus.Success;
			}

			return TaskStatus.Failure;
		}

		public override void OnEnd()
		{

		}
	}
}