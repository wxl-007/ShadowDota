using System;
using System.Collections;
using AW.Resources;
using AW.Data;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;
using AW.Framework;

namespace AW.War {

    /// <summary>
    /// 服务器端只创建NPC
    /// </summary>
    public class ServerCreator : ServerBaseCreator {
        //ai加载器
        private AILoader AiLoader;
        //地图编辑器模块
        private SceneEditorDataRead reader;
        //虚拟npc加载器
        private VirtualNpcLoader virNpcLoader;
        //英雄队伍管理类
		private WarServerCharactor Charactors;

		//创建的依赖信息
		private ChapterConfigData curChapConfig;

        public ServerCreator(WarServerManager server) {
            WarSMgr  = server;

            AiLoader = Core.ResEng.getLoader<AILoader>();
            virNpcLoader = Core.ResEng.getLoader<VirtualNpcLoader>();
            reader       = Core.Data.getIModelConfig<SceneEditorDataRead>();
        }

		/// <summary>
		/// 创建地图
		/// </summary>
		public void CreateMap () { 
			Charactors   = WarSMgr.realServer.monitor.CharactorPool;

			MapInfo map  = WarSMgr.realServer.mWar.Map;
			//将数据发给客户端
			WarSMgr.realServer.proxyCli.CtorEnv(map);

			if(map.type == ConfigType.PVEChapter) {
				ChapterModel chatperModel = Core.Data.getIModelConfig<ChapterModel>();
				curChapConfig = chatperModel.get(map.ID);
			} else {
				PVPBattleModel pvpModel = Core.Data.getIModelConfig<PVPBattleModel>();
				curChapConfig = pvpModel.get(map.ID);
			}
				
		}

        public void CreateUnVision () {
            //触发场景
            createTrigger();
        }

        public void CreateNpc () {
            
			Utils.Assert(curChapConfig == null, "Chapter Configdata is null.");
            
            if (reader.loadSceneConfig (curChapConfig.scene_config)) {
                NPCInSceneData[] npcInScene = reader.GetSceneEditorElementData<NPCInSceneData>();
                if (npcInScene != null && npcInScene.Length > 0) {
					int len = npcInScene.Length;
                    CrtHero[] toClient = new CrtHero[len];

					for(int i = 0; i < len; i++) {
						NPCInSceneData sceneD = npcInScene [i];
						ServerNPC npc = virNpcLoader.Load (sceneD.npcID, sceneD.camp, WarPoint);
                        if (npc == null) {
							ConsoleEx.DebugWarning ("Create npc fail!!!!!!!!!   id::  " + sceneD.npcID);
                            continue;
                        }

						npc.transform.localPosition    = new Vector3 (sceneD.pos [0], sceneD.pos [1], sceneD.pos [2]);
						npc.transform.localScale       = new Vector3 (sceneD.scale [0], sceneD.scale [1], sceneD.scale [2]);
						npc.transform.localEulerAngles = new Vector3 (sceneD.rotation [0], sceneD.rotation [1], sceneD.rotation [2]);

                        npc.spawnPos = npc.transform.position;
                        npc.spawnRot = npc.transform.rotation;

						npc.dataInScene     = sceneD;
						npc.data.btData.way = sceneD.way;

                        //初始化buff
                        InitBuff (npc);
                        //初始化AI
                        InitAi (npc);

                        toClient[i] = new CrtHero() { 
                            npcID = sceneD.npcID,
							uniqueId = npc.UniqueID,
                            pos = VectorWrap.ToVector(npc.transform.position),
                            rotation = VectorWrap.ToVector(npc.transform.eulerAngles),
                            camp = (int)npc.Camp,
						};
                    }
					//send creating npc message to client
					IpcCreateNpcMsg msg = new IpcCreateNpcMsg() {
						npclist = toClient,
					};
					WarSMgr.realServer.proxyCli.CtorNpc(msg);
                }
            } else {
                ConsoleEx.DebugWarning(curChapConfig.scene_config + " scene config is not find.");
            }
            
        }

        //初始化Buff
		private void InitBuff(ServerNPC npc) {
            //初始化buff
            if (npc.dataInScene.buffs != null && npc.dataInScene.buffs.Length > 0) {
                for (int j = 0; j < npc.dataInScene.buffs.Length; j++) {
                    BuffCtorParam buffParam = new BuffCtorParam ();
                    buffParam.bufNum = npc.dataInScene.buffs [j];
                    buffParam.fromNpcId = npc.UniqueID;
                    buffParam.toNpcId = npc.UniqueID;
                    buffParam.origin = OriginOfBuff.Alone;
                    buffParam.initLayer = 1;
                    WarSMgr.bufMgr.createBuff (buffParam);
                }
            }
        }

