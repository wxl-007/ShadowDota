using System;
using LuaInterface;

public class UIWidgetContainerWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("New", _CreateUIWidgetContainer),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateUIWidgetContainer(IntPtr L)
	{
		LuaDLL.luaL_error(L, "UIWidgetContainer class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(UIWidgetContainer));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "UIWidgetContainer", typeof(UIWidgetContainer), regs, fields, "UnityEngine.MonoBehaviour");
	}
}

