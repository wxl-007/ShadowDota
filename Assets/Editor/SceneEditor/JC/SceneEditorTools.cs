using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using fastJSON;
using System.IO;
using SceneEditorSttingData;
//场景编辑器具体功能
public class SceneEditorTools  
{
	//创建地图
	public static GameObject CreateMap(string sourcePath, Vector3 worldPos = default(Vector3), Vector2 mapGridSize = default(Vector2) ,Color GridGolor  = default(Color) )
	{
		Object mapSource = AssetDatabase.LoadMainAssetAtPath(sourcePath);
		if(mapSource != null)
			return CreateMap(mapSource , worldPos , mapGridSize);
		EditorUtility.DisplayDialog(EditorStringConfig.getString(10042),"Not found: "+sourcePath,EditorStringConfig.getString(10025));
		return null;
	}

	//创建地图
	public static GameObject CreateMap(Object mapSource, Vector3 worldPos = default(Vector3), Vector2 mapGridSize = default(Vector2) ,Color GridGolor = default(Color)  )
	{
		if(mapGridSize == default(Vector2)) 
			mapGridSize = new Vector2(72f,40f);
		if(GridGolor == default(Color)) 
			GridGolor.a = 1f;

		GameObject map = PrefabUtility.InstantiatePrefab(mapSource) as GameObject;
		SceneEditorSettings.currMap = map;
		
		map.transform.localPosition = worldPos;
		map.transform.localScale = Vector3.one;

		UISceneMap uimap = map.AddComponent<UISceneMap>();

		uimap.mapSettingData.sourcePath = AssetDatabase.GetAssetPath(mapSource);
		uimap.mapSettingData.Type = SceneElementType.MAP;
		uimap.axisColors = GridGolor;
		uimap.renderFrom = new Vector3( -mapGridSize.x, -mapGridSize.y, 0);
		uimap.renderTo = new Vector3(mapGridSize.x, mapGridSize.y, 0);
		uimap.InitData();
		return map;
	}


	//创建种植物
	public static GameObject CreatePlant(string sourcePath,Vector3 worldpos, Vector3 scale ,SceneElementType type,   bool BornAtMap = true ,Vector3 worldrotation = default(Vector3))
	{
		Object plantSource = AssetDatabase.LoadMainAssetAtPath(sourcePath);
		if(plantSource != null)
		   return CreatePlant(plantSource , worldpos , scale, type , BornAtMap , worldrotation);
		EditorUtility.DisplayDialog(EditorStringConfig.getString(10042),"Not found: "+sourcePath,EditorStringConfig.getString(10025));
		return null;
	}

	//创建种植物
	public static GameObject CreatePlant(Object resource, Vector3 worldpos, Vector3 scale, SceneElementType type , bool BornAtMap = true ,Vector3 worldrotation = default(Vector3))
	{
		GameObject createObj = PrefabUtility.InstantiatePrefab(resource) as GameObject;
		UISceneMap uiSceneMap = SceneEditorSettings.currMap.GetComponent<UISceneMap>();
		int TypeIndex = (int)type;
		if(uiSceneMap != null)
		{
			createObj.transform.parent = uiSceneMap.ElementParents[TypeIndex - 1];
			createObj.transform.localScale = scale;
			createObj.transform.rotation = Quaternion.Euler(worldrotation);
			createObj.transform.position = worldpos;
		}

		switch(type)
		{
			case SceneElementType.NPC:
			{
			    UINPC npc = null;
				if(createObj != null)   
				npc = createObj.AddComponent<UINPC>();
			    if(npc != null)
				{
					//保存数据
				    npc.npcSettingData.isAtMap = BornAtMap;
					npc.npcSettingData.Type = type;
				    npc.npcSettingData.sourcePath = AssetDatabase.GetAssetPath(resource);

				    CharacterController controller = npc.GetComponent<CharacterController>();					
					if(controller != null)
					{
					    CapsuleCollider  capsule = npc.gameObject.AddComponent<CapsuleCollider>();
						if(capsule != null)
						{
							capsule.center = controller.center;
							capsule.radius = controller.radius;
							capsule.height = controller.height;
							capsule.direction = 1;
						}
					}		
				}
			    break;
			}
			case SceneElementType.BUILDING:
			{
				break;
			}
			case SceneElementType.TREE:
			{
				break;
			}
		    default:
			{
				UISpecialArea area = createObj.GetComponent<UISpecialArea>();
				if( area == null )
					area = createObj.AddComponent<UISpecialArea>(); 
			    //保存数据
			    area.areaSettingData.isAtMap = BornAtMap;
				area.areaSettingData.Type = type;
				area.areaSettingData.sourcePath = AssetDatabase.GetAssetPath(resource);
			    break;
			}
		}
		Selection.activeObject = createObj;
		return createObj;
	}

