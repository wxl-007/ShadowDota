using System;
using System.Net;
using System.ComponentModel;
using LuaInterface;

public class DownLoadFromWebWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("DownLoad", DownLoad),
		new LuaMethod("DownLoadTask", DownLoadTask),
		new LuaMethod("ProgressChanged", ProgressChanged),
		new LuaMethod("DownloadCompleted", DownloadCompleted),
		new LuaMethod("New", _CreateDownLoadFromWeb),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("luaM", get_luaM, set_luaM),
		new LuaField("luaName", get_luaName, set_luaName),
		new LuaField("callback_completed", get_callback_completed, set_callback_completed),
		new LuaField("callback_progress", get_callback_progress, set_callback_progress),
		new LuaField("uri", get_uri, set_uri),
		new LuaField("savePath", get_savePath, set_savePath),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateDownLoadFromWeb(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			DownLoadFromWeb obj = new DownLoadFromWeb();
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: DownLoadFromWeb.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(DownLoadFromWeb));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "DownLoadFromWeb", typeof(DownLoadFromWeb), regs, fields, "System.Object");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_luaM(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name luaM");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index luaM on a nil value");
			}
		}

		DownLoadFromWeb obj = (DownLoadFromWeb)o;
		LuaScriptMgr.Push(L, obj.luaM);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_luaName(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name luaName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index luaName on a nil value");
			}
		}

		DownLoadFromWeb obj = (DownLoadFromWeb)o;
		LuaScriptMgr.Push(L, obj.luaName);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_callback_completed(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name callback_completed");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index callback_completed on a nil value");
			}
		}

		DownLoadFromWeb obj = (DownLoadFromWeb)o;
		LuaScriptMgr.Push(L, obj.callback_completed);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_callback_progress(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name callback_progress");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index callback_progress on a nil value");
			}
		}

		DownLoadFromWeb obj = (DownLoadFromWeb)o;
		LuaScriptMgr.Push(L, obj.callback_progress);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_uri(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name uri");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index uri on a nil value");
			}
		}

		DownLoadFromWeb obj = (DownLoadFromWeb)o;
		LuaScriptMgr.Push(L, obj.uri);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_savePath(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name savePath");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index savePath on a nil value");
			}
		}

		DownLoadFromWeb obj = (DownLoadFromWeb)o;
		LuaScriptMgr.Push(L, obj.savePath);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_luaM(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name luaM");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index luaM on a nil value");
			}
		}

		DownLoadFromWeb obj = (DownLoadFromWeb)o;
		obj.luaM = LuaScriptMgr.GetNetObject<LuaManager>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_luaName(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name luaName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index luaName on a nil value");
			}
		}

		DownLoadFromWeb obj = (DownLoadFromWeb)o;
		obj.luaName = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_callback_completed(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name callback_completed");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index callback_completed on a nil value");
			}
		}

		DownLoadFromWeb obj = (DownLoadFromWeb)o;
		obj.callback_completed = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_callback_progress(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name callback_progress");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index callback_progress on a nil value");
			}
		}

		DownLoadFromWeb obj = (DownLoadFromWeb)o;
		obj.callback_progress = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_uri(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name uri");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index uri on a nil value");
			}
		}

		DownLoadFromWeb obj = (DownLoadFromWeb)o;
		obj.uri = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_savePath(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name savePath");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index savePath on a nil value");
			}
		}

		DownLoadFromWeb obj = (DownLoadFromWeb)o;
		obj.savePath = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DownLoad(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 6);
		DownLoadFromWeb obj = LuaScriptMgr.GetNetObject<DownLoadFromWeb>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		string arg2 = LuaScriptMgr.GetLuaString(L, 4);
		string arg3 = LuaScriptMgr.GetLuaString(L, 5);
		string arg4 = LuaScriptMgr.GetLuaString(L, 6);
		obj.DownLoad(arg0,arg1,arg2,arg3,arg4);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DownLoadTask(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		DownLoadFromWeb obj = LuaScriptMgr.GetNetObject<DownLoadFromWeb>(L, 1);
		object arg0 = LuaScriptMgr.GetVarObject(L, 2);
		obj.DownLoadTask(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ProgressChanged(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		DownLoadFromWeb obj = LuaScriptMgr.GetNetObject<DownLoadFromWeb>(L, 1);
		object arg0 = LuaScriptMgr.GetVarObject(L, 2);
		DownloadProgressChangedEventArgs arg1 = LuaScriptMgr.GetNetObject<DownloadProgressChangedEventArgs>(L, 3);
		obj.ProgressChanged(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DownloadCompleted(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		DownLoadFromWeb obj = LuaScriptMgr.GetNetObject<DownLoadFromWeb>(L, 1);
		object arg0 = LuaScriptMgr.GetVarObject(L, 2);
		AsyncCompletedEventArgs arg1 = LuaScriptMgr.GetNetObject<AsyncCompletedEventArgs>(L, 3);
		obj.DownloadCompleted(arg0,arg1);
		return 0;
	}
}

