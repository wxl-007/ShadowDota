using System;
using LuaInterface;

public class CoreWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("Initialize", Initialize),
		new LuaMethod("New", _CreateCore),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("DevFSM", get_DevFSM, set_DevFSM),
		new LuaField("GameFSM", get_GameFSM, set_GameFSM),
		new LuaField("EVC", get_EVC, set_EVC),
		new LuaField("DPM", get_DPM, set_DPM),
		new LuaField("EntityMgr", get_EntityMgr, set_EntityMgr),
		new LuaField("NetEng", get_NetEng, set_NetEng),
		new LuaField("TimerEng", get_TimerEng, set_TimerEng),
		new LuaField("AsyncEng", get_AsyncEng, set_AsyncEng),
		new LuaField("SoundEng", get_SoundEng, set_SoundEng),
		new LuaField("ResEng", get_ResEng, set_ResEng),
		new LuaField("Data", get_Data, set_Data),
		new LuaField("EngCfg", get_EngCfg, set_EngCfg),
		new LuaField("Coroutine", get_Coroutine, set_Coroutine),
		new LuaField("ZeroMQ", get_ZeroMQ, set_ZeroMQ),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateCore(IntPtr L)
	{
		LuaDLL.luaL_error(L, "Core class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(Core));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "Core", typeof(Core), regs, fields, null);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_DevFSM(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Core.DevFSM);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_GameFSM(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Core.GameFSM);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_EVC(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Core.EVC);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_DPM(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Core.DPM);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_EntityMgr(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Core.EntityMgr);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_NetEng(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Core.NetEng);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_TimerEng(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Core.TimerEng);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_AsyncEng(IntPtr L)
	{
		LuaScriptMgr.Push(L, Core.AsyncEng);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_SoundEng(IntPtr L)
	{
		LuaScriptMgr.Push(L, Core.SoundEng);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ResEng(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Core.ResEng);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Data(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Core.Data);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_EngCfg(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Core.EngCfg);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Coroutine(IntPtr L)
	{
		LuaScriptMgr.Push(L, Core.Coroutine);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ZeroMQ(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Core.ZeroMQ);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_DevFSM(IntPtr L)
	{
		Core.DevFSM = LuaScriptMgr.GetNetObject<AW.FSM.DeviceFSM>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_GameFSM(IntPtr L)
	{
		Core.GameFSM = LuaScriptMgr.GetNetObject<AW.FSM.GamePlayFSM>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_EVC(IntPtr L)
	{
		Core.EVC = LuaScriptMgr.GetNetObject<AW.Event.EventCenter>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_DPM(IntPtr L)
	{
		Core.DPM = LuaScriptMgr.GetNetObject<AW.IO.LocalIOManager>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_EntityMgr(IntPtr L)
	{
		Core.EntityMgr = LuaScriptMgr.GetNetObject<AW.Entity.EntityManager>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_NetEng(IntPtr L)
	{
		Core.NetEng = LuaScriptMgr.GetNetObject<NetworkEngine>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_TimerEng(IntPtr L)
	{
		Core.TimerEng = LuaScriptMgr.GetNetObject<AW.Timer.TimerMaster>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_AsyncEng(IntPtr L)
	{
		Core.AsyncEng = LuaScriptMgr.GetNetObject<AsyncTask>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_SoundEng(IntPtr L)
	{
		Core.SoundEng = LuaScriptMgr.GetNetObject<SoundEngine>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ResEng(IntPtr L)
	{
		Core.ResEng = LuaScriptMgr.GetNetObject<AW.Resources.Loader>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Data(IntPtr L)
	{
		Core.Data = LuaScriptMgr.GetNetObject<AW.Data.DataCore>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_EngCfg(IntPtr L)
	{
		Core.EngCfg = LuaScriptMgr.GetNetObject<EngineCfg>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Coroutine(IntPtr L)
	{
		Core.Coroutine = LuaScriptMgr.GetNetObject<AW.Resources.CoroutineProvider>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ZeroMQ(IntPtr L)
	{
		Core.ZeroMQ = LuaScriptMgr.GetNetObject<NetMQ.NetMQContext>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Initialize(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EngineCfg arg0 = LuaScriptMgr.GetNetObject<EngineCfg>(L, 1);
		Core.Initialize(arg0);
		return 0;
	}
}

