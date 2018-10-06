using UnityEngine;
using System.Collections;
using AW.Entity;
using AW.Message;
using AW.Framework;
using AW.FSM;
using AW.Data;

namespace AW.Controller {

	using AW.War;

	/// <summary>
	/// 战斗的核心控制层
	/// </summary>

	[Controller(ctrlType = LogicalType.War)]
	public class War : ControllerEx, IDeviceState {
	
		public void OnLevelWasLoaded (int level) {
			//根据WarInfo来决定加载服务器端模块和客户端模块
			if(Application.loadedLevelName == SceneName.BattleScene) {

				//创建出来战斗的客户端控制层
				GameObject goCli     = GameObject.FindGameObjectWithTag("WarClient");
				WarClientManager warCliMgr = goCli.GetComponentInChildren<WarClientManager>();
				warCliMgr.realCli    = Client;
				warCliMgr.Init();

				if(warInfo.Side == WarSide.ServerAndClient) {
					//创建出来战斗的服务器控制层
					GameObject goSer     = GameObject.FindGameObjectWithTag("WarServer");
					GameObject goSerMgr  = new GameObject("WarMgr");
					UnityUtils.AddChild_Reverse(goSerMgr, goSer);
					WarServerManager warSerMgr = goSerMgr.AddComponent<WarServerManager>();
					warSerMgr.realServer = Server;
				}
			}
		}

		#region IDeviceState implementation
		public void OnGameLaunched (StateParam<DeviceState> obj) { }
		public void OnPaused (StateParam<DeviceState> obj) { }
		public void OnResume (StateParam<DeviceState> obj) { }

		public void OnQuit (StateParam<DeviceState> obj) {
			if(Server != null) Server.Dispose();
			if(Client != null) Client.Quit();
		}
		#endregion

		private RealServer Server;
		private RealClient Client;
		//这个信息可能是ServerAndClient， 也可能是OnlyClient
		private WarInfo warInfo;

		public override void UI_OnReceive (MsgParam param) {
			base.UI_OnReceive (param);

			if(param is WarStartParam) {
				WarStartParam OkParam = (WarStartParam) param;
				warInfo = OkParam.warinfo;
				WarBegin();
			}
		}

		/// <summary>
		/// 创建Server服务器，地址绑定成功后，使用PubSocket发布出去ServerReady信息，
		/// 客户端的SubSocket接收到信息后，开始链接Server服务器
		/// 
		/// </summary>
		void WarBegin() {
			//War Data Model is passed by Lua（what ever) 
			IWarModel warModel = new DebugModel();

			warInfo = new WarInfo() {
				warMo = WarMode.NativeWar,
				#if Server
				Side  = WarSide.ServerAndClient,
				RequiredClientCount = 2,

				Map  = warModel.getMap(),
				Charactor = warModel.getCharactor(WarCamp.FirstCamp),

				ServerIp = "127.0.0.1",
				#else 
				Side  = WarSide.OnlyClient,
				Charactor = warModel.getCharactor(WarCamp.SecondCamp),
				ServerIp = "192.168.1.176",//TODO : real ip (not loopback)
				#endif

			};

			if(warInfo.Side == WarSide.ServerAndClient) //启动server
				Server = new RealServer(warInfo, ServerRepOk, ServerPubOK);
			else //启动client ， 这个client不和服务器端在一个物理设备上
				Client = new RealClient(warInfo);
		}

		/// <summary>
		/// PublisherSocket准备好了
		/// </summary>
		void ServerPubOK () {
			//启动client, 这个客户端和服务器端在一个物理设备上
			Client = new RealClient(warInfo);
		}

		/// <summary>
		/// ResponseSocket准备好了
		/// </summary>
		void ServerRepOk () {

			int condition = warInfo.RequiredClientCount;

			ServerInfo info = Server.cached.curServer;

			long now = Core.TimerEng.curTime;
			TimerTask timer = new TimerTask(now + 2, TimerTask.INFINITY, 1);
			timer.onEvent = ( (obj) => {
				if( Server.monitor.ReadyClientCount >= condition ) {
					obj.Enabled = false;
					Server.monitor.CharactorPool.EnterWarScene();
					Server.proxyCli.EnterWar(info, warInfo.Map);
				} else {
					Server.proxyCli.ServerReady(info);
				}
			});
			timer.DispatchToRealHandler();
		}

		#region 测试代码
		/// <summary>
		/// 不停的测试网络发送
		/// </summary>
		void loopTest() {
			long now = Core.TimerEng.curTime;
			TimerTask timer = new TimerTask(now + 5, TimerTask.INFINITY, 1);

			timer.onEvent = ( (obj) => {
				for(int i = 0; i < 1; ++ i) {
					Server.proxyCli.CtorEnv(warInfo.Map);
				}
			});
			timer.DispatchToRealHandler();
		}

		/// <summary>
		/// 测试连接-断线-连接-断线-
		/// </summary>
		void ReconnectTest() {
			long now = Core.TimerEng.curTime;
			TimerTask timer = new TimerTask(now + 2, now + 10, 1);

			ServerInfo info = Server.cached.curServer;

			timer.onEvent = ( (obj) => {
				Server.proxyCli.CtorEnv(warInfo.Map);
				Server.proxyCli.ServerReady(info);
			});

			timer.onEventEnd = (
				(obj) => {
					try {
						if(Server != null) Server.Dispose();
						if(Client != null) Client.Quit();
					} catch(System.Exception ex) {
						ConsoleEx.DebugLog(ex.ToString());
					}

					AsyncTask.QueueOnMainThread(
						() => {
							Invoke("WarBegin", 1f);
						}
					);
				}
			);

			timer.DispatchToRealHandler();
		}

		#endregion
	}
}
