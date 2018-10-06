using UnityEngine;
using AW.Message;
using AW.War;
using AW.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using AW.Resources;
using fastJSON;
using UVec3 = UnityEngine.Vector3;

namespace AW.War {
	/// 
	/// 战斗的核心管理类, 客户端
	/// 
	public class WarClientManager : MonoBehaviour {
		public static WarClientManager Instance;

		//主3D摄像机
		public Camera Main3DCamera;

		/// <summary>
		/// ui相机
		/// </summary>
		public Camera uiCamera;

		/// <summary>
		/// 主相机跟随
		/// </summary>
		private CameraFollow mainCameraFollow;

		/// <summary>
		/// 振屏
		/// </summary>
		private ShakeCamera Shake;
		/// <summary>
		/// 指向技的箭头
		/// </summary>
		private GameObject skillArrow;
		/// <summary>
		/// npc容器
		/// </summary>
		public WarClientNpcMgr npcMgr;

		/// <summary>
		/// 战斗环境的创建器
		/// </summary>
		public ClientCreator creator;

		/// <summary>
		/// 本地客户端
		/// </summary>
		[HideInInspector]
		public RealClient realCli;

		private ClientCached cached;

		[HideInInspector]
		public WarInfo warInfo;

		public WarClientTeam clientTeam;

		//自动战斗标示
		private bool mAutoBattle = true;
		public bool AutoBattle
		{
			get
			{
				return mAutoBattle;
			}
			set
			{
				if (mAutoBattle != value)
				{
					mAutoBattle = value;
					//npcMgr.ActiveHero.SwitchAutoBattle (value);
				}
			}
		}

		/// <summary>
		/// 战斗的入口在这里
		/// </summary>
		void Awake() {
			Instance = this;
			mainCameraFollow = Main3DCamera.GetComponent<CameraFollow>();

			///
			/// 获得NPC管理容器
			///
			npcMgr = new WarClientNpcMgr();
			npcMgr.Init();
			//初始化场景的创建器
            creator = new ClientCreator(this);

			Shake = Main3DCamera.GetComponent<ShakeCamera>();
			AutoBattle = false;
		}

		public void Init() {
			realCli.CtorEnvironment = creator.CreateMap;
			realCli.CtorNPC         = creator.CreateNpc;
            realCli.CreatHero       = creator.CreateHero;
			//creator.CreateUnVision();
			//creator.CreateNpcCache ();

			warInfo = realCli.war;

			///
			/// 准备Team的信息
			///
			clientTeam = new WarClientTeam(realCli);


			UIReadyInfo uiReady = new UIReadyInfo() {
				ClientID   = DeviceInfo.GUID,
				ClientName = "AW_Client",
			};

			string plainJoin = JSON.Instance.ToJSON(uiReady);
			realCli.proxyServer.UIReady(plainJoin);

			if(warInfo.warMo == WarMode.NativeWar) {

				///
				/// 虚拟客户端
				///
				uiReady = new UIReadyInfo() {
					ClientID   = "-1",
					ClientName = "AWClient",
				};

				plainJoin = JSON.Instance.ToJSON(uiReady);
				realCli.proxyServer.UIReady(plainJoin);

			}

			///
			/// 创建默认的地图
			/// 
			cached = ClientCached.Instance;
			creator.CreateMap(cached.map);

		}

		void OnDestory() {
			npcMgr.Destory();
		}

		#region UI操作的入口


		public void ShakeMainCamera()
		{
			Shake.ShakeNearAndFar();
		}

		/// <summary>
		/// 切换主英雄之后的UI操作
		/// </summary>
		/// <param name="tran">target.</param>
        public void SwitchHero(ClientNPC npc) {
//            npcMgr.ActiveHero = npc;
            mainCameraFollow.SetFollowTarget (npc.transform, false);
            isSwitch = true;
            StartCoroutine(SwitchFlag());
		}

        bool isSwitch = false;
        public bool IsSwitch
        {
            get{ return isSwitch;}
        }

        public IEnumerator SwitchFlag()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            isSwitch = false;
        }

		#endregion

		#region 对外提供的代理方法
		/// <summary>
		/// 根据3D坐标获取对照的UI坐标
		/// </summary>
		/// <param name="tran">Tran.</param>
        public Vector3 GetUIPosRef3DPos(Vector3 posIn3D)
		{
            Vector3 v = Main3DCamera.WorldToScreenPoint(posIn3D);
            v.z = 0.01f;
            v = uiCamera.ScreenToWorldPoint(v);
            return v;
		}

        public Action onCreateHeroFinished = null;
        public void CreateHeroFinished()
        {
            if(onCreateHeroFinished != null)
            {
                onCreateHeroFinished();
            }
        }
		#endregion


        // Update is called once per frame
        void FixedUpdate () {
            npcMgr.Update(Time.deltaTime);
        }
	}

}
