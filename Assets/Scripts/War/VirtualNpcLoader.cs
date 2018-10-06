using System;
using AW.Framework;
using AW.Data;
using UnityEngine;
using AW.War;
using System.IO;
using UObj = UnityEngine.Object;
using URes = UnityEngine.Resources;
using SPath= System.IO.Path;
using Pathfinding;

namespace AW.Resources {
	/// <summary>
	/// 服务器虚拟的加载器
	/// </summary>
	public class VirtualNpcLoader : ILoaderDispose {
		#region ILoaderDispose implementation

		public void ClearCache (bool gc) {
			cached = null;
		}

		#endregion

		//缓存的对象
		private UObj cached;
		//NPC加载器
		private NPCModel NpcModel = null;

		private WarServerNpcMgr serNpcMgr;

		private const string NPC = "NPC";
		private const string VIRTUAL = "VirtualNpc";

		public VirtualNpcLoader() {

		}

		public void OnWarStart(WarServerNpcMgr npcMgr) {
			serNpcMgr = npcMgr;
		}

		/// <summary>
		/// 战斗结束
		/// </summary>
		public void OnWarEnd() {
			serNpcMgr = null;
			cached = null;
		}

		public ServerNPC Load(int num, CAMP camp, GameObject WarPoint) {
			if(NpcModel == null) NpcModel = Core.Data.getIModelConfig<NPCModel>();

			NPCConfigData configData = NpcModel.get (num);

			#if DEBUG
			Utils.Assert(configData == null, "Virtual Npc load can't find npc configure. NPC id = " + num);
			#endif

			if(cached == null) {
				string path = SPath.Combine(ResourceSetting.PACKROOT, NPC);
				path = SPath.Combine(path, VIRTUAL);
				cached = URes.Load(path);
			}

			GameObject go = GameObject.Instantiate(cached) as GameObject;
			UnityUtils.AddChild_Reverse(go, WarPoint);

            if(configData.type == LifeNPCType.Build) {
                go.layer = LayerMask.NameToLayer(Consts.LAYER_BUILD);
            } else {
                go.layer = LayerMask.NameToLayer(Consts.LAYER_NPC);
            }

            ServerNPC npc = null;
            if (configData.healthpoint > 0)
                npc = go.AddComponent<ServerLifeNpc>();
            else
                npc = go.AddComponent<ServerNPC>();

            //如果是可以移动的，添加寻路脚本
            if (configData.moveable == Moveable.Movable)
            {
                Seeker seek = go.AddComponent<Seeker>();
                seek.drawGizmos = false;

                AIPath pathFinding = go.AddComponent<AIPath>();

                pathFinding.speed = configData.speed;
                pathFinding.slowdownDistance = 0.0f;
                pathFinding.pickNextWaypointDist = 2;
                pathFinding.forwardLook = 1f;

                FunnelModifier modifer = go.AddComponent<FunnelModifier>();
                modifer.Priority = 2;

                CharacterController box = go.AddComponent<CharacterController>();
               
                box.radius = configData.radius;
                box.height = 2;
               
                if (box.radius >= box.height)
                    box.center = Vector3.up * box.radius;
                else
                    box.center = Vector3.up * box.height / 2;

            }
            else if (configData.radius > 0)
            {
                CapsuleCollider cap = go.AddComponent<CapsuleCollider>();
                cap.radius = configData.radius;
                cap.height = 2;

                if (cap.radius >= cap.height)
                    cap.center = Vector3.up * cap.radius;
                else
                    cap.center = Vector3.up * cap.height / 2;

                NavmeshCut cut = go.AddComponent<NavmeshCut>();
                cut.type = NavmeshCut.MeshType.Circle;
                cut.circleRadius = configData.radius;
                cut.height = 10;
                cut.center = Vector3.up * 5;
            }

            DynamicDataInit(npc, configData, camp);

			go.name = "NPC_" + configData.ID + "_" + npc.UniqueID;

			return npc;
		}

