using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using fastJSON;

namespace AW.IO {


	/// 
	/// 本地存储的方法，这些方法都是基于FastJson做为序列化工具
	/// 换句话说这些都是基于String的读写方法
	/// 

	public class SharedPrefs {
		//get salt  
		private byte[] _salt;

		public SharedPrefs()
		{
			salt saltFac = new salt();
			_salt = saltFac.getSalt();
			Crypto._salt = _salt;
		}


		public bool saveValue(DataObject save, bool encrypt, FileMode mode = FileMode.Create)
		{
			bool success = false;

			try
			{
				if (save != null)
				{
					string outdata = JSON.Instance.ToJSON(save);
					if (outdata == null || outdata == string.Empty)
						return false;
					if (encrypt)
						outdata = Crypto.EncryptStringAES(outdata, Consts.sharedSecret);

					using (StreamWriter sw = new StreamWriter(File.Open(save.Path, mode)))
					{
						sw.Write(outdata);
					}
					success = true;
				}
				else
				{
					success = false;
				}
			}
			catch (Exception e)
			{
				// Let the user know what went wrong.
				ConsoleEx.DebugLog(e.Message);
				success = false;
			}

			return success;
		}


		public DataObject loadValue(string path, Type type, bool decrypt)
		{
			DataObject obj = null;

			string encrpytStr = null;
			string decryptStr = null;

			try
			{
				// Create an instance of StreamReader to read from a file.
				// The using statement also closes the StreamReader.
				using (StreamReader sr = new StreamReader(path))
				{
					// Read and display lines from the file until the end of the file is reached.
					encrpytStr = sr.ReadToEnd();
				}

				if (decrypt)
				{
					decryptStr = Crypto.DecryptStringAES(encrpytStr, Consts.sharedSecret);
					obj = JSON.Instance.ToObject(decryptStr, type) as DataObject;
				}
				else
				{
					obj = JSON.Instance.ToObject(encrpytStr, type) as DataObject;
				}

			}
			catch (Exception e)
			{
				// Let the user know what went wrong.
				ConsoleEx.DebugLog(e.Message);
			}

			return obj;
		}

		/// <summary>
		/// 静态的方法，返回List对象
		/// </summary>
		/// <returns>The value.</returns>
		/// <param name="memory">Memory.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> loadValue<T> (byte[] memory) where T : class {
			List<T> configs = new List<T>();

			using (StreamReader sr = new StreamReader(new MemoryStream(memory))) {
				string line;

				while ((line = sr.ReadLine()) != null) {
					try{
						var oneLine = fastJSON.JSON.Instance.ToObject<T> (line);
						configs.Add(oneLine);
					} catch(Exception e) {
						ConsoleEx.DebugLog(e.ToString() + "\n" + line);
					}
				}
			}

			return configs;
		}

	}
}

