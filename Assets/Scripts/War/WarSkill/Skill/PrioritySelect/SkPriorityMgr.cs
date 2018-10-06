using System;
using System.Collections.Generic;
using AW.Data;

namespace AW.War {
	/// <summary>
	/// 技能优先级的容器管理
	/// </summary>
	public class SkPriorityMgr : IocMgr {
		//查找出类
		private Dictionary<SkTargetPriority, Type> Container = null;
		//缓存类的实例
		private Dictionary<SkTargetPriority, IPrioritySelect> ImpleContainer = null;

		public SkPriorityMgr() {
			Container = new Dictionary<SkTargetPriority, Type>();
			ImpleContainer = new Dictionary<SkTargetPriority, IPrioritySelect>();

			ScanSkPriorityClasses( typeof(IPrioritySelect), Container);
		}

		public static SkPriorityMgr instance {
			get {
				return GenericSingleton<SkPriorityMgr>.Instance;
			}
		}

		/// <summary>
		/// 返回处理器的实例
		/// </summary>
		/// <returns>The implement.</returns>
		/// <param name="op">Op.</param>
		public IPrioritySelect getImplement(SkTargetPriority priority) {
			IPrioritySelect imp = null;
			Type type = null;

			if(ImpleContainer.TryGetValue(priority, out imp)) {
				return imp;
			} else {
				if(Container.TryGetValue(priority, out type)) {
					imp = (IPrioritySelect)Activator.CreateInstance(type);
					ImpleContainer[priority] = imp;
					return imp;
				} else {
					ConsoleEx.DebugLog("[Skill Priority]  = " + priority.ToString() + ". isn't finished yet.");
					return null;
				}
			}
		}
	}

}
