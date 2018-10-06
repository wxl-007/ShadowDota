using UnityEngine;
using System.Collections;

namespace AW.Entity {

	///
	/// Entity的类型
	/// 
	public enum EntityType {
		//对应MonoBehaviorEx
		Entity_UI = 0x01,
		//对应ControllerEx
		Entity_Control = 0x02,
		//父类型
		Entity_Base = 0x03,
	}

	/// 
	/// Entity 实体类，是对于MonoBehavior的封装。这个类不允许直接使用。
	/// MonoBehaviorEx 和ControllerEx 都是Entity的子类。
	/// MonoBehaviorEx用来决定UI的显示
	/// ControllerEx用来控制MonoBehaviorEx
	/// 
    public class Entity : MonoBehaviour {
		protected EntityType mEntityType;
		public EntityType getEntityType {
			get { 
				return mEntityType;
			}
		}

		//唯一ID
		protected int UniqueId;
		public int UniqueID {
			get {
				return UniqueId;
			}
			set {
				UniqueId = value;
			}
		}
	}
}

