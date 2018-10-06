using System;
using UnityEngine;
using System.Collections;
using AW.Data;
using System.IO;
using fastJSON;

public class EngineCfg : BaseData {
	/// <summary>
	/// 游戏版本号
	/// </summary>
	public int VersionCode;

	/// <summary>
	/// 游戏显示出来的版本号
	/// </summary>
	public string VersionName;

	/// <summary>
	/// 平台的ID号
	/// </summary>
	public int PlatformID;

	/// <summary>
	/// Http 超时事件
	/// </summary>
	public int HttpTimeOut;

	/// <summary>
	/// 游戏启动时候的用户中心
	/// </summary>
	public string UserCenter;

	/// 
	/// 游戏的语言设定
	/// 
	public string Country;

	/// <summary>
	/// 下载资源的位置
	/// </summary>
	public string ResourceURL;

	/// <summary>
	/// 默认的错误字符
	/// </summary>
	public string InvalidJson;

	/// <summary>
	/// 无效的网络连接
	/// </summary>
	public string InvalidNetwork;

	//默认的帧率
	public int FrameRate;

	//发布端口
	public int PubPort;

	//一对一端口
	public int PairPort;

	//心跳端口
	public int HeartBeatPort;

	//通讯数据的上限
	public int HighWatermark;
}

/// 
/// 这是最先启动起来的一块功能，用来读取核心的底层Framework依赖的数据
/// 
public class EngineCfgReader {

	private EngineCfgReader(){ }
	private static EngineCfgReader _instance;
	public static EngineCfgReader instance {
		get {
			return _instance ?? (_instance = new EngineCfgReader());
		}
	}

	public void ReadCfg(MonoBehaviour worker, Action<EngineCfg> ReadFinished) {
		if(worker != null) worker.StartCoroutine(ReadAsync(ReadFinished));
	}

	/// <summary>
	/// 异步读取
	/// </summary>
	private IEnumerator ReadAsync(Action<EngineCfg> ReadFinished) {
		EngineCfg config = null;
		WWW www = null;

		string srcPath = null;
		#if UNITY_IOS
		srcPath = "file://" + Path.Combine(DeviceInfo.StreamingPath, "Config/Engine.cfg");
		#elif UNITY_ANDROID
		srcPath = Path.Combine(DeviceInfo.StreamingPath, "Config/Engine.cfg");
		#endif

		#if UNITY_EDITOR
		srcPath = "file:///" + Path.Combine(DeviceInfo.StreamingPath, "Config/Engine.cfg");
		#endif

		www = new WWW(srcPath);
		yield return www;
		byte[] data = www.bytes;

		using(StreamReader sr = new StreamReader(new MemoryStream(data))) {
			string wholeTxt = sr.ReadToEnd();
			config = JSON.Instance.ToObject<EngineCfg>(wholeTxt);
		}

		www.Dispose();

		if(ReadFinished != null) ReadFinished(config);
	}


}