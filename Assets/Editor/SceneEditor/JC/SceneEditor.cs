using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using fastJSON;
using System.IO;

public class SceneEditor : EditorWindow 
{

	public static SceneEditor _this;
	[MenuItem ("ShadowGame/SceneEditor %r")]
	static void AddWindow ()
	{       
		//创建窗口
		//_this = (SceneEditor)EditorWindow.GetWindowWithRect (typeof (SceneEditor),wr,false,EditorStringConfig.getString(10028));	
		_this = EditorWindow.GetWindow<SceneEditor>(EditorStringConfig.getString(10028),true);
		_this.Show();
	}


	
	public void Awake () 
	{

	}

	static bool isOpenSceneCamera = false;
	static void OnSceneDraw(SceneView sceneView)
	{
		if(Event.current.isKey)
		{
			if(Event.current.type == EventType.KeyDown)
			{
				if(Event.current.keyCode == KeyCode.A)
				{
					SceneEditorSettings.isOpenFllowMouse =!SceneEditorSettings.isOpenFllowMouse;
				}
			}
		}
		if(Event.current.isMouse)
		{
			if(Event.current.type == EventType.MouseDown)
			{
				/*拖拽鼠标具备选中功能*/
				if(Tools.current == Tool.View)
				{
					Vector3 mousePosition = Event.current.mousePosition;
					float  height = SceneView.currentDrawingSceneView.position.height;
					mousePosition.y = height - mousePosition.y - 15f;
					Ray ray =  sceneView.camera.ScreenPointToRay(mousePosition);
					RaycastHit hit;
					Physics.Raycast(ray,out hit);		
					if(hit.transform != null && hit.transform.name != "Grid")
					{
						GameObject OldSelection = Selection.activeObject as GameObject;
						if(OldSelection != hit.transform.gameObject)
						{
			                UISceneElementBase _old = null;
							if(OldSelection != null)
							   _old = OldSelection.GetComponent<UISceneElementBase>();
							if(_old != null)
								_old.OnLostFocus();
							UISceneElementBase _new = hit.transform.gameObject.GetComponent<UISceneElementBase>();
							if(_new != null)
								_new.OnFocus();
							Selection.activeObject = hit.transform.gameObject;
						}
					}
				}
			}
		}

		//开始绘制GUI
		Handles.BeginGUI();

		GUILayout.BeginArea(new Rect(0,sceneView.position.height-70,200,200));
		string currMapName = "";
		if(SceneEditorSettings.currMap != null)
			currMapName = SceneEditorSettings.currMap.name;

		/*正在编辑*/
		GUILayout.Label(EditorStringConfig.getString(10030)+": "+currMapName);	

		#region 鼠标跟随
		/*是否打开了鼠标跟随功能*/
		bool isOpenFllowMouseOld = SceneEditorSettings.isOpenFllowMouse;
		SceneEditorSettings.isOpenFllowMouse = GUILayout.Toggle(SceneEditorSettings.isOpenFllowMouse,EditorStringConfig.getString(10032)+"("+EditorStringConfig.getString(10045)+":A)");
		/*从拖拽状态切出*/ 
		if(isOpenFllowMouseOld != SceneEditorSettings.isOpenFllowMouse && isOpenFllowMouseOld)
			Tools.current = Tool.Move;
		/*强制鼠标拖拽*/
		if(SceneEditorSettings.isOpenFllowMouse)Tools.current = Tool.View;
		#endregion

		if(SceneEditorSettings.currMap != null)
		{
			UISceneMap map = SceneEditorSettings.currMap.GetComponent<UISceneMap>();
			if(map != null)
			{
				if(map.EditorCamera != null)
				{
					//bool isOldOpen = isOpenSceneCamera;
					/*打开场景摄像机*/
					isOpenSceneCamera = GUILayout.Toggle(isOpenSceneCamera,EditorStringConfig.getString(10029));
					map.EditorCamera.enabled = isOpenSceneCamera;
					//map.EditorCamera.hideFlags = HideFlags.NotEditable;
					if(map.EditorCamera.isOrthoGraphic)map.EditorCamera.isOrthoGraphic = false;
					map.EditorCamera.transform.rotation = sceneView.camera.transform.rotation;
					map.EditorCamera.transform.position = sceneView.camera.transform.position;
				}
			}
		}


		GUILayout.EndArea();
		Handles.EndGUI();
	}



//	Color color = Color.white;

//	private List<string> popNames = new List<string>();
//	private Dictionary<string,Texture> dic_textrues = new Dictionary<string, Texture>();
//	int popIndex = 0;
	//绘制窗口时调用


