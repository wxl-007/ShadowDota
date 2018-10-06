using System;

namespace AW.Resources {

	public class CountryRegion {
		public static string country = "China";
	}


	public static class ResourceSetting  {
		//打包目录
		public const string PACKROOT = "Pack";
		//不打包目录
		public const string UNPACKROOT = "UnPack";

		//战斗特效的子目录
		public const string WAR_EFFECT = "Effect";

        #if UNITY_IPHONE
        public static string m_RemoteFolder = "ios/";
        #elif UNITY_ANDROID
        public static string m_RemoteFolder = "android/";
        #else
        public static string m_RemoteFolder = "editor/";
        #endif
        public const string EXTENSION_FILENAME = @".unity3d";

		public static string URL;
        
        //传入Prefab的文件名，不带有路径
        public static string ConvertToAssetBundleName(string name) {
            return name + EXTENSION_FILENAME;
        }

        public static string ConverToFtpPath(string name) {
			return URL + m_RemoteFolder + Core.EngCfg.VersionCode.ToString() + "/" + name;
        }


		/// <summary>
		/// 传入Prefab的名字，生成AssetBundle的路径名
		/// </summary>
		/// <param name="prefabName">Prefab name.</param>
		public static string ConvertToABPath(string prefabName) {
			//TODO : 将来这里必须写上正确的逻辑

			return prefabName + EXTENSION_FILENAME;
		}

    }
}

