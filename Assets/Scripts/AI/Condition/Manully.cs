using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace AW.AI {
	/// 
	/// 用来检测是否是手动控制的英雄
	/// 
	[TaskCategory("Dota")]
	[TaskDescription("Manully or Automatic")]
	public class Manully : Conditional 
	{
		public bool manully;

		public override TaskStatus OnUpdate ()
		{
			if (manully)
				return TaskStatus.Success;

			return TaskStatus.Failure;
		}
	}
}
