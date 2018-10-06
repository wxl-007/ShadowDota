using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace AW.AI {
	///
	/// 检测是否遇见敌人，这个遇见敌人依靠两者视野距离来判定.
	/// 如果遇见了某群敌人中的一个，则整个群体敌人都判定为遇敌
	/// 
	/// 发现敌人返回Success，没有返回Failure
	/// 
	[TaskCategory("Dota")]
	[TaskDescription("Do our Heros find enemy?")]
	public class InSight : Conditional {
		public SharedGameObjectList EnemyList;


	}
}
