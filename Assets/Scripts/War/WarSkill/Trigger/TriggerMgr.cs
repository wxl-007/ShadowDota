using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using AW.Message;
using AW.Data;
using AW.Framework;

#if DEBUG
using System.Diagnostics;
#endif
namespace AW.War {

	/// <summary>
	/// 触发器的容器，所有的人都将消息发到这个触发器这里
	/// 实际上是WarNpcMgr转发过来的
	/// 
	/// 同时自己也有个小的IOC容器
	/// 
	/// 触发条件的模式有2种
	/// 1. 监视WarNpcMgr的消息
	/// 2. 每一个WarManager的FixedUpdate都默认触发Trigger，这个Trigger再检测条件是否触发（比如判定有没有超出距离)
	/// </summary>
	public class TriggerMgr : IocMgr {

		//分配给Trigger的唯一ID
		static int allocIdx = 0;

		private Dictionary<WarMsg_Type, Type> Worker = null;

		/// <summary>
		/// 未使用的触发器
		/// </summary>
		private Dictionary<WarMsg_Type, List<ITriggerItem>> OnIdle = null;

		/// <summary>
		/// 正在工作的触发器
		/// </summary>
		private Dictionary<WarMsg_Type, List<ITriggerItem>> OnWork = null;

		/// <summary>
		/// Trigger依赖的基础模块
		/// </summary>
		private WarServerNpcMgr npcMgr;

		//把要工作的放入这个临时的工作列表，这样子就
		private List<ITriggerItem> workingCache = null;

		//Trigger配置文件的加载
		private SkTriggerModel triggerLoader;
		//trigger的释放技能
		public readonly TriggerCastor trigCastor;

		private TriggerMgr() : base() {
			Worker = new Dictionary<WarMsg_Type, Type>();
			triggerLoader = Core.Data.getIModelConfig<SkTriggerModel>();
			//找到所有的Trigger
			ScanTriggerClasses(Worker);
			//初始化备份和工作容器
			OnIdle = new Dictionary<WarMsg_Type, List<ITriggerItem>>();
			OnWork = new Dictionary<WarMsg_Type, List<ITriggerItem>>();

			workingCache = new List<ITriggerItem>();
			trigCastor = TriggerCastor.instance;
		}

		public static TriggerMgr Instance {
			get {
				return GenericSingleton<TriggerMgr>.Instance;
			}
		}

		public void Init(WarServerNpcMgr npcMgr) {
			this.npcMgr = npcMgr;
			trigCastor.init(npcMgr);
		}
	
		/// <summary>
		/// 监视消息
		/// </summary>
		public void Watching(MsgParam msg) {
			if(msg == null) return;

			WarMsgParam warMsg = msg as WarMsgParam;
			if(warMsg != null) {

				List<ITriggerItem> list = null;
				bool care = OnWork.TryGetValue(warMsg.cmdType, out list);
				if(care) {

					workingCache.Clear();
					workingCache.AddRange(list);
					int count = workingCache.Count;
					if(count > 0) {
						for(int i = 0; i < count; ++ i) {
							ITriggerItem handler = workingCache[i];
							handler.OnHappen(msg, npcMgr);
						}
					}

				}
			}
		}

		#region 创建Trigger的方法

		//一定有TriggerConfigData的触发器
		//使用Buff创建的Trigger（也可以传递null)
		public ITriggerItem CreateTrigger(int npcId, int trCfgNum, RtBufData buf) {
			TriggerConfigData cfg = triggerLoader.get(trCfgNum);
			Utils.Assert(cfg == null, "Create Trigger is fail because of trigger configure num = " + trCfgNum);
			return CreateTrigger(cfg.tEvent, npcId, trCfgNum, buf);
		}

		/// <summary>
		/// 没有TriggerConfigData的触发器
		/// 这个偏向于更加底层的逻辑
		/// </summary>
		/// <returns>The trigger.</returns>
		/// <param name="care">Care.</param>
		/// <param name="npcId">Npc identifier.</param>
		public ITriggerItem CreateTrigger(WarMsg_Type care, int npcId) {
			return CreateTrigger(care, npcId, -1, null);
		}

