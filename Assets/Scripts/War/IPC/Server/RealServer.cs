using System;
using System.Reflection;
using fastJSON;
using System.Collections.Generic;
using System.Threading;

namespace AW.War {
	using NetMQ;

	public class RealServer : IServer {
		#region Req的远程方法
		/// <summary>
		/// 加入战斗
		/// </summary>
		/// <param name="ClientID">Client ID</param>
		public void Join(string ClientInfo) {
			JoinInfo join = JSON.Instance.ToObject<JoinInfo>(ClientInfo);
			///加入监控
			monitor.AddJoin(join);
			processedMsg = new NetMQMessage();
			processedMsg.Append(WarMsgConsts.JOINRep);
			processedMsg.Append(join.ClientID);

			if(monitor.validateSyncClient) {
				proxyCli.SyncClient(monitor.getSyncMsg);
			}
		}

		public void Ready(string info) {
			ReadyInfo readyInfo = JSON.Instance.ToObject<ReadyInfo>(info);

			processedMsg = new NetMQMessage();

			///加入监控
			bool found = monitor.Ready(readyInfo);
			if(found)
				processedMsg.Append(WarMsgConsts.ReadyRep);
			else 
				processedMsg.Append(WarMsgConsts.ReadyRepE);

			if(monitor.validateSyncClient) {
				proxyCli.SyncClient(monitor.getSyncMsg);
			}
		}

		public void NotReady(string info) {
			ReadyInfo notReadyInfo = JSON.Instance.ToObject<ReadyInfo>(info);
			processedMsg = new NetMQMessage();

			///加入监控
			bool found = monitor.NotReady(notReadyInfo);
			if(found) {
				processedMsg.Append(WarMsgConsts.NotReadyRep);
			} else {
				processedMsg.Append(WarMsgConsts.NotReadyRepE);
			}

			if(monitor.validateSyncClient) {
				proxyCli.SyncClient(monitor.getSyncMsg);
			}
		}

		public void UIReady(string info) {
			UIReadyInfo ready = JSON.Instance.ToObject<UIReadyInfo>(info);
			processedMsg = new NetMQMessage();

			///加入监控
			bool found = monitor.UIReady(ready);
			if(found) {
				processedMsg.Append(WarMsgConsts.UIReadyRep);
			} else {
				processedMsg.Append(WarMsgConsts.UIReadyRepE);
			}

			if(monitor.validateSyncClient) {
				proxyCli.SyncClient(monitor.getSyncMsg);
			}
		}

		public void Quit(string ClientInfo) {
			JoinInfo join = JSON.Instance.ToObject<JoinInfo>(ClientInfo);
			processedMsg = new NetMQMessage();

			///加入监控
			bool found = monitor.QuitJoin(join);
			if(found) {
				processedMsg.Append(WarMsgConsts.QuitRep);
			} else {
				processedMsg.Append(WarMsgConsts.QuitRepE);
			}

			if(monitor.validateSyncClient) {
				proxyCli.SyncClient(monitor.getSyncMsg);
			}
		}

		public void Switch(string switchinfo) {

            SwitchInfo si = JSON.Instance.ToObject<SwitchInfo>(switchinfo);

            processedMsg = new NetMQMessage();

            bool ok = monitor.CharactorPool.SwitchActiveHero(si);

            if(ok)
            {
                processedMsg.Append(WarMsgConsts.SwitchRep);
                processedMsg.Append(si.UniqueID.ToString());
            }
            else
            {
                processedMsg.Append(WarMsgConsts.SwitchRepE);
            }
		}

        public void ManualAuto(string autoInfo) {
			ManualOrAuto auto = JSON.Instance.ToObject<ManualOrAuto>(autoInfo);

			processedMsg = new NetMQMessage();

			bool isAuto = false;
			bool ok = monitor.CharactorPool.SwitchManulOrAuto(auto, ref isAuto);
            
			if(ok) {
				processedMsg.Append(WarMsgConsts.ManualOrAutoRep);
				processedMsg.Append(isAuto ? "1" : "0");


				//如果战斗开始了，切换AI
				if(WarServerManager.Instance.battleStart) {
					WarUIInfo ui = new WarUIInfo();
					ui.camp = auto.camp;
					ui.ClientID = auto.ClientID;
					ui.uniqueId = auto.UniqueID;

					ServerLifeNpc npc = monitor.CharactorPool.findActionNpc(ui);

					//TODO : switch AI
					AsyncTask.QueueOnMainThread( () =>{
						if(npc != null) npc.SwitchAutoBattle(isAuto);
					});
				} 

			} else {
				processedMsg.Append(WarMsgConsts.ManualOrAutoRepE);
			}
		}

