using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.AI;
using AW.Data;

namespace AW.AI
{
	[TaskDescription("比较建筑状态")]
	[TaskCategory("Hero")]
	public class CmpBuildType : Conditional
	{
		public SharedLifeNPC hero;

		public BuildNPCType curType;
		public bool self;
        private ServerLifeNpc npc;

		public override void OnStart()
		{
			if (self)
                npc = GetComponent<ServerLifeNpc> ();
			else
				npc = hero.Value;
		}

		public override TaskStatus OnUpdate()
		{
			if (hero.Value == null)
				return TaskStatus.Failure;

			//如果npc不是建筑，返回失败
			if (npc.data.configData.type != LifeNPCType.Build)
				return TaskStatus.Failure;


			if (curType == npc.data.configData.bldType)
				return TaskStatus.Success;

			return TaskStatus.Failure;
		}
	}
}