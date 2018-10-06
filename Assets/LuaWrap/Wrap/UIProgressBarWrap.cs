using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;

public class UIProgressBarWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("ForceUpdate", ForceUpdate),
		new LuaMethod("New", _CreateUIProgressBar),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("current", get_current, set_current),
		new LuaField("onDragFinished", get_onDragFinished, set_onDragFinished),
		new LuaField("thumb", get_thumb, set_thumb),
		new LuaField("numberOfSteps", get_numberOfSteps, set_numberOfSteps),
		new LuaField("onChange", get_onChange, set_onChange),
		new LuaField("cachedTransform", get_cachedTransform, null),
		new LuaField("cachedCamera", get_cachedCamera, null),
		new LuaField("foregroundWidget", get_foregroundWidget, set_foregroundWidget),
		new LuaField("backgroundWidget", get_backgroundWidget, set_backgroundWidget),
		new LuaField("fillDirection", get_fillDirection, set_fillDirection),
		new LuaField("value", get_value, set_value),
		new LuaField("alpha", get_alpha, set_alpha),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateUIProgressBar(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			UIProgressBar obj = new UIProgressBar();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: UIProgressBar.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(UIProgressBar));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "UIProgressBar", typeof(UIProgressBar), regs, fields, "UIWidgetContainer");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_current(IntPtr L)
	{
		LuaScriptMgr.Push(L, UIProgressBar.current);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_onDragFinished(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name onDragFinished");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index onDragFinished on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		LuaScriptMgr.PushObject(L, obj.onDragFinished);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_thumb(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name thumb");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index thumb on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		LuaScriptMgr.Push(L, obj.thumb);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_numberOfSteps(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name numberOfSteps");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index numberOfSteps on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		LuaScriptMgr.Push(L, obj.numberOfSteps);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_onChange(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name onChange");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index onChange on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		LuaScriptMgr.PushObject(L, obj.onChange);
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

		UIProgressBar obj = (UIProgressBar)o;
		LuaScriptMgr.Push(L, obj.cachedTransform);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_cachedCamera(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name cachedCamera");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index cachedCamera on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		LuaScriptMgr.Push(L, obj.cachedCamera);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_foregroundWidget(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name foregroundWidget");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index foregroundWidget on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		LuaScriptMgr.Push(L, obj.foregroundWidget);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_backgroundWidget(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name backgroundWidget");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index backgroundWidget on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		LuaScriptMgr.Push(L, obj.backgroundWidget);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_fillDirection(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillDirection");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillDirection on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		LuaScriptMgr.PushEnum(L, obj.fillDirection);
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

		UIProgressBar obj = (UIProgressBar)o;
		LuaScriptMgr.Push(L, obj.value);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_alpha(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name alpha");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index alpha on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		LuaScriptMgr.Push(L, obj.alpha);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_current(IntPtr L)
	{
		UIProgressBar.current = LuaScriptMgr.GetNetObject<UIProgressBar>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_onDragFinished(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name onDragFinished");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index onDragFinished on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		obj.onDragFinished = LuaScriptMgr.GetNetObject<UIProgressBar.OnDragFinished>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_thumb(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name thumb");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index thumb on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		obj.thumb = LuaScriptMgr.GetNetObject<Transform>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_numberOfSteps(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name numberOfSteps");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index numberOfSteps on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		obj.numberOfSteps = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_onChange(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name onChange");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index onChange on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		obj.onChange = LuaScriptMgr.GetNetObject<List<EventDelegate>>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_foregroundWidget(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name foregroundWidget");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index foregroundWidget on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		obj.foregroundWidget = LuaScriptMgr.GetNetObject<UIWidget>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_backgroundWidget(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name backgroundWidget");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index backgroundWidget on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		obj.backgroundWidget = LuaScriptMgr.GetNetObject<UIWidget>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_fillDirection(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillDirection");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillDirection on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		obj.fillDirection = LuaScriptMgr.GetNetObject<UIProgressBar.FillDirection>(L, 3);
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

		UIProgressBar obj = (UIProgressBar)o;
		obj.value = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_alpha(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name alpha");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index alpha on a nil value");
			}
		}

		UIProgressBar obj = (UIProgressBar)o;
		obj.alpha = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ForceUpdate(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UIProgressBar obj = LuaScriptMgr.GetNetObject<UIProgressBar>(L, 1);
		obj.ForceUpdate();
		return 0;
	}
}

