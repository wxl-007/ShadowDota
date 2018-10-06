using System;
using UnityEngine;
using LuaInterface;

public class NGUIDebugWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("CreateInstance", CreateInstance),
		new LuaMethod("Log", Log),
		new LuaMethod("Clear", Clear),
		new LuaMethod("DrawBounds", DrawBounds),
		new LuaMethod("New", _CreateNGUIDebug),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("debugRaycast", get_debugRaycast, set_debugRaycast),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateNGUIDebug(IntPtr L)
	{
		LuaDLL.luaL_error(L, "NGUIDebug class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(NGUIDebug));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "NGUIDebug", typeof(NGUIDebug), regs, fields, "UnityEngine.MonoBehaviour");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_debugRaycast(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIDebug.debugRaycast);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_debugRaycast(IntPtr L)
	{
		NGUIDebug.debugRaycast = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateInstance(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		NGUIDebug.CreateInstance();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Log(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);
		object[] objs0 = LuaScriptMgr.GetParamsObject(L, 1, count);
		NGUIDebug.Log(objs0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Clear(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		NGUIDebug.Clear();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DrawBounds(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Bounds arg0 = LuaScriptMgr.GetNetObject<Bounds>(L, 1);
		NGUIDebug.DrawBounds(arg0);
		return 0;
	}
}

