using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using AW.Data;
#if DEBUG
using System.Diagnostics;
#endif

namespace AW.War {

	public class IocMgr {
		//Hold a copy of the assembly so we aren't retrieving this multiple times. 
		protected Assembly assembly;

		#if DEBUG
		protected Stopwatch watch = null;
		#endif

		protected IocMgr() {
			assembly = Assembly.GetExecutingAssembly();
			#if DEBUG
			watch = new Stopwatch();
			#endif
		}

		/// <summary>
		/// 寻找特定的命名空间下的特定Attribute的类
		/// 这些类的默认构造函数都有参数
		/// </summary>
		protected virtual void ScanInterfaceAttriClasses(Type interfacetype, Dictionary<SkConditionType, Type> container) {
			#if DEBUG
			watch.Start();
			#endif
			if (assembly != null) {
				IEnumerable<Type> types = assembly.GetExportedTypes();
				Type[] classes = null;
				classes = types.Where(t => t.GetInterfaces().Contains(interfacetype)).ToArray();

				foreach (Type type in classes) {
					object[] implements = type.GetCustomAttributes(typeof(ConditionAttribute), false);
					if(implements.Any()) {
						ConditionAttribute mat = (ConditionAttribute) implements[0];
						container[mat.Con] = type;
					}
				}
			}

			#if DEBUG
			ConsoleEx.DebugLog("Scan For Namespace & Attribe Classes costs " + watch.ElapsedMilliseconds + " miliseconds to be done!");
			watch.Reset();
			#endif
		}

		/// <summary>
		/// 寻找特定的命名空间下的特定Attribute的类
		/// 这些类的默认构造函数都有参数
		/// </summary>
		protected virtual void ScanInterfaceAttriClasses(Type interfacetype, Dictionary<EffectOp, Type> container) {
			#if DEBUG
			watch.Start();
			#endif
			if (assembly != null) {
				IEnumerable<Type> types = assembly.GetExportedTypes();
				Type[] classes = null;
				classes = types.Where(t => t.GetInterfaces().Contains(interfacetype)).ToArray();

				foreach (Type type in classes) {
					object[] implements = type.GetCustomAttributes(typeof(EffectAttribute), false);
					if(implements.Any()) {
						EffectAttribute mat = (EffectAttribute) implements[0];
						container[mat.OP] = type;
					}
				}
			}

			#if DEBUG
			ConsoleEx.DebugLog("Scan For Namespace & Attribe Classes costs " + watch.ElapsedMilliseconds + " miliseconds to be done!");
			watch.Reset();
			#endif
		}

		/// <summary>
		/// 寻找特定的命名空间下的特定Attribute的类
		/// 这些类的默认构造函数都有参数
		/// </summary>
		protected virtual void ScanTriggerClasses(Dictionary<WarMsg_Type, Type> container) {
			#if DEBUG
			watch.Start();
			#endif
			if (assembly != null) {
				IEnumerable<Type> types = assembly.GetExportedTypes();
				Type[] classes = null;
				classes = types.Where(t => t.GetInterfaces().Contains( typeof(ITriggerItem) )).ToArray();

				foreach (Type type in classes) {
					object[] implements = type.GetCustomAttributes(typeof(TriggerAttribute), false);
					if(implements.Any()) {
						TriggerAttribute mat = (TriggerAttribute) implements[0];
						container[mat.Cmd] = type;
					}
				}
			}

			#if DEBUG
			ConsoleEx.DebugLog("Scan For Trigger Classes costs " + watch.ElapsedMilliseconds + " miliseconds to be done!");
			watch.Reset();
			#endif
		}

		/// <summary>
		/// 寻找特定的Attribute的类，这些类都是有默认构造函数的（这里用来寻找算子）
		/// </summary>
		/// <param name="container">Container.</param>
		protected virtual void ScanOperatorClasses(Dictionary<EffectOp, Type> container, Type op) {
			#if DEBUG
			watch.Start();
			#endif

			if(assembly != null) {
				Type[] classes = assembly.GetExportedTypes().Where(t => t.BaseType == op).ToArray();
				int len = classes.Length;
				if(len > 0) {
					for(int i = 0; i < len; ++ i) {
						Type type = classes[i];
						object[] implements = type.GetCustomAttributes(typeof(EffectAttribute), false);
						if(implements.Any()) {
							EffectAttribute mat = (EffectAttribute) implements[0];
							if(mat.OP == EffectOp.Injury) {
								container[EffectOp.Treat] = type;
							}
							container[mat.OP] = type;
						}
					}
				}
			}

			#if DEBUG
			ConsoleEx.DebugLog("Scan For Operator Classes costs " + watch.ElapsedMilliseconds + " miliseconds to be done!");
			watch.Reset();
			#endif
		}

		/// <summary>
		/// 依赖特定的接口，获取技能优先级的判定类
		/// </summary>
		/// <param name="interfaceType">Interface type.</param>
		/// <param name="container">Container.</param>
		protected virtual void ScanSkPriorityClasses(Type interfacetype, Dictionary<SkTargetPriority, Type> container) {
			#if DEBUG
			watch.Start();
			#endif
			if (assembly != null) {
				IEnumerable<Type> types = assembly.GetExportedTypes();
				Type[] classes = null;
				classes = types.Where(t => t.GetInterfaces().Contains(interfacetype)).ToArray();

				foreach (Type type in classes) {
					object[] implements = type.GetCustomAttributes(typeof(SkPriorityAttribute), false);
					if(implements.Any()) {
						SkPriorityAttribute mat = (SkPriorityAttribute) implements[0];
						container[mat.priority] = type;
					}
				}
			}

			#if DEBUG
			ConsoleEx.DebugLog("Scan For Namespace & Attribe Classes costs " + watch.ElapsedMilliseconds + " miliseconds to be done!");
			watch.Reset();
			#endif
		}
	}
}
