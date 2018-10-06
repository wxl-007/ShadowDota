using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UObj = UnityEngine.GameObject;
using AW.War;

namespace AW.AI {

	[TaskCategory("Dota/Building")]
	[TaskDescription("Is the Building alive?")]
	public class BuildingAlive : Conditional {

		[Tooltip("Tag Of Building")]
		public string Tag;
		//基地
		private UObj MilitaryBase;

		public override void OnAwake() {

		}

		public override TaskStatus OnUpdate() {
			if (MilitaryBase == null) {
				ConsoleEx.DebugLog("MilitaryBase is null");
				return TaskStatus.Failure;
			}

			return TaskStatus.Success ;
		}

		public override void OnReset() {

		}
	}
}