	//自动生成游戏数据
	public static void AutoGenerateGameData()
	{
		if( SceneEditorSettings.currMap == null )
		{
			EditorUtility.DisplayDialog("友情提示","当前是场景为空,请先创建场景","确定");
			return;
		}
		//获取地图
		UISceneMap mapData = SceneEditorSettings.currMap.GetComponent<UISceneMap>();
		//获取NPC
		UINPC[] npcDatas = SceneEditorSettings.currMap.GetComponentsInChildren<UINPC>();
		//获取碰撞盒子
		UISpecialArea[] areaData = SceneEditorSettings.currMap.GetComponentsInChildren<UISpecialArea>();

		string path = EditorUtility.SaveFilePanel("导出配置文件",Application.dataPath+"/StreamingAssets/Config" , "SceneEditorData","cfg");
		if( string.IsNullOrEmpty(path) ) return;

		try
		{
			FileStream aFile = new FileStream(path, FileMode.Create);
			StreamWriter sw = new StreamWriter(aFile);
			//写入地图
			mapData.UpdateData();
			sw.WriteLine( "M"+fastJSON.JSON.Instance.ToJSON(mapData.mapData));

			//写入NPC
			foreach(UINPC data  in  npcDatas)
			{
				data.UpdateData();
				sw.WriteLine( "N"+fastJSON.JSON.Instance.ToJSON(data.npcData));
			}

			//写入碰撞盒子
			foreach(UISpecialArea data in areaData)
			{
				data.UpdateData();
				sw.WriteLine( "C"+fastJSON.JSON.Instance.ToJSON(data.areaData) );
			}
			sw.Close();
		}
		catch (IOException ex)
		{
			EditorUtility.DisplayDialog("错误",ex.ToString(),"确定");
		    
		}

	}

	//自动生成配置文件
	public static void AutoGenerateSettingFile()
	{
		string path = EditorUtility.SaveFilePanel("导出配置文件",Application.dataPath+"/Editor/SceneEditor/JC" , "SceneSetting","bytes");
		if( string.IsNullOrEmpty(path) ) return;
		UISceneElementBase[] allData = SceneEditorSettings.currMap.GetComponentsInChildren<UISceneElementBase>();
		string[] contents = GetEeditorSettingWriteString(allData);
		try
		{
			FileStream aFile = new FileStream(path, FileMode.Create);
			StreamWriter sw = new StreamWriter(aFile);			
			foreach(string str in contents)
				sw.WriteLine(str);
			sw.Close();
		}
		catch (IOException ex)
		{
			EditorUtility.DisplayDialog("错误",ex.ToString(),"确定");			
		}
	}

	public static string[] GetEeditorSettingWriteString(UISceneElementBase[] allData)
	{
		List<string> result = new List<string>();
		foreach(UISceneElementBase uiBase in allData)
		{
			if(uiBase != null)
				uiBase.UpdateData();
			if(uiBase.settingData != null) 
			{
				switch( uiBase.settingData.Type )
				{
					case  SceneElementType.MAP:
					{
						MapSettingData setting = (MapSettingData)uiBase.settingData;					
					    result.Add( "M"+fastJSON.JSON.Instance.ToJSON(setting) );
						break;
					}
					case SceneElementType.NPC:
					{
					    NPCSettingData setting = (NPCSettingData)uiBase.settingData;
						result.Add( "N"+fastJSON.JSON.Instance.ToJSON(setting) );
						break;
					}
					default:
					{
						SpecialAreaSettingData setting = (SpecialAreaSettingData)uiBase.settingData;
					    result.Add( "C"+fastJSON.JSON.Instance.ToJSON(setting) );
						break;
					}
				}
			}
		}
		return result.ToArray();
	}





