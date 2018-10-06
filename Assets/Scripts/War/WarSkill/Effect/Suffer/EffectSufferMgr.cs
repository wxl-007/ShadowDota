using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using AW.Data;
#if DEBUG
using System.Diagnostics;
#endif
namespace AW.War {
	/// <summary>
	/// 负责一个Effect打到身上后，处理的逻辑
	/// 这里面也是一个ISufferCastor的IOC的容器
	/// 姑且目前可以做一个简单缓存机制：就是按照EffectOp缓存一个处理单元
	/// </summary>
	public class EffectSufferMgr : IocMgr {

		/// <summary>
		/// 所有承受技能效果，实现了ISufferCastor接口都找出来
		/// </summary>
		private Dictionary<EffectOp, Type> ISufEff = null;

		/// <summary>
		/// 把所有的创建出来的实例都缓存起来
		/// </summary>
		private Dictionary<EffectOp, ISufferEffect> ImpleSuf = null;


		private EffectSufferMgr () : base() {
			ISufEff  = new Dictionary<EffectOp, Type>();
			ImpleSuf = new Dictionary<EffectOp, ISufferEffect>();

			ScanInterfaceAttriClasses(typeof(ISufferEffect), ISufEff);
		}

		public static EffectSufferMgr instance {
			get {
				return GenericSingleton<EffectSufferMgr>.Instance;
			}
		}

		/// <summary>
		/// 返回处理器的实例
		/// </summary>
		/// <returns>The implement.</returns>
		/// <param name="op">Op.</param>
		public T getImplement<T>(EffectOp op) {
			ISufferEffect imp = null;
			Type type = null;

			if(ImpleSuf.TryGetValue(op, out imp)) {
				return (T)imp;
			} else {
				if(ISufEff.TryGetValue(op, out type)) {
					imp = (ISufferEffect)Activator.CreateInstance(type);
					ImpleSuf[op] = imp;
					return (T)imp;
				} else {
					ConsoleEx.DebugLog("[SufferMgr] Op = " + op.ToString() + ". isn't finished yet.");
					return default(T);
				}
			}
		}

	}
}
