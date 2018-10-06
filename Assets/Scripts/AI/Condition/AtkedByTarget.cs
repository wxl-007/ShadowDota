using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.AI;
using AW.Data;

namespace AW.AI
{
	[TaskDescription("正在被target攻击")]
	[TaskCategory("Hero")]
	public class AtkedByTarget : Conditional
	{
		public SharedLifeNPC target;
        private ServerLifeNpc npc;

		public override void OnStart()
		{
            npc = GetComponent<ServerLifeNpc> ();
		}

		public override TaskStatus OnUpdate()
		{
			if (npc.data.btData.atkerID == target.Value.UniqueID)
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}