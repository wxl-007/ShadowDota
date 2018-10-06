using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
[InitializeOnLoad]
public class EditorStringConfig
{
	public class ChineseString
	{
		public int ID;
		public string txt;
	}

	static Dictionary<int,string> config = new Dictionary<int, string>();

	static string configPath;
	static EditorStringConfig()
	{
		configPath = Application.dataPath + "/Editor/SceneEditor/JC/EditorChinese.cfg" ;
		fillconfig(configPath);
	}

	static void fillconfig(string path)
	{
		try
		{
			config.Clear();
			FileStream aFile = new FileStream(path,FileMode.Open);
			StreamReader sr = new StreamReader(aFile);
			string strLine = sr.ReadLine();
			while(strLine != null)
			{
				ChineseString chinese = fastJSON.JSON.Instance.ToObject<ChineseString>(strLine);
				if(chinese != null)
				{
					//string v = null;
					if(!config.ContainsKey(chinese.ID))
					{
						config.Add(chinese.ID,chinese.txt);
						//Debug.LogError("config["+chinese.ID.ToString()+"]="+config[chinese.ID]);
					}
				}
				strLine = sr.ReadLine();
			}
			sr.Close();
		}
		catch (IOException ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	public  static string getString(params int[] stringID)
	{
		string result = "";

		foreach(int id in stringID)
		{
			string v = null;
			config.TryGetValue(id,out v);
			if(!string.IsNullOrEmpty(v))
				result += v;
		}
		return result;
	}
}
