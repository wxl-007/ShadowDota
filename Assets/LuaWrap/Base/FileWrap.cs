using System;
using System.IO;
using System.Text;
using System.Security.AccessControl;
using LuaInterface;

public class FileWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("AppendAllText", AppendAllText),
		new LuaMethod("AppendText", AppendText),
		new LuaMethod("Copy", Copy),
		new LuaMethod("Create", Create),
		new LuaMethod("CreateText", CreateText),
		new LuaMethod("Delete", Delete),
		new LuaMethod("Exists", Exists),

		new LuaMethod("GetAttributes", GetAttributes),
		new LuaMethod("GetCreationTime", GetCreationTime),
		new LuaMethod("GetCreationTimeUtc", GetCreationTimeUtc),
		new LuaMethod("GetLastAccessTime", GetLastAccessTime),
		new LuaMethod("GetLastAccessTimeUtc", GetLastAccessTimeUtc),
		new LuaMethod("GetLastWriteTime", GetLastWriteTime),
		new LuaMethod("GetLastWriteTimeUtc", GetLastWriteTimeUtc),
		new LuaMethod("Move", Move),
		new LuaMethod("Open", Open),
		new LuaMethod("OpenRead", OpenRead),
		new LuaMethod("OpenText", OpenText),
		new LuaMethod("OpenWrite", OpenWrite),
		new LuaMethod("Replace", Replace),

		new LuaMethod("SetAttributes", SetAttributes),
		new LuaMethod("SetCreationTime", SetCreationTime),
		new LuaMethod("SetCreationTimeUtc", SetCreationTimeUtc),
		new LuaMethod("SetLastAccessTime", SetLastAccessTime),
		new LuaMethod("SetLastAccessTimeUtc", SetLastAccessTimeUtc),
		new LuaMethod("SetLastWriteTime", SetLastWriteTime),
		new LuaMethod("SetLastWriteTimeUtc", SetLastWriteTimeUtc),
		new LuaMethod("ReadAllBytes", ReadAllBytes),
		new LuaMethod("ReadAllLines", ReadAllLines),
		new LuaMethod("ReadAllText", ReadAllText),
		new LuaMethod("WriteAllBytes", WriteAllBytes),
		new LuaMethod("WriteAllLines", WriteAllLines),
		new LuaMethod("WriteAllText", WriteAllText),
		new LuaMethod("Encrypt", Encrypt),
		new LuaMethod("Decrypt", Decrypt),
		new LuaMethod("New", _CreateFile),
		new LuaMethod("GetClassType", GetClassType),
	};


	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateFile(IntPtr L)
	{
		LuaDLL.luaL_error(L, "File class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(File));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "System.IO.File", regs);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AppendAllText(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			File.AppendAllText(arg0,arg1);
			return 0;
		}
		else if (count == 3)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			Encoding arg2 = LuaScriptMgr.GetNetObject<Encoding>(L, 3);
			File.AppendAllText(arg0,arg1,arg2);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: File.AppendAllText");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AppendText(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		StreamWriter o = File.AppendText(arg0);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Copy(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			File.Copy(arg0,arg1);
			return 0;
		}
		else if (count == 3)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 3);
			File.Copy(arg0,arg1,arg2);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: File.Copy");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Create(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			FileStream o = File.Create(arg0);
			LuaScriptMgr.PushObject(L, o);
			return 1;
		}
		else if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			FileStream o = File.Create(arg0,arg1);
			LuaScriptMgr.PushObject(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: File.Create");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateText(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		StreamWriter o = File.CreateText(arg0);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Delete(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		File.Delete(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Exists(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		bool o = File.Exists(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAttributes(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		FileAttributes o = File.GetAttributes(arg0);
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCreationTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime o = File.GetCreationTime(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCreationTimeUtc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime o = File.GetCreationTimeUtc(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLastAccessTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime o = File.GetLastAccessTime(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLastAccessTimeUtc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime o = File.GetLastAccessTimeUtc(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLastWriteTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime o = File.GetLastWriteTime(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLastWriteTimeUtc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime o = File.GetLastWriteTimeUtc(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Move(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		File.Move(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Open(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			FileMode arg1 = LuaScriptMgr.GetNetObject<FileMode>(L, 2);
			FileStream o = File.Open(arg0,arg1);
			LuaScriptMgr.PushObject(L, o);
			return 1;
		}
		else if (count == 3)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			FileMode arg1 = LuaScriptMgr.GetNetObject<FileMode>(L, 2);
			FileAccess arg2 = LuaScriptMgr.GetNetObject<FileAccess>(L, 3);
			FileStream o = File.Open(arg0,arg1,arg2);
			LuaScriptMgr.PushObject(L, o);
			return 1;
		}
		else if (count == 4)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			FileMode arg1 = LuaScriptMgr.GetNetObject<FileMode>(L, 2);
			FileAccess arg2 = LuaScriptMgr.GetNetObject<FileAccess>(L, 3);
			FileShare arg3 = LuaScriptMgr.GetNetObject<FileShare>(L, 4);
			FileStream o = File.Open(arg0,arg1,arg2,arg3);
			LuaScriptMgr.PushObject(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: File.Open");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OpenRead(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		FileStream o = File.OpenRead(arg0);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OpenText(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		StreamReader o = File.OpenText(arg0);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OpenWrite(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		FileStream o = File.OpenWrite(arg0);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Replace(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 3)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			string arg2 = LuaScriptMgr.GetLuaString(L, 3);
			File.Replace(arg0,arg1,arg2);
			return 0;
		}
		else if (count == 4)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			string arg2 = LuaScriptMgr.GetLuaString(L, 3);
			bool arg3 = LuaScriptMgr.GetBoolean(L, 4);
			File.Replace(arg0,arg1,arg2,arg3);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: File.Replace");
		}

		return 0;
	}

	
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetAttributes(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		FileAttributes arg1 = LuaScriptMgr.GetNetObject<FileAttributes>(L, 2);
		File.SetAttributes(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCreationTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime arg1 = LuaScriptMgr.GetNetObject<DateTime>(L, 2);
		File.SetCreationTime(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCreationTimeUtc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime arg1 = LuaScriptMgr.GetNetObject<DateTime>(L, 2);
		File.SetCreationTimeUtc(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLastAccessTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime arg1 = LuaScriptMgr.GetNetObject<DateTime>(L, 2);
		File.SetLastAccessTime(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLastAccessTimeUtc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime arg1 = LuaScriptMgr.GetNetObject<DateTime>(L, 2);
		File.SetLastAccessTimeUtc(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLastWriteTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime arg1 = LuaScriptMgr.GetNetObject<DateTime>(L, 2);
		File.SetLastWriteTime(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLastWriteTimeUtc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		DateTime arg1 = LuaScriptMgr.GetNetObject<DateTime>(L, 2);
		File.SetLastWriteTimeUtc(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReadAllBytes(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		byte[] o = File.ReadAllBytes(arg0);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReadAllLines(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string[] o = File.ReadAllLines(arg0);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			Encoding arg1 = LuaScriptMgr.GetNetObject<Encoding>(L, 2);
			string[] o = File.ReadAllLines(arg0,arg1);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: File.ReadAllLines");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReadAllText(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string o = File.ReadAllText(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			Encoding arg1 = LuaScriptMgr.GetNetObject<Encoding>(L, 2);
			string o = File.ReadAllText(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: File.ReadAllText");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WriteAllBytes(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		byte[] objs1 = LuaScriptMgr.GetArrayNumber<byte>(L, 2);
		File.WriteAllBytes(arg0,objs1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WriteAllLines(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string[] objs1 = LuaScriptMgr.GetArrayString(L, 2);
			File.WriteAllLines(arg0,objs1);
			return 0;
		}
		else if (count == 3)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string[] objs1 = LuaScriptMgr.GetArrayString(L, 2);
			Encoding arg2 = LuaScriptMgr.GetNetObject<Encoding>(L, 3);
			File.WriteAllLines(arg0,objs1,arg2);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: File.WriteAllLines");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WriteAllText(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			File.WriteAllText(arg0,arg1);
			return 0;
		}
		else if (count == 3)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetLuaString(L, 2);
			Encoding arg2 = LuaScriptMgr.GetNetObject<Encoding>(L, 3);
			File.WriteAllText(arg0,arg1,arg2);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: File.WriteAllText");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Encrypt(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		File.Encrypt(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Decrypt(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		File.Decrypt(arg0);
		return 0;
	}
}