		/// <summary>
		/// 创建触发器, 
		/// </summary>
		private ITriggerItem CreateTrigger(WarMsg_Type care, int npcId, int trCfgNum, RtBufData buff) {
			int id = 0;
			ITriggerItem one = null;

			List<ITriggerItem> idleList = null;
			if(OnIdle.TryGetValue(care, out idleList)) {
				one = idleList.Count > 0 ? idleList[0] : null;
				if(one == null) {
					//Create the new one
					one = CreateNewTrigger(care, trCfgNum);
				} else {
					idleList.RemoveAt(0);
					TriggerConfigData cfg = null;
					if(trCfgNum > 0) {
						cfg = triggerLoader.get(trCfgNum);
					}
					((Trigger)one).Init(cfg, -1);
				}
			} else {
				//Create the new one
				one = CreateNewTrigger(care, trCfgNum);
			}

			id = one.GetID();

			List<ITriggerItem> workList = null;
			if(OnWork.TryGetValue(care, out workList)) {
				workList.Add(one);
			} else {
				workList = new List<ITriggerItem>();
				workList.Add(one);
				OnWork[care] = workList;
			}

			//挂在Npc上
			ServerNPC npc = WarServerManager.Instance.npcMgr.GetNPCByUniqueID(npcId);
			npc.addTrigger(id);

			((Trigger)one).HangUpNpcId = npcId;

			//关联的Buff是否存在
			if(buff != null) {
				((Trigger)one).HangUpBuffId = buff.ID;
				buff.TriggerID = id;
			}
				

			return one;
		}
			
		ITriggerItem CreateNewTrigger(WarMsg_Type care, int trCfgNum) {
			ITriggerItem imp = null;
			Type created = null;
			if(Worker.TryGetValue(care, out created)) {
				imp = (ITriggerItem)Activator.CreateInstance(created);
				TriggerConfigData cfg = null;
				if(trCfgNum > 0) {
					cfg = triggerLoader.get(trCfgNum);
				}

				((Trigger)imp).Init(cfg, allocIdx ++);
			}
			return imp;
		}

		#endregion

		#region 删除Trigger的方法
		/// <summary>
		/// 删除NPC身上特定的触发器
		/// </summary>
		/// <returns>The trigger.</returns>
		/// <param name="TriggerId">Trigger identifier.</param>
		/// <param name="NPCId">NPC identifier.</param>
		public ITriggerItem RemoveTrigger(WarMsg_Type care, int TriggerId, int npcId) {
			ITriggerItem one = null;

			List<ITriggerItem> list = null;
			//有队列
			if(OnWork.TryGetValue(care, out list)) {

				int count = list.Count;
				if(count > 0) {
					int idx = -1;
					for(int i = 0; i < count ; ++ i) {
						if(list[i].GetID() == TriggerId) {
							idx = i;
							one = list[i];
							break;
						}
					}
					//删除管理器里面的
					if(idx >= 0) {
						list.RemoveAt(idx);

						//移回Idle
						List<ITriggerItem> idleList = null;
						if(!OnIdle.TryGetValue(care, out idleList)) {
							idleList = new List<ITriggerItem>();
							OnIdle[care] = idleList;
						}

						one.OnRest();
						OnIdle[care].Add(one);
					}
				}

			}

			//挂在Npc上删除
			ServerNPC npc = WarServerManager.Instance.npcMgr.GetNPCByUniqueID(npcId);
			npc.rmTrigger(TriggerId);

			return one;
		}