	//自动导入配置文件
	public static void AutoImportSettingFile()
	{
		string path = EditorUtility.OpenFilePanel("导入场景配置文件",Application.dataPath+"/Editor/SceneEditor/JC","bytes");
		if( string.IsNullOrEmpty(path) ) return;

		List<SceneEditorSettingBaseData> settingDataList = new List<SceneEditorSettingBaseData>();
		string strLine = null;
		try
		{
			FileStream aFile = new FileStream(path,FileMode.Open);
			StreamReader sr = new StreamReader(aFile);
			strLine = sr.ReadLine();
			while(strLine != null)
			{
				if(strLine.Length > 0)
				{
					SceneEditorSettingBaseData settingData = null;
					string content = strLine.Substring(1);
					switch( strLine[0] )
					{
						case 'M':
						{
						    MapSettingData mapData = fastJSON.JSON.Instance.ToObject<MapSettingData>(content);
						    settingData = (SceneEditorSettingBaseData)mapData;
							break;
						}
						case 'N':
						{
						    NPCSettingData npcData = fastJSON.JSON.Instance.ToObject<NPCSettingData>(content);
						    settingData = (SceneEditorSettingBaseData)npcData;
							break;
						}
						case 'C':
						{
						    SpecialAreaSettingData areaData = fastJSON.JSON.Instance.ToObject<SpecialAreaSettingData>(content);
						    settingData = (SceneEditorSettingBaseData)areaData;
							break;
						}
					}
					if(settingData != null)
						settingDataList.Add(settingData);
				}
				strLine = sr.ReadLine();
			}
			sr.Close();
		}
		catch (IOException ex)
		{
			EditorUtility.DisplayDialog("错误",ex.ToString(),"确定");
			return;
		}

		//Debug.LogError("settingDataList.count="+settingDataList.Count.ToString() );

		//生成场景
		foreach(SceneEditorSettingBaseData baseData in settingDataList)
		{
			GameObject create = null;
			if(baseData.Type == SceneElementType.MAP)
			{
				MapSettingData data = (MapSettingData)baseData;
				create = CreateMap(data.sourcePath,new Vector3(data.Attribute.pos[0],data.Attribute.pos[1],data.Attribute.pos[2]),
				          new Vector2(data.GridSize[0],data.GridSize[1]),new Color(data.GridColor[0],data.GridColor[1],data.GridColor[2],data.GridColor[3] ) );
				if(create != null)
				{
					UISceneMap uimap = create.GetComponent<UISceneMap>();
					if(uimap != null)
					{
						uimap.settingData = data;
						uimap.mapData = data.Attribute;
						uimap.InitData();
					}
				}
				else
					break;
			}
			else if(baseData.Type == SceneElementType.NPC)
			{
				NPCSettingData data = (NPCSettingData)baseData;
				create = CreatePlant(data.sourcePath,new Vector3(data.Attribute.pos[0],data.Attribute.pos[1],data.Attribute.pos[2]),new Vector3(data.Attribute.scale[0],data.Attribute.scale[1],data.Attribute.scale[2]),
				            data.Type, data.isAtMap ,new Vector3(data.Attribute.rotation[0],data.Attribute.rotation[1],data.Attribute.rotation[2]) );
				if(create != null)
				{
					//Debug.LogError(data.Attribute.startIntervalTime.ToString());
					UINPC uinpc = create.GetComponent<UINPC>();
					if(uinpc != null)
					{
						uinpc.settingData = data;
						uinpc.npcData = data.Attribute;
						uinpc.InitData();
					}
				}
			}
			else
			{
				SpecialAreaSettingData data = (SpecialAreaSettingData)baseData;
				create = CreatePlant(data.sourcePath,new Vector3(data.Attribute.pos[0],data.Attribute.pos[1],data.Attribute.pos[2]),new Vector3(data.Attribute.scale[0],data.Attribute.scale[1],data.Attribute.scale[2]),
				            data.Type, data.isAtMap ,new Vector3(data.Attribute.rotation[0],data.Attribute.rotation[1],data.Attribute.rotation[2]) );
				UISpecialArea uiarea = create.GetComponent<UISpecialArea>();
				if(uiarea != null)
				{
					uiarea.settingData = data;
					uiarea.areaData = data.Attribute;
					uiarea.InitData();
				}

			}
		}
	}

	//自动生成代码文件(编译器枚举   SceneEditorEnum.cs)
	public static void AutoGenerateEnumCode(string outClass, params string[] inClass)
	{
		if(inClass == null || inClass.Length == 0)
			return;
	
		string path = EditorUtility.SaveFilePanel("导出配置文件",Application.dataPath+"/Scripts/SceneEditor" , "SceneEditorEnum","cs");
		if( string.IsNullOrEmpty(path) ) return;
		try
		{
			FileStream aFile = new FileStream(path, FileMode.Create);
			StreamWriter sw = new StreamWriter(aFile);			

			sw.Write("using UnityEngine;\nusing System.Collections;\n\n");
			if(!string.IsNullOrEmpty(outClass))
			sw.Write(outClass);
			sw.Write("public class SceneEditorEnum\n{\n");
			//写入代码
			foreach(string data  in  inClass)
			{
				sw.Write(data);
			}
			sw.Write("}");
			sw.Close();
		}
		catch (IOException ex)
		{
			EditorUtility.DisplayDialog("错误",ex.ToString(),"确定");			
		}
	}

}
