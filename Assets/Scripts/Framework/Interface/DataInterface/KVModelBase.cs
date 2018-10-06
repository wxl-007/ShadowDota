#if DEBUG
using System.Diagnostics;
#endif
using System;
using fastJSON;
using System.IO;
using AW.Framework;
using System.Collections.Generic;

namespace AW.Data {
	public class KVModelBase<K, V> : ModelBase where V : UniqueBaseData <K> {

		protected Dictionary<K, V> instance = new Dictionary<K, V>();

		/// <summary>
		/// 根据key 获取value
		/// </summary>
		/// <param name="key">Key.</param>
		public V get(K key) {
			V v = default(V);
			bool found = instance.TryGetValue(key, out v);
			if(found) 
				return v;
			else
				return default(V);
		}


		#if DEBUG
		private Stopwatch stopwatch = new Stopwatch();
		#endif

		protected bool load(ConfigType type) {
			return readFromLocalConfigFile(type, instance);
		}
	
		#region 读取有Key的配表
		public virtual bool readFromLocalFile(string path, IDictionary<K, V> container, Type type)
		{
			bool success = false;

			string localpath = Path.Combine(getBasePath(), path);

			success = File.Exists(localpath);
			if(success) {
				success = readFile(localpath, container, type);
			}

			return success;
		}

		public virtual bool readFromLocalConfigFile(ConfigType configType, IDictionary<K, V> container) {
			type = configType;

			Utils.Assert(container == null, "Read Config file :" + configType.ToString() + " null.");

			return this.readFromLocalFile(Config.LocalConfigs[configType].path, container, Config.LocalConfigs[configType].format);
		}

		/// <summary>
		/// Reads the config files.
		/// </summary>
		/// <param name="path">Path.</param>

		private bool readFile(string path, IDictionary<K, V> container, Type type)
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
						if(!line.StartsWith ("#")) {
							V t = JSON.Instance.ToObject(line, type) as V;
							container[t.getKey()] = t;
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


		#region 写入有KEY的配表
		public virtual bool writeToLocalFile(string path, IDictionary<K, V> container)
		{
			bool success = false;

			string localpath = Path.Combine(getBasePath(), path);

			success = File.Exists(localpath);
			if(success) {
				File.Delete(localpath);
			}
			success = writeFile(localpath, container);

			return success;
		}

		public virtual bool writeToLocalConfigFile(ConfigType configType, IDictionary<K, V> container) {
			type = configType;

			Utils.Assert(container == null, "Read Config file :" + configType.ToString() + " null.");

			return this.writeToLocalFile(Config.LocalConfigs[configType].path, container);
		}

		private bool writeFile(string path, IDictionary<K, V> container) {
			bool success = true;
			Utils.Assert( string.IsNullOrEmpty(path), "Path is empty When reads " + container.ToString() + " File.");

			#if DEBUG
			stopwatch.Start();
			#endif
			File.Delete(path);
			StreamWriter sw = null;
			FileStream fs = File.Open(path, FileMode.OpenOrCreate);
			string line = null;
			try {
				sw = new StreamWriter(fs);
				if(sw != null) {
					foreach(V value in container.Values)
					{
						string s = JSON.Instance.ToJSON(value);
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
