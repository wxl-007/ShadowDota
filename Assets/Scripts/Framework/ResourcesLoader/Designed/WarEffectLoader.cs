using UnityEngine;
using System.Collections;
using System.IO;
using URes = UnityEngine.Resources;

namespace AW.Resources {

	/// <summary>
	/// 专门加载战斗中的特效，所有经过此加载的都被缓存住，
	/// 在Resource的目录 Resources/UnPack/Effect/...
	/// 
	/// 跳转场景的时候，缓存会全部删除
	/// </summary>

	public class WarEffectLoader : ILoaderDispose {
		//特效的数据比较多，具体的数字要调整
		private const int MAX_CAPACITY = 100;
		//缓存组件
		private IResourceLoader<Object> ObjLoader = null;

		public WarEffectLoader() {
			ObjLoader = new ResourceMgrMore(MAX_CAPACITY);
		}

		#region ILoaderDispose implementation

		public void ClearCache (bool gc) {
			ObjLoader.ClearCache(gc);
		}

		#endregion

		/// 
		/// 
		/// </summary>
		public Object load(string name) {
			if(string.IsNullOrEmpty(name)) return null;

			string path = Path.Combine (ResourceSetting.WAR_EFFECT, name);
			path = Path.Combine(ResourceSetting.UNPACKROOT, path);

			return ObjLoader.Load(path);
		}

		//
		// Debug Only
		public static GameObject Load(string name) {
			if(string.IsNullOrEmpty(name)) return null;

			string path = Path.Combine (ResourceSetting.WAR_EFFECT, name);
			path = Path.Combine(ResourceSetting.UNPACKROOT, path);

			return URes.Load(path) as GameObject;
		}
	}
}