	#region New
	private int currMapId = 0;

	//private Vector2 mapGridSize = new Vector2(72f,39f);
	//Vector2 gridPos2 = new Vector2(0,4f);
	#endregion



	#region 种植系统源
	private List<string> PlantSourceName = new List<string>();
	private List<Object> PlantSourceObject = new List<Object>();
	private int PlantSourceIndex = 0;
	#endregion


	
	void OnGUI () 
	{
//		NewDrawTest();
//		return;
		SetFontStyle(Color.gray,18,FontStyle.Italic,EditorStyles.label);
		GUILayout.BeginHorizontal();

		/*版本:1.0.1*/
		GUILayout.Label(EditorStringConfig.getString(10000),style);

		GUILayout.EndHorizontal();
		DrawLine();
		GUILayout.Space(15);


		GUILayout.BeginHorizontal();
		SetFontStyle(Color.gray,15,FontStyle.Italic);
		/*正在编辑地图:*/
		GUILayout.Label(EditorStringConfig.getString(10001),style);
		SceneEditorSettings.currMap = EditorGUILayout.ObjectField(SceneEditorSettings.currMap,typeof(GameObject),true) as GameObject;
		/*刷新数据*/
		if(GUILayout.Button(EditorStringConfig.getString(10002,10003),GUILayout.Width(60),GUILayout.Height(16)))
		{
			RefreshAllData();
		}
		GUILayout.EndHorizontal();

		GameObject currMap = SceneEditorSettings.currMap;
		bool isHaveMap = currMap == null;
		if(isHaveMap)
		{
			/*未检测到场景,你可以\n1.创建一个新场景. \n2.导入一个已经保存的场景配置表.\n3.从Hierarchy视图中拖入一个已有的场景*/
			EditorGUILayout.HelpBox(EditorStringConfig.getString(10021),MessageType.Error);
			ShowMapMenu();
		}
		else
		{
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			/*销毁地图*/
			if(GUILayout.Button(EditorStringConfig.getString(10022),GUILayout.Width(150),GUILayout.Height(40)))
			{
				/*"友情提示","你确定要销毁地图","确定","取消"*/
				bool result = EditorUtility.DisplayDialog(EditorStringConfig.getString(10023),EditorStringConfig.getString(10024)+currMap.name,EditorStringConfig.getString(10025),EditorStringConfig.getString(10026));
				if(result)
				{
					GameObject currMapObj = GameObject.Find(currMap.name);
					DestroyImmediate(currMapObj);
					currMapId = 0;
					SceneEditorSettings.currMap = null;
				}
			}

			/*载入配置表*/
			if(GUILayout.Button(EditorStringConfig.getString(10016),GUILayout.Width(150),GUILayout.Height(40)))
			{
				/*"友情提示","是否先保存当前场景?","确定","取消"*/
				bool result = EditorUtility.DisplayDialog(EditorStringConfig.getString(10023),EditorStringConfig.getString(10027),EditorStringConfig.getString(10025),EditorStringConfig.getString(10026));
				if(result)
				{
					SceneEditorTools.AutoGenerateSettingFile();
				}
				else
				{
					GameObject currMapObj = GameObject.Find(currMap.name);
					DestroyImmediate(currMapObj);
					SceneEditorSettings.currMap = null;
				}
				SceneEditorTools.AutoImportSettingFile();
			}
			GUILayout.EndHorizontal();

			//如果当前创建了地图
			if(currMap != null)
			{
				GUILayout.Space(10);

				DrawLine();

				SetFontStyle(Color.gray,12,FontStyle.Italic,EditorStyles.foldout);
				/*地图编辑*/
				SceneEditorSettings.isEditorMapGrid = EditorGUILayout.Foldout(SceneEditorSettings.isEditorMapGrid, EditorStringConfig.getString(10004),style);
				if(SceneEditorSettings.isEditorMapGrid)
				{
					++EditorGUI.indentLevel;
					GameObject map = SceneEditorSettings.currMap;
					UISceneMap uimap = map.GetComponent<UISceneMap>();
					if(uimap != null)
					{
						bool oldShowMap = uimap.isShowMap;
						/*显示网格*/
						uimap.isShowMap = EditorGUILayout.Toggle(EditorStringConfig.getString(10005),uimap.isShowMap );
						if(oldShowMap != uimap.isShowMap)
							SceneView.RepaintAll();

						float gridwidth = uimap.renderTo.x - uimap.renderFrom.x;
						float gridheight = uimap.renderTo.y - uimap.renderFrom.y;
						Vector2 gridSize = new Vector3(gridwidth,gridheight);
						Vector2 oldgridSize = gridSize;
						/*网格大小*/
						gridSize = EditorGUILayout.Vector2Field(EditorStringConfig.getString(10006), gridSize);
						uimap.renderFrom = new Vector3(-gridSize.x/2f, -gridSize.y/2f ,0);
						uimap.renderTo = new Vector3(gridSize.x/2f, gridSize.y/2f, 0);
						if(oldgridSize != gridSize)
							SceneView.RepaintAll();

						Color oldaxisColors = uimap.axisColors;
						/*网络颜色*/
						uimap.axisColors = EditorGUILayout.ColorField(EditorStringConfig.getString(10007),uimap.axisColors);
						if(oldaxisColors != uimap.axisColors)
							SceneView.RepaintAll();

					}

					--EditorGUI.indentLevel;
				}



				//显示网络编辑模块
				GUILayout.Space(10);
				DrawLine();

				string[] FarmingType = System.Enum.GetNames( typeof(SceneElementType) );
				/*请选择*/
				FarmingType[0] = EditorStringConfig.getString(10008);
		
				SetFontStyle(Color.gray,12,FontStyle.Italic,EditorStyles.foldout);
				/*地图种怪编辑*/
				SceneEditorSettings.isOpenFarming = EditorGUILayout.Foldout(SceneEditorSettings.isOpenFarming, EditorStringConfig.getString(10009) ,style );

	
				if(SceneEditorSettings.isOpenFarming)
				{
					++EditorGUI.indentLevel;
					EditorGUILayout.BeginHorizontal();
					/*种植类型*/
					SceneEditorSettings.farmingTypeIndex = EditorGUILayout.Popup(EditorStringConfig.getString(10010),SceneEditorSettings.farmingTypeIndex,FarmingType );
					/*编辑*/
					if(GUILayout.Button(EditorStringConfig.getString(10011),GUILayout.Width(50),GUILayout.Height(16)))
					{
						SceneEditorGridWindow window = (SceneEditorGridWindow) EditorWindow.GetWindow(typeof(SceneEditorGridWindow));
						window.Init();
					}
					EditorGUILayout.EndHorizontal();

					GUILayout.Space(5);
					//如果用户选择了一种类型
					if(SceneEditorSettings.farmingTypeIndex > 0)
					{
						Object PlantSource = SceneEditorSettings.GetPlantFileObject((SceneElementType)SceneEditorSettings.farmingTypeIndex);
						EditorGUILayout.BeginHorizontal();
						/*资源来源*/
						Object currPlantSource = EditorGUILayout.ObjectField(EditorStringConfig.getString(10012),PlantSource, typeof(Object));
						//资源文件夹是否发生了改变
						bool isChanged = PlantSource != currPlantSource;

						SceneEditorSettings.SetPlantFileObject((SceneElementType)SceneEditorSettings.farmingTypeIndex,currPlantSource);

						/*刷新*/
						if(GUILayout.Button(EditorStringConfig.getString(10002),GUILayout.Width(50),GUILayout.Height(16)))
						{
							if(currPlantSource != null)
							GetGameObjectsNameInFolder(currPlantSource);
						}
						EditorGUILayout.EndHorizontal();

						if(PlantSource != null)
						{
							if(isChanged)
							{
								GetGameObjectsNameInFolder(currPlantSource);
							}

							GUILayout.Space(5);
							PlantSourceIndex = EditorGUILayout.Popup(FarmingType[SceneEditorSettings.farmingTypeIndex]+"名称:",PlantSourceIndex,PlantSourceName.ToArray());
						}
						GUILayout.Space(5);

						GUILayout.BeginHorizontal();
						GUILayout.Space(this.position.width - 100);

						/*更新地图分类*/
						if(GUILayout.Button(EditorStringConfig.getString(10013),GUILayout.Width(100),GUILayout.Height(40)))
						{
							SceneEditorSettings.currMap.GetComponent<UISceneMap>().RefreshClassification();
						}
						GUILayout.EndHorizontal();

						GUILayout.Space(5);
						ShowZhongGuai(FarmingType);
					}
				
					--EditorGUI.indentLevel;
				}

			}
		}

		GUILayout.Space(5);
		DrawLine();
   }

