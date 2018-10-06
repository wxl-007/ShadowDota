using System;
using UIEventCenter;
using System.Collections.Generic;
using LuaInterface;

public class EventSenderWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("SendEvent", SendEvent),
		new LuaMethod("Registere", Registere),
		new LuaMethod("Remove", Remove),
		new LuaMethod("New", _CreateEventSender),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("listeners", get_listeners, set_listeners),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateEventSender(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			EventSender obj = new EventSender();
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: EventSender.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(EventSender));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "UIEventCenter.EventSender", typeof(EventSender), regs, fields, "System.Object");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_listeners(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, EventSender.listeners);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_listeners(IntPtr L)
	{
		EventSender.listeners = LuaScriptMgr.GetNetObject<List<BaseLua>>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SendEvent(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types1 = {typeof(string)};

		if (count == 1)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			EventSender.SendEvent(arg0);
			return 0;
		}
		else if (LuaScriptMgr.CheckTypes(L, types1, 1) && LuaScriptMgr.CheckParamsType(L, typeof(object), 2, count - 1))
		{
			string arg0 = LuaScriptMgr.GetString(L, 1);
			object[] objs1 = LuaScriptMgr.GetParamsObject(L, 2, count - 1);
			EventSender.SendEvent(arg0,objs1);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: EventSender.SendEvent");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Registere(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		BaseLua arg0 = LuaScriptMgr.GetNetObject<BaseLua>(L, 1);
		bool o = EventSender.Registere(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Remove(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		BaseLua arg0 = LuaScriptMgr.GetNetObject<BaseLua>(L, 1);
		bool o = EventSender.Remove(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}

