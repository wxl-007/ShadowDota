using System;
using UnityEngine;
using LuaInterface;

public class UIWidgetWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("SetDimensions", SetDimensions),
		new LuaMethod("GetSides", GetSides),
		new LuaMethod("CalculateFinalAlpha", CalculateFinalAlpha),
		new LuaMethod("Invalidate", Invalidate),
		new LuaMethod("CalculateCumulativeAlpha", CalculateCumulativeAlpha),
		new LuaMethod("SetRect", SetRect),
		new LuaMethod("ResizeCollider", ResizeCollider),
		new LuaMethod("FullCompareFunc", FullCompareFunc),
		new LuaMethod("PanelCompareFunc", PanelCompareFunc),
		new LuaMethod("CalculateBounds", CalculateBounds),
		new LuaMethod("SetDirty", SetDirty),
		new LuaMethod("RemoveFromPanel", RemoveFromPanel),
		new LuaMethod("MarkAsChanged", MarkAsChanged),
		new LuaMethod("CreatePanel", CreatePanel),
		new LuaMethod("CheckLayer", CheckLayer),
		new LuaMethod("ParentHasChanged", ParentHasChanged),
		new LuaMethod("UpdateVisibility", UpdateVisibility),
		new LuaMethod("UpdateTransform", UpdateTransform),
		new LuaMethod("UpdateGeometry", UpdateGeometry),
		new LuaMethod("WriteToBuffers", WriteToBuffers),
		new LuaMethod("MakePixelPerfect", MakePixelPerfect),
		new LuaMethod("OnFill", OnFill),
		new LuaMethod("New", _CreateUIWidget),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("onChange", get_onChange, set_onChange),
		new LuaField("onPostFill", get_onPostFill, set_onPostFill),
		new LuaField("mOnRender", get_mOnRender, set_mOnRender),
		new LuaField("autoResizeBoxCollider", get_autoResizeBoxCollider, set_autoResizeBoxCollider),
		new LuaField("hideIfOffScreen", get_hideIfOffScreen, set_hideIfOffScreen),
		new LuaField("keepAspectRatio", get_keepAspectRatio, set_keepAspectRatio),
		new LuaField("aspectRatio", get_aspectRatio, set_aspectRatio),
		new LuaField("hitCheck", get_hitCheck, set_hitCheck),
		new LuaField("panel", get_panel, set_panel),
		new LuaField("geometry", get_geometry, set_geometry),
		new LuaField("fillGeometry", get_fillGeometry, set_fillGeometry),
		new LuaField("drawCall", get_drawCall, set_drawCall),
		new LuaField("onRender", get_onRender, set_onRender),
		new LuaField("drawRegion", get_drawRegion, set_drawRegion),
		new LuaField("pivotOffset", get_pivotOffset, null),
		new LuaField("width", get_width, set_width),
		new LuaField("height", get_height, set_height),
		new LuaField("color", get_color, set_color),
		new LuaField("alpha", get_alpha, set_alpha),
		new LuaField("isVisible", get_isVisible, null),
		new LuaField("hasVertices", get_hasVertices, null),
		new LuaField("rawPivot", get_rawPivot, set_rawPivot),
		new LuaField("pivot", get_pivot, set_pivot),
		new LuaField("depth", get_depth, set_depth),
		new LuaField("raycastDepth", get_raycastDepth, null),
		new LuaField("localCorners", get_localCorners, null),
		new LuaField("localSize", get_localSize, null),
		new LuaField("localCenter", get_localCenter, null),
		new LuaField("worldCorners", get_worldCorners, null),
		new LuaField("worldCenter", get_worldCenter, null),
		new LuaField("drawingDimensions", get_drawingDimensions, null),
		new LuaField("material", get_material, set_material),
		new LuaField("mainTexture", get_mainTexture, set_mainTexture),
		new LuaField("shader", get_shader, set_shader),
		new LuaField("hasBoxCollider", get_hasBoxCollider, null),
