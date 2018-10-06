using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class TweenScaleWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("Begin", Begin),
		new LuaMethod("SetStartToCurrentValue", SetStartToCurrentValue),
		new LuaMethod("SetEndToCurrentValue", SetEndToCurrentValue),
		new LuaMethod("New", _CreateTweenScale),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("from", get_from, set_from),
		new LuaField("to", get_to, set_to),
		new LuaField("updateTable", get_updateTable, set_updateTable),
		new LuaField("cachedTransform", get_cachedTransform, null),
		new LuaField("value", get_value, set_value),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateTweenScale(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			TweenScale obj = new TweenScale();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: TweenScale.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(TweenScale));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "TweenScale", typeof(TweenScale), regs, fields, "UITweener");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_from(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name from");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index from on a nil value");
			}
		}

		TweenScale obj = (TweenScale)o;
		LuaScriptMgr.PushValue(L, obj.from);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_to(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name to");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index to on a nil value");
			}
		}

		TweenScale obj = (TweenScale)o;
		LuaScriptMgr.PushValue(L, obj.to);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_updateTable(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name updateTable");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index updateTable on a nil value");
			}
		}

		TweenScale obj = (TweenScale)o;
		LuaScriptMgr.Push(L, obj.updateTable);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_cachedTransform(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name cachedTransform");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index cachedTransform on a nil value");
			}
		}

		TweenScale obj = (TweenScale)o;
		LuaScriptMgr.Push(L, obj.cachedTransform);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_value(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name value");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index value on a nil value");
			}
		}

		TweenScale obj = (TweenScale)o;
		LuaScriptMgr.PushValue(L, obj.value);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_from(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name from");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index from on a nil value");
			}
		}

		TweenScale obj = (TweenScale)o;
		obj.from = LuaScriptMgr.GetNetObject<Vector3>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_to(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name to");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index to on a nil value");
			}
		}

		TweenScale obj = (TweenScale)o;
		obj.to = LuaScriptMgr.GetNetObject<Vector3>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_updateTable(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name updateTable");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index updateTable on a nil value");
			}
		}

		TweenScale obj = (TweenScale)o;
		obj.updateTable = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_value(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name value");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index value on a nil value");
			}
		}

		TweenScale obj = (TweenScale)o;
		obj.value = LuaScriptMgr.GetNetObject<Vector3>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Begin(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		float arg1 = (float)LuaScriptMgr.GetNumber(L, 2);
		Vector3 arg2 = LuaScriptMgr.GetNetObject<Vector3>(L, 3);
		TweenScale o = TweenScale.Begin(arg0,arg1,arg2);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetStartToCurrentValue(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TweenScale obj = LuaScriptMgr.GetNetObject<TweenScale>(L, 1);
		obj.SetStartToCurrentValue();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetEndToCurrentValue(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TweenScale obj = LuaScriptMgr.GetNetObject<TweenScale>(L, 1);
		obj.SetEndToCurrentValue();
		return 0;
	}
}

