using System;
using System.Reflection;
using System.Collections.Generic;

namespace AW.Resources {
	/// <summary>
	/// 所有的Loader都在这里, 
	/// 每个Loader实际上都不会去缓存GameObject，只缓存Object（即需要Instanciate)
	/// GameObject的缓存策略都应该是最上层的逻辑来控制。
	/// </summary>
	public class Loader  {
		//类型查找器
		private ImplicitBinder binder = null;
		//所有实例的存放地
		public Dictionary<Type, ILoaderDispose> Loaders = null;

		public Loader() {
			binder = ImplicitBinder.Instance;
			Loaders = new Dictionary<Type, ILoaderDispose>();

			List<Type> allLoader = binder.ILoader;
			int count = allLoader.Count;
			if(count > 0) {
				for(int i = 0; i < count; ++ i) {
					Loaders.Add(allLoader[i], Activator.CreateInstance(allLoader[i]) as ILoaderDispose);
				}
			}
		}

		public T getLoader<T>() where T : ILoaderDispose {
			Type type = typeof(T);
			ILoaderDispose iDis = null;
			if(Loaders.TryGetValue(type, out iDis)) {
				return (T) iDis;
			}
			return default(T);
		}

	}
}
