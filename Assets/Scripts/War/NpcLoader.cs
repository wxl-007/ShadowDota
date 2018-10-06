using UnityEngine;
using System.Reflection;
using System.Collections;
using AW.War;
using AW.Data;
using System.IO;
using URes = UnityEngine.Resources;
using AW.Framework;

namespace AW.Resources {

	/// <summary>
	/// Model loader. 载入3D模型的帮助类
	/// 只缓存自己队伍的信息，如果不在队伍，则不缓存. 跳转场景也不释放
	/// 额外缓存的模型，跳转场景一定会释放
	/// 
	/// NPC的路径：Resources/Pack/NPC
	/// 
	/// </summary>
	public class NpcLoader : ILoaderDispose {

		private const int MAX_CAPACITY = 20;
		//NPC的位置
		private const string NPC = "NPC";

		//保存自己队伍的缓存
		private IResourceLoader<Object> ObjLoader = null;
		//战斗过程中临时使用的缓存
        private IResourceLoader<Object> WeakObjLoader = null;

		private NPCModel NpcModel = null;

		/// 
		/// WarMgr会在战斗中注册，战斗结束会变为Null
		/// 
		private WarClientNpcMgr cliNpcMgr;

        protected Assembly assembly;

		public NpcLoader() {
			ObjLoader = new ResourceMgrMore(MAX_CAPACITY);
			WeakObjLoader = new ResourceMgrMore(MAX_CAPACITY);
		}

		///
		/// 先尝试从ObjLoader里面取，如果不存在则再去WeakObjLoader取
		/// 
		/// 但是如果是自己队伍里NPC则必须使用ObjLoader去创建
		/// 
		public ClientNPC Load(int num, int id, CAMP camp, GameObject WarPoint) {
			if(NpcModel == null) NpcModel = Core.Data.getIModelConfig<NPCModel>();

			NPCConfigData configData = NpcModel.get (num);

			if (configData == null)
			{
                ConsoleEx.DebugWarning ("not find npc config data ::  " + num);
				return null;
            }

			string path = Path.Combine(ResourceSetting.PACKROOT, NPC);
			path = Path.Combine(path, configData.model.ToString());

			//TODO: 是自己队伍里NPC则必须使用ObjLoader去创建

			bool cached = ObjLoader.hitCache(path);

			Object obj = null;
			if(cached) {
				obj = ObjLoader.Load(path);
			} else {
				obj = WeakObjLoader.Load(path);
			}
			if (obj == null)
			{
                ConsoleEx.DebugWarning (configData.model + " not find models. Npc Num = " + num);
				return null;
			}

			GameObject go = GameObject.Instantiate(obj) as GameObject;
			UnityUtils.AddChild_Reverse(go, WarPoint);

			///
			/// 数据的初始化过程
			///
			DynamicDataInit(go, num, camp, id);

			ClientNPC npc = go.GetComponent<ClientNPC> ();
            if (npc != null)
            {
                go.name = "NPC_" + npc.data.configData.ID + "_" + npc.UniqueID;
                if(!string.IsNullOrEmpty(configData.controlScript))
                {
                    ClientNpcAnimState animState = go.AddComponent(configData.controlScript) as ClientNpcAnimState;
                    npc.animState = animState;
                    animState.CachedNpc = npc;
                    animState.AttackCount = configData.normalHit.Length;
                    npc.broadcast = animState.OnNewStateReceived;
                }
                cliNpcMgr.CreateNpcUI(npc);
            }

			return npc;
		}

		/// <summary>
		/// 创建NPC, 使用初始化好的数据
		/// </summary>
		/// <param name="num">Number.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="camp">Camp.</param>
		public ClientNPC Load(CAMP camp, NPCData initedData, GameObject WarPoint) {
			#if DEBUG
			Utils.Assert(initedData == null, "NpcLoad can't be null ");
			#endif

			NPCConfigData configData = initedData.configData;

			string path = Path.Combine(ResourceSetting.PACKROOT, NPC);
			path = Path.Combine(path, configData.model.ToString());

			//TODO: 是自己队伍里NPC则必须使用ObjLoader去创建

			bool cached = ObjLoader.hitCache(path);
			Object obj = null;
			if(cached) {
				obj = ObjLoader.Load(path);
			} else {
				obj = WeakObjLoader.Load(path);
			}

			if (obj == null) {
				ConsoleEx.DebugWarning (configData.model + " not find models. Npc Num = " + configData.ID);
				return null;
			}

			GameObject go = GameObject.Instantiate(obj) as GameObject;
			UnityUtils.AddChild_Reverse(go, WarPoint);
			///
			/// 填充数据
			///
			InitedData(go, camp, initedData);

			ClientNPC npc = go.GetComponent<ClientNPC> ();
            if (npc != null)
            {
                go.name = "NPC_" + npc.data.configData.ID + "_" + npc.UniqueID;
                if(!string.IsNullOrEmpty(configData.controlScript))
                {
                    ClientNpcAnimState animState = go.AddComponent(configData.controlScript) as ClientNpcAnimState;
                    npc.animState = animState;
                    npc.broadcast = animState.OnNewStateReceived;
                }
                cliNpcMgr.CreateNpcUI(npc);
            }

			return npc;
		}

		/// <summary>
		/// 数据的初始化
		/// </summary>
		/// <param name="go">Go.</param>
		/// <param name="num">Number.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="camp">Camp.</param>
		void DynamicDataInit(GameObject go, int num, CAMP camp, int Id) {

			ClientNPC curHero = go.GetComponent<ClientNPC>();
			///
			/// 填充阵营
			///
			curHero.Camp = camp;

			///
			/// 填充NPC数据
			///
			NPCData dynamicData = new NPCData ();
			NPCConfigData econfig = NpcModel.get (num);
			dynamicData.rtData = new NPCRuntimeData (econfig);
			dynamicData.configData = econfig;
			dynamicData.btData = new NPCBattleData ();
			curHero.data = dynamicData;

			if(Id != 0) {
                curHero.UniqueID = Id;
				cliNpcMgr.SignExistID(curHero);
			} else {
				///
				/// 向WarClientNpcManager注册
				///
				cliNpcMgr.SignID (curHero);
			}

		}

		/// <summary>
		/// 数据的初始化
		/// </summary>
		/// <param name="go">Go.</param>
		/// <param name="camp">Camp.</param>
		/// <param name="initedData">Inited data.</param>
		void InitedData(GameObject go, CAMP camp, NPCData initedData) {
			ClientNPC curHero = go.GetComponent<ClientNPC>();
			///
			/// 填充阵营
			///
			curHero.Camp = camp;

			///
			/// 填充NPC数据
			///
			NPCData dynamicData = initedData;
			dynamicData.btData = new NPCBattleData ();

			curHero.data = dynamicData;

			///
			/// 向WarClientNpcManager注册
			///
			cliNpcMgr.SignID (curHero);
		}


		public void ClearCache (bool gc) {
			#if DEBUG
			ConsoleEx.DebugLog("Model Loader don't need to clear team cache when jumping scene.");
			#endif

			WeakObjLoader.ClearCache(gc);
		}

		/// <summary>
		/// 战斗开始
		/// </summary>
		public void OnWarStart(WarClientNpcMgr regWar) {
			cliNpcMgr = regWar;
		}

		/// <summary>
		/// 战斗结束
		/// </summary>
		public void OnWarEnd() {
			cliNpcMgr = null;
			ClearCache(true);
		}

	}
}