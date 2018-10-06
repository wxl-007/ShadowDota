using System;

/// <summary>
/// 底层本地存储的IO封装
/// </summary>
namespace AW.IO {

	/// <summary>
	/// 所有要保存到本地的数据，都继承自DataObject
	/// 所有的子类都要往 mType和 KindOfRelevant里面赋值
	/// </summary>

	[Serializable]
	public class DataObject {
		[NonSerialized]
		public DataType mType; //是何种类型的数据

		[NonSerialized]
		public string Path;    //保存生成的路径

		[NonSerialized]
		public Relevant KindOfRelevant; //是否是账号唯一,或者是机器唯一的

		public enum Relevant {
			//账号相关
			AccountRelevant, 
			//机器相关
			DeviceRelevant,
		}


		//采用AES加密
		public const bool AES_ENCRYPT = true;
		//不采用加密
		public const bool NO_ENCRYPT  = false;
	}


	public interface ILocalIO {
		/// <summary>
		/// 将内存数据写入本地磁盘，覆盖的方式写入
		/// </summary>
		/// <returns><c>true</c>, if to local file system was writed, <c>false</c> otherwise.</returns>
		/// <param name="toBeSaved">待保存的数据结构</param>
		/// <param name="crypto">是否加密</param>
		bool WriteToLocalFileSystem(DataObject toBeSaved, bool crypto = true);



		/// <summary>
		/// 将内存数据写入本地磁盘，往原始文件后增加的方式写入
		/// </summary>
		/// <returns><c>true</c>, if to local file system was appended, <c>false</c> otherwise.</returns>
		/// <param name="toBeSaved">待保存的数据结构</param>
		/// <param name="crypto"> 是否加密 </param>
		bool AppendToLocalFileSystem(DataObject toBeSaved, bool crypto = true);


		/// <summary>
		/// 从本地数据里，读到内存里面
		/// </summary>
		/// <returns>The from local file system.</returns>
		/// <param name="curType">数据格式</param>
		/// <param name="decrypto">是否解密的方式打开</param>
		DataObject ReadFromLocalFileSystem(DataType curType, DataObject.Relevant KindOfRelevant, bool decrypto = true);
	}

}

