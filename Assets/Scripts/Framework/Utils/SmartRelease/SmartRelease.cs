/*
 * Created By Allen Wu. All rights reserved.
 */

using UnityEngine;
using System.Collections;
using AW.Framework;

/*
 * 以下都是Allen的个人猜测
 * NGUI，没有任何引用关系的NGUI物体，即使在场景跳转的时候，也不会删除资源
 * 所以目前能做的事情，只有强制删除特定的资源
 * 
 */
public class SmartRelease {

	static void releaseAsset<T>(string[] assets) {
		Object[] atlas = Resources.FindObjectsOfTypeAll(typeof(T));
		ConsoleEx.DebugLog("total " + typeof(T).ToString() + "  " + atlas.Length);

		if(atlas != null) {
			foreach(var at in atlas)
				if(Utils.inArray<string>(at.name, assets)) {
					Resources.UnloadAsset(at);
				}
		}
	}


}