	//获取文件下面所有的GameObject名称
	void GetGameObjectsNameInFolder(Object currPlantSource)
	{
		PlantSourceName.Clear();
		PlantSourceObject.Clear();
		PlantSourceIndex = 0;
		Object oldSelect = Selection.activeObject;
		Selection.activeObject = currPlantSource;
		Dictionary<string,Object> temp_dic = new Dictionary<string, Object>();
		foreach(Object o in Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets))
		{
			if(o is GameObject)
			{
				//PlantSourceObject.Add(o);
				if(!temp_dic.ContainsKey(o.name))
				{
					temp_dic.Add(o.name,o);
					PlantSourceName.Add(o.name);
				}
			}
		}
		Selection.activeObject = oldSelect;
		PlantSourceName.Sort();
		foreach(string oname in PlantSourceName)
		{
			PlantSourceObject.Add( temp_dic[oname] );
		}
	}

	//刷新地图资源
	void RefreshMapResources()
	{
		mapNames.Clear();
		mapObject.Clear();
		Object oldSelection = Selection.activeObject;
		Selection.activeObject = SceneEditorSettings.SceneSourceFileObject;
		foreach(Object o in Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets))
		{
			if( o.name!= null && o.name.Length > 0 && o is GameObject)
			{
				mapNames.Add(o.name);
				mapObject.Add(o);
			}
		}
		Selection.activeObject = oldSelection;
	}



	//显示地图菜单
	private List<string> mapNames = new List<string>();
	private List<Object> mapObject = new List<Object>();

	void ShowMapMenu()
	{
		if(SceneEditorSettings.SceneSourceFileObject == null)
			SceneEditorSettings.SceneSourceFileObject = AssetDatabase.LoadMainAssetAtPath("Assets/Resources/UnPack/Scenes");

		/*资源来源*/
		Object resultObject = EditorGUILayout.ObjectField(EditorStringConfig.getString(10012),SceneEditorSettings.SceneSourceFileObject, typeof(Object));

		bool isDifferentFromOld = resultObject != SceneEditorSettings.SceneSourceFileObject;
		SceneEditorSettings.SceneSourceFileObject = resultObject;

		if(SceneEditorSettings.SceneSourceFileObject != null )
		{
			if(isDifferentFromOld)
			{
				RefreshMapResources();
			}

			GUILayout.BeginHorizontal();

			/*地图名称*/
			currMapId = EditorGUILayout.Popup(EditorStringConfig.getString(10014),currMapId, mapNames.ToArray());
			/*刷新*/
			if(GUILayout.Button(EditorStringConfig.getString(10002),GUILayout.Width(50),GUILayout.Height(16)))
			{
				RefreshMapResources();
			}
			GUILayout.EndHorizontal();


			GUILayout.BeginHorizontal();
			/*创建*/
			if(GUILayout.Button(EditorStringConfig.getString(10015),GUILayout.Width(150),GUILayout.Height(40)))
			{
				SceneEditorTools.CreateMap(mapObject[currMapId] , Vector3.zero );
			}

			/*载入配置表*/
			if(GUILayout.Button(EditorStringConfig.getString(10016),GUILayout.Width(150),GUILayout.Height(40)))
			{
				SceneEditorTools.AutoImportSettingFile();
			}
			GUILayout.EndHorizontal();
		}
	}


	bool BornAtMap = true;
	void ShowZhongGuai(string[] FarmingType)
	{
		/*出生到地图上(锁定Y轴)*/
		BornAtMap = EditorGUILayout.Toggle(EditorStringConfig.getString(10017),BornAtMap);
		if(BornAtMap)
		{
			Vector2 bornPos = new Vector2(SceneEditorSettings.BronPostionX,SceneEditorSettings.BronPostionZ);
			/*出生坐标*/
			bornPos = EditorGUILayout.Vector2Field(EditorStringConfig.getString(10018), bornPos);
			SceneEditorSettings.BronPostionX = (int)bornPos.x;
			SceneEditorSettings.BronPostionZ = (int)bornPos.y;
		}
		else
		{
			Vector3 bornPos = new Vector3(SceneEditorSettings.BronPostionX,SceneEditorSettings.BronPostionY,SceneEditorSettings.BronPostionZ);
			/*出生坐标*/
			bornPos = EditorGUILayout.Vector3Field(EditorStringConfig.getString(10018),bornPos);
			SceneEditorSettings.BronPostionX = (int)bornPos.x;
			SceneEditorSettings.BronPostionY = (int)bornPos.y;
			SceneEditorSettings.BronPostionZ = (int)bornPos.z;
		}
			
		GUILayout.Space(20f);
		GUILayout.BeginHorizontal();
		/*创建*/
		if(GUILayout.Button(EditorStringConfig.getString(10015)+FarmingType[SceneEditorSettings.farmingTypeIndex],GUILayout.Width(150),GUILayout.Height(40)))
		{
			SceneEditorTools.CreatePlant(PlantSourceObject[PlantSourceIndex] , 
			                             new Vector3(SceneEditorSettings.BronPostionX,SceneEditorSettings.BronPostionY,SceneEditorSettings.BronPostionZ) ,
			                             Vector3.one,
			                             (SceneElementType)SceneEditorSettings.farmingTypeIndex,
			                             BornAtMap);
		}
		/*保存配置表*/
		if(GUILayout.Button(EditorStringConfig.getString(10019),GUILayout.Width(150),GUILayout.Height(40)))
		{
			SceneEditorTools.AutoGenerateSettingFile();
		}
		/*导出游戏数据*/
		if(GUILayout.Button(EditorStringConfig.getString(10020),GUILayout.Width(150),GUILayout.Height(40)))
		{
			SceneEditorTools.AutoGenerateGameData();
		}
		GUILayout.EndHorizontal();
	}


	void OnFocus()
	{
		bool isAlreadyRegistered = false;
		if(SceneView.onSceneGUIDelegate != null)
		{
			System.Delegate[] dele = SceneView.onSceneGUIDelegate.GetInvocationList();
			foreach(System.Delegate d in dele)
			{
				if(d.Method.ToString().Contains("OnSceneDraw"))
				{
					isAlreadyRegistered = true;
					break;
				}
			}
		}
		if(!isAlreadyRegistered)
			SceneView.onSceneGUIDelegate += OnSceneDraw;
	}



	void OnLostFocus()
	{
		//Debug.Log("当窗口丢失焦点时调用一次");
	}
	
	void OnHierarchyChange()
	{
		//Debug.Log("当Hierarchy视图中的任何对象发生改变时调用一次");
	}
	
	void OnProjectChange()
	{
		//Debug.Log("当Project视图中的资源发生改变时调用一次");
	}
	
	void OnInspectorUpdate()
	{
		//Debug.Log("窗口面板的更新");
		//这里开启窗口的重绘，不然窗口信息不会刷新
		this.Repaint();
	}
	
	void OnSelectionChange()
	{
		//当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
//		foreach(Transform t in Selection.transforms)
//		{
//			//有可能是多选，这里开启一个循环打印选中游戏对象的名称
//			Debug.Log("Hierarchy视图选择了:" + t.name);
//		}
	}


	void OnDestroy()
	{
		//GameObject.CreatePrimitive(PrimitiveType.Cube);
		//Debug.Log("当窗口关闭时调用");
	}

	//刷新全部数据
	void RefreshAllData()
	{
		UISceneElementBase[] baseDatas = SceneEditorSettings.currMap.GetComponentsInChildren<UISceneElementBase>();
		foreach(UISceneElementBase data in baseDatas)
			data.UpdateData();
	}



	void DrawLine(float height = 1)
	{
		//GUI.color = Color.grey;
		GUI.color = new Color(72f/255f,72f/255f,72f/255f,1f);
		Rect lastRect = GUILayoutUtility.GetLastRect();
		Rect temp = lastRect;
		temp.y = lastRect.yMax;
		temp.width = this.position.width;
		temp.height = height;
		GUI.DrawTexture(temp, EditorGUIUtility.whiteTexture);
		GUI.color = Color.white;
	}

	GUIStyle style = new GUIStyle();
	Color foldTextColor = Color.grey;
	void SetFontStyle(Color fontColor,int fontSize,FontStyle fontstyle = FontStyle.Normal,GUIStyle other = null)
	{
		if(other != null)
			style = new GUIStyle(other);

		foldTextColor = fontColor;
		style.fontSize = fontSize;
		style.fontStyle = fontstyle;
		style.onActive.textColor = foldTextColor;
		style.onFocused.textColor = foldTextColor;
		style.onHover.textColor = foldTextColor;
		style.onNormal.textColor = foldTextColor;
		style.active.textColor = foldTextColor;
		style.focused.textColor = foldTextColor;
		style.hover.textColor = foldTextColor;
		style.normal.textColor = foldTextColor;
	}





	bool posGroupEnabled = false;

	void NewDrawTest()
	{
		//posGroupEnabled = GUILayout.Toggle(posGroupEnabled,"");
		EditorGUI.DrawRect(new Rect( 0,0,100,100 ) ,Color.black);
		EditorGUI.BeginDisabledGroup(posGroupEnabled);
		  GUILayout.Toggle(false,"");
		EditorGUI.EndDisabledGroup();
	}







}
