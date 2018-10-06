using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UObj = UnityEngine.GameObject;
using AW.War;

namespace AW.AI {
	[TaskCategory("Hero")]
	[TaskDescription("设置npc的战斗状态")]
	public class SetNPCStatus : Action 
	{
        private ServerLifeNpc myHero;
		public NPCBattle_Status curStatus;

		public override void OnAwake()
		{
            myHero = GetComponent<ServerLifeNpc>();
		}

		public override TaskStatus OnUpdate() 
		{
			myHero.data.btData.btStatus = curStatus;
			return TaskStatus.Success;
		}
	}
}