using UnityEngine;
using AW.Message;
using AW.War;
using AW.Data;
using AW.Framework;
using System;
using System.Collections.Generic;
using AW.Resources;
using System.Linq;

namespace AW.War {
	/// 
	/// 服务器端的逻辑
	/// 
	/// WarServerNpcMgr的管理类，WarManager会知道所有的NPC（LifeNPC和NoneLifeNPC）的存活情况。
	/// WarServerNpcMgr会分别向LifeNPC和NoneLifeNPC分配不同区间的唯一ID
	/// WarServerNpcMgr也是底层战斗逻辑向UI层发送动画请求的入口
	/// 
	/// WarServerNpcMgr在场景跳转的时候会删除所有的资源
	/// 

	public class WarServerNpcMgr : NpcMgr<ServerNPC>, IMsgSender, IMsgSendWar {

		/// <summary>
		/// 按照group来划分的NPC 表
		/// </summary>
		private Dictionary<int, List<ServerNPC>> groupNpcDic = null;

		/// <summary>
		/// 按照阵营来划分的NPC 表
		/// </summary>
		private Dictionary<CAMP, List<ServerNPC>> campNicDic = null;

		/// <summary>
		/// 特殊NPC，比如Trigger
		/// Key = Tag
		/// </summary>
		private Dictionary<String, ServerNPC> specNpcDic = null;

		/// <summary>
		/// 触发器
		/// </summary>
		public TriggerMgr triMgr;

		/// <summary>
		/// 战斗的入口在这里
		/// </summary>
		public override void Init() {
			base.Init();

			groupNpcDic= new Dictionary<int, List<ServerNPC>>();
			campNicDic = new Dictionary<CAMP, List<ServerNPC>>();
			specNpcDic = new Dictionary<string, ServerNPC>();
			//war开始之后，注册所有NPC的ID
			Core.ResEng.getLoader<VirtualNpcLoader>().OnWarStart(this);

			//触发器的初始化
			triMgr = TriggerMgr.Instance;
		}
		/// <summary>
		/// 战斗结束的入口
		/// </summary>
		public override void Destory() {
			base.Destory();
			Core.ResEng.getLoader<VirtualNpcLoader>().OnWarEnd();
			triMgr.Recycle();
		}

		/// <summary>
		/// 完成加载后，分析数据的内容
		/// </summary>
		public virtual void AnalyzeIfComplete() {
			throw new NotImplementedException();
		}

		#region MsgSender implementation

		public bool SendMessage (int senderID, int recID, MsgParam param) {

			bool sent = false;
			if(param == null) {
				ConsoleEx.DebugLog("We can't send empty message to other.", ConsoleEx.RED);
				return sent;
			}

			ServerNPC npcTarget = null;
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

			triMgr.Watching(param);
			return sent;
		}

		public bool SendMessageAsync (int senderID, int recID, MsgParam param) {
			bool sent = false;
			if(param == null) {
				ConsoleEx.DebugLog("We can't send empty message to other.", ConsoleEx.RED);
				return sent;
			}

			ServerNPC npcTarget = null;
			if(npcDic.TryGetValue(recID, out npcTarget)) {
				sent = true;
				AsyncSheduleTask(npcTarget, param);
			} else {
				sent = false;
				ConsoleEx.DebugLog("NPC doesn't instanciate yet.", ConsoleEx.YELLOW);
			}

			triMgr.Watching(param);
			return sent;
		}

		#endregion

		#region IMsgSendWar implementation

		public bool sendMessage (CAMP sender, CAMP receiver, WarCampMsg param, bool anonymous) {

			throw new NotSupportedException();

			/*
			 * 所有的消息，都基于异步，不再支持同步方法
			 * 
			bool sent = false;
			if(param == null) {
				ConsoleEx.DebugLog("We can't send empty message to other.", ConsoleEx.RED);
				return sent;
			}

			List<ServerNPC> npcList = null;

			if(campNicDic.TryGetValue(receiver, out npcList)) {
				int count = npcList.Count;
				if(count > 0) {
					for(int i = 0; i < count; ++ i) {
						ServerNPC npc = npcList[i];
						npc.OnHandleMessage(param);
					}
					sent = true;
				} else {
					sent = false;
					ConsoleEx.DebugLog("NPC doesn't instanciate yet. Receive Camp = " + receiver.ToString(), ConsoleEx.YELLOW);
				}
			} else {
				sent = false;
				ConsoleEx.DebugLog("NPC doesn't instanciate yet. Receive Camp = " + receiver.ToString(), ConsoleEx.YELLOW);
			}

			triMgr.Watching(param);

			return sent;
			*/
		}

