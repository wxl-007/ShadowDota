using System;
using System.Text;
using UnityEngine;
using System.IO;

public static class DeviceInfo {	

	public static string GUID {
		get {
			return UniqueGUID.getInstance().getUniqueIdetify();
		}
	}

	public static string GetDeviceUniqueID(){
	#if !UNITY_EDITOR && (UNITY_IOS || UNITY_IPHONE)
		return string.Empty;
	#else
		return SystemInfo.deviceUniqueIdentifier;
	#endif
	}

	private static string _streaming;
	public static string StreamingPath {
		get {
			if(string.IsNullOrEmpty(_streaming)) 
				_streaming = Application.streamingAssetsPath;
			return _streaming;
		}
	}

	/// <summary>
	/// 所有的游戏里面的2D资源下载到本地的存放位置
	/// </summary>
	private static string _artdownload;
	public static string ArtPath {
		get {
			if(string.IsNullOrEmpty(_artdownload))
				_artdownload = Path.Combine(PersistRootPath, "Art");
			return _artdownload;
		}
	}

	private static string _configdownload;
	public static string ConfigDownload {
		get {
			if(string.IsNullOrEmpty(_configdownload)) {
				_configdownload = Path.Combine(PersistRootPath, "Config");
			}
			return _configdownload;
		}
	}

	//We will Keep Path in Cached.
	private static string _persist;
	public static string PersistRootPath {
		get {
			if(string.IsNullOrEmpty(_persist))
				_persist = GetDocumentsPath();
			return _persist;
		}
	}

	//return full path which is stored in the documents
	//Ios under Documents
	//Android under /data/data/$packname/files/
	//Windows phone under
	public static string PersisitFullPath (string name)
	{
		string path = null;

		switch(Application.platform){
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXPlayer:
				string activeDir = Directory.GetCurrentDirectory (); 
				path = Path.Combine (activeDir, name);
				return path;
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.Android:
			case RuntimePlatform.WP8Player:
				path = Path.Combine (PersistRootPath, name);
				return path;
			default:
				break;
		}
		return string.Empty;
	}

	//return the root of document path
	private static string GetDocumentsPath() {
		string path = string.Empty;

		switch(Application.platform) {
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.OSXPlayer:
			path = Directory.GetCurrentDirectory ();
			break;
		case RuntimePlatform.IPhonePlayer:
			path = GetiPhoneDocumentsPath();
			break;
		case RuntimePlatform.Android:
			path = GetAndroidDocumentsPath();
			break;
		case RuntimePlatform.WP8Player:
			path = GetWPDocumentsPath();
			break;
		default:
			break;
		}
		return path;
	}

	/// <summary>
	/// Gets the WP documents path.
	/// Needed to be finished.
	/// </summary>
	/// <returns>The WP documents path.</returns>
	private static string GetWPDocumentsPath () {
		return string.Empty;
	}

	//Get Android Documents
	//not in the sdcard
	private static string GetAndroidDocumentsPath() {
		//Application.persistentDataPath 是指向android /data/data/xxx.xxx.xxx/files/
		//Application.dataPath 是指向 /data/app/xxx.xxx.xxx.apk
		return Application.persistentDataPath;
	}

	//Get iphone Documents
	private static string GetiPhoneDocumentsPath () 
	{ 
		// Your game has read+write access to /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/Documents 
		// Application.dataPath returns              
		// /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data 
		// Strip "/Data" from path 

		//  IOS 8 has new document path: /var/mobile/Containers/Data/Application/<XXXXX>/Documents


		/* 所以这些代码都不在使用
		string path = string.Empty;
		path = Application.dataPath.Substring (0, Application.dataPath.Length - 5); 
		// Strip application name 
		path = path.Substring(0, path.LastIndexOf('/'));  
		return path + "/Documents"; */

		return Application.persistentDataPath;
	}

	//pass user id to "code"
	/*	public static string Exec_CheckSum(int code) {
	#if UNITY_IPHONE
		string exec_path = IOS.getExecutablePath();
		string execHash = MessageDigest_Algorithm.getFileMd5Hash(exec_path);

		string codeHash = System.Convert.ToString(code);
		string finalText = execHash + codeHash;
		string finalExecHash = MessageDigest_Algorithm.getMd5Hash(finalText);
		return finalExecHash;
	#elif UNITY_ANDROID
		return null;
	#else
	    return null;
	#endif
	}
	*/
	}

