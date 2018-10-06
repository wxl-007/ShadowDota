using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using AW.Framework;
using System;

using URes = UnityEngine.Resources;
using UObj = UnityEngine.Object;


namespace AW.Resources {


	/// 和ResourceMgrMore互为补充
	/// 
	/// 前提：ResourceMgr只负责生成Object，还没有instanciate。
	/// 只缓存Assetbundle对象，所有人调用此方法拿到的UObj都不允许缓存.
	/// 真正需要全部释放的时候是跳转场景的时候，但也未必正确，比如自己队伍里3D模型可能就不需要释放
	/// 
	/// 补充：是不是要缓存Assetbundle对象？ 
	///      如果是单模型单Assetbundle的，就完全不需要缓存Assetbundle
	///      所以这个类里面定义的方法未必合适
	/// 

	public class ResourceMgr : RefManager<AssetBundle>, IResourceLoader<UObj> {

		//Load出来的资源，是否还有用
		//key = string存储Assetbundle的名字, value = int 没用
		//因为RefAsset函数里面，会将超出缓存空间上限的AssetBundle删除，如果证明某个AssetBundle确实不在使用了，就会调用Unload(true).否则调用Unload(false)
		private Dictionary<string, int> Usless = null;
		private const int MAX_CAPACITY = 20;

		#region IResourceLoader implementation

		///
		///填写预设体的名字，其他都不包括
		/// 
		public UObj Load(string filename) {
			Utils.Assert(string.IsNullOrEmpty(filename), "Can't Load Empty Resource");

			UObj obj = null;
			#if Package
			AssetBundle asset = RefAsset(filename); 
			if(asset != null) {
				obj = asset.Load(filename);
			}
			#else
			obj = URes.Load<UObj>(filename);
			#endif

			return obj;
		}

		/// <summary>
		/// 异步加载资源的话，都会有Asset
		/// </summary>
		/// <param name="name">Name.</param>

		public void LoadAsync (string filename, System.Action<UObj, string> finished) {
			Utils.Assert(string.IsNullOrEmpty(filename), "Can't Load Empty Resource");
			#if Package 
			CoroutineProvider.Instance().StartCoroutine(AsyncAb(filename, finished));
			#else
			CoroutineProvider.Instance().StartCoroutine(Async(filename, finished));
			#endif
		}

		public void Destory(string filename) {
			Usless[filename] = 1;
		}

		/// <summary>
		/// 清空资源
		/// </summary>
		/// <param name="gc">If set to <c>true</c> gc.</param>
		public void ClearCache(bool gc) {
			KeyValuePair<string, AssetBundle>[] all = lstRefAsset.Clear();
			if(all != null) {
				int len = all.Length;
				if(len <= 0) return;
				for(int i = 0; i < len ; ++ i) {
					all[i].Value.Unload(true);
				}
			}

			if(gc) URes.UnloadUnusedAssets();
		}

		public bool hitCache (string name) {
			return lstRefAsset.Touch(name);
		}
		 

		#endregion

		//异步加载Resource目录上的东西
		IEnumerator Async(string filename, Action<UObj, string> finished) {
			ResourceRequest request = URes.LoadAsync(filename);
			yield return request;

			if(finished != null) {
				finished(request.asset, filename);
			}
		}
		//异步加载Assetbundle上的东西
		IEnumerator AsyncAb(string filename, Action<UObj, string> finished) {
			yield return null;

			bool cached = false;
			AssetBundle asset = lstRefAsset.Get(filename);
			cached = asset != null;

			if(!cached) {
				//TODO : 后期合成真实的本地路劲
				string path = ResourceSetting.ConvertToABPath (filename);
				asset = AssetBundle.CreateFromFile(path);

				/// add Reference if it does't exist
				KeyValuePair<string, AssetBundle>[] toBeRm = lstRefAsset.Add(filename, asset);
				if(toBeRm != null && toBeRm.Length > 0) {

					foreach(KeyValuePair<string, AssetBundle> rm in toBeRm) {
						bool useless = Usless.ContainsKey(rm.Key);
						if(useless) {
							Usless.Remove(rm.Key);
							rm.Value.Unload(true);
						} else {
							rm.Value.Unload(false);
						}
					}

				}

				toBeRm = null;

				AssetBundleRequest request = asset.LoadAsync(filename, typeof(UObj));
				yield return request;

				if(finished != null) {
					finished(request.asset, filename);
				}
			} 

		}


		public ResourceMgr() : base(MAX_CAPACITY, MAX_CAPACITY) { 
			Usless = new Dictionary<string, int>();
		}

		/// <summary>
		/// 添加引用, 如果已经有引用的时候，则会放到最近使用的位置，所以最不可能被删除
		/// </summary>
		/// <param name="name">Name.</param>
		protected override AssetBundle RefAsset (string filename) {
			bool cached = false;
			AssetBundle asset = lstRefAsset.Get(filename);
			cached = asset != null;

			if(!cached) {
				//TODO : 后期合成真实的本地路劲
				string path = ResourceSetting.ConvertToABPath (filename);

				///
				/// WWW.LoadFromCacheOrDownload 这个方法建议大家以后不要再用了
				/// 因为是异步方法，而且还占用内存。
				///
				asset = AssetBundle.CreateFromFile(path);

				/// add Reference if it does't exist
				KeyValuePair<string, AssetBundle>[] toBeRm = lstRefAsset.Add(filename, asset);
				if(toBeRm != null && toBeRm.Length > 0) {

					foreach(KeyValuePair<string, AssetBundle> rm in toBeRm) {
						bool useless = Usless.ContainsKey(rm.Key);
						if(useless) {
							Usless.Remove(rm.Key);
							rm.Value.Unload(true);
						} else {
							rm.Value.Unload(false);
						}
					}

				}

				toBeRm = null;
			} 

			return asset;
		}


	}
}
