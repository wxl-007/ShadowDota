using UnityEngine;
using System.Collections;
using LuaInterface;
using System.IO;
using System.Net;

public class LuaTools : MonoBehaviour {

	private static LuaTools _instance = null;
	private LuaManager luaManager = null;
	public static LuaTools _this
	{
		get
		{
			if(_instance == null)
			{
				GameObject o = new GameObject();
				o.name = "LuaTools";
				_instance = o.AddComponent<LuaTools>();
				o.transform.parent = LuaManager.Instance.transform;
				_instance.luaManager = LuaManager.Instance;
			}
			return _instance;
		}
	}


    public readonly string localPath = "UI/LS/";



	public static void Invoke(LuaFunction function,float time,params object[] args)
	{
		_this.StartCoroutine(_this.InvokeFinished(function,time,args));
	}

	IEnumerator InvokeFinished(LuaFunction function,float time,params object[] args)
	{
		yield return new WaitForSeconds(time);
		if(function != null)
		{
			if(args == null)
				function.Call();
			else
			{
				function.Call(args);
			}
		}
	}


	public static void Invoke(string luaName,string functionName,float time,params object[] args)
	{
		_this.StartCoroutine(_this.InvokeFinished(luaName,functionName,time,args));
	}

	IEnumerator InvokeFinished(string luaName,string functionName,float time,params object[] args)
	{
		yield return new WaitForSeconds(time);
		if(luaManager != null)
		{
			LuaScriptMgr lua = luaManager.GetLua(luaName);
			if(lua != null)
			{
				LuaFunction function = lua.GetLuaFunction(functionName);
				if(function != null)
				{
					if(args == null)
						function.Call();
					else
					{
						function.Call(args);
					}
				}
			}
		}
	}


	//加载一个Assetbundle资源,使用方式封装成Resource.Load()
	public static void LoadResource(string luaName,string ResourceName,string functionName)
	{
		_this.StartCoroutine(_this.LoadResourceSync(luaName,ResourceName,functionName));
	}

	IEnumerator LoadResourceSync(string luaName, string ResourceName,string functionName)
	{
		string path = luaManager.ResourcePath(ResourceName);
		try 
		{
			byte[] content = System.IO.File.ReadAllBytes(path);
			AssetBundle bundle = AssetBundle.CreateFromMemoryImmediate(content);
			if(bundle == null)
				Debug.LogError(path);
			else
			   _this.luaManager.CallFunction (luaName, functionName, new object[]{ bundle, ResourceName });
		}
		catch (System.Exception ex) 
		{
			Debug.LogError(ex.ToString());
		}

		yield return 0;
	}

	public static System.Type GetUIModule()
	{
		return typeof(UIModule);
	}	

	//解压ZIP包
	public static bool UnZip(string zipPath,string targetPath)
	{
		string []FileProperties=new string[2]; 
		FileProperties[0]= zipPath;
		FileProperties[1]= targetPath;		
		DeCompression.UnZipClass UnZc=new DeCompression.UnZipClass();  
		return UnZc.UnZip(FileProperties);
	}

	//获取文件MD5码
	public static string GetMD5(string filePath)
	{
		return MessageDigest_Algorithm.getFileMd5Hash(filePath);
	}


	public static System.Type GetType(object o)
	{
		return o.GetType();
	}

	public static void CallLuaFunction(LuaFunction f)
	{
		Debug.Log(f);
		f.Call();
	}

}
