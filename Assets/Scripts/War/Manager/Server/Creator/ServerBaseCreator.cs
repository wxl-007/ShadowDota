using System;
using UnityEngine;
using AW.Data;

namespace AW.War {
	public class ServerBaseCreator {

		//服务器端的Manager
		protected WarServerManager WarSMgr;

		//挂载的点
		private GameObject mWarPoint;
		protected GameObject WarPoint {
			get {
				return mWarPoint ?? ( mWarPoint = GameObject.FindGameObjectWithTag("WarServer") );
			}
		}
		/// <summary>
		/// 虚拟 触发器的接收器
		/// </summary>
		private ServerNPC mTriggerPoint;
		protected ServerNPC WarTriggerPoint {
			get {
				return mTriggerPoint ?? ( mTriggerPoint = createTrigger() );
			}
		}

		/// <summary>
		/// 目前，Trigger的这个触发器，仅仅用来触发场景里面的，和技能无关
		/// 技能触发器在WarManager里面
		/// </summary>
		/// <returns>The trigger.</returns>
		protected ServerNPC createTrigger() {
			GameObject go = new GameObject("Trigger");
			UnityUtils.AddChild_Reverse(go, WarPoint);
			ServerNPC npc = go.AddComponent<ServerNPC>();

			npc.Group = 0;
			npc.Camp  = CAMP.None;

			///
			/// 向WarManager注册
			///

			if(WarSMgr != null) {
				WarSMgr.npcMgr.SignID(npc);
				WarSMgr.npcMgr.RegisterTagNpc("Trigger", npc);

				///
				/// --- 这里创建的都是逻辑型触发器，没有配置 ----
				/// --- UseSkill 和 OnKilled都有转发的功能 ----
				///

				WarSMgr.triMgr.CreateTrigger(WarMsg_Type.BeKilled,   npc.UniqueID);
				WarSMgr.triMgr.CreateTrigger(WarMsg_Type.OnAttacked, npc.UniqueID);
				WarSMgr.triMgr.CreateTrigger(WarMsg_Type.BeAttacked, npc.UniqueID);
				WarSMgr.triMgr.CreateTrigger(WarMsg_Type.UseSkill,   npc.UniqueID);
				WarSMgr.triMgr.CreateTrigger(WarMsg_Type.OnKilled,   npc.UniqueID);

			}

			return npc;
		}

	}

}