//		new LuaField("showHandlesWithMoveTool", get_showHandlesWithMoveTool, set_showHandlesWithMoveTool),
//		new LuaField("showHandles", get_showHandles, null),
		new LuaField("minWidth", get_minWidth, null),
		new LuaField("minHeight", get_minHeight, null),
		new LuaField("border", get_border, set_border),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateUIWidget(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			UIWidget obj = new UIWidget();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: UIWidget.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(UIWidget));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "UIWidget", typeof(UIWidget), regs, fields, "UIRect");
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

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushObject(L, obj.onChange);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_onPostFill(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name onPostFill");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index onPostFill on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushObject(L, obj.onPostFill);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mOnRender(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name mOnRender");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index mOnRender on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushObject(L, obj.mOnRender);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_autoResizeBoxCollider(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name autoResizeBoxCollider");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index autoResizeBoxCollider on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.autoResizeBoxCollider);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_hideIfOffScreen(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hideIfOffScreen");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hideIfOffScreen on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.hideIfOffScreen);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_keepAspectRatio(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name keepAspectRatio");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index keepAspectRatio on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushEnum(L, obj.keepAspectRatio);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_aspectRatio(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name aspectRatio");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index aspectRatio on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.aspectRatio);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_hitCheck(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hitCheck");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hitCheck on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushObject(L, obj.hitCheck);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_panel(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name panel");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index panel on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.panel);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_geometry(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name geometry");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index geometry on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushObject(L, obj.geometry);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_fillGeometry(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillGeometry");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillGeometry on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.fillGeometry);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_drawCall(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name drawCall");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index drawCall on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.drawCall);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_onRender(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name onRender");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index onRender on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushObject(L, obj.onRender);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_drawRegion(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name drawRegion");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index drawRegion on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushValue(L, obj.drawRegion);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pivotOffset(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pivotOffset");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pivotOffset on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushValue(L, obj.pivotOffset);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_width(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name width");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index width on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.width);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_height(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name height");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index height on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.height);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_color(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name color");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index color on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushValue(L, obj.color);
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

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.alpha);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_isVisible(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name isVisible");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index isVisible on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.isVisible);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_hasVertices(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hasVertices");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hasVertices on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.hasVertices);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_rawPivot(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name rawPivot");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index rawPivot on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushEnum(L, obj.rawPivot);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pivot(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pivot");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pivot on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushEnum(L, obj.pivot);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_depth(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name depth");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index depth on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.depth);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_raycastDepth(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name raycastDepth");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index raycastDepth on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.raycastDepth);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_localCorners(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name localCorners");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index localCorners on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushArray(L, obj.localCorners);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_localSize(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name localSize");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index localSize on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushValue(L, obj.localSize);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_localCenter(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name localCenter");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index localCenter on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushValue(L, obj.localCenter);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_worldCorners(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name worldCorners");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index worldCorners on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushArray(L, obj.worldCorners);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_worldCenter(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name worldCenter");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index worldCenter on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushValue(L, obj.worldCenter);
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

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushValue(L, obj.drawingDimensions);
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

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.material);
		return 1;
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

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.mainTexture);
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

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.shader);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_hasBoxCollider(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hasBoxCollider");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hasBoxCollider on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.hasBoxCollider);
		return 1;
	}

//	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
//	static int get_showHandlesWithMoveTool(IntPtr L)
//	{
//		LuaScriptMgr.Push(L, UIWidget.showHandlesWithMoveTool);
//		return 1;
//	}

//	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
//	static int get_showHandles(IntPtr L)
//	{
//		LuaScriptMgr.Push(L, UIWidget.showHandles);
//		return 1;
//	}

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

		UIWidget obj = (UIWidget)o;
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

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.Push(L, obj.minHeight);
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

		UIWidget obj = (UIWidget)o;
		LuaScriptMgr.PushValue(L, obj.border);
		return 1;
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

		UIWidget obj = (UIWidget)o;
		obj.onChange = LuaScriptMgr.GetNetObject<UIWidget.OnDimensionsChanged>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_onPostFill(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name onPostFill");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index onPostFill on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.onPostFill = LuaScriptMgr.GetNetObject<UIWidget.OnPostFillCallback>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mOnRender(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name mOnRender");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index mOnRender on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.mOnRender = LuaScriptMgr.GetNetObject<UIDrawCall.OnRenderCallback>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_autoResizeBoxCollider(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name autoResizeBoxCollider");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index autoResizeBoxCollider on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.autoResizeBoxCollider = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_hideIfOffScreen(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hideIfOffScreen");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hideIfOffScreen on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.hideIfOffScreen = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_keepAspectRatio(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name keepAspectRatio");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index keepAspectRatio on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.keepAspectRatio = LuaScriptMgr.GetNetObject<UIWidget.AspectRatioSource>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_aspectRatio(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name aspectRatio");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index aspectRatio on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.aspectRatio = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_hitCheck(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hitCheck");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hitCheck on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.hitCheck = LuaScriptMgr.GetNetObject<UIWidget.HitCheck>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_panel(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name panel");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index panel on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.panel = LuaScriptMgr.GetNetObject<UIPanel>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_geometry(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name geometry");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index geometry on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.geometry = LuaScriptMgr.GetNetObject<UIGeometry>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_fillGeometry(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillGeometry");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillGeometry on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.fillGeometry = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_drawCall(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name drawCall");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index drawCall on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.drawCall = LuaScriptMgr.GetNetObject<UIDrawCall>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_onRender(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name onRender");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index onRender on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.onRender = LuaScriptMgr.GetNetObject<UIDrawCall.OnRenderCallback>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_drawRegion(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name drawRegion");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index drawRegion on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.drawRegion = LuaScriptMgr.GetNetObject<Vector4>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_width(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name width");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index width on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.width = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_height(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name height");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index height on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.height = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_color(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name color");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index color on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.color = LuaScriptMgr.GetNetObject<Color>(L, 3);
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

		UIWidget obj = (UIWidget)o;
		obj.alpha = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_rawPivot(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name rawPivot");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index rawPivot on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.rawPivot = LuaScriptMgr.GetNetObject<UIWidget.Pivot>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_pivot(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pivot");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pivot on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.pivot = LuaScriptMgr.GetNetObject<UIWidget.Pivot>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_depth(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name depth");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index depth on a nil value");
			}
		}

		UIWidget obj = (UIWidget)o;
		obj.depth = (int)LuaScriptMgr.GetNumber(L, 3);
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

		UIWidget obj = (UIWidget)o;
		obj.material = LuaScriptMgr.GetNetObject<Material>(L, 3);
		return 0;
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

		UIWidget obj = (UIWidget)o;
		obj.mainTexture = LuaScriptMgr.GetNetObject<Texture>(L, 3);
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

		UIWidget obj = (UIWidget)o;
		obj.shader = LuaScriptMgr.GetNetObject<Shader>(L, 3);
		return 0;
	}

