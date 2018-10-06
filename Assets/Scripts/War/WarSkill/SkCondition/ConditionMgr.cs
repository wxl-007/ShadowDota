using System;
using AW.Data;
using System.Collections.Generic;

namespace AW.War {
	/// <summary>
	/// 激发其他技能的IOC容器
	/// </summary>
	public class ConditionMgr : IocMgr { 
		/// <summary>
		/// 获取Type类型
		/// </summary>
		private Dictionary<SkConditionType, Type> IConType = null;
		/// <summary>
		/// 保存实例
		/// </summary>
		private Dictionary<SkConditionType, ICondition> ImpleCon = null;

		private ConditionMgr () {
			IConType = new Dictionary<SkConditionType, Type>();
			ImpleCon = new Dictionary<SkConditionType, ICondition>();

			ScanInterfaceAttriClasses(typeof(ICondition), IConType);
		}

		public static ConditionMgr instance {
			get { return GenericSingleton<ConditionMgr>.Instance; }
		}

		public ICondition getImplement(SkConditionType con) {
			Type exType = null;
			ICondition excutor = null;

			if(!ImpleCon.TryGetValue(con, out excutor)) {

				if(IConType.TryGetValue(con, out exType)) {
					excutor = Activator.CreateInstance(exType, true) as ICondition;
					ImpleCon[con] = excutor;
				} else {
					ConsoleEx.DebugLog("[Condition Type] = " + con.ToString() + " implement does't exist.", ConsoleEx.RED);
					return null;
				}

			}

			return excutor;
		}

	}

}