        //初始化AI
        private void InitAi(BNPC npc)
        {
            //如果是npc刷新点
            if (npc.data.num == NpcMgr<ServerNPC>.FRESH_NPC)
            {
                #if NO_SOLDIER
                #else
                BehaviorTree tree = npc.gameObject.GetComponent<BehaviorTree> ();
                if(tree == null)
                    tree = npc.gameObject.AddComponent<BehaviorTree>();

                tree.ExternalBehavior = AiLoader.load (AILoader.NPC_FRESH);
                tree.StartWhenEnabled = true;
                tree.RestartWhenComplete = false;
                #endif
            }
            else
            {
                NPCAIType type = (NPCAIType)npc.dataInScene.AIType;
                switch (type)
                {
                    case NPCAIType.Pathfind_Atk:
                        {
                            BehaviorTree tree = npc.gameObject.GetComponent<BehaviorTree> ();
                            if(tree == null)
                                tree = npc.gameObject.AddComponent<BehaviorTree>();

                            tree.ExternalBehavior = AiLoader.load (AILoader.PATHFIND_ATK);
                            tree.StartWhenEnabled = true;
                            tree.RestartWhenComplete = true;
                            break;
                        }
                    case NPCAIType.Simple_PfAtk:
                        {
                            BehaviorTree tree = npc.gameObject.GetComponent<BehaviorTree> ();
                            if(tree == null)
                                tree = npc.gameObject.AddComponent<BehaviorTree>();

                            tree.ExternalBehavior = AiLoader.load (AILoader.SIMPLE_PFATK);
                            tree.StartWhenEnabled = true;
                            tree.RestartWhenComplete = true;
                            break;
                        }

                    case NPCAIType.Patrol:
                        {
                            BehaviorTree tree = npc.gameObject.GetComponent<BehaviorTree> ();
                            if(tree == null)
                                tree = npc.gameObject.AddComponent<BehaviorTree>();

                            tree.ExternalBehavior = AiLoader.load (AILoader.NORMAL_ATTACK);
                            tree.StartWhenEnabled = true;
                            tree.RestartWhenComplete = true;
                            break;
                        }
                }
            }
        }


        //创建敌我双方的队伍
        public void CreateHero ()
        {
			List<ServerLifeNpc> FirstSide = null, SecondSide = null;
			List<CrtHero> heroList = new List<CrtHero>();

			WarCamp wc = WarCamp.FirstCamp;
			CreateOneSideHero(wc, out FirstSide, heroList);
            Charactors.SetHeroList(wc, FirstSide);

            wc = WarCamp.SecondCamp;
			CreateOneSideHero(wc, out SecondSide, heroList);
            Charactors.SetHeroList(wc, SecondSide);

            //send create hero msg
            IpcCreateHeroMsg msg = new IpcCreateHeroMsg();
            msg.npclist = heroList.ToArray();
            msg.op = OP.CtorHero;
            WarSMgr.realServer.proxyCli.CtorHero(msg);
        }  

		void CreateOneSideHero(WarCamp wc, out List<ServerLifeNpc> NpcContainer, List<CrtHero> heroList) {
			NpcContainer = new List<ServerLifeNpc>();

			List<RoomCharactor> OneSide = Charactors.get(wc);

			int count = OneSide.Count;
			for(int j = 0; j < count; ++ j) {
				WrapperTeam wrapperTeam = (WrapperTeam) OneSide[j].team;
				List<NPCData> dataTeam = wrapperTeam.dataTeam;
				int cnt = dataTeam.Count;
                #if SINGLE_HERO
                cnt = 1;
                #endif

				if (dataTeam != null && cnt > 0) {

					//一方出生点
					CAMP camp = WarCamp2Camp.toCamp(wc);

					List<ServerNPC> npcList = WarSMgr.npcMgr.GetNPCListByNum (NpcMgr<ServerNPC>.BORN_POINT, camp);

					for(int i = 0; i < cnt; i++) {
                        ServerNPC npc = virNpcLoader.Load(dataTeam[i].configData.ID, camp, WarPoint);

						//设置出生点
						npc.transform.position = npcList[i].transform.position;

                        npc.spawnPos = npc.transform.position;
                        npc.spawnRot = npc.transform.rotation;

						ServerLifeNpc hero = npc as ServerLifeNpc;
						NpcContainer.Add (hero);

						wrapperTeam.AddNpcMember(hero, i);

						BehaviorTree tree = hero.gameObject.AddComponent<BehaviorTree>();
						tree.ExternalBehavior = AiLoader.load (AILoader.PATHFIND_ATK);
						tree.StartWhenEnabled = true;
						tree.RestartWhenComplete = true;

						hero.SwitchAutoBattle (true);
						hero.SwitchAutoBattle (false);

						CrtHero crt = new CrtHero();
						crt.camp = (int)npc.Camp;
						crt.npcID = npc.data.configData.ID;
						crt.uniqueId = npc.UniqueID;
						crt.pos = VectorWrap.ToVector(npc.transform.position);
						crt.rotation = VectorWrap.ToVector(npc.transform.eulerAngles);
						crt.ClientID = OneSide[j].UID;
						crt.index    = i;

						heroList.Add(crt);
					}
				}
			}
		}

    }

}