		public bool sendMessage (int groupSend, int groupRece, WarGroupMsg param, bool anonymous) {

			throw new NotSupportedException();

			/*
			 * 所有的消息，都基于异步，不再支持同步方法
			 * 
			bool sent = false;
			if(param == null) {
				ConsoleEx.DebugLog("We can't send empty message to other.", ConsoleEx.RED);
				return sent;
			}

			List<ServerNPC> npcList = null;

			if(groupNpcDic.TryGetValue(groupRece, out npcList)) {
				int count = npcList.Count;
				if(count > 0) {
					for(int i = 0; i < count; ++ i) {
						ServerNPC npc = npcList[i];
						npc.OnHandleMessage(param);
					}
					sent = true;
				} else {
					sent = false;
					ConsoleEx.DebugLog("NPC doesn't instanciate yet. Receive Group ID = " + groupRece.ToString(), ConsoleEx.YELLOW);
				}
			} else {
				sent = false;
				ConsoleEx.DebugLog("NPC doesn't instanciate yet. Receive Group ID = " + groupRece.ToString(), ConsoleEx.YELLOW);
			}

			triMgr.Watching(param);

			return sent;
			*/
		}

		public bool SendMessageAsync (CAMP sender, CAMP receiver, WarCampMsg param, bool anonymous) {
			bool sent = false;
			if(param == null) {
				ConsoleEx.DebugLog("We can't send empty message to other.", ConsoleEx.RED);
				return sent;
			}

			List<ServerNPC> npcList = null;

			if(campNicDic.TryGetValue(receiver, out npcList)) {
				int count = npcList.Count;
				if(count > 0) {
					for(int i = 0; i < count; ++ i) {
						ServerNPC npc = npcList[i];
						AsyncSheduleTask(npc, param);
					}
					sent = true;
				} else {
					sent = false;
					ConsoleEx.DebugLog("NPC doesn't instanciate yet.", ConsoleEx.YELLOW);
				}
			} else {
				sent = false;
				ConsoleEx.DebugLog("NPC doesn't instanciate yet.", ConsoleEx.YELLOW);
			}

			triMgr.Watching(param);

			return sent;
		}

		public bool SendMessageAsync (int groupsend, int groupRece, WarGroupMsg param, bool anonymous) {
			bool sent = false;
			if(param == null) {
				ConsoleEx.DebugLog("We can't send empty message to other.", ConsoleEx.RED);
				return sent;
			}

			List<ServerNPC> npcList = null;

			if(groupNpcDic.TryGetValue(groupRece, out npcList)) {
				int count = npcList.Count;
				if(count > 0) {
					for(int i = 0; i < count; ++ i) {
						ServerNPC npc = npcList[i];
						AsyncSheduleTask(npc, param);
					}
					sent = true;
				} else {
					sent = false;
					ConsoleEx.DebugLog("NPC doesn't instanciate yet. Receive Group ID = " + groupRece.ToString(), ConsoleEx.YELLOW);
				}
			} else {
				sent = false;
				ConsoleEx.DebugLog("NPC doesn't instanciate yet. Receive Group ID = " + groupRece.ToString(), ConsoleEx.YELLOW);
			}

			triMgr.Watching(param);

			return sent;
		}

		#endregion

		public WarServerNpcMgr() { }

		/// <summary>
		/// 如果npc已经加入容器，则不考虑重复加入
		/// </summary>
		public virtual int SignID (ServerNPC npc) {
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

			List<ServerNPC> npcList = null;
			if(groupNpcDic.TryGetValue(npc.Group, out npcList)) {
				npcList.Add(npc);
			} else {
				npcList = new List<ServerNPC>();
				npcList.Add(npc);
				groupNpcDic[npc.Group] = npcList;
			}

			npcList = null;
			if(campNicDic.TryGetValue(npc.Camp, out npcList)) {
				npcList.Add(npc);
			} else {
				npcList = new List<ServerNPC>();
				npcList.Add(npc);
				campNicDic[npc.Camp] = npcList;
			}

			return uniqueId;
		}

		/// <summary>
		/// 主次一个带有Tag的NPC，这些NPC都是特殊的NPC
		/// </summary>
		/// <param name="tag">Tag.</param>
		/// <param name="npc">Npc.</param>
		public void RegisterTagNpc (string tag, ServerNPC npc) {
			if(npc != null && tag != null) specNpcDic[tag] = npc;
		}

		/// <summary>
		/// 获取一个特殊Tag的NPC
		/// </summary>
		/// <returns>The npc.</returns>
		/// <param name="tag">Tag.</param>
		public ServerNPC TagNpc (string tag) {
			ServerNPC tagged = null;
			if(specNpcDic.TryGetValue(tag, out tagged)) {
				return tagged;
			} else {
				return null;
			}
		}

		#region 获取npc数据的对外方法

