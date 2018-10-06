using System;
using LuaInterface;

public class JumperWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("LoadSceneAsyncWithLoading", LoadSceneAsyncWithLoading),
		new LuaMethod("SendEnterWarMsg", SendEnterWarMsg),
		new LuaMethod("EnterWarDataInitFinished", EnterWarDataInitFinished),
		new LuaMethod("New", _CreateJumper),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("_this", get__this, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateJumper(IntPtr L)
	{
		LuaDLL.luaL_error(L, "Jumper class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(Jumper));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "Jumper", typeof(Jumper), regs, fields, "UnityEngine.MonoBehaviour");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get__this(IntPtr L)
	{
		LuaScriptMgr.Push(L, Jumper._this);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadSceneAsyncWithLoading(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		LuaFunction arg1 = LuaScriptMgr.GetLuaFunction(L, 2);
		LuaFunction arg2 = LuaScriptMgr.GetLuaFunction(L, 3);
		Jumper.LoadSceneAsyncWithLoading(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SendEnterWarMsg(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		Jumper.SendEnterWarMsg();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EnterWarDataInitFinished(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		Jumper.EnterWarDataInitFinished();
		return 0;
	}
}