        /// <summary>
        /// Loads the bullet.
        /// </summary>
        /// <returns>The bullet.</returns>
        /// <param name="num">Number.</param>
        /// <param name="camp">Camp.</param>
        /// <param name="pos">Position.</param>
        /// <param name="rot">Rot.</param>
        public GameObject LoadNpcObj(int num, CAMP camp, Vector3 pos, Quaternion rot)
        {
			if(NpcModel == null) NpcModel = Core.Data.getIModelConfig<NPCModel>();

			NPCConfigData configData = NpcModel.get (num);

            #if DEBUG
            Utils.Assert(configData == null, "Virtual Npc load can't find npc configure. NPC id = " + num);
            #endif

            string path = SPath.Combine(ResourceSetting.PACKROOT, NPC);

            if (configData.healthpoint > 0)
            {
                path = SPath.Combine(path, "LifeSummonNpc");
            }
            else
            {
                path = SPath.Combine(path, "NoneLifeSummonNpc");
            }

            UObj obj = URes.Load(path);

            GameObject go = GameObject.Instantiate(obj) as GameObject;
            go.transform.position = pos;
            go.transform.rotation = rot;

            if (configData.moveable == Moveable.Movable)
            {
                Seeker seek = go.AddComponent<Seeker>();
                seek.drawGizmos = false;

                AIPath pathFinding = go.AddComponent<AIPath>();

                pathFinding.speed = configData.speed;
                pathFinding.slowdownDistance = 0.0f;
                pathFinding.pickNextWaypointDist = 2;
                pathFinding.forwardLook = 1f;

                FunnelModifier modifer = go.AddComponent<FunnelModifier>();
                modifer.Priority = 2;

                CharacterController box = go.AddComponent<CharacterController>();

                box.radius = configData.radius;
                box.height = 2;

                if (box.radius >= box.height)
                    box.center = Vector3.up * box.radius;
                else
                    box.center = Vector3.up * box.height / 2;

            }
            else if (configData.radius > 0)
            {
                CapsuleCollider cap = go.AddComponent<CapsuleCollider>();
                cap.radius = configData.radius;
                cap.height = 2;

                if (cap.radius >= cap.height)
                    cap.center = Vector3.up * cap.radius;
                else
                    cap.center = Vector3.up * cap.height / 2;

                NavmeshCut cut = go.AddComponent<NavmeshCut>();
                cut.type = NavmeshCut.MeshType.Circle;
                cut.circleRadius = configData.radius;
                cut.height = 10;
                cut.center = Vector3.up * 5;
            }

            ServerNPC npc = go.GetComponent<ServerNPC>();
            if(npc != null)
            {
                DynamicDataInit(npc, configData, camp);
            }

            return go;
        }

        /// <summary>
        /// 加载子弹型npc
        /// </summary>
        /// <returns>The bullet npc.</returns>
        /// <param name="num">Number.</param>
        /// <param name="camp">Camp.</param>
        /// <param name="pos">Position.</param>
        /// <param name="rot">Rot.</param>
        public GameObject LoadBulletNpc(int num, CAMP camp, Vector3 pos, Quaternion rot)
        {
            if(NpcModel == null) NpcModel = Core.Data.getIModelConfig<NPCModel>();

            NPCConfigData configData = NpcModel.get (num);

            #if DEBUG
            Utils.Assert(configData == null, "Virtual Npc load can't find npc configure. NPC id = " + num);
            #endif

            string path = SPath.Combine(ResourceSetting.PACKROOT, NPC);
            path = SPath.Combine(path, "ServerBulletNpc");

            UObj obj = URes.Load(path);

            GameObject go = GameObject.Instantiate(obj) as GameObject;
            go.transform.position = pos;
            go.transform.rotation = rot;

            ServerNPC npc = go.GetComponent<ServerNPC>();
            if(npc != null)
            {
                DynamicDataInit(npc, configData, camp);
            }

            return go;
        }

		/// <summary>
		/// 数据的初始化
		/// </summary>
		/// <param name="go">Go.</param>
		/// <param name="num">Number.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="camp">Camp.</param>
        void DynamicDataInit(ServerNPC curHero, NPCConfigData econfig, CAMP camp) {
        
			///
			/// 填充阵营
			///
			curHero.Camp = camp;

			///
			/// 填充NPC数据
			///
			NPCData dynamicData = new NPCData ();
			dynamicData.rtData = new NPCRuntimeData (econfig);
			dynamicData.configData = econfig;
			dynamicData.btData = new NPCBattleData ();
			curHero.data = dynamicData;

			///
			/// 向WarManager注册
			///
			serNpcMgr.SignID (curHero);

			WarServerManager warMgr = WarServerManager.Instance;
			///
			/// 填充技能数据
			///
            RtNpcSkillModel skMd = new RtNpcSkillModel(econfig.ID, curHero.UniqueID);
			ServerLifeNpc life = curHero as ServerLifeNpc;
			if(life != null)
				life.runSkMd = skMd;

			///
			/// 填充默认的buff
			///
			for(short i = 0; i < Consts.MAX_SKILL_COUNT; ++ i) {
				RtSkData sk = skMd.getRuntimeSkill(i);
				if(sk != null) {
					int passive = sk.skillCfg.PassiveBuff;
					if(passive > 0) {
						BuffCtorParam ctor = new BuffCtorParam() {
							bufNum = passive,
							fromNpcId = curHero.UniqueID,
							toNpcId= curHero.UniqueID,
							origin = OriginOfBuff.BornWithSkill,
							initLayer = 1,
							duration = Consts.USE_BUFF_CONFIG_DURATION,
						};
						warMgr.bufMgr.createBuff(ctor);
					}
				}
			}

		}

	}
}