//	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
//	static int set_showHandlesWithMoveTool(IntPtr L)
//	{
//		UIWidget.showHandlesWithMoveTool = LuaScriptMgr.GetBoolean(L, 3);
//		return 0;
//	}

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

		UIWidget obj = (UIWidget)o;
		obj.border = LuaScriptMgr.GetNetObject<Vector4>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetDimensions(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		obj.SetDimensions(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetSides(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		Vector3[] o = obj.GetSides(arg0);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CalculateFinalAlpha(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		float o = obj.CalculateFinalAlpha(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Invalidate(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.Invalidate(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CalculateCumulativeAlpha(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		float o = obj.CalculateCumulativeAlpha(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetRect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		float arg0 = (float)LuaScriptMgr.GetNumber(L, 2);
		float arg1 = (float)LuaScriptMgr.GetNumber(L, 3);
		float arg2 = (float)LuaScriptMgr.GetNumber(L, 4);
		float arg3 = (float)LuaScriptMgr.GetNumber(L, 5);
		obj.SetRect(arg0,arg1,arg2,arg3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResizeCollider(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		obj.ResizeCollider();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int FullCompareFunc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIWidget arg0 = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		UIWidget arg1 = LuaScriptMgr.GetNetObject<UIWidget>(L, 2);
		int o = UIWidget.FullCompareFunc(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PanelCompareFunc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIWidget arg0 = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		UIWidget arg1 = LuaScriptMgr.GetNetObject<UIWidget>(L, 2);
		int o = UIWidget.PanelCompareFunc(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CalculateBounds(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
			Bounds o = obj.CalculateBounds();
			LuaScriptMgr.PushValue(L, o);
			return 1;
		}
		else if (count == 2)
		{
			UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			Bounds o = obj.CalculateBounds(arg0);
			LuaScriptMgr.PushValue(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: UIWidget.CalculateBounds");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetDirty(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		obj.SetDirty();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveFromPanel(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		obj.RemoveFromPanel();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int MarkAsChanged(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		obj.MarkAsChanged();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreatePanel(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		UIPanel o = obj.CreatePanel();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckLayer(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		obj.CheckLayer();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ParentHasChanged(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		obj.ParentHasChanged();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateVisibility(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		bool o = obj.UpdateVisibility(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateTransform(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		bool o = obj.UpdateTransform(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateGeometry(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		bool o = obj.UpdateGeometry(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WriteToBuffers(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 6);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		BetterList<Vector3> arg0 = LuaScriptMgr.GetNetObject<BetterList<Vector3>>(L, 2);
		BetterList<Vector2> arg1 = LuaScriptMgr.GetNetObject<BetterList<Vector2>>(L, 3);
		BetterList<Color32> arg2 = LuaScriptMgr.GetNetObject<BetterList<Color32>>(L, 4);
		BetterList<Vector3> arg3 = LuaScriptMgr.GetNetObject<BetterList<Vector3>>(L, 5);
		BetterList<Vector4> arg4 = LuaScriptMgr.GetNetObject<BetterList<Vector4>>(L, 6);
		obj.WriteToBuffers(arg0,arg1,arg2,arg3,arg4);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int MakePixelPerfect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		obj.MakePixelPerfect();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnFill(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		UIWidget obj = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		BetterList<Vector3> arg0 = LuaScriptMgr.GetNetObject<BetterList<Vector3>>(L, 2);
		BetterList<Vector2> arg1 = LuaScriptMgr.GetNetObject<BetterList<Vector2>>(L, 3);
		BetterList<Color32> arg2 = LuaScriptMgr.GetNetObject<BetterList<Color32>>(L, 4);
		obj.OnFill(arg0,arg1,arg2);
		return 0;
	}
}

