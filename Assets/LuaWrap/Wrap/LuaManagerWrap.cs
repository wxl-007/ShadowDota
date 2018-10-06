using System;
using LuaInterface;

public class LuaManagerWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("GetLua", GetLua),
		new LuaMethod("AddLua", AddLua),
		new LuaMethod("RemoveLua", RemoveLua),
		new LuaMethod("CallFunction", CallFunction),
		new LuaMethod("LuaPath", LuaPath),
		new LuaMethod("ResourcePath", ResourcePath),
		new LuaMethod("New", _CreateLuaManager),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("DEBUG_LUA", get_DEBUG_LUA, set_DEBUG_LUA),
		new LuaField("Instance", get_Instance, null),
		new LuaField("ReadWritePath", get_ReadWritePath, set_ReadWritePath),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateLuaManager(IntPtr L)
	{
		LuaDLL.luaL_error(L, "LuaManager class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(LuaManager));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "LuaManager", typeof(LuaManager), regs, fields, "UnityEngine.MonoBehaviour");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_DEBUG_LUA(IntPtr L)
	{
		LuaScriptMgr.Push(L, LuaManager.DEBUG_LUA);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		LuaScriptMgr.Push(L, LuaManager.Instance);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ReadWritePath(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name ReadWritePath");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index ReadWritePath on a nil value");
			}
		}

		LuaManager obj = (LuaManager)o;
		LuaScriptMgr.Push(L, obj.ReadWritePath);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_DEBUG_LUA(IntPtr L)
	{
		LuaManager.DEBUG_LUA = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ReadWritePath(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name ReadWritePath");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index ReadWritePath on a nil value");
			}
		}

		LuaManager obj = (LuaManager)o;
		obj.ReadWritePath = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLua(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		LuaManager obj = LuaScriptMgr.GetNetObject<LuaManager>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		LuaScriptMgr o = obj.GetLua(arg0);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddLua(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		LuaManager obj = LuaScriptMgr.GetNetObject<LuaManager>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		LuaScriptMgr arg1 = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 3);
		bool o = obj.AddLua(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveLua(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		LuaManager obj = LuaScriptMgr.GetNetObject<LuaManager>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		bool o = obj.RemoveLua(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CallFunction(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);
		LuaManager obj = LuaScriptMgr.GetNetObject<LuaManager>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		object[] objs2 = LuaScriptMgr.GetParamsObject(L, 4, count - 3);
		obj.CallFunction(arg0,arg1,objs2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LuaPath(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = LuaManager.LuaPath(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResourcePath(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		LuaManager obj = LuaScriptMgr.GetNetObject<LuaManager>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		string o = obj.ResourcePath(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}

