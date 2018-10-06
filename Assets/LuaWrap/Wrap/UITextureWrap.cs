using System;
using UnityEngine;
using LuaInterface;

public class UITextureWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("MakePixelPerfect", MakePixelPerfect),
		new LuaMethod("OnFill", OnFill),
		new LuaMethod("New", _CreateUITexture),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("mainTexture", get_mainTexture, set_mainTexture),
		new LuaField("material", get_material, set_material),
		new LuaField("shader", get_shader, set_shader),
		new LuaField("premultipliedAlpha", get_premultipliedAlpha, null),
		new LuaField("border", get_border, set_border),
		new LuaField("uvRect", get_uvRect, set_uvRect),
		new LuaField("drawingDimensions", get_drawingDimensions, null),
		new LuaField("fixedAspect", get_fixedAspect, set_fixedAspect),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateUITexture(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			UITexture obj = new UITexture();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: UITexture.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(UITexture));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "UITexture", typeof(UITexture), regs, fields, "UIBasicSprite");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mainTexture(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name mainTexture");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index mainTexture on a nil value");
			}
		}

		UITexture obj = (UITexture)o;
		LuaScriptMgr.Push(L, obj.mainTexture);
		return 1;
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

		UITexture obj = (UITexture)o;
		LuaScriptMgr.Push(L, obj.material);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_shader(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name shader");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index shader on a nil value");
			}
		}

		UITexture obj = (UITexture)o;
		LuaScriptMgr.Push(L, obj.shader);
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

		UITexture obj = (UITexture)o;
		LuaScriptMgr.Push(L, obj.premultipliedAlpha);
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

		UITexture obj = (UITexture)o;
		LuaScriptMgr.PushValue(L, obj.border);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_uvRect(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name uvRect");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index uvRect on a nil value");
			}
		}

		UITexture obj = (UITexture)o;
		LuaScriptMgr.PushValue(L, obj.uvRect);
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

		UITexture obj = (UITexture)o;
		LuaScriptMgr.PushValue(L, obj.drawingDimensions);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_fixedAspect(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fixedAspect");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fixedAspect on a nil value");
			}
		}

		UITexture obj = (UITexture)o;
		LuaScriptMgr.Push(L, obj.fixedAspect);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mainTexture(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name mainTexture");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index mainTexture on a nil value");
			}
		}

		UITexture obj = (UITexture)o;
		obj.mainTexture = LuaScriptMgr.GetNetObject<Texture>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_material(IntPtr L)
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

		UITexture obj = (UITexture)o;
		obj.material = LuaScriptMgr.GetNetObject<Material>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_shader(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name shader");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index shader on a nil value");
			}
		}

		UITexture obj = (UITexture)o;
		obj.shader = LuaScriptMgr.GetNetObject<Shader>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_border(IntPtr L)
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

		UITexture obj = (UITexture)o;
		obj.border = LuaScriptMgr.GetNetObject<Vector4>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_uvRect(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name uvRect");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index uvRect on a nil value");
			}
		}

		UITexture obj = (UITexture)o;
		obj.uvRect = LuaScriptMgr.GetNetObject<Rect>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_fixedAspect(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fixedAspect");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fixedAspect on a nil value");
			}
		}

		UITexture obj = (UITexture)o;
		obj.fixedAspect = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int MakePixelPerfect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UITexture obj = LuaScriptMgr.GetNetObject<UITexture>(L, 1);
		obj.MakePixelPerfect();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnFill(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		UITexture obj = LuaScriptMgr.GetNetObject<UITexture>(L, 1);
		BetterList<Vector3> arg0 = LuaScriptMgr.GetNetObject<BetterList<Vector3>>(L, 2);
		BetterList<Vector2> arg1 = LuaScriptMgr.GetNetObject<BetterList<Vector2>>(L, 3);
		BetterList<Color32> arg2 = LuaScriptMgr.GetNetObject<BetterList<Color32>>(L, 4);
		obj.OnFill(arg0,arg1,arg2);
		return 0;
	}
}

