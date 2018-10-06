using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AW.Message;
using AW.Data;
using fastJSON;
using BehaviorDesigner.Runtime;

namespace AW.War {
	
	//npc刷新参数
	[Serializable]
	public class NPCRefreshParam
	{
		public float starDelayTime;				//多久以后开始刷新
		public float timePerCount;				//没波怪之间的时间间隔
		public float timePerNPC;				//同一波怪，每个npc之间的刷新间隔
		public int freshCount;					//刷怪次数（ -1 认为是无限刷怪）
		public int freshPoolID;					//刷新池的ID
	}

	
	/// 
	/// 战斗的NPC基类
	/// 
	public class ServerNPC : BNPC {

        private IpcNpcMoveMsg moveMsg;
        private Transform mTrans;

		public override void OnHandleMessage (MsgParam param) {
			base.OnHandleMessage(param);
            if(broadcast != null)
            {
                WarMsgParam msg = param as WarMsgParam;
                if (msg != null)
                {
                    broadcast(msg);
                }
            }
		}

        private WarSrcAnimParam animParam;

        #region 初始值
        public Vector3 spawnPos;
        public Quaternion spawnRot;
        #endregion

        #region 子NPC
        protected List<ServerNPC> childNpc;
        public List<ServerNPC> getChildNpc
        {
            get{ return childNpc;}
        }

        /// <summary>
        /// 将子npc加入列表
        /// </summary>
        /// <param name="child">Child.</param>
        public void addChildNpc(ServerNPC child)
        {
            if(!childNpc.Contains(child))
            {
                childNpc.Add(child);
            }
        }

        public ServerNPC getOneChildNpc(int id)
        {
            ServerNPC child = childNpc.Find(n => n.UniqueId == id);
            return child;
        }

        public ServerNPC removeChild(int id)
        {
            ServerNPC child = childNpc.Find(n => n.UniqueId == id);
            removeChild(child);
            return child;
        }

        /// <summary>
        /// 将子npc移出列表
        /// </summary>
        /// <param name="child">Child.</param>
        public void removeChild(ServerNPC child)
        {
            if(childNpc.Contains(child))
            {
                childNpc.Remove(child);
                childNpc.TrimExcess();
            }
        }
        #endregion

		#region Buff的操作
		/// <summary>
		/// Buff列表
		/// </summary>
		private List<int> BuffList;

		public List<int> getBuffList {
			get { return BuffList; }
		}

		public void addBuff(int bufId) {
			BuffList.Add(bufId);
		}

		/// <summary>
		/// 删除特定的Buff
		/// </summary>
		/// <param name="bufId">Buffer identifier.</param>
		public void rmBuff(int bufId) {

			int idx = -1;

			int count = BuffList.Count;
			if(count > 0) {
				for(int i = 0; i < count; ++ i) {
					int buf = BuffList[i];
					if(buf == bufId) {
						idx = i;
						break;
					}
				}
			}

			if(idx >= 0 && idx < count) {
				BuffList.RemoveAt(idx);
			}
		}

		/// <summary>
		/// 删除所有的Buff
		/// </summary>
		public void rmAllBuff() {
			BuffList.Clear();
		}

		#endregion

		#region Trigger的操作
		/// <summary>
		/// 触发器列表
		/// </summary>
		private List<int> TriggerList;
		public List<int> getTriggerList {
			get {
				return TriggerList;
			}
		}

		public void addTrigger(int triggerId) {
			TriggerList.Add(triggerId);
		}

		public void rmTrigger(int triggerId) {
			int idx = -1;

			int count = TriggerList.Count;
			if(count > 0) {
				for(int i = 0; i < count; ++ i) {
					int trig = TriggerList[i];
					if(trig == triggerId) {
						idx = i;
						break;
					}
				}
			}

			if(idx >= 0 && idx < count) {
				TriggerList.RemoveAt(idx);
			}
		}

		public void rmAllTrigger() {
			TriggerList.Clear();
		}

		#endregion

		#region 仇恨

		struct Hatredd {
			//仇恨值- 0和1
			public int hatred;
			//添加时间
			public float time;
		}

