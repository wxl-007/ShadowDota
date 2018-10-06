using UnityEngine;
using System.Collections.Generic;
using System.IO;
using URes = UnityEngine.Resources;

namespace AW.Resources {

	/// <summary>
	/// Prefab loader.
	/// 载入预设体模型的帮助类,
	/// 尽量不要缓存，
	/// 
	/// 所有缓存都会在跳转场景的时候清空
	/// </summary>

	public class PrefabLoader : ILoaderDispose {
		#region ILoaderDispose implementation
		public void ClearCache (bool gc) {
			ObjLoader.ClearCache(gc);
		}
		#endregion
		private const int MAX_CAPACITY = 20;
		private IResourceLoader<Object> ObjLoader = null;

		public PrefabLoader() {
			ObjLoader = new ResourceMgrMore(MAX_CAPACITY);
		}

		/// <summary>
		/// Loads from unpack folder. country参数决定是否要读取 国家分类
		/// </summary>
		/// <returns>The Object from unack.</returns>
		/// <param name="name">Name.</param>
		GameObject loadFromUnPack(string name, Quaternion qua, Vector3 pos, bool country, bool cached, bool ignoreSetting) {
			Object obj = null;
			GameObject go = null;

			if(!string.IsNullOrEmpty(name)) {
				string path = string.Empty;
				if(country) {
					path = Path.Combine (CountryRegion.country, ResourceSetting.UNPACKROOT);
					path = Path.Combine (path, name);
				} else {
					path = Path.Combine (ResourceSetting.UNPACKROOT, name);
				}

				if(cached) {
					obj = ObjLoader.Load(path);
				} else {
					obj = URes.Load(path);
				}

				if (obj == null)
				{
					ConsoleEx.DebugWarning (path + "  is not find");
					return null;
				}

				if(ignoreSetting)
					go = GameObject.Instantiate(obj) as GameObject;
				else
					go = GameObject.Instantiate(obj, pos, qua) as GameObject;
			}

			return go;
		}

		public GameObject loadFromUnPack (string name, Quaternion qua, Vector3 pos, bool country = true, bool cached = false) {
			return loadFromUnPack(name, qua, pos, country, cached, false);
		}

		public GameObject loadFromUnPack(string name, bool country) {
			return loadFromUnPack(name, Quaternion.identity, Vector3.zero, country, false, true);
		}

		public GameObject loadFromUnPack(string name, bool country, bool cached) {
			return loadFromUnPack(name, Quaternion.identity, Vector3.zero, country, cached, true);
		}

	}
}
