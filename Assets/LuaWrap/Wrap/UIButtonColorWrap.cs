using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class UIButtonColorWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("ResetDefaultColor", ResetDefaultColor),
		new LuaMethod("SetState", SetState),
		new LuaMethod("UpdateColor", UpdateColor),
		new LuaMethod("New", _CreateUIButtonColor),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("tweenTarget", get_tweenTarget, set_tweenTarget),
		new LuaField("hover", get_hover, set_hover),
		new LuaField("pressed", get_pressed, set_pressed),
		new LuaField("disabledColor", get_disabledColor, set_disabledColor),
		new LuaField("duration", get_duration, set_duration),
		new LuaField("state", get_state, set_state),
		new LuaField("defaultColor", get_defaultColor, set_defaultColor),
		new LuaField("isEnabled", get_isEnabled, set_isEnabled),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateUIButtonColor(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			UIButtonColor obj = new UIButtonColor();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: UIButtonColor.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(UIButtonColor));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "UIButtonColor", typeof(UIButtonColor), regs, fields, "UIWidgetContainer");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_tweenTarget(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name tweenTarget");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index tweenTarget on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		LuaScriptMgr.Push(L, obj.tweenTarget);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_hover(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hover");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hover on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		LuaScriptMgr.PushValue(L, obj.hover);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pressed(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressed");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressed on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		LuaScriptMgr.PushValue(L, obj.pressed);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_disabledColor(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name disabledColor");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index disabledColor on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		LuaScriptMgr.PushValue(L, obj.disabledColor);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_duration(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name duration");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index duration on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		LuaScriptMgr.Push(L, obj.duration);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_state(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name state");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index state on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		LuaScriptMgr.PushEnum(L, obj.state);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_defaultColor(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name defaultColor");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index defaultColor on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		LuaScriptMgr.PushValue(L, obj.defaultColor);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_isEnabled(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name isEnabled");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index isEnabled on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		LuaScriptMgr.Push(L, obj.isEnabled);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_tweenTarget(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name tweenTarget");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index tweenTarget on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		obj.tweenTarget = LuaScriptMgr.GetNetObject<GameObject>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_hover(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hover");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hover on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		obj.hover = LuaScriptMgr.GetNetObject<Color>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_pressed(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressed");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressed on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		obj.pressed = LuaScriptMgr.GetNetObject<Color>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_disabledColor(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name disabledColor");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index disabledColor on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		obj.disabledColor = LuaScriptMgr.GetNetObject<Color>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_duration(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name duration");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index duration on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		obj.duration = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_state(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name state");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index state on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		obj.state = LuaScriptMgr.GetNetObject<UIButtonColor.State>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_defaultColor(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name defaultColor");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index defaultColor on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		obj.defaultColor = LuaScriptMgr.GetNetObject<Color>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_isEnabled(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name isEnabled");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index isEnabled on a nil value");
			}
		}

		UIButtonColor obj = (UIButtonColor)o;
		obj.isEnabled = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResetDefaultColor(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UIButtonColor obj = LuaScriptMgr.GetNetObject<UIButtonColor>(L, 1);
		obj.ResetDefaultColor();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetState(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		UIButtonColor obj = LuaScriptMgr.GetNetObject<UIButtonColor>(L, 1);
		UIButtonColor.State arg0 = LuaScriptMgr.GetNetObject<UIButtonColor.State>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.SetState(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateColor(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIButtonColor obj = LuaScriptMgr.GetNetObject<UIButtonColor>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.UpdateColor(arg0);
		return 0;
	}
}

