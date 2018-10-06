using UnityEngine;
using AW.War;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("如果距离队长的距离超过这个范围，由战斗转为跟随")]
	[TaskCategory("Hero")]
	public class InFleeToFlowRange : Conditional
	{
		[Tooltip("队长")]
        private ServerLifeNpc leader;
		[Tooltip("脱战跟随范围")]
		private float fleetofollowRange;

		public override void OnStart()
		{
//            leader = WarServerManager.Instance.npcMgr.ActiveHero;
            ServerLifeNpc npc = GetComponent<ServerLifeNpc>();
			fleetofollowRange = npc.data.configData.fleeRange;
		}

		public override TaskStatus OnUpdate()
		{
			//如果在范围内，返回成功
			if (AITools.IsInRange (this.transform.position, fleetofollowRange, leader.transform.position))
				return TaskStatus.Success;

			return TaskStatus.Failure;
		}

		public override void OnEnd()
		{

		}

		public override void OnDrawGizmos()
		{
			#if UNITY_EDITOR
			var oldColor = UnityEditor.Handles.color;
			UnityEditor.Handles.color = Color.yellow;
			UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.up, fleetofollowRange);
			UnityEditor.Handles.color = oldColor;
			#endif
		}
	}
}
