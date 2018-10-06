using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using AW.Data;
using AW.Entity;
using AW.FSM;
using AW.Resources;
using AW.War;
#if DEBUG
using System.Diagnostics;
#endif
/// 
/// 查找整个应用的特定类
/// 
public class ImplicitBinder {

	/// <summary>
	/// 仅关心网络数据的数据层, 会经过反射创建出对象
	/// </summary>
	public List<Type> IIModelNetwork = null;
	/// 
	/// 只关心本地数据的数据层，也会经过反射被创建出对象 
	/// 
	public List<Type> IIModelConfig  = null;

	/// 
	/// 既关心网络数据，有关心本地数据。 经过反射创建出对象
	/// 
	public List<Type> IIModelBoth    = null;

	/// <summary>
	/// 所有的控制层都在这里，控制层不会被反射创建出来，只会使用Unity的方式添加上去
	/// </summary>
	private Dictionary<string, Type> IController = null;

	/// 
	/// 分析所有的控制层，将继承IGameState接口的都找出来
	/// 
	public List<LogicalType> IControllerGamePlay = null;

	/// 
	/// 分析所有的控制层，将继承IDeviceState接口的都找出来
	/// 
	public List<LogicalType> IControllerDevice = null;


	/// 
	/// 所有实现ILoaderDispose接口的都找出来
	/// 
	public List<Type> ILoader = null;



	//Hold a copy of the assembly so we aren't retrieving this multiple times. 
	private Assembly assembly;

	#if DEBUG
	Stopwatch watch = new Stopwatch();
	#endif

	private ImplicitBinder () {
		assembly = Assembly.GetExecutingAssembly();

		IIModelNetwork = new List<Type>();
		IIModelConfig  = new List<Type>();
		IIModelBoth    = new List<Type>();
		IController    = new Dictionary<string, Type>();

		IControllerGamePlay = new List<LogicalType>();
		IControllerDevice   = new List<LogicalType>();

		ILoader       = new List<Type>();

		ScanForAnnotatedClasses(new string[]{ "AW.Data", "AW.Controller" });

		ScanForImplementClasses(typeof(ILoaderDispose));
	}

	public static ImplicitBinder Instance {
		get { 
			return GenericSingleton<ImplicitBinder>.Instance; 
		}
	}

	// <summary>
	/// Search through indicated namespaces and scan for all annotated classes.
	/// Automatically create bindings
	/// </summary>
	/// <param name="usingNamespaces">Array of namespaces. Compared using StartsWith. </param>

	protected virtual void ScanForAnnotatedClasses(string[] usingNamespaces)
	{
		#if DEBUG
		watch.Start();
		#endif
		if (assembly != null) {
			IEnumerable<Type> types = assembly.GetExportedTypes();

			List<Type> typesInNamespaces = new List<Type>();
			int namespacesLength = usingNamespaces.Length;
			for (int ns = 0; ns < namespacesLength; ns++) {
				typesInNamespaces.AddRange(types.Where(t => !string.IsNullOrEmpty(t.Namespace) && t.Namespace.StartsWith(usingNamespaces[ns])));
			}

			foreach (Type type in typesInNamespaces) {
				object[] implements = type.GetCustomAttributes(typeof (ModleAttribute), false);
				if(implements.Any()) {
					ModleAttribute mat = (ModleAttribute) implements[0];
					if(mat.type == DataSource.FromNetwork) {
						IIModelNetwork.Add(type);
					} else if(mat.type == DataSource.FromLocal) {
						IIModelConfig.Add(type);
					} else if(mat.type == DataSource.FromBoth) {
						IIModelBoth.Add(type);
					}
				}

				implements = type.GetCustomAttributes(typeof(ControllerAttribute), false);
				if(implements.Any()) {
					ControllerAttribute mat = (ControllerAttribute) implements[0];
					IController.Add(mat.ctrlType.ToString(), type);

					///
					/// 查找Controller继承自IGameState接口
					///
					if(type.GetInterfaces().Contains(typeof(IGameState))) {
						IControllerGamePlay.Add(mat.ctrlType);
					}

					///
					/// 查找Controller继承自IDeviceState接口
					///
					if(type.GetInterfaces().Contains(typeof(IDeviceState))) {
						IControllerDevice.Add(mat.ctrlType);
					}

				}
			}

		}

		#if DEBUG
		ConsoleEx.DebugLog("Scan For Annotated Classes costs " + watch.ElapsedMilliseconds + " miliseconds to be done!");
		watch.Reset();
		#endif
	}

	///
	///所有继承自特定接口的类
	/// 
	protected virtual void ScanForImplementClasses(Type interfacetype) {
		#if DEBUG
		watch.Start();
		#endif
		if (assembly != null) {
			IEnumerable<Type> types = assembly.GetExportedTypes();
			Type[] classes = null;
			classes = types.Where(t => t.GetInterfaces().Contains(interfacetype)).ToArray();
			ILoader.AddRange(classes);
		}
		#if DEBUG
		ConsoleEx.DebugLog("Scan For Implemented " + interfacetype.ToString() + "  Classes costs " + watch.ElapsedMilliseconds + " miliseconds to be done!");
		watch.Reset();
		#endif
	}


	/// <summary>
	/// 获取控制层的Type
	/// </summary>
	public Type getController(string name) {
		Type type = null;
		if(string.IsNullOrEmpty(name)) 
			return type;

		if(IController.TryGetValue(name, out type)) {

		} else {
			ConsoleEx.DebugLog("Can't find " + name + " controller");
			type = null;
		}
		return type;
	}

}
