using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AW.Framework;
using System;
using AW.Cache;
using URes = UnityEngine.Resources;
using UObj = UnityEngine.Object;

namespace AW.Resources {


	/// 和ResourceMgr互为补充
	/// 
	/// 前提：会去缓存Resources.Load 和 AssetBundle.Load出来的对象UObj
	/// 
	/// 这个类采取了和ResourceMgr完全不一样的缓存策略，
	/// 他对Assetbundle完全不缓存，他只缓存Assetbundle.Load出来的对象.
	/// 所有AssetBundle一旦load出来后，直接调用Unload（false）对象
	/// 
	/// 
	/// 这个类可能更加合适于加载3D模型，但是什么时候释放资源也不一定， 比如自己队伍里3D模型可能就不需要释放
	/// 
	public class ResourceMgrMore : RefManager<UObj>, IResourceLoader<UObj> {
		#region IResourceLoader implementation

		public UObj Load (string filename) {
			Utils.Assert(string.IsNullOrEmpty(filename), "Can't Load Empty Resource");
			UObj obj = RefAsset(filename);
			return obj;
		}

		public void LoadAsync (string filename, Action<UObj, string> finished) {
			Utils.Assert(string.IsNullOrEmpty(filename), "Can't Load Empty Resource");

			///
			/// 有一定策略来决定是Resource加载还是Assetbundle加载
			///
			#if Package
			CoroutineProvider.Instance().StartCoroutine(AsyncAb(filename, finished));
			#else
			CoroutineProvider.Instance().StartCoroutine(AsyncRes(filename, finished));
			#endif

		}


		public void Destory (string name) {
			if(string.IsNullOrEmpty(name)) return;
			bool exist = lstRefAsset.Delete(name);

			if(exist) unUsedUObj ++;
			if(unUsedUObj >= MAX_UNUSED_OBJ) {
				URes.UnloadUnusedAssets();
			}
			unUsedUObj = 0;
		}

		/// <summary>
		/// 清空资源
		/// </summary>
		/// <param name="gc">If set to <c>true</c> gc.</param>
		public void ClearCache(bool gc) {
			lstRefAsset = new LRUCache<string, UObj>(MAX_CAPACITY, MAX_CAPACITY);
			unUsedUObj = 0;
			if(gc) URes.UnloadUnusedAssets();
		}

		/// <summary>
		/// 是否有缓存
		/// </summary>
		/// <returns><c>true</c>, if cache was hit, <c>false</c> otherwise.</returns>
		/// <param name="filename">Filename.</param>
		public bool hitCache(string filename) {
			return lstRefAsset.Touch(filename);
		}
		#endregion

		#region 异步获取UObj的方法

		IEnumerator AsyncRes(string filename, Action<UObj, string> finished) {
			bool cached = false;
			UObj obj = lstRefAsset.Get(filename);
			cached = obj != null;

			if(!cached) {
				ResourceRequest request = URes.LoadAsync(filename);
				yield return request;

				//加入引用列表的同时，释放资源
				releaseRes(lstRefAsset.Add(filename, request.asset));
				obj = request.asset;
			} 

			if(finished != null) {
				finished(obj, filename);
			}
		}

		IEnumerator AsyncAb(string filename, Action<UObj, string> finished) {
			yield return null;

			bool cached = false;
			UObj obj = lstRefAsset.Get(filename);
			cached = obj != null;

			if(!cached) {
				//TODO : 后期合成真实的本地路劲
				string path = ResourceSetting.ConvertToABPath (filename);
				AssetBundle asset = AssetBundle.CreateFromFile(path);

				AssetBundleRequest request = asset.LoadAsync(filename, typeof(UObj));
				yield return request;

				//加入引用列表的同时，释放资源
				releaseRes(lstRefAsset.Add(filename, request.asset));
				obj = request.asset;
			} 

			if(finished != null) {
				finished(obj, filename);
			}
		}

		#endregion

		#region RefManager的重载方法


		/// ------------------------------------------------
		/// **********************************************************
		/// lstRefAsset 是缓存容器
		/// lstRefAsset 这个的Key是string，Value是UObj.Value没有什么疑问都是确定的，
		/// 但是Key要分情况讨论。 1：是Resource.Load方法，Key就是预设体在Resource的目录位置（包含名字）
		///                     2：是AssetBundle，Key就只是预设体的名字
		/// 
		/// 
		/// ********************************************************
		/// ------------------------------------------------

	
		protected override UObj RefAsset (string filename) {
			//有一个什么策略来走 RefResource 和 RefAssetBundle 方法

			#if Package
			return RefAssetBundle(filename);
			#else
			return RefResource(filename);
			#endif

		}


		UObj RefResource(string filename) {
			UObj obj = lstRefAsset.Get(filename);
			if(obj == null) {
				obj = URes.Load(filename); 
				//加入引用列表的同时，释放资源
				releaseRes(lstRefAsset.Add(filename, obj));
			}
			return obj;
		}


		UObj RefAssetBundle(string filename) {
			UObj obj = lstRefAsset.Get(filename);
			if(obj == null) {
				//TODO : 后期合成真实的本地路劲
				string path = ResourceSetting.ConvertToABPath (filename);

				///
				/// WWW.LoadFromCacheOrDownload 这个方法建议大家以后不要再用了
				/// 因为是异步方法，而且还占用内存。
				///

				AssetBundle asset = AssetBundle.CreateFromFile(path);
				obj = asset.Load(filename);

				//释放Compressed资源
				asset.Unload(false);

				//加入引用列表的同时，释放资源
				releaseRes(lstRefAsset.Add(filename, obj));
			}

			return obj;
		}

		#endregion

		private readonly int MAX_CAPACITY;
		//记录下最近最久未使用的Uobj的个数，
		//从而能决定什么时候使用Resources。UnloadUnusedAsset
		private int unUsedUObj = 0;
		//最大允许的未被使用的UObj个数
		private const int MAX_UNUSED_OBJ = 5;

		void releaseRes (KeyValuePair<string, UObj>[] toBeRm) {
			if(toBeRm != null) {
				unUsedUObj += toBeRm.Length;
				if(unUsedUObj >= MAX_UNUSED_OBJ) {
					URes.UnloadUnusedAssets();
				}
				unUsedUObj = 0;
			}
			toBeRm = null;
		}



		public ResourceMgrMore(int capacity) : base(capacity, capacity) {
			MAX_CAPACITY = capacity;
		}

	}
}
