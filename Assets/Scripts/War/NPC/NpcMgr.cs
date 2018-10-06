using System;
using AW.Message;
using System.Collections.Generic;
using UnityEngine;

namespace AW.War {

	public class NpcMgr<T> where T : BNPC  {
		/// 
		/// 常量的定义
		///
		public const int SPRINGLIFE = 51006;	//泉水
		public const int HP_PROP = 55001;		//血道具
		public const int ATK_PROP = 55002;		//攻击道具
		public const int DEF_PROP = 55003;		//防御道具
		public const int BASE = 51005;			//基地
		public const int TOWER = 51004;			//塔
		public const int BORN_POINT = 56001;	//对战英雄出生点
		public const int FRESH_NPC = 56002;		//NPC刷新点


		/// 
		/// ----- 所有的在战斗场景里面的NPC都在本字典里面--------
		/// 
		protected Dictionary<int, T> npcDic = null;

		/// <summary>
		/// 缓存的npc
		/// KEY 是num
		/// </summary>
		protected Dictionary<int, List<BNPC>> cacheNpcDic = null;

		//实际干活的, 结果表示能否出Queue
		private Func<bool> work;
		/// 
		/// ---------  线程安全的队列 ----------
		/// 
		protected ThreadSafeQueue<Func<bool>> TaskQueue = null;

		/// <summary>
		/// ----------  未完成的队列 ----------
		/// </summary>
		private List<Func<bool>> cachedUnFinishedItem = null;

		//战斗开始
		public virtual void Init() {
			TaskQueue   = new ThreadSafeQueue<Func<bool>> (256);
			cachedUnFinishedItem = new List<Func<bool>>();

			npcDic      = new Dictionary<int, T>();
			cacheNpcDic = new Dictionary<int, List<BNPC>> ();
		}

		//战斗结束
		public virtual void Destory() {
			TaskQueue.Clear();
			cachedUnFinishedItem.Clear();
			npcDic.Clear();
			cacheNpcDic.Clear();
		}

		#region dead npc cache data

		/// <summary>
		/// 在已经挂掉的npc缓冲中注册该npc
		/// </summary>
		/// <param name="npc">Npc.</param>
		public void SignDeadNpcCache(BNPC npc) {
			List<BNPC> npcList = null;
			if (cacheNpcDic.TryGetValue (npc.data.num, out npcList)) {
				bool bFind = false;
				for (int i = 0; i < npcList.Count; i++)
				{
					if (npcList [i].UniqueID == npc.UniqueID)
					{
						npcList [i] = npc;
						bFind = true;
						break;
					}
				}
				if (!bFind)
					npcList.Add (npc);
			} else {
				npcList = new List<BNPC> ();
				npcList.Add (npc);
				cacheNpcDic.Add (npc.data.num, npcList);
			}
		}

		/// <summary>
		/// 从缓存中根据num得到一个npc
		/// </summary>
		/// <returns>The npc from cache.</returns>
		/// <param name="num">Number.</param>
		public T GetNpcFromCache(int num)
		{
			List<BNPC> npcList = null;
			if (cacheNpcDic.TryGetValue (num, out npcList))
			{
				if(npcList != null && npcList.Count > 0)
					return (T) npcList [0];
			}
			return null;
		}

		/// <summary>
		/// 从缓存中删除NPC
		/// </summary>
		/// <param name="npc">Npc.</param>
		public void RemoveNpcFromCache(BNPC npc) {
			List<BNPC> npcList = null;
			if (cacheNpcDic.TryGetValue (npc.data.num, out npcList)) {
				for (int i = 0; i < npcList.Count; i++) {
					if (npcList [i].UniqueID == npc.UniqueID) {
						npcList.Remove (npcList[i]);
						return;
					}
				}
			}
		}

		#endregion

		//将需要异步处理的加入队列
		protected void AsyncSheduleTask (T npcTarget, MsgParam param) {
			Func<bool> action = () => {

				bool suc = true;
				if(npcTarget != null) {
					if(param.Delay <= 0.001f) {
						npcTarget.OnHandleMessage(param);
						suc = true;
					}
					else {
						param.Delay -= Time.deltaTime;
						suc = false;
					}
				}

				return suc;
			};

			TaskQueue.Enqueue (action);
		}


		public virtual void Update (float deltaTime) {
			work = null;
			while(TaskQueue.TryDequeue(out work)) {
				bool suc = work();
				if(suc == false) cachedUnFinishedItem.Add(work);
			}

			//reimport task
			int count = cachedUnFinishedItem.Count;
			if(count > 0) {
				for(int i = 0; i < count; ++ i) {
					TaskQueue.Enqueue(cachedUnFinishedItem[i]);
				}
			}
		}

	}
}