using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class LuaLinkerWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("SendOkMsg", SendOkMsg),
		new LuaMethod("loginServerFinished", loginServerFinished),
		new LuaMethod("New", _CreateLuaLinker),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("LuaLink", get_LuaLink, set_LuaLink),
		new LuaField("Node", get_Node, set_Node),
		new LuaField("AddListener", get_AddListener, set_AddListener),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateLuaLinker(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			LuaLinker obj = new LuaLinker();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: LuaLinker.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(LuaLinker));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "LuaLinker", typeof(LuaLinker), regs, fields, "BaseLua");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LuaLink(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name LuaLink");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index LuaLink on a nil value");
			}
		}

		LuaLinker obj = (LuaLinker)o;
		LuaScriptMgr.Push(L, obj.LuaLink);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Node(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name Node");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index Node on a nil value");
			}
		}

		LuaLinker obj = (LuaLinker)o;
		LuaScriptMgr.PushArray(L, obj.Node);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_AddListener(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name AddListener");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index AddListener on a nil value");
			}
		}

		LuaLinker obj = (LuaLinker)o;
		LuaScriptMgr.Push(L, obj.AddListener);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_LuaLink(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name LuaLink");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index LuaLink on a nil value");
			}
		}

		LuaLinker obj = (LuaLinker)o;
		obj.LuaLink = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Node(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name Node");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index Node on a nil value");
			}
		}

		LuaLinker obj = (LuaLinker)o;
		obj.Node = LuaScriptMgr.GetNetObject<GameObject[]>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_AddListener(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name AddListener");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index AddListener on a nil value");
			}
		}

		LuaLinker obj = (LuaLinker)o;
		obj.AddListener = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SendOkMsg(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaLinker obj = LuaScriptMgr.GetNetObject<LuaLinker>(L, 1);
		obj.SendOkMsg();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int loginServerFinished(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaLinker obj = LuaScriptMgr.GetNetObject<LuaLinker>(L, 1);
		obj.loginServerFinished();
		return 0;
	}
}