		/// <summary>
		/// 获取阵营的BNPC列表, 默认获取活着的BNPC
		/// </summary>
		/// <returns>The camp bnpc.</returns>
		public IEnumerable<ServerNPC> getCampBnpc(CAMP camp, LiveOrDie live = LiveOrDie.Live) {
			List<ServerNPC> bnpc = new List<ServerNPC>();

			///
			/// ------- 依次测试CAMP，从容器中获取 -------
			///

			CAMP toTest = CAMP.None;
			List<ServerNPC> outValue = null;

			///检查是否有Enemy
			bool ck = camp.check(CAMP.Enemy);
			if(ck) {
				toTest = CAMP.Enemy;

				if (campNicDic.TryGetValue (toTest, out outValue))
				{
					bnpc.AddRange(outValue);
				}
			}

			ck = camp.check(CAMP.Player);
			if(ck) {
				toTest = CAMP.Player;

				if (campNicDic.TryGetValue (toTest, out outValue))
				{
					bnpc.AddRange(outValue);
				}
			}

			ck = camp.check(CAMP.Neutral);
			if(ck) {
				toTest = CAMP.Neutral;

				if (campNicDic.TryGetValue (toTest, out outValue))
				{
					bnpc.AddRange(outValue);
				}
			}

			IEnumerable<ServerNPC> itor = null;

			int count = bnpc.Count;
			if(count > 0) {
				if(live == LiveOrDie.Live)
					itor = bnpc.Where( n => n.data != null && n.data .rtData != null && n.data.rtData.curHp > 0);
				else if(live == LiveOrDie.Die)
					itor = bnpc.Where( n => n.data != null && n.data .rtData != null && n.data.rtData.curHp <= 0);
				else
					itor = bnpc.AsEnumerable<ServerNPC>();
			} else {
				itor = bnpc.AsEnumerable<ServerNPC>();
			}

			return itor;
		}

		/// <summary>
		/// 返回阵营Camp的LifeNPC列表
		/// </summary>
		/// <returns>The life NPC list by camp.</returns>
		/// <param name="camp">Camp.</param>
		public List<ServerLifeNpc> GetLifeNPCListByCamp(CAMP camp) {
			IEnumerable<ServerNPC> npcs = getCampBnpc (camp);
			List<ServerLifeNpc> list = new List<ServerLifeNpc>();
			foreach(ServerNPC npc in npcs) {
				ServerLifeNpc life = npc as ServerLifeNpc;
				if(life != null) list.Add(life);
			}
			return list;
		}

		/// <summary>
		/// 根据编队获得npc列表
		/// </summary>
		/// <returns>The NPC list by group.</returns>
		/// <param name="group">Group.</param>
		public List<ServerNPC> GetNPCListByGroup(int group)
		{
			List<ServerNPC> outValue = null;

			if (groupNpcDic.TryGetValue (group, out outValue))
				return outValue;
			return null;
		}

