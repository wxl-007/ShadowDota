using System;
using System.IO;
using AW.Framework;
using System.Collections.Generic;
using AW.Data;
using fastJSON;
using AW.War;

//向上层提供场景编译器元素数据
namespace AW.Data 
{
	[Modle(type = DataSource.FromLocal)]
	public class SceneEditorDataRead : ModelBase 
	{
		//资源数据
		private Dictionary<System.Type,List<ElementInSceneData>> Dic_SceneData = new Dictionary<System.Type, List<ElementInSceneData>>();

		public SceneEditorDataRead() 
		{

		}

		public override bool loadFromConfig ()
		{
			return true;
		}

		//从配表读取场景配置数据
		public bool loadSceneConfig (int sceneId)
		{
			Dic_SceneData.Clear ();
			bool success = false;
			string localpath = Path.Combine(getBasePath(), "Config/GameScene/Scene_" + sceneId + ".cfg");
			success = File.Exists(localpath);
			if(success) 
			{
				loadFromFile(localpath);
				return success;
			}
			ConsoleEx.DebugWarning (localpath + "  not find scene editor data !!!!!");
			return false;
		}

		private bool loadFromFile (string path) 
		{
			bool success = false;
			StreamReader sr = null;
			FileStream fs = File.OpenRead(path);
			string line = null;
			try
			{
				Dic_SceneData.Clear();
				List<ElementInSceneData> n_list  = new List<ElementInSceneData>();
				List<ElementInSceneData> m_list = new List<ElementInSceneData>();
				List<ElementInSceneData> c_list  =  new List<ElementInSceneData>();

				sr = new StreamReader(fs);
				if(sr != null) 
				{
					while( !string.IsNullOrEmpty(line = sr.ReadLine()) ) 
					{
					    if(line.Length>0)
						{
							string dataString = line.Substring(1);
							switch(line[0])
							{
								case 'M':
								{
								    MapInSceneData data = fastJSON.JSON.Instance.ToObject<MapInSceneData>(dataString);
								    if(data != null)
									   m_list.Add(data as ElementInSceneData);
									break;
								}
								case 'N':
								{
								    NPCInSceneData data = fastJSON.JSON.Instance.ToObject<NPCInSceneData>(dataString);
									if(data != null)
										n_list.Add(data as ElementInSceneData);
									break;
								}
								case 'C':
								{
								    SpecialAreaInSceneData data = fastJSON.JSON.Instance.ToObject<SpecialAreaInSceneData>(dataString);
									if(data != null)
										c_list.Add(data as ElementInSceneData);
									break;
								}
							}
						}

						//ConsoleEx.DebugError(line.ToString());
					}
				}

				Dic_SceneData.Add(typeof(MapInSceneData),m_list);
				Dic_SceneData.Add(typeof(NPCInSceneData),n_list);
				Dic_SceneData.Add(typeof(SpecialAreaInSceneData),c_list);
			} 
			catch(Exception ex) 
			{
				ConsoleEx.DebugLog(ex.ToString() + "\nError Line = " + line);
				success = false;
			} 
			finally 
			{
				if(sr != null) { sr.Close(); sr = null; }
				if(fs != null) { fs.Close(); fs = null; }
			}
			return success;
		}		
			
		//获取元素资源<NPC，Map , Collider>
		public T[] GetSceneEditorElementData<T>() where T : ElementInSceneData
		{
			System.Type dataType = typeof(T);
			if( !Dic_SceneData.ContainsKey(dataType) )
				return null;

			List<ElementInSceneData> templist2 = Dic_SceneData[ dataType ];
			List<T> dataList = new List<T>();

			foreach( ElementInSceneData data in templist2)
				dataList.Add(data as T);

			return dataList.ToArray();
		}
	}

	[System.Serializable]
	public class ElementInSceneData
	{
		/// <summary>
		/// 初始位置
		/// </summary>
		public float[] pos;

		/// <summary>
		/// 初始朝向（旋转角度）
		/// </summary>
		public float[] rotation;

		/// <summary>
		/// 初始大小（缩放比例）
		/// </summary>
		public float[] scale;
	}

	//NPC在场景中的基本信息
	[System.Serializable]
	public class NPCInSceneData:ElementInSceneData
	{
		/// <summary>
		/// npcID
		/// </summary>
		public int npcID;

		/// <summary>
		/// 阵营(CAMP)
		/// </summary>
		public CAMP camp;

		/// <summary>
		/// 是否是boss
		/// </summary>
		public int isBoss;

		/// <summary>
		/// 怪物组（用于小怪编队）
		/// </summary>
		public int group;

		/// <summary>
		/// 生存时间 （>= 0的生存时间有效）
		/// </summary>
		public int lifeTime;

		/// <summary>
		/// 初始的AItype
		/// </summary>
		public int AIType;

		/// <summary>
		//初始buff
		/// </summary>
		public int[] buffs;

		/// <summary>
		///刷新规则
		/// </summary>
		public NPCRefreshRules refreshRules;

		/// <summary>
		/// npc刷新参数
		/// </summary>
		public NPCRefreshParam freshParam;

		/// <summary>
		///是否可以穿透身体
		/// </summary>
		public bool CanThrough;

		/// <summary>
		///是否是一个NPC刷新点(编译器使用,非安全字段)
		/// </summary>
		public bool isNPCRefreshPoint;


		#region  对战时候建筑配置
		/// <summary>
		///是否是一个建筑物(编译器使用,非安全字段)
		/// </summary>
		public bool isBuliding;

		//建筑（塔和兵营的分组（上中下路））
		public BATTLE_WAY way;
		//建筑的索引
		public int index;
		//是否需要营救
		public bool bSaved;
		#endregion

		public int freshParam1;
		public int freshParam2;



		public NPCInSceneData()
		{
			CanThrough = true;
		}
	}

	//地图在场景中的基本信息
	[System.Serializable]
	public class MapInSceneData:ElementInSceneData
	{
		//死后是否可以复活
		public int canReborn;
	}

	//碰撞盒在场景中的基本信息
	[System.Serializable]
	public class SpecialAreaInSceneData : ElementInSceneData
	{
		//碰撞盒类型
		public SceneElementType Type;
	}
}

