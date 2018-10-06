using System;
using fastJSON;
using System.Collections.Generic;
using AW.Message;
using AW.Framework;
using AW.FSM;
using AW.Data;
using System.Threading;

namespace AW.War {
	/// <summary>
	/// 实际的客户端
	/// </summary>
	public class RealClient : IClient {
		#region SubSocket接收到的信息

		/// 
		/// 抛出的各种消息回调
		/// 
		public Action<MapInfo> CtorEnvironment;
		public Action<IpcCreateNpcMsg> CtorNPC;
        public Action<IpcCreateHeroMsg> CreatHero;

		/// <summary>
		/// 创建地形
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="MapId">Map identifier.</param>
		public void CtorEnv(MapInfo Map) {
			ConsoleEx.DebugLog(war.Side.ToString() + " Sub Received : mapid = " + Map.ID, ConsoleEx.YELLOW);

			if( !cached.map.IsSame(Map) ) {
				if(CtorEnvironment != null) {
					AsyncTask.QueueOnMainThread(
						() => { 
							CtorEnvironment(Map);
						}
					);
				}
			}
		}

		/// <summary>
		/// 创建Npc
		/// </summary>
		public void CtorNpc(IpcCreateNpcMsg msg) {
			ConsoleEx.DebugLog(war.Side.ToString() + " Sub Received : CtorNpc.", ConsoleEx.YELLOW);
			if(CtorNPC != null) {
				AsyncTask.QueueOnMainThread (
					() => {
						CtorNPC(msg);
					}
				);
			}
		}

        /// <summary>
        /// 创建Hero
        /// </summary>
        public void CtorHero(IpcCreateHeroMsg msg) {
            ConsoleEx.DebugLog(war.Side.ToString() + " Sub Received : CtorHero.", ConsoleEx.YELLOW);
            if(CreatHero != null) {
                AsyncTask.QueueOnMainThread (
                    () => {
						CreatHero(msg);
					}
                );
            }
        }

        //npc位移消息
        public void NPCMove(IpcNpcMoveMsg msg)
        {
            //ConsoleEx.DebugLog(war.Side.ToString() + " Sub Received : NPCMove.", ConsoleEx.YELLOW);
            WarMsgParam param = new WarMsgParam();
            param.Sender = msg.uniqueId;
            param.Receiver = msg.uniqueId;
            param.cmdType = WarMsg_Type.Move;
            param.param = msg;
            WarClientManager.Instance.npcMgr.SendMessageAsync(msg.uniqueId, msg.uniqueId, param);
        }

        //npc血量变化
        public void NPChp(IpcNpcHpMsg msg)  
        {
            ConsoleEx.DebugLog(war.Side.ToString() + " Sub Received : NPChp.", ConsoleEx.YELLOW);
            AsyncTask.QueueOnMainThread(
                () => { 
                int id = msg.uniqueId;
                WarClientManager mgr = WarClientManager.Instance;
                if(mgr != null)
                {
                    ClientNPC npc = mgr.npcMgr.GetNpc(id);
                    if(npc != null)
                    {
                        npc.UpdateHp(msg);
                    }
                }
            });
        }

        //npc动画
        public void NPCAnim(IpcNpcAnimMsg msg) {
            //ConsoleEx.DebugLog(war.Side.ToString() + " Sub Received : NPCAnim.", ConsoleEx.YELLOW);
            AsyncTask.QueueOnMainThread(
                () => { 
                WarMsgParam param = new WarMsgParam();
                param.cmdType = (WarMsg_Type)Enum.Parse(typeof(WarMsg_Type), msg.nextAnim);
                if(!string.IsNullOrEmpty(msg.data))
                {
                    param.param = msg.data;
                }
                WarClientManager mgr = WarClientManager.Instance;
                if(mgr != null)
                {
                    mgr.npcMgr.SendMessageAsync(msg.uniqueId, msg.uniqueId, param);
                }
            });
        }

        //npc状态
        public void NPCStatus(IpcNpcStatusMsg msg)
        {
            ConsoleEx.DebugLog(war.Side.ToString() + " Sub Received : NPCStatus.", ConsoleEx.YELLOW);
        }


