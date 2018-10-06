using System;
using System.Collections.Generic;

namespace AW.Data {

	/// <summary>
	/// 数据的来源
	/// </summary>
	public enum DataSource {
		FromNetwork = 0x0,
		FromLocal   = 0x1,
		FromBoth    = 0x2,
	}

	/// <summary>
	/// 所有不具有ID字段的数据，都继承自BaseData
	/// </summary>
	[Serializable]
	public class BaseData {

	}

	/// <summary>
	/// 所有含有ID字段的数据都继承自UniqueBaseData
	/// </summary>
	[Serializable]
	public abstract class UniqueBaseData <K> {
		public abstract K getKey();
	}

	/// <summary>
	/// 配置如何读取数据
	/// </summary>
	public class HowToRead {
		public ConfigType configType;
		//I will put it under StreamingAssets for reading local configure, while i will put it under Documents for product.
		public string path;
		//Which data struct will be
		public Type format;

		public HowToRead(ConfigType type, string p, Type whichType)
		{
			configType = type;
			path = p;
			format = whichType;
		}
	}

}


