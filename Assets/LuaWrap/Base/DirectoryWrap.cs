using System;
using System.IO;
using System.Security.AccessControl;
using LuaInterface;

public class DirectoryWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("CreateDirectory", CreateDirectory),
		new LuaMethod("Delete", Delete),
		new LuaMethod("Exists", Exists),
		new LuaMethod("GetLastAccessTime", GetLastAccessTime),
		new LuaMethod("GetLastAccessTimeUtc", GetLastAccessTimeUtc),
		new LuaMethod("GetLastWriteTime", GetLastWriteTime),
		new LuaMethod("GetLastWriteTimeUtc", GetLastWriteTimeUtc),
		new LuaMethod("GetCreationTime", GetCreationTime),
		new LuaMethod("GetCreationTimeUtc", GetCreationTimeUtc),
		new LuaMethod("GetCurrentDirectory", GetCurrentDirectory),
		new LuaMethod("GetDirectories", GetDirectories),
		new LuaMethod("GetDirectoryRoot", GetDirectoryRoot),
		new LuaMethod("GetFiles", GetFiles),
		new LuaMethod("GetFileSystemEntries", GetFileSystemEntries),
		new LuaMethod("GetLogicalDrives", GetLogicalDrives),
		new LuaMethod("GetParent", GetParent),
		new LuaMethod("Move", Move),
		new LuaMethod("SetCreationTime", SetCreationTime),
		new LuaMethod("SetCreationTimeUtc", SetCreationTimeUtc),
		new LuaMethod("SetCurrentDirectory", SetCurrentDirectory),
		new LuaMethod("SetLastAccessTime", SetLastAccessTime),
		new LuaMethod("SetLastAccessTimeUtc", SetLastAccessTimeUtc),
		new LuaMethod("SetLastWriteTime", SetLastWriteTime),
		new LuaMethod("SetLastWriteTimeUtc", SetLastWriteTimeUtc),
		new LuaMethod("New", _CreateDirectory),
		new LuaMethod("GetClassType", GetClassType),
	};


	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateDirectory(IntPtr L)
	{
		LuaDLL.luaL_error(L, "Directory class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(Directory));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "System.IO.Directory", regs);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateDirectory(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			DirectoryInfo o = Directory.CreateDirectory(arg0);
			LuaScriptMgr.PushObject(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Directory.CreateDirectory");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Delete(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			Directory.Delete(arg0);
			return 0;
		}
		else if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
			Directory.Delete(arg0,arg1);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Directory.Delete");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Exists(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		bool o = Directory.Exists(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLastAccessTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime o = Directory.GetLastAccessTime(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLastAccessTimeUtc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime o = Directory.GetLastAccessTimeUtc(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLastWriteTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime o = Directory.GetLastWriteTime(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLastWriteTimeUtc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime o = Directory.GetLastWriteTimeUtc(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCreationTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime o = Directory.GetCreationTime(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCreationTimeUtc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime o = Directory.GetCreationTimeUtc(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentDirectory(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = Directory.GetCurrentDirectory();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetDirectories(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string[] o = Directory.GetDirectories(arg0);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			string[] o = Directory.GetDirectories(arg0,arg1);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else if (count == 3)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			SearchOption arg2 = LuaScriptMgr.GetNetObject<SearchOption>(L, 3);
			string[] o = Directory.GetDirectories(arg0,arg1,arg2);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Directory.GetDirectories");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetDirectoryRoot(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = Directory.GetDirectoryRoot(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetFiles(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string[] o = Directory.GetFiles(arg0);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			string[] o = Directory.GetFiles(arg0,arg1);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else if (count == 3)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			SearchOption arg2 = LuaScriptMgr.GetNetObject<SearchOption>(L, 3);
			string[] o = Directory.GetFiles(arg0,arg1,arg2);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Directory.GetFiles");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetFileSystemEntries(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string[] o = Directory.GetFileSystemEntries(arg0);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			string[] o = Directory.GetFileSystemEntries(arg0,arg1);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Directory.GetFileSystemEntries");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLogicalDrives(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string[] o = Directory.GetLogicalDrives();
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetParent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DirectoryInfo o = Directory.GetParent(arg0);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Move(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		Directory.Move(arg0,arg1);
		return 0;
	}


	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCreationTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime arg1 = LuaScriptMgr.GetNetObject<DateTime>(L, 2);
		Directory.SetCreationTime(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCreationTimeUtc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime arg1 = LuaScriptMgr.GetNetObject<DateTime>(L, 2);
		Directory.SetCreationTimeUtc(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCurrentDirectory(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		Directory.SetCurrentDirectory(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLastAccessTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime arg1 = LuaScriptMgr.GetNetObject<DateTime>(L, 2);
		Directory.SetLastAccessTime(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLastAccessTimeUtc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime arg1 = LuaScriptMgr.GetNetObject<DateTime>(L, 2);
		Directory.SetLastAccessTimeUtc(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLastWriteTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime arg1 = LuaScriptMgr.GetNetObject<DateTime>(L, 2);
		Directory.SetLastWriteTime(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLastWriteTimeUtc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime arg1 = LuaScriptMgr.GetNetObject<DateTime>(L, 2);
		Directory.SetLastWriteTimeUtc(arg0,arg1);
		return 0;
	}

}

