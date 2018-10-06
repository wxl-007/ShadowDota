using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class LuaScriptMgrWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("ReloadAll", ReloadAll),
		new LuaMethod("Update", Update),
		new LuaMethod("Start", Start),
		new LuaMethod("Destroy", Destroy),
		new LuaMethod("DoString", DoString),
		new LuaMethod("DoFile", DoFile),
		new LuaMethod("CallLuaFunction", CallLuaFunction),
		new LuaMethod("GetLuaFunction", GetLuaFunction),
		new LuaMethod("IsFuncExists", IsFuncExists),
		new LuaMethod("Loader", Loader),
		new LuaMethod("GetLuaTable", GetLuaTable),
		new LuaMethod("RemoveLuaRes", RemoveLuaRes),
		new LuaMethod("RegisterLib", RegisterLib),
		new LuaMethod("RegisterFunc", RegisterFunc),
		new LuaMethod("CreateMetaTable", CreateMetaTable),
		new LuaMethod("__gc", __gc),
		new LuaMethod("GetNumber", GetNumber),
		new LuaMethod("GetBoolean", GetBoolean),
		new LuaMethod("GetString", GetString),
		new LuaMethod("GetFunction", GetFunction),
		new LuaMethod("GetTable", GetTable),
		new LuaMethod("GetLuaObject", GetLuaObject),
		new LuaMethod("GetTypeObject", GetTypeObject),
		new LuaMethod("PushVarObject", PushVarObject),
		new LuaMethod("Push", Push),
		new LuaMethod("PushObject", PushObject),
		new LuaMethod("PushValue", PushValue),
		new LuaMethod("CheckTypes", CheckTypes),
		new LuaMethod("CheckParamsType", CheckParamsType),
		new LuaMethod("GetParamsObject", GetParamsObject),
		new LuaMethod("GetLuaString", GetLuaString),
		new LuaMethod("GetParamsString", GetParamsString),
		new LuaMethod("GetArrayString", GetArrayString),
		new LuaMethod("GetArrayBool", GetArrayBool),
		new LuaMethod("GetStringBuffer", GetStringBuffer),
		new LuaMethod("SetValueObject", SetValueObject),
		new LuaMethod("CheckArgsCount", CheckArgsCount),
		new LuaMethod("GetVarObject", GetVarObject),
		new LuaMethod("Xml_read", Xml_read),
		new LuaMethod("IndexArray", IndexArray),
		new LuaMethod("NewIndexArray", NewIndexArray),
		new LuaMethod("PushArray", PushArray),
		new LuaMethod("DumpStack", DumpStack),
		new LuaMethod("IsEnumEquals", IsEnumEquals),
		new LuaMethod("PushEnum", PushEnum),
		new LuaMethod("GetMgrFromLuaState", GetMgrFromLuaState),
		new LuaMethod("New", _CreateLuaScriptMgr),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("lua", get_lua, set_lua),
		new LuaField("refGCList", get_refGCList, set_refGCList),
		new LuaField("Instance", get_Instance, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateLuaScriptMgr(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			LuaScriptMgr obj = new LuaScriptMgr();
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: LuaScriptMgr.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(LuaScriptMgr));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "LuaScriptMgr", typeof(LuaScriptMgr), regs, fields, "System.Object");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_lua(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name lua");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index lua on a nil value");
			}
		}

		LuaScriptMgr obj = (LuaScriptMgr)o;
		LuaScriptMgr.PushObject(L, obj.lua);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_refGCList(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name refGCList");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index refGCList on a nil value");
			}
		}

		LuaScriptMgr obj = (LuaScriptMgr)o;
		LuaScriptMgr.PushObject(L, obj.refGCList);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, LuaScriptMgr.Instance);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_lua(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name lua");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index lua on a nil value");
			}
		}

		LuaScriptMgr obj = (LuaScriptMgr)o;
		obj.lua = LuaScriptMgr.GetNetObject<LuaInterface.LuaState>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_refGCList(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name refGCList");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index refGCList on a nil value");
			}
		}

		LuaScriptMgr obj = (LuaScriptMgr)o;
		obj.refGCList = LuaScriptMgr.GetNetObject<LockFreeQueue<int>>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReloadAll(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaScriptMgr obj = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 1);
		obj.ReloadAll();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Update(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaScriptMgr obj = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 1);
		obj.Update();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Start(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaScriptMgr obj = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 1);
		obj.Start();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Destroy(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaScriptMgr obj = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 1);
		obj.Destroy();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DoString(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		LuaScriptMgr obj = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		object[] o = obj.DoString(arg0);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DoFile(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		LuaScriptMgr obj = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		object[] o = obj.DoFile(arg0);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CallLuaFunction(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);
		LuaScriptMgr obj = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		object[] objs1 = LuaScriptMgr.GetParamsObject(L, 3, count - 2);
		object[] o = obj.CallLuaFunction(arg0,objs1);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLuaFunction(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(IntPtr), typeof(int)};
		Type[] types1 = {typeof(LuaScriptMgr), typeof(string)};

		if (count == 2 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			LuaInterface.LuaFunction o = LuaScriptMgr.GetLuaFunction(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			LuaScriptMgr obj = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 1);
			string arg0 = LuaScriptMgr.GetString(L, 2);
			LuaInterface.LuaFunction o = obj.GetLuaFunction(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: LuaScriptMgr.GetLuaFunction");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsFuncExists(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		LuaScriptMgr obj = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		bool o = obj.IsFuncExists(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Loader(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		LuaScriptMgr obj = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		byte[] o = obj.Loader(arg0);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLuaTable(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(IntPtr), typeof(int)};
		Type[] types1 = {typeof(LuaScriptMgr), typeof(string)};

		if (count == 2 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			LuaInterface.LuaTable o = LuaScriptMgr.GetLuaTable(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			LuaScriptMgr obj = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 1);
			string arg0 = LuaScriptMgr.GetString(L, 2);
			LuaInterface.LuaTable o = obj.GetLuaTable(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: LuaScriptMgr.GetLuaTable");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveLuaRes(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		LuaScriptMgr obj = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		obj.RemoveLuaRes(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RegisterLib(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(IntPtr), typeof(string), typeof(LuaMethod[])};
		Type[] types1 = {typeof(IntPtr), typeof(string), typeof(LuaEnum[])};

		if (count == 3 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			string arg1 = LuaScriptMgr.GetString(L, 2);
			LuaMethod[] objs2 = LuaScriptMgr.GetArrayObject<LuaMethod>(L, 3);
			LuaScriptMgr.RegisterLib(arg0,arg1,objs2);
			return 0;
		}
		else if (count == 3 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			string arg1 = LuaScriptMgr.GetString(L, 2);
			LuaEnum[] objs2 = LuaScriptMgr.GetArrayObject<LuaEnum>(L, 3);
			LuaScriptMgr.RegisterLib(arg0,arg1,objs2);
			return 0;
		}
		else if (count == 6)
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			Type arg2 = LuaScriptMgr.GetTypeObject(L, 3);
			LuaMethod[] objs3 = LuaScriptMgr.GetArrayObject<LuaMethod>(L, 4);
			LuaField[] objs4 = LuaScriptMgr.GetArrayObject<LuaField>(L, 5);
			string arg5 = LuaScriptMgr.GetLuaString(L, 6);
			LuaScriptMgr.RegisterLib(arg0,arg1,arg2,objs3,objs4,arg5);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: LuaScriptMgr.RegisterLib");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RegisterFunc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		LuaInterface.LuaCSFunction arg2 = LuaScriptMgr.GetNetObject<LuaInterface.LuaCSFunction>(L, 3);
		string arg3 = LuaScriptMgr.GetLuaString(L, 4);
		LuaScriptMgr.RegisterFunc(arg0,arg1,arg2,arg3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateMetaTable(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		LuaMethod[] objs2 = LuaScriptMgr.GetArrayObject<LuaMethod>(L, 3);
		Type arg3 = LuaScriptMgr.GetTypeObject(L, 4);
		int o = LuaScriptMgr.CreateMetaTable(arg0,arg1,objs2,arg3);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int __gc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int o = LuaScriptMgr.__gc(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetNumber(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		double o = LuaScriptMgr.GetNumber(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetBoolean(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		bool o = LuaScriptMgr.GetBoolean(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetString(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		string o = LuaScriptMgr.GetString(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetFunction(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		LuaInterface.LuaFunction o = LuaScriptMgr.GetFunction(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTable(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		LuaInterface.LuaTable o = LuaScriptMgr.GetTable(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLuaObject(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		object o = LuaScriptMgr.GetLuaObject(arg0,arg1);
		LuaScriptMgr.PushVarObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTypeObject(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		Type o = LuaScriptMgr.GetTypeObject(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PushVarObject(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		object arg1 = LuaScriptMgr.GetVarObject(L, 2);
		LuaScriptMgr.PushVarObject(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Push(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(IntPtr), typeof(LuaInterface.LuaTable)};
		Type[] types1 = {typeof(IntPtr), typeof(LuaInterface.ILuaGeneratedType)};
		Type[] types2 = {typeof(IntPtr), typeof(LuaInterface.LuaCSFunction)};
		Type[] types3 = {typeof(IntPtr), typeof(LuaInterface.LuaFunction)};
		Type[] types4 = {typeof(IntPtr), typeof(IntPtr)};
		Type[] types5 = {typeof(IntPtr), typeof(Object)};
		Type[] types6 = {typeof(IntPtr), typeof(Type)};
		Type[] types7 = {typeof(IntPtr), typeof(double)};
		Type[] types8 = {typeof(IntPtr), typeof(string)};

		if (count == 2 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			LuaTable arg1 = LuaScriptMgr.GetLuaTable(L, 2);
			LuaScriptMgr.Push(arg0,arg1);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			LuaInterface.ILuaGeneratedType arg1 = LuaScriptMgr.GetNetObject<LuaInterface.ILuaGeneratedType>(L, 2);
			LuaScriptMgr.Push(arg0,arg1);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types2, 1))
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			LuaInterface.LuaCSFunction arg1 = LuaScriptMgr.GetNetObject<LuaInterface.LuaCSFunction>(L, 2);
			LuaScriptMgr.Push(arg0,arg1);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types3, 1))
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			LuaFunction arg1 = LuaScriptMgr.GetLuaFunction(L, 2);
			LuaScriptMgr.Push(arg0,arg1);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types4, 1))
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			IntPtr arg1 = (IntPtr)LuaScriptMgr.GetNumber(L, 2);
			LuaScriptMgr.Push(arg0,arg1);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types5, 1))
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			Object arg1 = LuaScriptMgr.GetNetObject<Object>(L, 2);
			LuaScriptMgr.Push(arg0,arg1);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types6, 1))
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			Type arg1 = LuaScriptMgr.GetTypeObject(L, 2);
			LuaScriptMgr.Push(arg0,arg1);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types7, 1))
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			double arg1 = (double)LuaScriptMgr.GetNumber(L, 2);
			LuaScriptMgr.Push(arg0,arg1);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types8, 1))
		{
			IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
			string arg1 = LuaScriptMgr.GetString(L, 2);
			LuaScriptMgr.Push(arg0,arg1);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: LuaScriptMgr.Push");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PushObject(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		object arg1 = LuaScriptMgr.GetVarObject(L, 2);
		LuaScriptMgr.PushObject(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PushValue(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		object arg1 = LuaScriptMgr.GetVarObject(L, 2);
		LuaScriptMgr.PushValue(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckTypes(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		Type[] objs1 = LuaScriptMgr.GetArrayObject<Type>(L, 2);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
		bool o = LuaScriptMgr.CheckTypes(arg0,objs1,arg2);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckParamsType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		Type arg1 = LuaScriptMgr.GetTypeObject(L, 2);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
		int arg3 = (int)LuaScriptMgr.GetNumber(L, 4);
		bool o = LuaScriptMgr.CheckParamsType(arg0,arg1,arg2,arg3);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetParamsObject(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
		object[] o = LuaScriptMgr.GetParamsObject(arg0,arg1,arg2);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLuaString(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		string o = LuaScriptMgr.GetLuaString(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetParamsString(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
		string[] o = LuaScriptMgr.GetParamsString(arg0,arg1,arg2);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetArrayString(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		string[] o = LuaScriptMgr.GetArrayString(arg0,arg1);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetArrayBool(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		bool[] o = LuaScriptMgr.GetArrayBool(arg0,arg1);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetStringBuffer(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		LuaStringBuffer o = LuaScriptMgr.GetStringBuffer(arg0,arg1);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetValueObject(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		object arg2 = LuaScriptMgr.GetVarObject(L, 3);
		LuaScriptMgr.SetValueObject(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckArgsCount(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		LuaScriptMgr.CheckArgsCount(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetVarObject(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		object o = LuaScriptMgr.GetVarObject(arg0,arg1);
		LuaScriptMgr.PushVarObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Xml_read(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int o = LuaScriptMgr.Xml_read(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IndexArray(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int o = LuaScriptMgr.IndexArray(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int NewIndexArray(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int o = LuaScriptMgr.NewIndexArray(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PushArray(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		object arg1 = LuaScriptMgr.GetVarObject(L, 2);
		LuaScriptMgr.PushArray(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DumpStack(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		LuaScriptMgr.DumpStack(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsEnumEquals(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		int o = LuaScriptMgr.IsEnumEquals(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PushEnum(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		object arg1 = LuaScriptMgr.GetVarObject(L, 2);
		LuaScriptMgr.PushEnum(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetMgrFromLuaState(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		IntPtr arg0 = (IntPtr)LuaScriptMgr.GetNumber(L, 1);
		LuaScriptMgr o = LuaScriptMgr.GetMgrFromLuaState(arg0);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}
}

