using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LuaInterface;

public class LuaManager : MonoBehaviour 
{
	private static LuaManager _this;
	public static LuaManager Instance
	{
		get
		{
			if(_this == null)
			{
				GameObject o = new GameObject();
				o.name = "LuaManager";
				GameObject UIController = GameObject.FindWithTag("UIController");
				if(UIController != null)
				{
					o.transform.parent = UIController.transform;
				}
				else
				DontDestroyOnLoad(o);

				_this = o.AddComponent<LuaManager>();
				_this.dic_Lua = new Dictionary<string, LuaScriptMgr>();
				//LuaStatic.Load = Loader;
			}
			return _this;
		}
	}

	private Dictionary<string,LuaScriptMgr> dic_Lua ;



#if DEBUG_LUA
	public static  bool DEBUG_LUA = true;
#else
	public static  bool DEBUG_LUA = false;
#endif


	//游戏的读写路径(手机上的)
	string defalutReadWritePath =  DeviceInfo.PersistRootPath+"/ReadWritePath";

	public string ReadWritePath
	{
		get{return defalutReadWritePath;}
		set{defalutReadWritePath = value;}
	}


	public LuaScriptMgr GetLua(string LuaName)
	{
		LuaScriptMgr lua = null;
		dic_Lua.TryGetValue(LuaName,out lua);
		return lua;
	}

	public bool AddLua(string LuaName,LuaScriptMgr lua)
	{
		bool result = false;
		if(!dic_Lua.ContainsKey(LuaName))
		{
			result = true;
			dic_Lua.Add(LuaName,lua);
		}
		return result;
	}

	public bool RemoveLua(string LuaName)
	{
		bool result = false;
		if(dic_Lua.ContainsKey(LuaName))
		{
			result = true;
			dic_Lua.Remove(LuaName);
		}
		return result;
	}

	public void CallFunction(string luaName, string functionName , params object[] args )
	{
		LuaScriptMgr lua = GetLua(luaName);
		if(lua == null) return;
		//lua.CallLuaFunction(functionName,args);
		LuaFunction func = lua.GetLuaFunction(functionName);
		if(func != null)
		{
			if(args == null)
				func.Call();
			else
				func.Call(args);
		}
		else
			Debug.LogError("Not found lua function:"+functionName);
	} 

	public static string LuaPath(string luaName)
	{
		string luaFileName = luaName;
		if(!luaName.Contains(".lua")) 
			luaFileName = luaName+".lua";

		//优先读取手机上的路径
#if DEBUG_LUA
		string path = System.IO.Path.Combine( _this.ReadWritePath+"/Scripts" , luaFileName);
		if(!System.IO.File.Exists(path))
			path = System.IO.Path.Combine( Application.streamingAssetsPath+"/ReadWritePath/Scripts" , luaFileName);
#else
		string path = System.IO.Path.Combine( Application.streamingAssetsPath+"/ReadWritePath/Scripts" , luaFileName);
#endif
		return  path;
	}

	public string ResourcePath(string resourceName)
	{
		string resName = resourceName;
		if(!resName.Contains(".assetbundle")) 
			resName += ".assetbundle";
#if DEBUG_LUA
		string path = System.IO.Path.Combine( ReadWritePath+"/Source" , resName);
		if(!System.IO.File.Exists(path))
			path = System.IO.Path.Combine( Application.streamingAssetsPath+"/ReadWritePath/Source" , resName);
#else
		string path = System.IO.Path.Combine( Application.streamingAssetsPath+"/ReadWritePath/Source" , resName);
#endif
		return path;
	}
	
}
