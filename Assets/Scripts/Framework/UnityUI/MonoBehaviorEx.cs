using UnityEngine;
using System.Collections;

namespace AW.Entity {
	/// 
	/// UI展示类
	/// 
	public class MonoBehaviorEx : Entity {
		void Awake() {
			mEntityType = EntityType.Entity_UI;
			Core.EntityMgr.SignID(this);
		}

		void OnDestory() {
			Core.EntityMgr.ClearEntity(this);
		}

	}
}