        public void CastSkill(string CastInfo)
        {
            NpcAnimInfo info = JSON.Instance.ToObject<NpcAnimInfo>(CastInfo);
            ServerLifeNpc npc = monitor.CharactorPool.findActionNpc(info.ui);
            AsyncTask.QueueOnMainThread(
            () => 
            {
                if(npc != null)
                {
                    npc.SwitchAutoBattle(false);
                    npc.CastSkill(info.index);
                }
            });

            CastSkillInfo cInfo = new CastSkillInfo()
            {
                ClientID = DeviceInfo.GUID,
                index = info.index,
                cdTime = npc.runSkMd.getRuntimeSkill(info.index).skillCfg.BaseCD,
            };

            processedMsg = new NetMQMessage();
            processedMsg.Append(WarMsgConsts.CastSkRep);
            processedMsg.Append(JSON.Instance.ToJSON(cInfo));
		}

        public void Attack(string AttackInfo)
        {
            AsyncTask.QueueOnMainThread(
                () => 
                {
                    WarServerManager mgr = WarServerManager.Instance;
                    if(mgr != null)
                    {
                        NpcAnimInfo info = JSON.Instance.ToObject<NpcAnimInfo>(AttackInfo);
                        ServerLifeNpc npc = monitor.CharactorPool.findActionNpc(info.ui);
                        if(npc != null)
                        {
                            npc.Attack();
                        }
                    }
                }
            );
            processedMsg = new NetMQMessage();
            processedMsg.Append(WarMsgConsts.AttackRep);
        }

        public void Move(string MoveInfo)
        {
            AsyncTask.QueueOnMainThread(
                () => { 

                WarServerManager mgr = WarServerManager.Instance;
                if(mgr != null)
                {
                    NpcAnimInfo info = JSON.Instance.ToObject<NpcAnimInfo>(MoveInfo);
                    ServerLifeNpc npc = monitor.CharactorPool.findActionNpc(info.ui);
                    WarSrcAnimParam param = new WarSrcAnimParam();
                    param.cmdType = WarMsg_Type.ManualInput;
                    param.param = MoveInfo;
                    mgr.npcMgr.SendMessageAsync(npc.UniqueID, npc.UniqueID, param);
                }
            }
            );
            processedMsg = new NetMQMessage();
            processedMsg.Append(WarMsgConsts.MoveRep);
        }

        public void MoveStop(string MoveStopInfo)
        {
            AsyncTask.QueueOnMainThread(
                () => { 

                WarServerManager mgr = WarServerManager.Instance;
                if(mgr != null)
                {
                    NpcAnimInfo info = JSON.Instance.ToObject<NpcAnimInfo>(MoveStopInfo);
                    ServerLifeNpc npc = monitor.CharactorPool.findActionNpc(info.ui);
                    WarSrcAnimParam param = new WarSrcAnimParam();
                    param.cmdType = WarMsg_Type.Stand;
                    param.param = MoveStopInfo;
                    mgr.npcMgr.SendMessageAsync(npc.UniqueID, npc.UniqueID, param);
                }
            }
            );
            processedMsg = new NetMQMessage();
            processedMsg.Append(WarMsgConsts.MoveStopRep);
        }

		#endregion
		  
		//战斗消息推送
		private PubServer publisher;

		//一对一，数据核对
		private ResponseServer Resper;

		/// <summary>
		/// 代理客户端, 因为NetMQ已经能很好的应付 1:N 的情况，
		/// 所以这里也不再需要代理客户端池
		/// </summary>
		public readonly ProxyClient proxyCli;

		//处理过的数据
		private NetMQMessage processedMsg;

		//监控客户端
		public readonly MonitorClient monitor = null;

		public readonly ServerCached cached;

		private Type type;
		private BindingFlags c_bf;
		private Dictionary<string, MethodInfo> cmdMethod = null;

		/// <summary>
		/// ResponseSocket绑定成功后的回调
		/// </summary>
		private Action RepBindCompleted;

