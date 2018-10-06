using System;
using UnityEngine;
using LuaInterface;

public class UISpriteWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("GetAtlasSprite", GetAtlasSprite),
		new LuaMethod("MakePixelPerfect", MakePixelPerfect),
		new LuaMethod("OnFill", OnFill),
		new LuaMethod("New", _CreateUISprite),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("material", get_material, null),
		new LuaField("atlas", get_atlas, set_atlas),
		new LuaField("spriteName", get_spriteName, set_spriteName),
		new LuaField("isValid", get_isValid, null),
		new LuaField("border", get_border, null),
		new LuaField("pixelSize", get_pixelSize, null),
		new LuaField("minWidth", get_minWidth, null),
		new LuaField("minHeight", get_minHeight, null),
		new LuaField("drawingDimensions", get_drawingDimensions, null),
		new LuaField("premultipliedAlpha", get_premultipliedAlpha, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateUISprite(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			UISprite obj = new UISprite();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: UISprite.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(UISprite));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "UISprite", typeof(UISprite), regs, fields, "UIBasicSprite");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_material(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name material");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index material on a nil value");
			}
		}

		UISprite obj = (UISprite)o;
		LuaScriptMgr.Push(L, obj.material);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_atlas(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name atlas");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index atlas on a nil value");
			}
		}

		UISprite obj = (UISprite)o;
		LuaScriptMgr.Push(L, obj.atlas);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_spriteName(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name spriteName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index spriteName on a nil value");
			}
		}

		UISprite obj = (UISprite)o;
		LuaScriptMgr.Push(L, obj.spriteName);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_isValid(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name isValid");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index isValid on a nil value");
			}
		}

		UISprite obj = (UISprite)o;
		LuaScriptMgr.Push(L, obj.isValid);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_border(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name border");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index border on a nil value");
			}
		}

		UISprite obj = (UISprite)o;
		LuaScriptMgr.PushValue(L, obj.border);
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

		UISprite obj = (UISprite)o;
		LuaScriptMgr.Push(L, obj.pixelSize);
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

		UISprite obj = (UISprite)o;
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

		UISprite obj = (UISprite)o;
		LuaScriptMgr.Push(L, obj.minHeight);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_drawingDimensions(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name drawingDimensions");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index drawingDimensions on a nil value");
			}
		}

		UISprite obj = (UISprite)o;
		LuaScriptMgr.PushValue(L, obj.drawingDimensions);
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

		UISprite obj = (UISprite)o;
		LuaScriptMgr.Push(L, obj.premultipliedAlpha);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_atlas(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name atlas");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index atlas on a nil value");
			}
		}

		UISprite obj = (UISprite)o;
		obj.atlas = LuaScriptMgr.GetNetObject<UIAtlas>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_spriteName(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name spriteName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index spriteName on a nil value");
			}
		}

		UISprite obj = (UISprite)o;
		obj.spriteName = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAtlasSprite(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UISprite obj = LuaScriptMgr.GetNetObject<UISprite>(L, 1);
		UISpriteData o = obj.GetAtlasSprite();
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int MakePixelPerfect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UISprite obj = LuaScriptMgr.GetNetObject<UISprite>(L, 1);
		obj.MakePixelPerfect();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnFill(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		UISprite obj = LuaScriptMgr.GetNetObject<UISprite>(L, 1);
		BetterList<Vector3> arg0 = LuaScriptMgr.GetNetObject<BetterList<Vector3>>(L, 2);
		BetterList<Vector2> arg1 = LuaScriptMgr.GetNetObject<BetterList<Vector2>>(L, 3);
		BetterList<Color32> arg2 = LuaScriptMgr.GetNetObject<BetterList<Color32>>(L, 4);
		obj.OnFill(arg0,arg1,arg2);
		return 0;
	}
}

