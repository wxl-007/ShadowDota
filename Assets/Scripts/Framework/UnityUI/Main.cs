using UnityEngine;
using System.Collections;
using AW.FSM;
using NetMQ;

/// 
/// 游戏唯一的启动点，游戏所有的底层都在这里被初始化起来
/// 
public class Main : MonoBehaviour {


	public LuaCoroutine.Timer timer;

	void Awake() {
		DontDestroyOnLoad(gameObject);
		//读取完毕后的回调
		EngineCfgReader.instance.ReadCfg(this, ReadFinished);
	}

	void ReadFinished(EngineCfg cfg) {
		ConsoleEx.DebugLog("---Read Engine Configure Finished ----");
		Application.targetFrameRate = cfg.FrameRate;
		Core.Initialize(cfg);
	}

	void Update() {
		timer.OnUpdate(Time.deltaTime);
	}

	void OnApplicationPause (bool pauseStatus) {
		if(Core.DevFSM != null)
			Core.DevFSM.handleStateChg(pauseStatus ? DeviceState.GamePaused : DeviceState.GameResume);
	}

	void OnApplicationQuit() {
		ConsoleEx.DebugLog("--------- OnApplicationQuit ---------", ConsoleEx.YELLOW);
		if(Core.DevFSM != null) {
			Core.DevFSM.handleStateChg(DeviceState.GameQuit);
		}
		if(Core.ZeroMQ != null)
			Core.ZeroMQ.Dispose();
	}

}
