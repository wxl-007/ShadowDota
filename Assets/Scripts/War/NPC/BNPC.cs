using UnityEngine;
using AW.Message;
using fastJSON;
using AW.Data;
using System.Collections.Generic;
using System;

namespace AW.War {
    public class BNPC : MonoBehaviour {
		//阵营
		public CAMP Camp {
			get;
			set;
		}

		//NPC逻辑组的概念， 逻辑组必须是1，2，4，8...
		public int Group;

		/// <summary>
		/// npc数据类
		/// </summary>
		public NPCData data
		{
			get;
			set;
		}
           
        /// <summary>
        /// 场景编辑器里面的配置数据
        /// </summary>
        /// <value>The data in scene.</value>
        public NPCInSceneData dataInScene
        {
            get;
            set;
        }

        //上中下哪一路
        public BATTLE_WAY way
        {
            get;
            set;
        }

		//唯一ID
		protected int UniqueId = -1;
		public int UniqueID {
			get {
				return UniqueId;
			}
			set {
				UniqueId = value;
			}
		}

		/// <summary>
		/// 敌对的NPC ID
		/// </summary>
		protected int TargetUniqueId = -1;
		public int TargetID {
			get {
				return TargetUniqueId;
			}
			set {
				if(value != -1) RstTimeout();
				TargetUniqueId = value;
			}
		}

		protected float TargetTimeout = Consts.Target_TimeOut;
		public void RstTimeout() {
			TargetTimeout = Consts.Target_TimeOut;
		}

		/// <summary>
		/// 判定锁定目标的超时
		/// </summary>
		/// <param name="delta">Delta.</param>
		public void UpdateTarget(float delta) {
			TargetTimeout -= delta;
			if(TargetTimeout <= 0) {
				TargetID = -1;
				TargetTimeout = 0f;
			}
		}

        /// <summary>
        /// 是否远程单位
        /// 0:近战
        /// 1:远程
        /// </summary>
        /// <value><c>true</c> if is renged unit; otherwise, <c>false</c>.</value>
        public bool isRengedUnit
        {
            get
            { 
                return data.configData.ranged == 1;
            }
        }

		//是否要输出log
		public bool outLog = false;

        #region 接收消息
        public Action<WarMsgParam> broadcast = null;
        /// <summary>
        /// 收到发送过来的消息
        /// </summary>
        /// <param name="param">Parameter.</param>
        public virtual void OnHandleMessage (MsgParam param) {
            #if DEBUG
			if(param != null) {
				WarMsgParam msg = param as WarMsgParam;
				bool print = true;
				if(msg != null) {
					IpcMsg ipc = msg.param as IpcMsg;
					if(ipc != null && ipc.op == OP.NpcMove)
						print = false;
				}
					
				if(print)
					ConsoleEx.DebugLog("Msg is Received : \n " + JSON.Instance.ToJSON(param));
			}
                
            #endif
        }

        #endregion

        /// 
        /// 是什么类型
        /// 
		public virtual KindOfNPC WhatKindOf() {
			return KindOfNPC.Life;
		}

    }
}