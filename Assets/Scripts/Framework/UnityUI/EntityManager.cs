using UnityEngine;
using System.Collections.Generic;
using AW.Message;
using AW.Framework;
using System.IO;

namespace AW.Entity {
	/// 
	/// Entity的管理类，EntityManager会知道所有的UI（MonobehaviorEx和ControllerEx）的存活情况。
	/// EnityManager会分别向MonobehaviorEx和ControllerEx分配不同区间的唯一ID
	/// EventCenter的回调会通过EntityManager来决定回调给谁，是否回调等问题。
	/// 
	public class EntityManager : IMsgSender, MsgSenderEx {
		#region MsgSenderEx implementation

		public bool sendMessage (LogicalType sender, LogicalType receiver, MsgParam param, bool anonymous, MsgRecType sure) {
			bool sent = false;

			int senderID = -1;
			int recID    = -1;
			bool found1  = false;
			bool found2  = false;

			if(TypeIDRelation.TryGetValue(receiver, out recID)) {
				found1 = true;
			}

			if(anonymous || TypeIDRelation.TryGetValue(sender, out senderID)) {
				found2 = true;
			}

			if(found1 && found2) {
				sent = SendMessage(senderID, recID, param);
			} else {
				if(found1 == false) {
					ConsoleEx.DebugLog("We can't find " + receiver + " Controller 's instance", ConsoleEx.RED);
					if(sure == MsgRecType.MakeSure) {
						createController(senderID, receiver, param);
					}
				}

				if(found2 == false) {
					ConsoleEx.DebugLog("We can't find " + sender + "Controller 's instance", ConsoleEx.RED);
				}
			}

			return sent;
		}

		public bool SendMessageAsync (LogicalType sender, LogicalType receiver, MsgParam param, bool anonymous, MsgRecType sure) {
			bool sent = false;

			int senderID = -1;
			int recID    = -1;
			bool found1  = false;
			bool found2  = false;

			if(TypeIDRelation.TryGetValue(receiver, out recID)) {
				found1 = true;
			}

			if(anonymous || TypeIDRelation.TryGetValue(sender, out senderID)) {
				found2 = true;
			}

			if(found1 && found2) {
				sent = SendMessageAsync(senderID, recID, param);
			} else {
				if(found1 == false) {
					ConsoleEx.DebugLog("We can't find " + receiver + "Controller 's instance", ConsoleEx.RED);
					if(sure == MsgRecType.MakeSure) {
						createController(senderID, receiver, param);
					}
				}

				if(found2 == false) {
					ConsoleEx.DebugLog("We can't find " + sender + "Controller 's instance", ConsoleEx.RED);
				}
			}

			return sent;
		}

		#endregion

		#region MsgSender implementation
		public bool SendMessage (int senderID, int recID, MsgParam param) {
			param.Sender   = senderID;
			param.Receiver = recID;

			bool sent = false;
			if(param == null) {
				ConsoleEx.DebugLog("We can't send empty message to other.", ConsoleEx.RED);
				return sent;
			}
			
			if(uiCollection.ContainsKey(recID)) {
				ConsoleEx.DebugLog("We can't send message to UI layer.", ConsoleEx.RED);
				return sent;
			}
				
			ControllerEx CtrlEx = null;
			if(ctlCollection.TryGetValue(recID, out CtrlEx)) {
				if(CtrlEx != null) {
					sent = true;
					CtrlEx.UI_OnReceive(param);
				} else {
					sent = false;
				}
			} else {
				sent = false;
				ConsoleEx.DebugLog("Controller doesn't instanciate yet.", ConsoleEx.YELLOW);
			}

			return sent;
		}

		public bool SendMessageAsync (int senderID, int recID, MsgParam param) {
			param.Sender   = senderID;
			param.Receiver = recID;

			bool sent = false;
			if(param == null) {
				ConsoleEx.DebugLog("We can't send empty message to other.", ConsoleEx.RED);
				return sent;
			}

			if(uiCollection.ContainsKey(recID)) {
				ConsoleEx.DebugLog("We can't send message to UI layer.", ConsoleEx.RED);
				return sent;
			}

			ControllerEx CtrlEx = null;
			if(ctlCollection.TryGetValue(recID, out CtrlEx)) {
				sent = true;
				AsyncTask.QueueOnMainThread (
					() => { if(CtrlEx != null) CtrlEx.UI_OnReceive(param); }, param.Delay
				);
			} else {
				sent = false;
				ConsoleEx.DebugLog("Controller doesn't instanciate yet.", ConsoleEx.YELLOW);
			}

			return sent;
		}

