using System;
using LuaInterface;

public class DeviceInfoWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("GetDeviceUniqueID", GetDeviceUniqueID),
		new LuaMethod("PersisitFullPath", PersisitFullPath),
		new LuaMethod("New", _CreateDeviceInfo),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("GUID", get_GUID, null),
		new LuaField("StreamingPath", get_StreamingPath, null),
		new LuaField("ArtPath", get_ArtPath, null),
		new LuaField("ConfigDownload", get_ConfigDownload, null),
		new LuaField("PersistRootPath", get_PersistRootPath, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateDeviceInfo(IntPtr L)
	{
		LuaDLL.luaL_error(L, "DeviceInfo class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(DeviceInfo));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "DeviceInfo", typeof(DeviceInfo), regs, fields, null);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_GUID(IntPtr L)
	{
		LuaScriptMgr.Push(L, DeviceInfo.GUID);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_StreamingPath(IntPtr L)
	{
		LuaScriptMgr.Push(L, DeviceInfo.StreamingPath);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ArtPath(IntPtr L)
	{
		LuaScriptMgr.Push(L, DeviceInfo.ArtPath);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ConfigDownload(IntPtr L)
	{
		LuaScriptMgr.Push(L, DeviceInfo.ConfigDownload);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_PersistRootPath(IntPtr L)
	{
		LuaScriptMgr.Push(L, DeviceInfo.PersistRootPath);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetDeviceUniqueID(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = DeviceInfo.GetDeviceUniqueID();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PersisitFullPath(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = DeviceInfo.PersisitFullPath(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}

