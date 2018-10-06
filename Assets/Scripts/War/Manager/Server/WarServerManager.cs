using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AW.War {
	public class WarServerManager : MonoBehaviour {
		public static WarServerManager Instance;

		/// 
		/// npc容器
		/// 
		public WarServerNpcMgr npcMgr;

		/// <summary>
		/// 技能的控制逻辑
		/// </summary>
		public SkillCastor npcSkill;

		/// <summary>
		/// 触发器
		/// </summary>
		public TriggerMgr triMgr;

		/// <summary>
		/// Buff / Debuff
		/// </summary>
		public BuffMgr    bufMgr;

		/// <summary>
		/// 效果的承受器
		/// </summary>
		public EffectSufferMgr sufMgr;

        /// <summary>
        /// 战斗环境创建器
        /// </summary>
        public ServerCreator creator;

        /// <summary>
        /// 是否初始化完成
        /// </summary>
        public bool bInit
        {
            get;
            private set;
        }

        //战斗开始
        public bool battleStart
        {
            get;
            private set;
        }
            
        //是否自动战斗
        public bool selfAutoBattle = true;
        public bool enemyAutoBattle = true;

		//数据通讯
		public RealServer realServer;

		void Awake() {
			Instance = this;

			npcMgr = new ServerNpcMgrFactory().getNpcMgr();
			npcMgr.Init();

            creator = new ServerCreator(this);

			sufMgr  = EffectSufferMgr.instance;

			npcSkill = SkillCastor.Instance;
			npcSkill.init(npcMgr);

			triMgr = TriggerMgr.Instance;
			triMgr.Init(npcMgr);

			bufMgr = BuffMgr.Instance;
			bufMgr.init(npcMgr, triMgr);

			StartCoroutine(checkClientUIReady());
		}

		//检查客户端UI是否准备妥当
		IEnumerator checkClientUIReady() {
			bool exist = false;
			do {
				if(realServer == null) {
					yield return new WaitForEndOfFrame();
				} else {
					exist = true;
				}
			} while(!exist);

			if(realServer != null) {
				bool keep = true;

				do {
					if(realServer.mWar.RequiredClientCount <= realServer.monitor.UIReadyClientCount) {
						keep = false;
						StartWar();
					} else {
						yield return new WaitForSeconds(0.1f);
					}
				} while(keep);

			}
		}

		// Use this for initialization
		void StartWar () {
			creator.CreateMap();
			creator.CreateUnVision();
			creator.CreateNpc();
            creator.CreateHero();
            npcMgr.AnalyzeIfComplete();
            bInit = true;
            battleStart = false;
            Invoke("StartBattle", 7.0f);
		}

		void OnDestory() {
			npcMgr.Destory();
		}

		// Update is called once per frame
		void FixedUpdate () {
			float del = Time.deltaTime;
			npcMgr.Update(del);
			bufMgr.Update(del);
			triMgr.Update(del);
		}

        void StartBattle()
        {
            Dictionary<WarCamp, List<RoomCharactor>> allCharactors = realServer.monitor.CharactorPool.allCharactors;
            foreach (KeyValuePair<WarCamp, List<RoomCharactor>> itor in allCharactors)
            {
                List<RoomCharactor> list = itor.Value;
                int len = list.Count;
                for (int i = 0; i < len; i++)
                {
                    WrapperTeam team = list[i].team as WrapperTeam;
                    List<ServerLifeNpc> npcList = team.realTeam;
                    int npcCnt = npcList.Count;
                    for (int j = 0; j < npcCnt; j++)
                    {
                        if (realServer.monitor.CharactorPool.IsHeroActive(npcList[j]))
                        {
                            npcList[j].SwitchAutoBattle(list[i].autoBattle);
                        }
                        else
                        {
                            npcList[j].SwitchAutoBattle(true);
                        }
                    }
                }
            }

            battleStart = true;
        }
	}
}
