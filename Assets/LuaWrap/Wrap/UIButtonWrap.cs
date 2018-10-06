using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;

public class UIButtonWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("SetState", SetState),
		new LuaMethod("New", _CreateUIButton),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("current", get_current, set_current),
		new LuaField("dragHighlight", get_dragHighlight, set_dragHighlight),
		new LuaField("hoverSprite", get_hoverSprite, set_hoverSprite),
		new LuaField("pressedSprite", get_pressedSprite, set_pressedSprite),
		new LuaField("disabledSprite", get_disabledSprite, set_disabledSprite),
		new LuaField("hoverSprite2D", get_hoverSprite2D, set_hoverSprite2D),
		new LuaField("pressedSprite2D", get_pressedSprite2D, set_pressedSprite2D),
		new LuaField("disabledSprite2D", get_disabledSprite2D, set_disabledSprite2D),
		new LuaField("pixelSnap", get_pixelSnap, set_pixelSnap),
		new LuaField("onClick", get_onClick, set_onClick),
		new LuaField("isEnabled", get_isEnabled, set_isEnabled),
		new LuaField("normalSprite", get_normalSprite, set_normalSprite),
		new LuaField("normalSprite2D", get_normalSprite2D, set_normalSprite2D),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateUIButton(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			UIButton obj = new UIButton();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: UIButton.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(UIButton));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "UIButton", typeof(UIButton), regs, fields, "UIButtonColor");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_current(IntPtr L)
	{
		LuaScriptMgr.Push(L, UIButton.current);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_dragHighlight(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name dragHighlight");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index dragHighlight on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		LuaScriptMgr.Push(L, obj.dragHighlight);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_hoverSprite(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hoverSprite");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hoverSprite on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		LuaScriptMgr.Push(L, obj.hoverSprite);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pressedSprite(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressedSprite");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressedSprite on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		LuaScriptMgr.Push(L, obj.pressedSprite);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_disabledSprite(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name disabledSprite");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index disabledSprite on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		LuaScriptMgr.Push(L, obj.disabledSprite);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_hoverSprite2D(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hoverSprite2D");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hoverSprite2D on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		LuaScriptMgr.Push(L, obj.hoverSprite2D);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pressedSprite2D(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressedSprite2D");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressedSprite2D on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		LuaScriptMgr.Push(L, obj.pressedSprite2D);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_disabledSprite2D(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name disabledSprite2D");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index disabledSprite2D on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		LuaScriptMgr.Push(L, obj.disabledSprite2D);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pixelSnap(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pixelSnap");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pixelSnap on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		LuaScriptMgr.Push(L, obj.pixelSnap);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_onClick(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name onClick");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index onClick on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		LuaScriptMgr.PushObject(L, obj.onClick);
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

		UIButton obj = (UIButton)o;
		LuaScriptMgr.Push(L, obj.isEnabled);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_normalSprite(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name normalSprite");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index normalSprite on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		LuaScriptMgr.Push(L, obj.normalSprite);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_normalSprite2D(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name normalSprite2D");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index normalSprite2D on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		LuaScriptMgr.Push(L, obj.normalSprite2D);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_current(IntPtr L)
	{
		UIButton.current = LuaScriptMgr.GetNetObject<UIButton>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_dragHighlight(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name dragHighlight");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index dragHighlight on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		obj.dragHighlight = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_hoverSprite(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hoverSprite");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hoverSprite on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		obj.hoverSprite = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_pressedSprite(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressedSprite");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressedSprite on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		obj.pressedSprite = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_disabledSprite(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name disabledSprite");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index disabledSprite on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		obj.disabledSprite = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_hoverSprite2D(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hoverSprite2D");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hoverSprite2D on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		obj.hoverSprite2D = LuaScriptMgr.GetNetObject<Sprite>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_pressedSprite2D(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressedSprite2D");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressedSprite2D on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		obj.pressedSprite2D = LuaScriptMgr.GetNetObject<Sprite>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_disabledSprite2D(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name disabledSprite2D");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index disabledSprite2D on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		obj.disabledSprite2D = LuaScriptMgr.GetNetObject<Sprite>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_pixelSnap(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pixelSnap");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pixelSnap on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		obj.pixelSnap = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_onClick(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name onClick");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index onClick on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		obj.onClick = LuaScriptMgr.GetNetObject<List<EventDelegate>>(L, 3);
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

		UIButton obj = (UIButton)o;
		obj.isEnabled = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_normalSprite(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name normalSprite");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index normalSprite on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		obj.normalSprite = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_normalSprite2D(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name normalSprite2D");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index normalSprite2D on a nil value");
			}
		}

		UIButton obj = (UIButton)o;
		obj.normalSprite2D = LuaScriptMgr.GetNetObject<Sprite>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetState(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		UIButton obj = LuaScriptMgr.GetNetObject<UIButton>(L, 1);
		UIButtonColor.State arg0 = LuaScriptMgr.GetNetObject<UIButtonColor.State>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.SetState(arg0,arg1);
		return 0;
	}
}