		/// <summary>
		/// 根据camp和group得到npc
		/// </summary>
		/// <returns>The NPC list.</returns>
		/// <param name="camp">Camp.</param>
		/// <param name="Group">Group.</param>
		public List<ServerNPC> GetNPCList(CAMP camp, int Group)
		{
			List<ServerNPC> list = GetNPCListByGroup (Group);
			if (list != null)
			{
				int count = list.Count;
				if(count > 0) {
					List<ServerNPC> final = new List<ServerNPC> ();
					for (int i = 0; i < count; i++)
					{
						if (list [i].Camp == camp)
						{
							final.Add (list [i]);
						}
					}
					return final;
				}
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>根据唯一ID得到npc
		/// <param name="uniqeuID">Uniqeu I.</param>
		public ServerNPC GetNPCByUniqueID(int uniqeuID)
		{
			ServerNPC npc = null;
			if (npcDic.TryGetValue (uniqeuID, out npc))
			{
				return npc;
			}
			return null;
		}

		/// <summary>
		/// 根据配表ID得到npc
		/// </summary>
		/// <returns>The NPC by number.</returns>
		/// <param name="num">Number.</param>
		/// <param name="camp">Camp.</param>
		public List<ServerNPC> GetNPCListByNum(int num, CAMP camp)
		{
			List<ServerNPC> list = new List<ServerNPC> ();
			foreach(KeyValuePair<int, ServerNPC> itor in npcDic)
			{
				if (itor.Value != null && itor.Value.data != null && itor.Value.data.configData != null && itor.Value.data.configData.ID == num)
				{
					if (camp == CAMP.None)
					{
						list.Add (itor.Value);
					} 
					else if (itor.Value.Camp == camp)
					{
						list.Add (itor.Value);
					}
				}
			}
			return list;
		}

		/// <summary>
		/// 根据LifeNPCType类型和camp得到npc
		/// </summary>
		/// <returns>The life NPC by type.</returns>
		/// <param name="type">Type.</param>
		/// <param name="camp">Camp.</param>
		public List<ServerLifeNpc> GetLifeNPCByType(LifeNPCType type, CAMP camp)
		{
			List<ServerLifeNpc> list = new List<ServerLifeNpc> ();
			foreach(KeyValuePair<int, ServerNPC> itor in npcDic)
			{
				ServerLifeNpc lifenpc = itor.Value as ServerLifeNpc;
				if (lifenpc != null && lifenpc.WhatTypeOf == type)
				{
					if (camp == CAMP.None)
					{
						list.Add (lifenpc);
					} 
					else if (lifenpc.Camp == camp)
					{
						list.Add (lifenpc);
					}
				}
			}
			return list;
		}

		public List<ServerNPC> GetNPCByType(LifeNPCType type, CAMP camp)
		{
			List<ServerNPC> list = null;
			foreach(KeyValuePair<int, ServerNPC> itor in npcDic)
			{
				if (itor.Value != null && itor.Value.data != null && itor.Value.data.configData.type == type)
				{
					if (camp == CAMP.None || camp == CAMP.All)
					{
						if (list == null)
							list = new List<ServerNPC> ();
						list.Add (itor.Value);
					} 
					else if (itor.Value.Camp == camp)
					{
						if (list == null)
							list = new List<ServerNPC> ();
						list.Add (itor.Value);
					}
				}
			}
			return list;
		}

		//按兵路得到建筑
		public List<ServerLifeNpc> GetBuildByWay(CAMP camp, BATTLE_WAY way, bool bAlive = false)
		{
			List<ServerLifeNpc> list = GetLifeNPCByType (LifeNPCType.Build, camp);
			if (list != null && list.Count > 0)
			{
				List<ServerLifeNpc> finalList = new List<ServerLifeNpc> ();
				for (int i = 0; i < list.Count; i++)
				{
					ServerLifeNpc bld = list[i];
					if (bAlive)
					{
						if (bld.dataInScene != null && bld.dataInScene.way == way && bld.IsAlive)
						{
							finalList.Add (list [i]);
						}
					}
					else
					{
						if (list [i].dataInScene != null && list [i].dataInScene.way == way)
						{
							finalList.Add (list [i]);
						}
					}
				}
				return finalList;
			}
			return null;
		}

        //按兵路得到建筑
        public List<ServerLifeNpc> GetBuildByType(CAMP camp, BuildNPCType type)
        {
            List<ServerLifeNpc> list = new List<ServerLifeNpc>();
            foreach(KeyValuePair<int, ServerNPC> itor in npcDic)
            {
                ServerLifeNpc lifenpc = itor.Value as ServerLifeNpc;
                if (lifenpc != null && lifenpc.WhatTypeOf == LifeNPCType.Build && lifenpc.IsAlive && lifenpc.data.configData.bldType == type)
                {
                    if (camp == CAMP.None || camp == CAMP.All)
                    {
                        list.Add (lifenpc);
                    } 
                    else if (lifenpc.Camp == camp)
                    {
                        list.Add (lifenpc);
                    }
                }
            }

            return list;
        }

        //删除npc
        public void RemoveNpc(int uniqueID)
        {
            ServerNPC npc = null;
            if (npcDic.TryGetValue(uniqueID, out npc))
            {
                npcDic.Remove(uniqueID);
            }

            if (npc == null)
                return;

            List<ServerNPC> list = null;
            if (campNicDic.TryGetValue(npc.Camp, out list))
            {
                int len = list.Count;
                for (int i = 0; i < len; i++)
                {
                    if (list[i].UniqueID == uniqueID)
                    {
                        list.Remove(list[i]);
                        break;
                    }
                }
            }

            list.Clear();
            if (groupNpcDic.TryGetValue(npc.Group, out list))
            {
                int len = list.Count;
                for (int i = 0; i < len; i++)
                {
                    if (list[i].UniqueID == uniqueID)
                    {
                        list.Remove(list[i]);
                        break;
                    }
                }
            }

            foreach (KeyValuePair<string, ServerNPC> itor in specNpcDic)
            {
                if (itor.Value.UniqueID == uniqueID)
                {
                    specNpcDic.Remove(itor.Key);
                    break;
                }
            }
        }

		#endregion

		public override void Update (float deltaTime) {
			base.Update(deltaTime);
			#region 更新NPC

			int count = npcDic.Count;
			if(count > 0) {
				foreach(BNPC bnpc in npcDic.Values) {
					ServerLifeNpc lnpc = bnpc as ServerLifeNpc;
					if(lnpc != null) {
						lnpc.UpdateTarget(deltaTime);

						if(lnpc.runSkMd != null)
							lnpc.runSkMd.Update(deltaTime);
					}
				}
			}

			#endregion
		}
	}
}
