using System;

namespace AW.Event {

	public interface IDispatch {

		/// <summary>
		/// 将每个任务分发的功能
		/// </summary>
		void Dispatch(BaseTaskAbstract task);
	}
}