		//Key is BNPC ID， Value is hatredValue
		private Dictionary<int, Hatredd> hatredList;
		//获取最高仇恨
		//现在Value不保存，实际的仇恨值，而是保持的bool变量
		//根据时间的先后，来判定谁是最高优先级
		//这是一个比较高复杂度的逻辑
		public int getHighestHatred {
			get {
				int high = -1;
				float max = 0F;
				foreach(int key in hatredList.Keys) {
					Hatredd edd = hatredList[key];
					//只要>=1，就算有嘲讽
					if(edd.hatred >= 1) {

						ServerNPC bnpc = WarServerManager.Instance.npcMgr.GetNPCByUniqueID(key);
						if(bnpc != null && bnpc.data.rtData.curHp > 0) {

							if(edd.time > max) {
								max = edd.time;
								high = key;
							}

						}
					}
				} 

				return high;
			}
		}

		//设置仇恨值
		public void addHatred (int BNPCID, int Hatred) {
			float cur = Time.time;
			Hatredd h = new Hatredd() {
				hatred = Hatred,
				time   = cur,
			};

			if(hatredList.ContainsKey(BNPCID)) {
				h.hatred = hatredList[BNPCID].hatred + h.hatred;
				hatredList[BNPCID] = h;
			} else {
				hatredList[BNPCID] = h;
			}

		}

		//清楚所有的仇恨值
		public void clearHatred() {
			hatredList.Clear();
		}

		//清除特定的仇恨值
		public void clearSpecHatred(int BNPCID) {
			if(hatredList.ContainsKey(BNPCID)) {
				hatredList.Remove(BNPCID);
			}
		}

		#endregion

		//战斗结束，停止所有的AI行为
		void BattleOver()
		{
			BehaviorTree[] trees = GetComponents<BehaviorTree>();
			if (trees != null && trees.Length > 0)
			{
				for (int i = 0; i < trees.Length; i++)
				{
					trees [i].DisableBehavior ();
					trees [i].enabled = false;
				}
			}
		}

		public virtual void Awake() {
			BuffList    = new List<int>();
			TriggerList = new List<int>();
			hatredList  = new Dictionary<int, Hatredd>();
            childNpc    = new List<ServerNPC>();

            animParam = new WarSrcAnimParam();
		}

		public virtual void Start() {
            mTrans = transform;
            InvokeRepeating("SendNpcMoveMsg", 0.1f, 0.1f); 
		}

		public virtual void OnDestroy() {
            if(data != null && data.configData != null) {
				data.rtData = new NPCRuntimeData(data.configData);
				data.rtData.curHp = 0;
			}

			///
			/// ---- 解除挂载的Trigger ----
			///
			WarServerManager.Instance.triMgr.RemoveAllTrigger(UniqueId);

			///
			/// ---- 解除挂载的Buff ----
			/// 
			WarServerManager.Instance.bufMgr.rmAllBuff(UniqueId);

			//--- 解除仇恨信息 ----
            if(hatredList != null)
			    hatredList.Clear();
		}

        /// <summary>
        /// 这个方法主要用于在技能中创建npc的初始化工作
        /// </summary>
        /// <param name="owner">Owner.</param>
        /// <param name="param">Parameter.</param>
        public virtual void Init(ServerNPC owner, WarMsgParam param)
        {
            
        }
     
        //发送动画消息
        public void SendAnimMsg(WarMsg_Type type)
        {
            animParam.cmdType = type;
            animParam.Sender = UniqueID;
            animParam.Receiver = UniqueID;
            WarServerManager.Instance.npcMgr.SendMessage(UniqueID, UniqueID, animParam);
        }

        void SendNpcMoveMsg()
        {
            SendNpcMoveMsg(false);
        }

        public void SendNpcMoveMsg(bool forceMove = false)
        {
            if (data != null && data.configData.moveable == Moveable.Movable)
            {
                if (moveMsg == null)
                    moveMsg = new IpcNpcMoveMsg();

                moveMsg.uniqueId = UniqueID;
                moveMsg.pos = VectorWrap.ToVector(mTrans.position);
                moveMsg.rotation = QuaternionWrap.ToLpcQuaternion(mTrans.rotation);
                moveMsg.forceMove = forceMove;
                WarServerManager.Instance.realServer.proxyCli.NPCMove(moveMsg);
            }
        }
    }
}
