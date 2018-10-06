using System;
using System.IO;

namespace AW.Data {
	public class ModelBase  {
		//数据类型
		protected ConfigType type;

		protected string getBasePath() {
			#if CHECKCONFIG		
			string BasePath = DeviceInfo.PersistRootPath;
			string hasDownloaded = Path.Combine(DeviceInfo.PersistRootPath, "Config");
			if(Directory.Exists(hasDownloaded)) {
				string[] fileName = Directory.GetFiles(hasDownloaded);
				if ( !(fileName != null && fileName.Length >= 1) )
					BasePath = DeviceInfo.StreamingPath;
			}
			else
				BasePath = DeviceInfo.StreamingPath;
			#else
			string BasePath = DeviceInfo.StreamingPath;
			if(Core.DevFSM.rtPlatform == UnityEngine.RuntimePlatform.IPhonePlayer || 
				Core.DevFSM.rtPlatform == UnityEngine.RuntimePlatform.Android || 
				Core.DevFSM.rtPlatform == UnityEngine.RuntimePlatform.WP8Player) {

				string hasDownloaded = Path.Combine(DeviceInfo.PersistRootPath, "Config");
				if (Directory.Exists(hasDownloaded)) {
					string[] fileName = Directory.GetFiles(hasDownloaded);
					if(fileName != null && fileName.Length >= 1) 
						BasePath = DeviceInfo.PersistRootPath;
				}
			}

			#endif			
			return BasePath;
		}

		/*
		 * 下面这些Virtual的方法必须实现
		 */ 
		public virtual bool loadFromConfig() {
			throw new NotImplementedException();
		}
	
	}
}