		/// <summary>
		/// PublisherSocket绑定成功后的回调
		/// </summary>
		private Action PubBindCompleted;

		public readonly WarInfo mWar;

		public RealServer(WarInfo war, Action RepBinded, Action PubBinded) {
			mWar      = war;

			cached    = ServerCached.Instance;
			cached.clear();

			RepBindCompleted = RepBinded;
			PubBindCompleted = PubBinded;

			monitor   = new MonitorClient();
			monitor.startMonitor(mWar);

			EngineCfg engCfg = Core.EngCfg;
			cached.curServer = new ServerInfo(){
				IpAddr   = "127.0.0.1",
				PubPort  = engCfg.PubPort,
				PairPort = engCfg.PairPort,
				HeartBeatPort = engCfg.HeartBeatPort,
				ServerName = "Allen",
				ServerID = DeviceInfo.GUID,
			};

			publisher = new PubServer(war, PubBindCompleted);
			Resper    = new ResponseServer(war, HandleMQMsg, RepBindCompleted);

			type      = GetType();
			c_bf      = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
			cmdMethod = new Dictionary<string, MethodInfo>();
			cachedMethod();

			proxyCli  = new ProxyClient(-1, publisher);
		}

		/// <summary>
		/// 退出清理socket
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="AW.War.RealServer"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="AW.War.RealServer"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="AW.War.RealServer"/> so the garbage
		/// collector can reclaim the memory that the <see cref="AW.War.RealServer"/> was occupying.</remarks>
		public void Dispose() {
			if(publisher != null) publisher.Quit();
			Thread.Sleep(16);
			if(Resper != null)    Resper.Quit();
			Thread.Sleep(16);
			if(monitor != null)   monitor.Quit();
		}

		void cachedMethod() {
			MethodInfo mi = null;

			//
			mi = type.GetMethod(WarMsgConsts.JOINReq, c_bf);
			cmdMethod[WarMsgConsts.JOINReq] = mi;

			mi = type.GetMethod(WarMsgConsts.ReadyReq, c_bf);
			cmdMethod[WarMsgConsts.ReadyReq] = mi;

			mi = type.GetMethod(WarMsgConsts.NotReadyReq, c_bf);
			cmdMethod[WarMsgConsts.NotReadyReq] = mi;

			mi = type.GetMethod(WarMsgConsts.UIReadyReq, c_bf);
			cmdMethod[WarMsgConsts.UIReadyReq] = mi;

			mi = type.GetMethod(WarMsgConsts.QuitReq, c_bf);
			cmdMethod[WarMsgConsts.QuitReq] = mi;

            mi = type.GetMethod(WarMsgConsts.AttackReq, c_bf);
            cmdMethod[WarMsgConsts.AttackReq] = mi;

            mi = type.GetMethod(WarMsgConsts.CastSkReq, c_bf);
            cmdMethod[WarMsgConsts.CastSkReq] = mi;

            mi = type.GetMethod(WarMsgConsts.MoveReq, c_bf);
            cmdMethod[WarMsgConsts.MoveReq] = mi;

            mi = type.GetMethod(WarMsgConsts.MoveStopReq, c_bf);
            cmdMethod[WarMsgConsts.MoveStopReq] = mi;

			mi = type.GetMethod(WarMsgConsts.SwitchReq, c_bf);
			cmdMethod[WarMsgConsts.SwitchReq] = mi;

			mi = type.GetMethod(WarMsgConsts.ManualOrAutoReq, c_bf);
			cmdMethod[WarMsgConsts.ManualOrAutoReq] = mi;

		}

		/// <summary>
		/// 处理所有客户端发动过来的数据
		/// </summary>
		/// <param name="msg">Message.</param>
		NetMQMessage HandleMQMsg(NetMQMessage msg) {
			if(msg != null) {
				int count = msg.FrameCount;
				string cmd = msg[0].ConvertToString();
				string arg = null;
				if(count > 1) {
					arg = msg[1].ConvertToString();
				}
				MethodInfo mi = null;
				if(cmdMethod.TryGetValue(cmd, out mi)) {
					if(arg != null)
						mi.Invoke(this, new object[]{arg});
					else 
						mi.Invoke(this, null);
				}

				if(processedMsg != null)
					return processedMsg;
			}
			return null;
		}

	}
}