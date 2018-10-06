using UnityEngine;
using System.Collections.Generic;
using System.IO;
using URes = UnityEngine.Resources;

namespace AW.Resources {

	/// <summary>
	/// 只缓存常用的背景音乐和按钮声效，
	/// 特殊缓存的声效文件（比如上阵人物身上的声效应该缓存）
	/// 
	/// 其他的缓存，比如战斗里的声效，跳转场景的时候都要释放
	/// 
	/// </summary>
	public class SoundLoader : ILoaderDispose {
		#region ILoaderDispose implementation

		public void ClearCache (bool gc) {

		}

		#endregion

		public AudioClip load(string name, bool cached) {
			return null;
		}
	}
}
