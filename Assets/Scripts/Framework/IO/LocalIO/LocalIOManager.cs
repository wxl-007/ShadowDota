using System;
using System.IO;
using System.Collections.Generic;
using AW.Framework;
using AW.FSM;

namespace AW.IO {

	public class RelationData {
		public string fileName;
		public Type typeOfData;

		public RelationData(string file, Type type) {
			this.fileName = file;
			this.typeOfData = type;
		}
	}


	/// <summary>
	/// 本地IO是非常底层的类，所以必须最先初始化Ok
	/// 同时，这个类又必须初始化两次。
	/// </summary>
	public class LocalIOManager : DataPersisterConfig, ILocalIO, IGameState {
		private string baseAccountPath;
		private string baseNonAccountPath;
		//this only can be read or set to null
		private SharedPrefs prefs;

		public string AccountPath
		{
			set { baseAccountPath = value; }
		}

		public string NonAccountPath
		{
			set { baseNonAccountPath = value; }
		}


		private LocalIOManager(string basePath) {
			prefs = new SharedPrefs();
			baseNonAccountPath = basePath;
		}

		private static LocalIOManager dsManager = null;
		public static LocalIOManager getInstance(string basePath)
		{
			if(dsManager == null)
				dsManager = new LocalIOManager(basePath);
			return dsManager;
		}

		public bool AppendToLocalFileSystem(DataObject toBeSaved, bool crypto = true) {
			bool success = true;
			if(toBeSaved == null) {
				success = false;
			} else {
				generatePath( ref toBeSaved );
				success = prefs.saveValue( toBeSaved, crypto, FileMode.Append);
			}
			return success;
		}

		public bool WriteToLocalFileSystem(DataObject toBeSaved, bool crypto = true) {
			bool success = true;
			if(toBeSaved == null) {
				success = false;
			} else {
				generatePath( ref toBeSaved );
				success = prefs.saveValue( toBeSaved, crypto, FileMode.Create );
			}
			return success;
		}

		public DataObject ReadFromLocalFileSystem(DataType curType, DataObject.Relevant KindOfRelevant, bool decrypto = true) {
			DataObject ob = null;
			ob = prefs.loadValue(generatePath(curType, KindOfRelevant), generateType(curType), decrypto);
			if (ob != null)
			{
				ob.mType = curType;
			}
			return ob;
		}

		/// *****************   we will generate storage path *******************
		/// 会将生成的路径信息写入DataObject
		private void generatePath(ref DataObject toBeSaved) {
			string fileName = PreDefined[toBeSaved.mType].fileName;
			if (toBeSaved.KindOfRelevant == DataObject.Relevant.DeviceRelevant) {
				toBeSaved.Path = Path.Combine(baseNonAccountPath, fileName);
			} else {
				toBeSaved.Path = Path.Combine(baseAccountPath, fileName);
			}
		}

		private string generatePath(DataType curType, DataObject.Relevant KindOfRelevant) {
			string fileName = PreDefined[curType].fileName;
			string curPath = string.Empty;
			if (KindOfRelevant == DataObject.Relevant.DeviceRelevant){
				curPath = Path.Combine(baseNonAccountPath, fileName);
			} else {
				curPath = Path.Combine(baseAccountPath, fileName);
			}
			return curPath;
		}

		private Type generateType (DataType curType) {
			return PreDefined[curType].typeOfData;
		}


		/// 
		/// ************************** 接口实现 ****************************
		/// 

		public void OnUnregister(StateParam<GameState> obj) {
			///
			/// 注销的时候，要清除关于账号的信息数据
			///
			baseAccountPath = null;
		}

		public void OnLogin(StateParam<GameState> obj) {
			LoginInfo info = obj.obj as LoginInfo;
			if(info != null) {
				baseAccountPath = Path.Combine(baseNonAccountPath, info.UniqueId);
				if(!Directory.Exists(baseAccountPath)) 
					Directory.CreateDirectory(baseAccountPath);

				baseAccountPath = Path.Combine(baseAccountPath, info.curServer);
				if(!Directory.Exists(baseAccountPath))
					Directory.CreateDirectory(baseAccountPath);

				info.LocalIOMgr = this;
			} else {
				throw new DragonException("DataPersisteManager OnLogin must get PathInfo Object.");
			}
		}

		/// <summary>
		/// 一天日期结束
		/// </summary>
		/// <param name="obj">Object.</param>
		public void OnDayChanged(StateParam<GameState> obj){
			//TODO : do nothing
		}
		/// <summary>
		/// 场景跳转
		/// </summary>
		public void OnLevelChanged(StateParam<GameState> obj) {
			//TODO : do nothing 
		}

	}


}


