using UnityEngine;
using System.Collections;
using AW.Resources;
using AW.Data;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using System.Text;
using Pathfinding;

namespace AW.War {
	/// 
	/// 奈河桥的战斗场景
	/// 
	public class ClientCreator : ClientBaseCreator {

		//地图加载器
		private PrefabLoader Maploader;
		//NPC的加载器
		private NpcLoader Npcloader;
		//地图编辑器模块
		private SceneEditorDataRead reader;

		//创建的依赖信息
		private ChapterConfigData curChapConfig = null;

        public ClientCreator(WarClientManager mgr) {
            warMgr = mgr;
            Maploader = Core.ResEng.getLoader<PrefabLoader>();
            Npcloader = Core.ResEng.getLoader<NpcLoader>();
            reader   = Core.Data.getIModelConfig<SceneEditorDataRead>();
        }

		#region IWarCreator implementation

		public void CreateMap (MapInfo map) {
			
            //设置当前配置信息
            if(map.type == ConfigType.PVEChapter) {
                ChapterModel chatperModel = Core.Data.getIModelConfig<ChapterModel>();
                curChapConfig = chatperModel.get(map.ID);
            } else {
                PVPBattleModel pvpModel = Core.Data.getIModelConfig<PVPBattleModel>();
                curChapConfig = pvpModel.get(map.ID);
            }

            //加载配表
            reader.loadSceneConfig(map.ID);

            //TODO : According to the type of map, we should make up the right path
			StringBuilder strBld = new StringBuilder ("Scenes/Stage_");
			strBld.Append (map.ID.ToString ());
			GameObject go = Maploader.loadFromUnPack(strBld.ToString(),false);
			UnityUtils.AddChild_Reverse(go, ScenePoint);

			MapInSceneData[] mapdata = reader.GetSceneEditorElementData<MapInSceneData>();
			if (mapdata != null && mapdata.Length > 0) {
				go.transform.localPosition = new Vector3 (mapdata [0].pos [0], mapdata [0].pos [1], mapdata [0].pos [2]);
				go.transform.localScale    = new Vector3 (mapdata [0].scale [0], mapdata [0].scale [1], mapdata [0].scale [2]);
				go.transform.localEulerAngles = new Vector3 (mapdata [0].rotation [0], mapdata [0].rotation [1], mapdata [0].rotation [2]);
			}

            strBld.Append("Graph");
            string strPath = System.IO.Path.Combine (ResourceSetting.UNPACKROOT, strBld.ToString());
            TextAsset mapTxt = UnityEngine.Resources.Load(strPath) as TextAsset;
            if(mapTxt == null)
                Debug.LogError("scene textAsseet data is null ::  " + strPath);

            AstarPath.active.astarData.DeserializeGraphs (mapTxt.bytes);

			CreateUnVision();

            GameObject helper = new GameObject("helper");
            helper.AddComponent<TileHandlerHelper>();
		}

		public void CreateNpc(IpcCreateNpcMsg msg) {

            int len = msg.npclist.Length;

            for(int i = 0; i < len; i++) 
            {
                CrtHero hero = msg.npclist[i];  
                ClientNPC npc = Npcloader.Load (hero.npcID, hero.uniqueId, (CAMP)hero.camp, WarPoint);
//                Debug.Log(fastJSON.JSON.Instance.ToJSON(msg));
                if (npc == null) {
                    ConsoleEx.DebugWarning ("Create npc fail!!!!!!!!!   id::  " + hero.npcID);
                    continue;
                }
                    
                npc.transform.localPosition    = new Vector3 (hero.pos.x, hero.pos.y, hero.pos.z);
                npc.transform.localScale = Vector3.one;
                npc.transform.localEulerAngles = new Vector3 (hero.rotation.x, hero.rotation.y, hero.rotation.z);
            }
		}