		///
		/// 给客户端一条战斗消息
		/// 
		public void Deliver(IpcMsg msg) {

		}

        public void NpcDestroy(IpcDestroyNpcMsg msg)
        {
            AsyncTask.QueueOnMainThread(
                () => { 
                int id = msg.id;
                WarClientManager mgr = WarClientManager.Instance;
                if(mgr != null)
                {
                    ClientNPC npc = mgr.npcMgr.GetNpc(id);
                    if(npc != null)
                    {
                        WarMsgParam param = new WarMsgParam();
                        param.cmdType = WarMsg_Type.Destroy;
                        mgr.npcMgr.SendMessageAsync(id, id, param);
                    }
                }
            });
        }

        public void NpcSkillCD(IpcSkillMsg msg)
        {
            NpcSkillAttr attr = new NpcSkillAttr()
            { 
                npcId = msg.uniqueID,
                skillIndex = msg.index,
                baseCd = msg.baseCD,
                cdValue = msg.baseCD,
                isInCd = true,
            };
            AsyncTask.QueueOnMainThread(
                () => 
                {
                    WarClientManager mgr = WarClientManager.Instance;
                    if(mgr != null)
                    {
                        ClientNPC npc = mgr.npcMgr.GetNpc(msg.uniqueID);
                        if(npc != null)
                        {
                            npc.AddSkillAttr(msg.index, attr);
                        }
                    }
                }
            );
        }

		/// <summary>
		/// 服务准备好了
		/// </summary>
		public void ServerReady(ServerInfo Server) {
			ConsoleEx.DebugLog(war.Side.ToString() + " Sub Received : Server is Ready ", ConsoleEx.YELLOW);
			//避免重复收听
			if(proxyReady == false) {
				proxyReady = true;
				monitor.ServerReady(Server);

				proxyServer = new ProxyServer(war, () => {

					JoinInfo join = new JoinInfo() {
						ClientID   = DeviceInfo.GUID,
						ClientName = "AW_Client",
						Charactor  = war.Charactor,
					};

					string plainJoin = JSON.Instance.ToJSON(join);
					proxyServer.Join(plainJoin);

					if(war.warMo == WarMode.NativeWar) {

						RoomCharactor vir = war.Charactor.DeepCopy();
						vir.camp = WarCamp.SecondCamp;
						vir.UID  = "-1";
						vir.team = new DebugModel().getTeam(WarCamp.SecondCamp);
						///
						/// 创建虚拟Client
						///
						JoinInfo virtualJoin = new JoinInfo() {
							ClientID   = "-1",
							ClientName = "AWClient",
							Charactor  = vir,
						};
						plainJoin = JSON.Instance.ToJSON(virtualJoin);
						proxyServer.Join(plainJoin);
					}

				});

				register();
			} /*else {
				//Test purpose
				JoinInfo join = new JoinInfo() {
					ClientID   = DeviceInfo.GUID,
					ClientName = "AW_Client",
					Charactor  = war.Charactor,
				};

				string plainJoin = JSON.Instance.ToJSON(join);
				proxyServer.Join(plainJoin);
			}*/
		}

		/// <summary>
		/// 服务器不存在了
		/// </summary>
		/// <param name="serverId">Server identifier.</param>
		public void ServerQuit(string serverId) {
			monitor.ServerQuit(serverId);

			Quit();
		}

		/// <summary>
		/// 进入战斗场景
		/// </summary>
		/// <param name="Server">Server.</param>
		private bool hasEntered = false;
		public void EnterWar(ServerInfo Server, MapInfo Map) { 
			ConsoleEx.DebugLog(war.Side.ToString() + " Sub Received : Enter War.", ConsoleEx.YELLOW);

			GamePlayFSM fsm = Core.GameFSM;
			if(fsm.CurScene != SceneName.BattleScene) {

				if(hasEntered == false) {
					hasEntered = true;

					monitor.EnterWar(Server.ServerID);
					cached.map = Map;
					AsyncTask.QueueOnMainThread (
						()=> {
							//UnityUtils.JumpToScene(Core.GameFSM, SceneName.BattleScene);
							Jumper.EnterWarDataInitFinished();
						} 
					);
				}

			}

		}

