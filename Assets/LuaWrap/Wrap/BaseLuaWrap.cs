using System;
using LuaInterface;

public class BaseLuaWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("OnEvent", OnEvent),
		new LuaMethod("New", _CreateBaseLua),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("LuaName", get_LuaName, set_LuaName),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateBaseLua(IntPtr L)
	{
		LuaDLL.luaL_error(L, "BaseLua class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(BaseLua));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "BaseLua", typeof(BaseLua), regs, fields, "UnityEngine.MonoBehaviour");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LuaName(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name LuaName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index LuaName on a nil value");
			}
		}

		BaseLua obj = (BaseLua)o;
		LuaScriptMgr.Push(L, obj.LuaName);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_LuaName(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name LuaName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index LuaName on a nil value");
			}
		}

		BaseLua obj = (BaseLua)o;
		obj.LuaName = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnEvent(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types1 = {typeof(BaseLua), typeof(string)};

		if (count == 2)
		{
			BaseLua obj = LuaScriptMgr.GetNetObject<BaseLua>(L, 1);
			string arg0 = LuaScriptMgr.GetLuaString(L, 2);
			obj.OnEvent(arg0);
			return 0;
		}
		else if (LuaScriptMgr.CheckTypes(L, types1, 1) && LuaScriptMgr.CheckParamsType(L, typeof(object), 3, count - 2))
		{
			BaseLua obj = LuaScriptMgr.GetNetObject<BaseLua>(L, 1);
			string arg0 = LuaScriptMgr.GetString(L, 2);
			object[] objs1 = LuaScriptMgr.GetParamsObject(L, 3, count - 2);
			obj.OnEvent(arg0,objs1);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: BaseLua.OnEvent");
		}

		return 0;
	}
}

