using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

namespace AW.War {

	/// <summary>
	/// 客户端当前的状态（保存在服务器的）
	/// </summary>
	[Flags]
	public enum ClientStatus {
		None    = 0x0,
		Join    = 0x1,
		Ready   = 0x2,
		UIReady = 0x4,
		Timeout = 0x8,
		//add more
	}

	public static class WarStatusExtension {
		//可检查单一
		public static bool check(this ClientStatus flags, ClientStatus totest ) {
			return (flags & totest) == totest;
		}

		public static ClientStatus set(this ClientStatus flags, ClientStatus totest) {
			return flags | totest;
		}

		public static ClientStatus clear(this ClientStatus flags, ClientStatus totest) {
			return flags & ~totest;
		}

		public static bool anysame(this ClientStatus flags, ClientStatus totest) {
			return (flags & totest) != 0;
		}
	}

	/// <summary>
	/// 客户端的代理，主要用来维护状态
	/// </summary>
	public class VirtualClient {
		//客户端唯一标示
		public string ClientID;
		//客户端当前状态
		public ClientStatus curStatus;
		//心跳的间隔
		public long HeartTick;

		public VirtualClient() { }

		public VirtualClient(JoinInfo info) {
			ClientID  = info.ClientID;
			curStatus = curStatus.set(ClientStatus.Join);
			HeartTick = 0;
		}
	}

	/// <summary>
	/// 服务器端 和 客户端， 都会对所有的客户端情况监视。
	/// 但是 服务器端的监视是主动的，客户端的监视信息是从服务器端推送过来的
	/// </summary>
	public class BaseMonitor {
		/// <summary>
		/// 获取里面的数据
		/// </summary>
		public readonly Dictionary<string, VirtualClient> ClientPool = null;

		public BaseMonitor() {
			ClientPool = new Dictionary<string, VirtualClient>();
		}
	}

	/// <summary>
	/// 存活于服务器端， 监控客户端情况的类
	/// </summary>
	public class MonitorClient : BaseMonitor {
		/// <summary>
		/// 获取角色的池, 目前设计的是不允许 在非战斗场景里的其他线程读取（比如UI），
		/// 在战斗场景里则没有限制（因为写操作都是在房间界面完成，而读操作不允许在房间界面）
		/// </summary>
		public readonly WarServerCharactor CharactorPool = null;

		private Stopwatch stopwatch;
		private TimerTask task;

		//10s
		private const long Timeout = 10000;

		/// <summary>
		/// 读写的共享锁
		/// </summary>
		private readonly ReaderWriterLockSlim readerWriterLock = null;

		/// <summary>
		/// 已经连接的客户端总数
		/// </summary>
		/// <value>The connected client count.</value>
		public int ConnectedClientCount {
			get;
			private set;
		}

		/// <summary>
		/// 已准备好的客户端数量（进入战斗场景前）
		/// </summary>
		/// <value>The ready client count.</value>
		public int ReadyClientCount {
			get;
			private set;
		}

		/// <summary>
		/// UI已准备好的客户端数量（进入战斗场景后）
		/// </summary>
		/// <value>The user interface ready client count.</value>
		public int UIReadyClientCount {
			get;
			private set;
		}

		/// <summary>
		/// UI准备好之后，断线连接的情况
		/// </summary>
		/// <value>The dis connected count.</value>
		public int DisConnectedCount {
			get;
			private set;
		}

		/// <summary>
		/// 心跳服务器，只有进程间通讯才需要
		/// </summary>
		private HeartBeatServer hbServer = null;

		/// <summary>
		/// 有需要同步的信息吗
		/// </summary>
		public bool validateSyncClient {
			get;
			private set;
		}
		private IpcSyncClientMsg sync = null;

		private WarInfo warInfo;

		public MonitorClient() : base () {
			CharactorPool = new WarServerCharactor();
			sync = new IpcSyncClientMsg();
			validateSyncClient = false;

			stopwatch  = Stopwatch.StartNew();

			///
			/// 因为NetMq的缘故，所有的对Dictionary，只有NetMq线程写入, Timer线程只负责读
			///
			readerWriterLock = new ReaderWriterLockSlim();

			task       = new TimerTask(TimerTask.RIGHTNOW, TimerTask.INFINITY, 1);
			task.onEvent = checkClientStatus;
			task.DispatchToRealHandler();
		}

		public void startMonitor (WarInfo war) {
			warInfo = war;
			if(warInfo.warMo == WarMode.InLanWar) 
				hbServer = new HeartBeatServer(warInfo, this);
		}

		public void AddJoin(JoinInfo joinin) {
			if(joinin != null) {

				if(ClientPool.ContainsKey(joinin.ClientID) == false) {
					ConnectedClientCount += 1;
					VirtualClient client = new VirtualClient(joinin);
					CharactorPool.JoinRoom(joinin.Charactor);

					//申请写锁
					readerWriterLock.EnterWriteLock();
					ClientPool.Add(joinin.ClientID, client);
					//释放写锁
					readerWriterLock.ExitWriteLock();

					AsyncClient();
				}

			}
		}