		public void SyncClient(IpcSyncClientMsg msg) {
			ConsoleEx.DebugLog(war.Side.ToString() + " Sub Received : SyncClient.", ConsoleEx.YELLOW);
			if(msg != null)
				cliMonitor.PullClientInfo(msg.AsyncClient);
		}

		#endregion

		#region ProxyServer的回调

		public Action<string> Switch;

        public Action<string> Auto;

		void Join_OK(string msg) {
			ConsoleEx.DebugLog(war.Side.ToString() + " received Join_OK. CLient ID = " + msg, ConsoleEx.YELLOW);

			///
			/// 默认直接准备，实际上应该由UI控制
			///
			Utils.Assert(proxyReady == false, "Server IS NOT READY yet !");

			ReadyInfo join = new ReadyInfo() {
				ClientID   = msg,
			};

			string plainJoin = JSON.Instance.ToJSON(join);
			proxyServer.Ready(plainJoin);
		}

		void Ready_OK(string msg) {

		}

		void Ready_Error(string msg) {

		}

		void NotReady_OK(string msg) {

		}

		void NotReady_Error(string msg) {

		}

		void UIReady_OK(string msg) {

		}

		void UIReady_Error(string msg) {

		}

		void Quit_OK(string msg) {

		}

		void Quit_Error(string msg) {

		}

        void CastSkill_OK(string msg)
        {

        }

        void Attack_OK(string msg)
        {
           
        }
			
        void Move_OK(string msg)
        {

        }

        void MoveStop_OK(string msg)
        {

        }

		void Switch_OK(string msg) {
            AsyncTask.QueueOnMainThread (
                () =>  {
                    if(Switch != null) Switch(msg);
                }
            );
		}

		void Switch_Error(string msg) {

		}

		void ManualAuto_OK(string msg) {
            AsyncTask.QueueOnMainThread (
                () =>  {
                    if(Auto != null) Auto(msg);
                }
            );
		}

		void ManualAuto_Error(string msg) {

		}

		#endregion

		//信息缓存地
		private MsgPool<IpcMsg> pool;

		//信息接受者
		private SubClient CliSub;
		//和客户端交互的中转地
		private ClientCached cached;
		//代理服务器
		public ProxyServer proxyServer;

		private bool proxyReady = false;
		//监控服务器端的情况
		public readonly MonitorServer monitor;
		//其他客户端的信息
		public readonly SimpleMonitorClient cliMonitor;
		public readonly WarInfo war;

		public RealClient(WarInfo war) {
			this.war = war;

			cached = ClientCached.Instance;
			cached.clear();

			pool = new MsgPool<IpcMsg>(HandleIpcMsg);
			CliSub = new SubClient(pool, war);

			monitor = new MonitorServer();
			monitor.startMonitor(this.war, HeartBeatDisConn);

			cliMonitor = new SimpleMonitorClient();

			proxyReady = false;
		}

		public void Quit() {
			proxyReady = false;

			if(pool != null)        pool.QuitMsgPool();
			if(proxyServer != null) proxyServer.Quit();
			Thread.Sleep(10);
			if(CliSub != null)      CliSub.Quit();
			Thread.Sleep(10);
			if(monitor != null)     monitor.Quit();
		}

