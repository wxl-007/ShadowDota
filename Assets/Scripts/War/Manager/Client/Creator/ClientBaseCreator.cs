using UnityEngine;
using AW.Data;

namespace AW.War {
	/// <summary>
	/// 战斗环境的创建的基类
	/// </summary>
	public class ClientBaseCreator {
		//War控制器
		protected WarClientManager warMgr;

		//挂载的点
		private GameObject mWarPoint;
		protected GameObject WarPoint {
			get {
				return mWarPoint ?? ( mWarPoint = GameObject.FindGameObjectWithTag("WarClient") );
			}
		}

		//场景挂载点
		private GameObject mScenePoint;
		protected GameObject ScenePoint
		{
			get
			{
				if (mScenePoint == null && WarPoint != null)
					mScenePoint = WarPoint.transform.FindChild ("Scene").gameObject;
				if (mScenePoint != null)
					return mScenePoint;
				return WarPoint;
			}
		}

	}

}
