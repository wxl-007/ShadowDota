using System;
using AW.Message;
using System.Collections.Generic;
using AW.Resources;
using AW.Framework;

namespace AW.War {

	/// <summary>
	/// 客户端的Npc管理类
	/// 
	/// </summary>
	public class WarClientNpcMgr : NpcMgr<ClientNPC>, IMsgSender {

		public WarClientNpcMgr() {

		}

        //当前控制的主英雄
        public ClientNPC ActiveHero;

		public override void Init () {
			base.Init ();
			//war开始之后，注册所有NPC的ID
			Core.ResEng.getLoader<NpcLoader>().OnWarStart(this);
		}

		public override void Destory() {
			base.Destory();
			Core.ResEng.getLoader<NpcLoader>().OnWarEnd();
		}

		#region MsgSender implementation

		public bool SendMessage (int senderID, int recID, MsgParam param) {

			bool sent = false;
			if(param == null) {
				ConsoleEx.DebugLog("We can't send empty message to other.", ConsoleEx.RED);
				return sent;
			}

			ClientNPC npcTarget = null;
			if(npcDic.TryGetValue(recID, out npcTarget)) {
				if(npcTarget != null) {
					sent = true;
					npcTarget.OnHandleMessage(param);
				} else {
					sent = false;
				}
			} else {
				sent = false;
				ConsoleEx.DebugLog("NPC doesn't instanciate yet.", ConsoleEx.YELLOW);
			}

			return sent;
		}

		public bool SendMessageAsync (int senderID, int recID, MsgParam param) {
			bool sent = false;
			if(param == null) {
				ConsoleEx.DebugLog("We can't send empty message to other.", ConsoleEx.RED);
				return sent;
			}

			ClientNPC npcTarget = null;
			if(npcDic.TryGetValue(recID, out npcTarget)) {
				sent = true;
				AsyncSheduleTask(npcTarget, param);
			} else {
				sent = false;
				ConsoleEx.DebugLog("NPC doesn't instanciate yet.", ConsoleEx.YELLOW);
			}

			return sent;
		}

		#endregion

		public int SignID(ClientNPC npc) {
			Utils.Assert(npc == null, "Entity is null when signing unique id.");
			if(npc == null) return -1;

			if(npc.UniqueID != -1) {
				bool found = npcDic.ContainsKey(npc.UniqueID);
				if(found) return npc.UniqueID;
			}

			//The instance id of an object is always guaranteed to be unique.
			int uniqueId    = npc.GetInstanceID();
			npc.UniqueID    = uniqueId;

			npcDic[uniqueId] = npc;

            if(createUIForNpc != null)
            {
                NGUIDebug.Log(npc.gameObject.name + "::::");
                createUIForNpc(npc);
            }

			return uniqueId;
		}

		//已经分配ID
		public int SignExistID(ClientNPC npc) {
			Utils.Assert(npc == null, "Entity is null when signing unique id.");
			if(npc == null) return -1;

			if(npc.UniqueID != -1) {
				bool found = npcDic.ContainsKey(npc.UniqueID);
				if(found) return npc.UniqueID;
			}

			//The instance id of an object is always guaranteed to be unique.
			int uniqueId = npc.UniqueID;
            npcDic[uniqueId] = npc;

			return uniqueId;
		}


        /// <summary>
        /// 根据uniqueID得到NPC
        /// </summary>
        /// <returns>The npc.</returns>
        /// <param name="uniqueID">Unique I.</param>
        public ClientNPC GetNpc(int uniqueID)
        {
            ClientNPC npc = null;
            if(npcDic.TryGetValue(uniqueID, out npc))
                return npc;

            ConsoleEx.DebugWarning("not find npc :: " + uniqueID);
            return null;
        }


        #region Npc的UI
        public Action<ClientNPC> createUIForNpc = null;
        public void CreateNpcUI(ClientNPC npc)
        {
            if(createUIForNpc != null)
            {
                createUIForNpc(npc);
            }
        }
        #endregion
	}

}