		/// <summary>
		/// 删除NPC身上特定的触发器
		/// </summary>
		/// <returns>The trigger.</returns>
		/// <param name="TriggerId">Trigger identifier.</param>
		/// <param name="npcId">Npc identifier.</param>
		public ITriggerItem RemoveTrigger (int TriggerId, int npcId) {
			if(TriggerId <= 0) return null;

			ITriggerItem one = null;

			foreach(WarMsg_Type key in OnWork.Keys) {
				List<ITriggerItem> list = null;
				//有队列
				if(OnWork.TryGetValue(key, out list)) {

					int count = list.Count;
					if(count > 0) {
						int idx = -1;
						for(int i = 0; i < count ; ++ i) {
							if(list[i].GetID() == TriggerId) {
								idx = i;
								one = list[i];
								break;
							}
						}

						//删除管理器里面的
						if(idx >= 0) {
							list.RemoveAt(idx);

							//移回Idle
							List<ITriggerItem> idleList = null;
							if(!OnIdle.TryGetValue(key, out idleList)) {
								idleList = new List<ITriggerItem>();
								OnIdle[key] = idleList;
							}

							one.OnRest();
							OnIdle[key].Add(one);

							break;
						}
					}

				}
			}

			//挂在Npc上删除
			ServerNPC npc = WarServerManager.Instance.npcMgr.GetNPCByUniqueID(npcId);
			npc.rmTrigger(TriggerId);

			return one;
		}

		/// <summary>
		/// 删除NPC身上所有的触发器
		/// </summary>
		/// <param name="NpcId">Npc identifier.</param>
		public void RemoveAllTrigger(int NpcId) {

			//挂在Npc上删除
			ServerNPC npc = WarServerManager.Instance.npcMgr.GetNPCByUniqueID(NpcId);
			int[] toBeRm = npc.getTriggerList.ToArray();

			int rmCnt = toBeRm.Length;
			if(rmCnt > 0) {

				foreach(WarMsg_Type key in OnWork.Keys) {
					List<ITriggerItem> list = OnWork[key];
					int cnt = list.Count;
					if(cnt > 0) {

						List<int> rmed = new List<int>();
						///
						/// ------------ 找到待删除的Trigger ------
						///
						for(int i = 0; i < cnt; ++ i) {
							ITriggerItem one = list[i];
							bool found = Utils.inArray<int>(one.GetID(), toBeRm);

							//清理脏数据
							if(found) {
								rmed.Add(i);

								one.OnRest();
								//移动到Idle
								//移回Idle
								List<ITriggerItem> idleList = null;
								if(!OnIdle.TryGetValue(key, out idleList)) {
									idleList = new List<ITriggerItem>();
									OnIdle[key] = idleList;
								}

								OnIdle[key].Add(one);
							} 

						}

						rmed.Sort();
						int rmCount = rmed.Count;
						for(int i = rmCount - 1; i >= 0; i-- ) {
							list.RemoveAt(rmed[i]);
						}
					}

				}

			}

			npc.rmAllTrigger();

		}

		#endregion


		/// <summary>
		/// 重置所有的触发器，当离开战斗的时候
		/// </summary>
		public void Recycle() {
			foreach(WarMsg_Type key in OnWork.Keys) {
				List<ITriggerItem> list = OnWork[key];
				if(list.Count > 0) {
					List<ITriggerItem> idleList = null;
					bool hasQueue = OnIdle.TryGetValue(key, out idleList);
					if(!hasQueue) {
						idleList = new List<ITriggerItem>();
						OnIdle[key] = idleList;
					}

					foreach(ITriggerItem trigger in list) {
						trigger.OnRest();
						OnIdle[key].Add(trigger);
					}

				}

			}
		}

		/// <summary>
		/// 每个Tick都要检查一下有没有特别的Trigger需要触发。
		/// 1. 
		/// </summary>
		public void Update(float deltaTime) {
			List<ITriggerItem> perFrame = null;

			foreach(WarMsg_Type key in OnWork.Keys) {
				if( OnWork.TryGetValue(key, out perFrame) ) {
					int count = perFrame.Count;
					if(count > 0) {
						for(int i = 0; i < count; ++ i) {
							PerFrameTrigger frameTri = perFrame[i] as PerFrameTrigger;
							if(frameTri != null && frameTri.BeTriggered()) {
								frameTri.OnFixedUpdate();
							}
						}
					}
				}
			}

		}

	}
}
