using System;
using LuaInterface;

public class MathWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("Abs", Abs),
		new LuaMethod("Ceiling", Ceiling),
		new LuaMethod("BigMul", BigMul),
		new LuaMethod("DivRem", DivRem),
		new LuaMethod("Floor", Floor),
		new LuaMethod("IEEERemainder", IEEERemainder),
		new LuaMethod("Log", Log),
		new LuaMethod("Max", Max),
		new LuaMethod("Min", Min),
		new LuaMethod("Round", Round),
		new LuaMethod("Truncate", Truncate),
		new LuaMethod("Sign", Sign),
		new LuaMethod("Sin", Sin),
		new LuaMethod("Cos", Cos),
		new LuaMethod("Tan", Tan),
		new LuaMethod("Sinh", Sinh),
		new LuaMethod("Cosh", Cosh),
		new LuaMethod("Tanh", Tanh),
		new LuaMethod("Acos", Acos),
		new LuaMethod("Asin", Asin),
		new LuaMethod("Atan", Atan),
		new LuaMethod("Atan2", Atan2),
		new LuaMethod("Exp", Exp),
		new LuaMethod("Log10", Log10),
		new LuaMethod("Pow", Pow),
		new LuaMethod("Sqrt", Sqrt),
		new LuaMethod("New", _CreateMath),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("E", get_E, null),
		new LuaField("PI", get_PI, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateMath(IntPtr L)
	{
		LuaDLL.luaL_error(L, "Math class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(Math));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "System.Math", typeof(Math), regs, fields, null);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_E(IntPtr L)
	{
		LuaScriptMgr.Push(L, Math.E);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_PI(IntPtr L)
	{
		LuaScriptMgr.Push(L, Math.PI);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Abs(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Abs(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Ceiling(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Ceiling(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int BigMul(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		long o = Math.BigMul(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DivRem(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(long), typeof(long), typeof(long)};
		Type[] types1 = {typeof(int), typeof(int), typeof(int)};

		if (count == 3 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			long arg0 = (long)LuaScriptMgr.GetNumber(L, 1);
			long arg1 = (long)LuaScriptMgr.GetNumber(L, 2);
			long arg2 = LuaScriptMgr.GetNetObject<long>(L, 3);
			long o = Math.DivRem(arg0,arg1,out arg2);
			LuaScriptMgr.Push(L, o);
			LuaScriptMgr.Push(L, arg2);
			return 2;
		}
		else if (count == 3 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			int arg2 = LuaScriptMgr.GetNetObject<int>(L, 3);
			int o = Math.DivRem(arg0,arg1,out arg2);
			LuaScriptMgr.Push(L, o);
			LuaScriptMgr.Push(L, arg2);
			return 2;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Math.DivRem");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Floor(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Floor(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IEEERemainder(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double arg1 = (double)LuaScriptMgr.GetNumber(L, 2);
		double o = Math.IEEERemainder(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Log(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
			double o = Math.Log(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2)
		{
			double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
			double arg1 = (double)LuaScriptMgr.GetNumber(L, 2);
			double o = Math.Log(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Math.Log");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Max(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(short), typeof(short)};
		Type[] types1 = {typeof(sbyte), typeof(sbyte)};
		Type[] types2 = {typeof(uint), typeof(uint)};
		Type[] types3 = {typeof(ushort), typeof(ushort)};
		Type[] types4 = {typeof(ulong), typeof(ulong)};
		Type[] types5 = {typeof(double), typeof(double)};
		Type[] types6 = {typeof(byte), typeof(byte)};
		Type[] types7 = {typeof(float), typeof(float)};
		Type[] types8 = {typeof(long), typeof(long)};
		Type[] types9 = {typeof(int), typeof(int)};

		if (count == 2 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			short arg0 = (short)LuaScriptMgr.GetNumber(L, 1);
			short arg1 = (short)LuaScriptMgr.GetNumber(L, 2);
			short o = Math.Max(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			sbyte arg0 = (sbyte)LuaScriptMgr.GetNumber(L, 1);
			sbyte arg1 = (sbyte)LuaScriptMgr.GetNumber(L, 2);
			sbyte o = Math.Max(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types2, 1))
		{
			uint arg0 = (uint)LuaScriptMgr.GetNumber(L, 1);
			uint arg1 = (uint)LuaScriptMgr.GetNumber(L, 2);
			uint o = Math.Max(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types3, 1))
		{
			ushort arg0 = (ushort)LuaScriptMgr.GetNumber(L, 1);
			ushort arg1 = (ushort)LuaScriptMgr.GetNumber(L, 2);
			ushort o = Math.Max(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types4, 1))
		{
			ulong arg0 = (ulong)LuaScriptMgr.GetNumber(L, 1);
			ulong arg1 = (ulong)LuaScriptMgr.GetNumber(L, 2);
			ulong o = Math.Max(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types5, 1))
		{
			double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
			double arg1 = (double)LuaScriptMgr.GetNumber(L, 2);
			double o = Math.Max(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types6, 1))
		{
			byte arg0 = (byte)LuaScriptMgr.GetNumber(L, 1);
			byte arg1 = (byte)LuaScriptMgr.GetNumber(L, 2);
			byte o = Math.Max(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types7, 1))
		{
			float arg0 = (float)LuaScriptMgr.GetNumber(L, 1);
			float arg1 = (float)LuaScriptMgr.GetNumber(L, 2);
			float o = Math.Max(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types8, 1))
		{
			long arg0 = (long)LuaScriptMgr.GetNumber(L, 1);
			long arg1 = (long)LuaScriptMgr.GetNumber(L, 2);
			long o = Math.Max(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types9, 1))
		{
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			int o = Math.Max(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Math.Max");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Min(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(short), typeof(short)};
		Type[] types1 = {typeof(sbyte), typeof(sbyte)};
		Type[] types2 = {typeof(uint), typeof(uint)};
		Type[] types3 = {typeof(ushort), typeof(ushort)};
		Type[] types4 = {typeof(ulong), typeof(ulong)};
		Type[] types5 = {typeof(double), typeof(double)};
		Type[] types6 = {typeof(byte), typeof(byte)};
		Type[] types7 = {typeof(float), typeof(float)};
		Type[] types8 = {typeof(long), typeof(long)};
		Type[] types9 = {typeof(int), typeof(int)};

		if (count == 2 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			short arg0 = (short)LuaScriptMgr.GetNumber(L, 1);
			short arg1 = (short)LuaScriptMgr.GetNumber(L, 2);
			short o = Math.Min(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			sbyte arg0 = (sbyte)LuaScriptMgr.GetNumber(L, 1);
			sbyte arg1 = (sbyte)LuaScriptMgr.GetNumber(L, 2);
			sbyte o = Math.Min(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types2, 1))
		{
			uint arg0 = (uint)LuaScriptMgr.GetNumber(L, 1);
			uint arg1 = (uint)LuaScriptMgr.GetNumber(L, 2);
			uint o = Math.Min(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types3, 1))
		{
			ushort arg0 = (ushort)LuaScriptMgr.GetNumber(L, 1);
			ushort arg1 = (ushort)LuaScriptMgr.GetNumber(L, 2);
			ushort o = Math.Min(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types4, 1))
		{
			ulong arg0 = (ulong)LuaScriptMgr.GetNumber(L, 1);
			ulong arg1 = (ulong)LuaScriptMgr.GetNumber(L, 2);
			ulong o = Math.Min(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types5, 1))
		{
			double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
			double arg1 = (double)LuaScriptMgr.GetNumber(L, 2);
			double o = Math.Min(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types6, 1))
		{
			byte arg0 = (byte)LuaScriptMgr.GetNumber(L, 1);
			byte arg1 = (byte)LuaScriptMgr.GetNumber(L, 2);
			byte o = Math.Min(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types7, 1))
		{
			float arg0 = (float)LuaScriptMgr.GetNumber(L, 1);
			float arg1 = (float)LuaScriptMgr.GetNumber(L, 2);
			float o = Math.Min(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types8, 1))
		{
			long arg0 = (long)LuaScriptMgr.GetNumber(L, 1);
			long arg1 = (long)LuaScriptMgr.GetNumber(L, 2);
			long o = Math.Min(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types9, 1))
		{
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			int o = Math.Min(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Math.Min");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Round(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types1 = {typeof(double), typeof(MidpointRounding)};
		Type[] types2 = {typeof(double), typeof(int)};

		if (count == 1)
		{
			double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
			double o = Math.Round(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
			MidpointRounding arg1 = LuaScriptMgr.GetNetObject<MidpointRounding>(L, 2);
			double o = Math.Round(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types2, 1))
		{
			double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			double o = Math.Round(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 3)
		{
			double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			MidpointRounding arg2 = LuaScriptMgr.GetNetObject<MidpointRounding>(L, 3);
			double o = Math.Round(arg0,arg1,arg2);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Math.Round");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Truncate(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Truncate(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Sign(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		int o = Math.Sign(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Sin(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Sin(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Cos(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Cos(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Tan(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Tan(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Sinh(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Sinh(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Cosh(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Cosh(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Tanh(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Tanh(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Acos(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Acos(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Asin(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Asin(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Atan(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Atan(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Atan2(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double arg1 = (double)LuaScriptMgr.GetNumber(L, 2);
		double o = Math.Atan2(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Exp(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Exp(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Log10(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Log10(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Pow(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double arg1 = (double)LuaScriptMgr.GetNumber(L, 2);
		double o = Math.Pow(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Sqrt(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = Math.Sqrt(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}

