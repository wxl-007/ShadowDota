using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.AI;
using AW.Data;

namespace AW.AI
{
	[TaskDescription("英雄是否是嘲讽状态")]
	[TaskCategory("Hero")]
	public class IsTaunted : Conditional
	{
		public SharedLifeNPC target;		//目标
        private ServerLifeNpc npc;

		public override void OnStart()
		{
            npc = GetComponent<ServerLifeNpc> ();
		}

		//如果是嘲讽状态，而且嘲讽者或者，返回成功
		public override TaskStatus OnUpdate()
		{
			if (npc.curStatus.AnySame (NpcStatus.Taunt))
			{
                BNPC caster = WarServerManager.Instance.npcMgr.GetNPCByUniqueID (npc.getHighestHatred);
                if (caster != null && caster is ServerLifeNpc)
				{
                    ServerLifeNpc lifeTarget = caster as ServerLifeNpc;
					if (lifeTarget != null && lifeTarget.IsAlive)
					{
						target.Value = lifeTarget;
						return TaskStatus.Success;
					}
				}
			}
			return TaskStatus.Failure;
		}
	}
}