        public void CreateHero(IpcCreateHeroMsg msg) 
        {
            int len = msg.npclist.Length;

            for(int i = 0; i < len; i++) 
            {
                CrtHero hero = msg.npclist[i];  
                ClientNPC npc = Npcloader.Load (hero.npcID, hero.uniqueId, (CAMP)hero.camp, WarPoint);

                if (npc == null) {
                    ConsoleEx.DebugWarning ("Create npc fail!!!!!!!!!   id::  " + hero.npcID);
                    continue;
                }

				warMgr.clientTeam.filterNpc(hero, npc);

                npc.transform.localPosition    = new Vector3 (hero.pos.x, hero.pos.y, hero.pos.z);
                npc.transform.localScale = Vector3.one;
                npc.transform.localEulerAngles = new Vector3 (hero.rotation.x, hero.rotation.y, hero.rotation.z);
            }

            warMgr.SwitchHero(warMgr.clientTeam.activeNpc); 
            warMgr.CreateHeroFinished();
        }

		public void CreateVision () {

		}

		public void CreateUnVision () {
		
			//创建碰撞提
			SpecialAreaInSceneData[] areaData = reader.GetSceneEditorElementData<SpecialAreaInSceneData>();
			if (areaData != null && areaData.Length > 0)
			{			
				GameObject ptObj = new GameObject ("boxcollider");
				UnityUtils.AddChild_Reverse(ptObj, ScenePoint);

				for (int i = 0; i < areaData.Length; i++)
				{
					GameObject obj = new GameObject (i.ToString ());
					UnityUtils.AddChild_Reverse(obj, ptObj);
				
					//调整大小
					obj.transform.position = new Vector3 (areaData [i].pos [0], areaData [i].pos [1], areaData [i].pos [2]);
					obj.transform.localScale = new Vector3 (areaData [i].scale [0], areaData [i].scale [1], areaData [i].scale [2]);
					obj.transform.eulerAngles = new Vector3 (areaData [i].rotation [0], areaData [i].rotation [1], areaData [i].rotation [2]);

					//添加碰撞
					BoxCollider box = obj.AddComponent<BoxCollider>();
					box.center = Vector3.zero;
					box.size = Vector3.one;
				}
			}

		}
	
		//从场景编辑器得到得到所有的npc刷新点配置信息
		private List<NPCInSceneData> GetFreshPtList() {
			List<NPCInSceneData> freshPtList = new List<NPCInSceneData> ();

			if (curChapConfig != null)
			{
//				if (reader.loadSceneConfig (curChapConfig.scene_config))
//				{
					NPCInSceneData[] npcInScene = reader.GetSceneEditorElementData<NPCInSceneData> ();
					if (npcInScene != null && npcInScene.Length > 0)
					{
						for (int i = 0; i < npcInScene.Length; i++)
						{
                            if (npcInScene [i].npcID == NpcMgr<ServerNPC>.FRESH_NPC)
							{
								freshPtList.Add (npcInScene [i]);
							}
						}
					}
//				}
			}
			return freshPtList;
		}

		/// <summary>
		/// 创建npc缓存, 服务通知后创建缓存
		/// </summary>
		public void CreateNpcCache()
		{
       
			List<NPCInSceneData> freshPtList = GetFreshPtList ();
			if (freshPtList != null && freshPtList.Count > 0)
			{
				FreshPoolModel poolModel = Core.Data.getIModelConfig<FreshPoolModel>();
				FreshGroupModel gropModel = Core.Data.getIModelConfig<FreshGroupModel>();
				for (int i = 0; i < freshPtList.Count; i++)
				{
					int poolId = freshPtList [i].freshParam.freshPoolID;
					NPCFreshPool pool = poolModel.GetNPCFreshPool(poolId);
					if (pool != null)
					{
						for (int j = 0; j < pool.freshPool.Count; j++)
						{
							NPCFreshGroup grop = gropModel.GetFreshGroup (pool.freshPool [j]);
							if (grop != null)
							{
								for (int m = 0; m < grop.freshGroup.Count; m++)
								{
									ClientNPC npcsript = Npcloader.Load (grop.freshGroup[m], -1, freshPtList[i].camp, WarPoint);
								
									npcsript.data.rtData.curHp = 0;
									npcsript.dataInScene = freshPtList[i];
									npcsript.data.btData.way = freshPtList[i].way;

									npcsript.gameObject.SetActive (false);

									//加入缓存中
									warMgr.npcMgr.SignDeadNpcCache (npcsript);
								}
							}
						}
					}
				}
			}
		}


		#endregion
	}

}