		#endregion

		/// 
		/// 下面两个，是Entity的实体类容器
		/// 
		public Dictionary<int, MonoBehaviorEx> uiCollection = null;
		public Dictionary<int, ControllerEx> ctlCollection  = null;

		/// 
		/// ControllerEx的ID和LogicalType关系的容器
		/// 
		public Dictionary<LogicalType, int> TypeIDRelation = null;

		/// 
		/// 分析整个Assembly，找到所有要反射生成的Type
		/// 
		private ImplicitBinder binder;

		public EntityManager() {
			uiCollection  = new Dictionary<int, MonoBehaviorEx>();
			ctlCollection = new Dictionary<int, ControllerEx>();

			TypeIDRelation = new Dictionary<LogicalType, int>();

			binder = ImplicitBinder.Instance;
		}

		/// 
		/// 根据Entity的类型分配唯一ID
		/// 
		public int SignID (Entity entity) {
			Utils.Assert(entity == null, "Entity is null when signing unique id.");
			if(entity == null) return -1;

			//The instance id of an object is always guaranteed to be unique.
			int uniqueId    = entity.GetInstanceID();
			entity.UniqueID = uniqueId;

			if(entity.getEntityType == EntityType.Entity_Control) {
				ControllerEx ctlEx = (ControllerEx) entity;
				ctlCollection[uniqueId] = ctlEx;
				TypeIDRelation[ctlEx.CtrlType] = uniqueId;

			} else if(entity.getEntityType == EntityType.Entity_UI) {
				uiCollection[uniqueId]  = (MonoBehaviorEx) entity;
			}

			return uniqueId;
		}

		/// 
		/// 当Entity调用OnDestory的时候， 清理对Entity的引用
		/// 
		public void ClearEntity (Entity entity) {
			Utils.Assert(entity == null, "Entity is null when signing unique id.");
			if(entity == null) return;

			int uniqueId = entity.UniqueID;

			if(entity.getEntityType == EntityType.Entity_Control) {
				ControllerEx ctlEx = (ControllerEx) entity;

				if(ctlCollection.ContainsKey(uniqueId)) 
					ctlCollection.Remove(uniqueId);

				if(TypeIDRelation.ContainsKey(ctlEx.CtrlType)) 
					TypeIDRelation.Remove(ctlEx.CtrlType);
				
			} else if(entity.getEntityType == EntityType.Entity_UI) {
				if(uiCollection.ContainsKey(uniqueId)) 
					uiCollection.Remove(uniqueId);
			}

		}

		/// <summary>
		/// 根据唯一ID获取Entity
		/// </summary>
		public ControllerEx getControllerByID (int ID) {
			ControllerEx entity = null;
			if(ctlCollection.ContainsKey(ID)) {
				ctlCollection.TryGetValue(ID, out entity);
			}
			return entity;
		}
		/// <summary>
		/// 根据LogicalType获取Entity
		/// </summary>
		public ControllerEx getEntityByLogicalType (LogicalType type) {
			ControllerEx entity = null;
			int ID = -1;
			if(TypeIDRelation.TryGetValue(type, out ID)) {
				entity = getControllerByID(ID);
			}
			return entity;
		}

		/// 
		/// 只有当消息的接受者不存在的时候，才会去考虑创建消息的接受者
		/// 创建Controller
		/// 
		void createController(int senderID, LogicalType receiver, MsgParam param) {
			string name = receiver.ToString();
			GameObject go = new GameObject(name);
			go.AddComponent(binder.getController(name));
			UnityUtils.AddChild(go, ControllerMain.CtrlMain.gameObject);

			ControllerEx ctrlEx = go.GetComponent<ControllerEx>();
			ctrlEx.CtrlType = receiver;
			AsyncTask.QueueOnMainThread( () => { 
				param.Sender = senderID;
				param.Receiver = go.GetInstanceID();
				ctrlEx.UI_OnReceive(param); 
			}, 1f );
		}

	}
}