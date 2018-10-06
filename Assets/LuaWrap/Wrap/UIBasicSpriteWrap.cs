using System;
using LuaInterface;

public class UIBasicSpriteWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("New", _CreateUIBasicSprite),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("centerType", get_centerType, set_centerType),
		new LuaField("leftType", get_leftType, set_leftType),
		new LuaField("rightType", get_rightType, set_rightType),
		new LuaField("bottomType", get_bottomType, set_bottomType),
		new LuaField("topType", get_topType, set_topType),
		new LuaField("type", get_type, set_type),
		new LuaField("flip", get_flip, set_flip),
		new LuaField("fillDirection", get_fillDirection, set_fillDirection),
		new LuaField("fillAmount", get_fillAmount, set_fillAmount),
		new LuaField("minWidth", get_minWidth, null),
		new LuaField("minHeight", get_minHeight, null),
		new LuaField("invert", get_invert, set_invert),
		new LuaField("hasBorder", get_hasBorder, null),
		new LuaField("premultipliedAlpha", get_premultipliedAlpha, null),
		new LuaField("pixelSize", get_pixelSize, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateUIBasicSprite(IntPtr L)
	{
		LuaDLL.luaL_error(L, "UIBasicSprite class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(UIBasicSprite));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "UIBasicSprite", typeof(UIBasicSprite), regs, fields, "UIWidget");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_centerType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name centerType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index centerType on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.PushEnum(L, obj.centerType);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_leftType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name leftType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index leftType on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.PushEnum(L, obj.leftType);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_rightType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name rightType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index rightType on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.PushEnum(L, obj.rightType);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_bottomType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name bottomType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index bottomType on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.PushEnum(L, obj.bottomType);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_topType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name topType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index topType on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.PushEnum(L, obj.topType);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_type(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name type");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index type on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.PushEnum(L, obj.type);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_flip(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name flip");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index flip on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.PushEnum(L, obj.flip);
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

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.PushEnum(L, obj.fillDirection);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_fillAmount(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillAmount");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillAmount on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.Push(L, obj.fillAmount);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_minWidth(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name minWidth");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index minWidth on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.Push(L, obj.minWidth);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_minHeight(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name minHeight");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index minHeight on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.Push(L, obj.minHeight);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_invert(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name invert");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index invert on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.Push(L, obj.invert);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_hasBorder(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hasBorder");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hasBorder on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.Push(L, obj.hasBorder);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_premultipliedAlpha(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name premultipliedAlpha");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index premultipliedAlpha on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.Push(L, obj.premultipliedAlpha);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pixelSize(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pixelSize");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pixelSize on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		LuaScriptMgr.Push(L, obj.pixelSize);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_centerType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name centerType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index centerType on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		obj.centerType = LuaScriptMgr.GetNetObject<UIBasicSprite.AdvancedType>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_leftType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name leftType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index leftType on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		obj.leftType = LuaScriptMgr.GetNetObject<UIBasicSprite.AdvancedType>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_rightType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name rightType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index rightType on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		obj.rightType = LuaScriptMgr.GetNetObject<UIBasicSprite.AdvancedType>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_bottomType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name bottomType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index bottomType on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		obj.bottomType = LuaScriptMgr.GetNetObject<UIBasicSprite.AdvancedType>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_topType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name topType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index topType on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		obj.topType = LuaScriptMgr.GetNetObject<UIBasicSprite.AdvancedType>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_type(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name type");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index type on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		obj.type = LuaScriptMgr.GetNetObject<UIBasicSprite.Type>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_flip(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name flip");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index flip on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		obj.flip = LuaScriptMgr.GetNetObject<UIBasicSprite.Flip>(L, 3);
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

		UIBasicSprite obj = (UIBasicSprite)o;
		obj.fillDirection = LuaScriptMgr.GetNetObject<UIBasicSprite.FillDirection>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_fillAmount(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillAmount");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillAmount on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		obj.fillAmount = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_invert(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name invert");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index invert on a nil value");
			}
		}

		UIBasicSprite obj = (UIBasicSprite)o;
		obj.invert = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}
}

