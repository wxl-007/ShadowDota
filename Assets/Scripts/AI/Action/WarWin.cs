using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UObj = UnityEngine.GameObject;
using AW.War;

namespace AW.AI {
	[TaskCategory("Dota/WarResult")]
	[TaskDescription("Does player win the war?")]
	public class WarWin : Action {

		public override TaskStatus OnUpdate() {
			ConsoleEx.DebugLog("");
			return TaskStatus.Success;
		}
	}

}