using System;
using LuaInterface;

public class LuaToolsWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("Invoke", Invoke),
		new LuaMethod("LoadResource", LoadResource),
		new LuaMethod("GetUIModule", GetUIModule),
		new LuaMethod("UnZip", UnZip),
		new LuaMethod("GetMD5", GetMD5),
		new LuaMethod("GetType", GetType),
		new LuaMethod("CallLuaFunction", CallLuaFunction),
		new LuaMethod("New", _CreateLuaTools),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("localPath", get_localPath, null),
		new LuaField("_this", get__this, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateLuaTools(IntPtr L)
	{
		LuaDLL.luaL_error(L, "LuaTools class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(LuaTools));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "LuaTools", typeof(LuaTools), regs, fields, "UnityEngine.MonoBehaviour");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_localPath(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name localPath");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index localPath on a nil value");
			}
		}

		LuaTools obj = (LuaTools)o;
		LuaScriptMgr.Push(L, obj.localPath);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get__this(IntPtr L)
	{
		LuaScriptMgr.Push(L, LuaTools._this);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Invoke(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(LuaInterface.LuaFunction), typeof(float)};
		Type[] types1 = {typeof(string), typeof(string), typeof(float)};

		if (LuaScriptMgr.CheckTypes(L, types0, 1) && LuaScriptMgr.CheckParamsType(L, typeof(object), 3, count - 2))
		{
			LuaFunction arg0 = LuaScriptMgr.GetLuaFunction(L, 1);
			float arg1 = (float)LuaScriptMgr.GetNumber(L, 2);
			object[] objs2 = LuaScriptMgr.GetParamsObject(L, 3, count - 2);
			LuaTools.Invoke(arg0,arg1,objs2);
			return 0;
		}
		else if (LuaScriptMgr.CheckTypes(L, types1, 1) && LuaScriptMgr.CheckParamsType(L, typeof(object), 4, count - 3))
		{
			string arg0 = LuaScriptMgr.GetString(L, 1);
			string arg1 = LuaScriptMgr.GetString(L, 2);
			float arg2 = (float)LuaScriptMgr.GetNumber(L, 3);
			object[] objs3 = LuaScriptMgr.GetParamsObject(L, 4, count - 3);
			LuaTools.Invoke(arg0,arg1,arg2,objs3);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: LuaTools.Invoke");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadResource(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		string arg2 = LuaScriptMgr.GetLuaString(L, 3);
		LuaTools.LoadResource(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetUIModule(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		Type o = LuaTools.GetUIModule();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UnZip(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		bool o = LuaTools.UnZip(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetMD5(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = LuaTools.GetMD5(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		object arg0 = LuaScriptMgr.GetVarObject(L, 1);
		Type o = LuaTools.GetType(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CallLuaFunction(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaFunction arg0 = LuaScriptMgr.GetLuaFunction(L, 1);
		LuaTools.CallLuaFunction(arg0);
		return 0;
	}
}

