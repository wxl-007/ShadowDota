#if DEBUG
using System.Diagnostics;
#endif
using System;
using fastJSON;
using System.IO;
using AW.Framework;
using System.Collections.Generic;

namespace AW.Data {
	public class ArraryModelBase <V> : ModelBase where V : BaseData {
		protected List<V> instance = new List<V>();

		public bool load(ConfigType cfgType) {
			return readFromLocalConfigFile(cfgType, instance);
		}

		#if DEBUG
		private Stopwatch stopwatch = new Stopwatch();
		#endif

		#region 读取没有KEY的配表
		public virtual bool readFromLocalFile(string path, IList<V> container, Type type)
		{
			bool success = false;

			string localpath = Path.Combine(getBasePath(), path);

			success = File.Exists(localpath);
			if(success) {
				success = readFile(localpath, container, type);
			}

			return success;
		}

		public virtual bool readFromLocalConfigFile(ConfigType configType, IList<V> container) 
		{
			type = configType;

			Utils.Assert(container == null, "Read Config file :" + configType.ToString() + " null.");

			return this.readFromLocalFile(Config.LocalConfigs[configType].path, container, Config.LocalConfigs[configType].format);
		}


		private bool readFile(string path, IList<V> container, Type type)
		{
			bool success = true;

			#if DEBUG
			stopwatch.Start();
			#endif

			StreamReader sr = null;
			FileStream fs = File.OpenRead(path);
			string line = null;
			try {
				sr = new StreamReader(fs);
				if(sr != null) {
					while( !string.IsNullOrEmpty(line = sr.ReadLine()) ) {
						if(!line.StartsWith("#")) {
							V t = JSON.Instance.ToObject(line, type) as V;
							container.Add(t);
						}
					}
				}
			} catch(Exception ex) {
				ConsoleEx.DebugLog(ex.ToString() + "\nError Line = " + line);
				success = false;
			} finally {
				if(sr != null) { sr.Close(); sr = null; }
				if(fs != null) { fs.Close(); fs = null; }
				#if DEBUG
				ConsoleEx.DebugLog(type.ToString() + " costs " + stopwatch.ElapsedMilliseconds + " miliseconds to be done!");
				stopwatch.Reset();
				#endif
			}
			return success;
		}
		#endregion


		#region 写入没有KEY的配表
		public virtual bool writeToLocalFile(string path, IList<V> container){
			bool success = false;
			string localpath = Path.Combine(getBasePath(), path);

			success = File.Exists(localpath);
			if(success) {
				File.Delete(localpath);
			}
			success = writeFile(localpath, container);
			return success;
		}

		public virtual bool writeToLocalConfigFile(ConfigType configType, IList<V> container) {
			type = configType;

			Utils.Assert(container == null, "Read Config file :" + configType.ToString() + " null.");

			return this.writeToLocalFile(Config.LocalConfigs[configType].path, container);
		}

		private bool writeFile(string path, IList<V> container) {
			bool success = true;
			#if DEBUG
			stopwatch.Start();
			#endif

			StreamWriter sw = null;
			FileStream fs = File.Open(path, FileMode.OpenOrCreate);

			string line = null;
			try {
				sw = new StreamWriter(fs);
				if(sw != null)
				{
					foreach(V t in container)
					{
						string s = JSON.Instance.ToJSON(t);

						sw.WriteLine(s);
					}
				}
			} catch(Exception ex) {
				ConsoleEx.DebugLog(ex.ToString() + "\nError Line = " + line);
				success = false;
			} finally {
				if(sw != null) { sw.Close(); sw = null; }
				if(fs != null) { fs.Close(); fs = null; }
				#if DEBUG
				ConsoleEx.DebugLog(type.ToString() + " costs " + stopwatch.ElapsedMilliseconds + " miliseconds to be done!");
				stopwatch.Reset();
				#endif
			}
			return success;
		}
		#endregion
	}
}
