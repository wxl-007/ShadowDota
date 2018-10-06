using System;
using System.Collections.Generic;
using AW.Data;
using System.Reflection;
using System.Linq;
#if DEBUG
using System.Diagnostics;
#endif

namespace AW.War {
	/// <summary>
	/// 负责Effect的IOC容器
	/// Effect实现的实例不用重复创建
	/// </summary>
	public class EffectCastMgr : IocMgr {
		/// <summary>
		/// 所有技能效果，实现了ICastEffect接口都找出来
		/// </summary>
		private Dictionary<EffectOp,Type> ISkEff  = null;

		/// <summary>
		/// 把所有的创建出来的实例都缓存起来
		/// </summary>
		private Dictionary<EffectOp, Queue<ICastEffect>> ImpleEff = null;

		private EffectCastMgr () : base() {
			ISkEff = new Dictionary<EffectOp, Type>();
			ImpleEff = new Dictionary<EffectOp, Queue<ICastEffect>>();

			ScanInterfaceAttriClasses(typeof(ICastEffect), ISkEff);
		}

		public static EffectCastMgr Instance {
			get {
				return GenericSingleton<EffectCastMgr>.Instance;
			}
		}

		/// <summary>
		/// 获取特定的Effect实例
		/// </summary>
		/// <returns>The implement.</returns>
		/// <param name="op">Op.</param>
		public ICastEffect getImplement(EffectConfigData cfg, SkillConfigData sk) {
			Queue<ICastEffect> impQueue = null;
			ICastEffect imp = null;
			EffectOp op = cfg.EffectType;

			Type type = null;
			if(ISkEff.TryGetValue(op, out type)) {

				if(ImpleEff.TryGetValue(op, out impQueue)) {
					if(impQueue.Count == 0) {
						imp = (ICastEffect) Activator.CreateInstance(type);
					} else {
						imp = impQueue.Dequeue();
					}
				} else {
					imp = (ICastEffect) Activator.CreateInstance(type);
				}

				if(imp != null) {
					imp.Init(cfg, sk);
				}
			}

			return imp;
		}

		/// <summary>
		/// 返回一个技能所有的Effect实例
		/// </summary>
		/// <returns>The effect list.</returns>
		/// <param name="efCfgList">Ef cfg list.</param>
		public ICastEffect[] getImplements(EffectConfigData[] efCfgList, SkillConfigData sk) {
			int count = efCfgList.Length;
			ICastEffect[] effList = new ICastEffect[count];

			int i = 0;
			for(; i < count; ++ i) {
				effList[i] = getImplement(efCfgList[i], sk);
			}

			return effList;
		}

		/// <summary>
		/// 回收Effect实例
		/// </summary>
		/// <param name="cfg">Cfg.</param>
		/// <param name="casteff">Casteff.</param>
		public void Recycle(EffectConfigData cfg, ICastEffect casteff) {
			if(casteff != null && cfg != null) {
				EffectOp op = cfg.EffectType;
				Queue<ICastEffect> impQueue = null;

				if(ImpleEff.TryGetValue(op, out impQueue)) {
					impQueue.Enqueue(casteff);
				} else {
					impQueue = new Queue<ICastEffect>();
					impQueue.Enqueue(casteff);
					ImpleEff[op] = impQueue;
				}
			}
		}

	}
}