		void register( ) {
			proxyServer.registerRep(WarMsgConsts.JOINRep, Join_OK);
			proxyServer.registerRep(WarMsgConsts.ReadyRep, Ready_OK);
			proxyServer.registerRep(WarMsgConsts.ReadyRepE, Ready_Error);

			proxyServer.registerRep(WarMsgConsts.NotReadyRep, NotReady_OK);
			proxyServer.registerRep(WarMsgConsts.NotReadyRepE, NotReady_Error);

			proxyServer.registerRep(WarMsgConsts.UIReadyRep, UIReady_OK);
			proxyServer.registerRep(WarMsgConsts.UIReadyRepE, UIReady_Error);

			proxyServer.registerRep(WarMsgConsts.QuitRep, Quit_OK);
			proxyServer.registerRep(WarMsgConsts.QuitRepE, Quit_Error);

            proxyServer.registerRep(WarMsgConsts.CastSkRep, CastSkill_OK);
            proxyServer.registerRep(WarMsgConsts.AttackRep, Attack_OK);
            proxyServer.registerRep(WarMsgConsts.MoveRep, Move_OK);
            proxyServer.registerRep(WarMsgConsts.MoveStopRep, MoveStop_OK);

			proxyServer.registerRep(WarMsgConsts.SwitchRep, Switch_OK);
			proxyServer.registerRep(WarMsgConsts.SwitchRepE, Switch_Error);

			proxyServer.registerRep(WarMsgConsts.ManualOrAutoRep, ManualAuto_OK);
			proxyServer.registerRep(WarMsgConsts.ManualOrAutoRepE, ManualAuto_Error);
		}

		/// <summary>
		/// 所有消息的处理地方
		/// </summary>
		/// <param name="msg">Message.</param>
		void HandleIpcMsg(IpcMsg msg) {
			if(msg != null) 
            {
				switch(msg.op) 
                {
				case OP.CtorMap:
					IpcCreateMapMsg ctor = msg as IpcCreateMapMsg;
					MapInfo amap = new MapInfo() {
						ID   = ctor.MapId,
						type = (ConfigType) Enum.ToObject(typeof(ConfigType), ctor.MapType),
					};
					CtorEnv(amap);
					break;

				case OP.CtorNpc:
					IpcCreateNpcMsg ctorNpc = msg as IpcCreateNpcMsg;
					CtorNpc(ctorNpc);
					break;

                case OP.CtorHero:
                    IpcCreateHeroMsg crtHero = msg as IpcCreateHeroMsg;
                    CtorHero(crtHero);
                    break;

                case OP.NpcMove:
                    IpcNpcMoveMsg moveMsg = msg as IpcNpcMoveMsg;
                    NPCMove(moveMsg);
                    break;

                case OP.NpcHp:
                    IpcNpcHpMsg hpMsg = msg as IpcNpcHpMsg;
                    NPChp(hpMsg);
                    break;

                case OP.NpcAnim:
                    IpcNpcAnimMsg animMsg = msg as IpcNpcAnimMsg;
                    NPCAnim(animMsg);
                    break;

                case OP.NpcStatus:
                    IpcNpcStatusMsg statusMsg = msg as IpcNpcStatusMsg;
                    NPCStatus(statusMsg);
                    break;

				case OP.ServerReady:
					IpcServerReadyMsg SerInfo = msg as IpcServerReadyMsg;
					ServerInfo server = new ServerInfo(SerInfo);
					ServerReady(server);
					break;

				case OP.AsyncClient:
					IpcSyncClientMsg Sync = msg as IpcSyncClientMsg;
					SyncClient(Sync);
					break;

				case OP.EnterWar:
					IpcEnterWar enter = msg as IpcEnterWar;
					ServerInfo aserver = new ServerInfo(){
						ServerName = enter.ServerName,
						ServerID   = enter.ServerID,
					};
					MapInfo map = new MapInfo() {
						ID   = enter.MapId,
						type = (ConfigType) Enum.ToObject( typeof(ConfigType), enter.MapType),
					};
					EnterWar(aserver, map);
					break;

				case OP.ServerQuit:
					IpcServerQuitMsg quit = msg as IpcServerQuitMsg;
					ServerQuit(quit.ServerID);
					break;

                case OP.DestroyNpc:
                    IpcDestroyNpcMsg des = msg as IpcDestroyNpcMsg;
                    NpcDestroy(des);
                    break;

                case OP.SkillCD:
                    IpcSkillMsg skMsg = msg as IpcSkillMsg;
                    NpcSkillCD(skMsg);
                    break;
				}
			}
		}

		/// <summary>
		/// 心跳包检测到超时了
		/// 监控服务器端的情况
		/// </summary>
		void HeartBeatDisConn() {
			if(CliSub != null) CliSub.ReceiveTimeout();
			if(proxyServer != null) proxyServer.reqCli.ReceiveTimeout();
		}
	}
}