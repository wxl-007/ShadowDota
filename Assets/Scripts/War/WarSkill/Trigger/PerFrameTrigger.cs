using System;

namespace AW.War {

	/// <summary>
	/// 当被处罚之后，每一帧都会被调用的Trigger，
	/// </summary>
	public abstract class PerFrameTrigger : Trigger {

		/// <summary>
		/// 这个需要检测每一个帧，有没有出区域
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public override bool TickPerFrame () {
			return true;
		}

		/// <summary>
		/// 被触发了吗？如果被触发了，则可以每一帧都调用
		/// </summary>
		/// <returns><c>true</c>, if triggered was been, <c>false</c> otherwise.</returns>
		public abstract bool BeTriggered();

		/// <summary>
		/// 每一帧要做的逻辑
		/// </summary>
		public abstract void OnFixedUpdate();
	}
}
