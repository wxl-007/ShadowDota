using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("如果在攻击范围内，返回true")]
	[TaskCategory("Hero")]
	public class InAtkRange : Conditional
	{
		[Tooltip("目标敌人")]
		public SharedLifeNPC target;
		[Tooltip("攻击范围")]
		public float atkRange;

		public override TaskStatus OnUpdate()
		{
			//如果在范围内，返回成功
			if (AITools.IsInRange (this.transform.position, atkRange, target.Value.transform.position))
				return TaskStatus.Success;

			return TaskStatus.Failure;
		}

		public override void OnEnd()
		{

		}
	}
}

