using System;
using AW.Event;
using AW.Timer;
using AW.Resources;
using AW.Data;
using AW.IO;
using AW.FSM;
using AW.Entity;
using NetMQ;

public static class Core {

    //Device States-Machine
	public static DeviceFSM DevFSM;
	//Game Play States-Machine
	public static GamePlayFSM GameFSM;
    //Game Event-Center
    public static EventCenter EVC;
    //Persist Data Manager
    public static LocalIOManager DPM;
	//Entity Manager
	public static EntityManager EntityMgr;
    //Network engine
    public static NetworkEngine NetEng;
	//Timer engine
    public static TimerMaster TimerEng;
	//AsynchTask engine
	public static AsyncTask AsyncEng;
	//Sound Manager engine;
	public static SoundEngine SoundEng;
    //Resource Download Engine
	public static Loader ResEng;
	//Data core
	public static DataCore Data;
	//the configure of framework
	public static EngineCfg EngCfg;
	//a Coroutine instance
	public static CoroutineProvider Coroutine;
	//The NetMQContext is what is used to create ALL sockets.
	public static NetMQContext ZeroMQ;

	/// <summary>
	/// Initialize this instance. 
	/// We must follow special sequnce.
	/// 仅且仅初始化一次
	/// </summary>
	public static void Initialize(EngineCfg cfg) {
		ConsoleEx.DebugLog("Core Engine is initializing ....", ConsoleEx.YELLOW);
		//Initial sequnce
		DevFSM  = DeviceFSM.Instance;
		GameFSM = GamePlayFSM.Instance;
		EngCfg  = cfg;

		if(GameFSM.InitOK == Consts.FAILURE) {
			//Have one NetMQContext ONLY. This will be used to created ALL sockets within the process.
			ZeroMQ = NetMQContext.Create();

			//DataPersisteManager should be initialize first. and tell the non-account path
			DPM = LocalIOManager.getInstance(DeviceInfo.PersistRootPath);
			//Unity UI Basically Manager
			EntityMgr = new EntityManager();
			//Timer should run.
			TimerEng = new TimerMaster();
			//Sound manager.
			SoundEng = SoundEngine.GetSingleton();
			//EventCenter must initialize later than Network Engine
			NetEng = new NetworkEngine();
			//EventCenter also must initialize later than Aysnc Engine
			AsyncEng = AsyncTask.Current;
			//Coroutine
			Coroutine = CoroutineProvider.Instance();

			EVC = new EventCenter(NetEng.httpEngine, NetEng.SockEngine, EntityMgr);
			ResEng = new Loader();

			Data = new DataCore();

			//register some vars.
			CoreParam();
		} else {
			//TODO: 此分支是初始化一次OK后，注销之后进入的分支情况
		}

		RegisterInterface();
		ConsoleEx.DebugLog("Core Engine is initialized ....", ConsoleEx.YELLOW);
		GameFSM.OnInitOk();
	}

	/// <summary>
	/// 初始化核心参数
	/// </summary>
	static void CoreParam() {
		CountryRegion.country           = EngCfg.Country;
		HttpRequestFactory.swInfo       = EngCfg.VersionCode;
		HttpRequestFactory.platformId   = EngCfg.PlatformID;
		HttpClient.USER_CENTER          = EngCfg.UserCenter;
		ResourceSetting.URL             = EngCfg.ResourceURL;
		HttpResponseFactory.InvalidJson = EngCfg.InvalidJson;
		HttpThread.UNABLE_GET_RESPONSE  = EngCfg.InvalidNetwork;
	}


	static void RegisterInterface() {
		///
		/// 关心游戏逻辑状态的, 第一个DPM是最先的。 其他不重要
		///
		GameFSM.registerStateReceiver(DPM);
		GameFSM.registerStateReceiver(TimerEng);
		GameFSM.registerStateReceiver(NetEng.httpEngine);

		///
		/// 关心设备逻辑状态的
		///
		DevFSM.registerStateReceiver(TimerEng);
	}

}

