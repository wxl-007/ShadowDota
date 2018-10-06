using System;
using AW.Cache;
using UObj = UnityEngine.Object;

namespace AW.Resources {

	/// <summary>
	/// 缓存AssetBundle的容器，Resources.Load不需要缓存
	/// 和资源有关的管理器都将继承自此类
	/// </summary>
	public abstract class RefManager<T>  {
		// 管理器所管理的资源列表，实际上是引用列表
		// value 是实际上的Asset
		protected LRUCache<string, T> lstRefAsset = null;

		// 增加引用的资源
		protected virtual T RefAsset(string name) { return default(T); }

		// 以一定的策略卸载资源
		public virtual bool UnloadAsset() { return true; }

		protected RefManager (int capacity, int max) {
			lstRefAsset = new LRUCache<string, T>(capacity, max);
		}
	}

	/// <summary>
	/// 具体的加载器的接口, 是个底层的接口，实现者只是加载到UObj的程度，不回去instanciate
	/// </summary>
	public interface IResourceLoader <T> where T : UObj  {
		/// <summary>
		/// 同步加载资源
		/// </summary>
		/// <param name="name">Name.</param>
		T Load(string name);

		/// <summary>
		/// 异步加载资源
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="finished">Finished.</param>
		void LoadAsync(string name, Action<T, string> finished);


		/// <summary>
		/// 释放资源，
		/// ResourceMgr实际上目前只对Assetbundle有效
		/// ResourceMgrMore对所有缓存的UObj有效
		/// 
		/// 也就是说这个方法可能会引发Resources.UnloadUnusedAssets
		/// </summary>
		/// <param name="name">Name.</param>
		void Destory(string name);

		/// 
		/// 释放所有的资源，把缓存清空
		/// 
		/// 参数gc控制，释放要调用Resources.UnloadUnusedAssets。
		/// 在场景跳转的时候，调用一次Resources.UnloadUnusedAssets就可以了。
		/// 
		void ClearCache(bool gc);

		/// <summary>
		/// 测试能否命中缓存里的数据
		/// </summary>
		bool hitCache(string name);
	}

	/// 
	/// 这是一个上层接口，他控制各种加载器控制器
	/// 所有的实现者都必须实现跳转场景释放资源方法，
	/// 
	public interface ILoaderDispose {
		void ClearCache(bool gc);
	}


}