		public bool Ready(ReadyInfo ready) {
			bool setOk = false;

			if(ready != null) {
				VirtualClient client = null;
				setOk = ClientPool.TryGetValue(ready.ClientID, out client);

				if(setOk) {
					if(client.curStatus.anysame(ClientStatus.Ready) == false) {
						client.curStatus = client.curStatus.set(ClientStatus.Ready);
						ReadyClientCount += 1;

						AsyncClient();
					}
				}
			}

			return setOk;
		}

		public bool NotReady(ReadyInfo notready) {
			bool setOk = false;

			if(notready != null) {

				VirtualClient client = null;
				setOk = ClientPool.TryGetValue(notready.ClientID, out client);
				if(setOk) {
					client.curStatus = client.curStatus.clear(ClientStatus.Ready);
					ReadyClientCount -= 1;

					AsyncClient();
				}

			}

			return setOk;
		}

		public bool UIReady(UIReadyInfo uiready) {
			bool setOk = false;

			if(uiready != null) {

				VirtualClient client = null;
				setOk = ClientPool.TryGetValue(uiready.ClientID, out client);
				if(setOk) {

					if(client.curStatus.check(ClientStatus.Ready)) {
						client.curStatus = client.curStatus.set(ClientStatus.UIReady);
						UIReadyClientCount += 1;
					} else {
						setOk = false;
					}

				}

			}

			return setOk;
		}

		public bool QuitJoin(JoinInfo joinin) {
			bool setOk = false;

			if(joinin != null) {

				VirtualClient client = null;
				setOk = ClientPool.TryGetValue(joinin.ClientID, out client);

				if(setOk) {

					CharactorPool.QuitRoom(joinin.Charactor);

					//申请写锁
					readerWriterLock.EnterWriteLock();
					ClientPool.Remove(joinin.ClientID);
					//释放写锁
					readerWriterLock.ExitWriteLock();

					ConnectedClientCount -= 1;
					if(client.curStatus.check(ClientStatus.Ready))
						ReadyClientCount -= 1;
					if(client.curStatus.check(ClientStatus.UIReady))
						UIReadyClientCount -= 1;

					//guard
					if(ConnectedClientCount < 0) ConnectedClientCount = 0;
					if(ReadyClientCount < 0) ReadyClientCount = 0;
					if(UIReadyClientCount < 0) UIReadyClientCount = 0;

					AsyncClient();
				}

			}

			return setOk;
		}

		public void OnHeartBeat(string clientID) {
			if(!string.IsNullOrEmpty(clientID)) {
				VirtualClient client = null;
				bool setOk = ClientPool.TryGetValue(clientID, out client);
				if(setOk) {
					client.HeartTick = stopwatch.ElapsedMilliseconds;
				}
			}
		}

		void checkClientStatus(TimerTask task) {
			long now = stopwatch.ElapsedMilliseconds;
			//申请读锁
			readerWriterLock.EnterReadLock();
			int dis = 0;
			foreach(VirtualClient client in ClientPool.Values) {
				long delta = now - client.HeartTick;
				if(delta > Timeout) {
					client.curStatus = client.curStatus.set(ClientStatus.Timeout);
					dis += 1;
					//TODO : 超时了，还算有效的链接吗，
					//需要 ConnectedClientCount -=1 & ReadyClientCount -= 1 吗？
				} else {
					client.curStatus = client.curStatus.clear(ClientStatus.Timeout);
				}
			}

			//释放读锁
			readerWriterLock.ExitReadLock();

			DisConnectedCount = dis;
		}

		public void Quit() {
			ClientPool.Clear();
			stopwatch.Stop();

			if(CharactorPool != null) CharactorPool.HostQuit();

			if(task != null) {
				task.Enabled = false;
			}

			if(hbServer != null) hbServer.Quit();
		}

		/// <summary>
		/// 监视客户端的情况，一旦发生改变，将广播通知各个客户端的情况
		/// </summary>
		void AsyncClient() {

			if(warInfo.warMo == WarMode.InLanWar) {

				//含有有效的同步数据
				validateSyncClient = true;

				List<VirCli> syncInfo = new List<VirCli>();
				//不需要读写锁
				foreach(VirtualClient client in ClientPool.Values) {
					syncInfo.Add( new VirCli() {
						ClientID = client.ClientID,
						curStatus = (int) client.curStatus,
					} );
				}
				sync.AsyncClient = syncInfo.ToArray();
			}

		}

		public IpcSyncClientMsg getSyncMsg {
			get {
				validateSyncClient = false;
				return sync;
			}
		}

	}


}
