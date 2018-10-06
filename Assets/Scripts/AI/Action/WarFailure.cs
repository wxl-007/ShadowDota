using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UObj = UnityEngine.GameObject;
using AW.War;

namespace AW.AI {
	[TaskCategory("Dota/WarResult")]
	[TaskDescription("Does player fail the war?")]
	public class WarFailure : Action {

		public override TaskStatus OnUpdate() {
			ConsoleEx.DebugLog("");
			return TaskStatus.Success;
		}
	}

}