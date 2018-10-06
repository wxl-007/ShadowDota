using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(UISceneElement))] 
//绘画出场景元素的特性
public class DrawSceneElement :DrawGridEditor 
{
	public UISceneElement element;


	public override void OnInspectorGUI () 
	{
		base.OnInspectorGUI();
	}


	//使种值元素跟随鼠标移动
	bool MoveFllowMouse = false;

	public  void OnSceneGUI()
	{

		if( element == null)
			element = (UISceneElement) target;

		if( element == null || !element.enabled) return;

		if(element.isDeleteGameObject)
		{
			//销毁
			EditorMemoryRecycle.RemoveMemory(element.gameObject);
			element.isDeleteGameObject = false;
		}


		base.OnSceneGUI();

		//反算屏幕坐标
		if(MoveFllowMouse)
		{
			//Debug.LogError("[MoveFllowMouse]"+MoveFllowMouse.ToString());
			Vector3 mypos = element.transform.position;
			if(element.settingData.Type == SceneElementType.NPC)
				mypos.y = 0;
			else
				mypos.y = element.transform.localScale.y/2f;

			element.transform.position = mypos;
			Vector3 ScenePos = SceneCamera.WorldToScreenPoint( element.transform.position );
			Vector3 mousePosition = Event.current.mousePosition;
			float  height = SceneView.currentDrawingSceneView.position.height;
			mousePosition.y = height - mousePosition.y - 15f;
			mousePosition.z = ScenePos.z;
			Vector3 resultPos = SceneCamera.ScreenToWorldPoint(mousePosition) ;
			element.transform.position = resultPos;
		}


		//绘制文本框
		Handles.Label(element.transform.position + Vector3.up*3,
		              element.transform.name +" : "+ element.transform.position.ToString() );

		//开始绘制GUI
		Handles.BeginGUI();

		//规定GUI显示区域
		GUILayout.BeginArea(new Rect(100, 100, 200, 400));

		//GUI绘制文本框
		/*正在编辑*/
		GUILayout.Label(EditorStringConfig.getString(10030)+":"+element.gameObject.name);	


		/*吸附到地图*/
		element.isAtMap = GUILayout.Toggle(element.isAtMap,EditorStringConfig.getString(10031));
//		/*鼠标跟随*/
//		element.isSeletctedFllowMouse = GUILayout.Toggle(element.isSeletctedFllowMouse,EditorStringConfig.getString(10032));
//
//		if(element.isSeletctedFllowMouse) element.isAtMap = true;

		//自动吸附到地图
		if(element.isAtMap)
		{
			element.AdsorptionToMap();
		}
		else
		{
			/*种植*/
			if(GUILayout.Button(EditorStringConfig.getString(10034),GUILayout.Width(100),GUILayout.Height(20)))
			{
				element.CorrectionPostion();
			}
		}

		/*克隆*/
		if(GUILayout.Button(EditorStringConfig.getString(10035),GUILayout.Width(100),GUILayout.Height(20) ))
		{
			Object prefab = EditorUtility.GetPrefabParent(Selection.activeObject);
			GameObject cloneSource = Selection.activeObject as GameObject;
			GameObject cloneObject = null;
			if (prefab)
			{
				Vector3 oldpos = element.gameObject.transform.position;
				cloneObject = SceneEditorTools.CreatePlant(prefab,new Vector3(oldpos.x+3f+element.transform.localScale.x , oldpos.y ,oldpos.z),element.transform.localScale,element.settingData.Type,element.isAtMap,element.transform.rotation.eulerAngles);
			}
			//克隆代码
			element.CloneScript(cloneSource,cloneObject);
		}

		/*销毁*/
		if(GUILayout.Button(EditorStringConfig.getString(10033),GUILayout.Width(100),GUILayout.Height(20) ))
		{
			element.isDeleteGameObject = true;
		}
		//GUILayout.BeginHorizontal();	
		element.isSetTransform = GUILayout.Toggle(element.isSetTransform,"Transform(World)",EditorStyles.foldout);
		//GUILayout.EndHorizontal();
		Rect lastRect = GUILayoutUtility.GetLastRect();
		if(element.isSetTransform)
		{
			++EditorGUI.indentLevel;
				GUILayout.BeginHorizontal();		
				    if( GUI.Button(new Rect(16,lastRect.yMax,20,18),"P") )
				        element.transform.position = Vector3.zero;
				    element.transform.position = EditorGUI.Vector3Field(new Rect(20,lastRect.yMax+2,150,20),"",element.transform.position);
			    GUILayout.EndHorizontal();

				lastRect.yMax += 20f;
				GUILayout.BeginHorizontal();		
					if( GUI.Button(new Rect(16,lastRect.yMax,20,18),"R") )
					     element.transform.rotation = Quaternion.Euler(Vector3.zero);
				    element.transform.rotation = Quaternion.Euler( EditorGUI.Vector3Field(new Rect(20,lastRect.yMax+2,150,20),"",element.transform.rotation.eulerAngles)  );
				GUILayout.EndHorizontal();

			    lastRect.yMax += 20f;
				GUILayout.BeginHorizontal();		
					if(GUI.Button(new Rect(16,lastRect.yMax,20,18),"S"))
				        element.transform.localScale = Vector3.one;
				    element.transform.localScale = EditorGUI.Vector3Field(new Rect(20,lastRect.yMax+2,150,20),"",element.transform.localScale);
				GUILayout.EndHorizontal();

			--EditorGUI.indentLevel;
		}
		GUILayout.EndArea();

		Handles.EndGUI();

	}


	//鼠标按下true,弹起是false
	public override void OnPressAtDrawScene(bool isPressed)
	{
		if(SceneEditorSettings.isOpenFllowMouse)
		{
			//鼠标弹起
			if(!isPressed)
			{
				if(isMouseTirggerGameObject)
				{
					MoveFllowMouse = element.isSeletctedFllowMouse;
					element.isSeletctedFllowMouse = !element.isSeletctedFllowMouse;
					if(!MoveFllowMouse)
						element.CorrectionPostion();		     	
				}
			}
		}
	}

	//鼠标是否触碰到物体
	bool isMouseTirggerGameObject
	{
		get
		{
			if(MoveFllowMouse) 
				return true;
			Vector3 mousePosition = Event.current.mousePosition;
			float  height = SceneView.currentDrawingSceneView.position.height;
			mousePosition.y = height - mousePosition.y - 15f;
			Ray ray = SceneCamera.ScreenPointToRay(mousePosition);
			RaycastHit hit;
			Physics.Raycast(ray,out hit);		
			if(hit.transform != null && hit.transform.name != "Grid" && hit.transform.gameObject == element.gameObject)
				return true;
			return false;
		}
	}

	public override void OnKeyBoardDown(KeyCode keyCode)
	{
		if(keyCode == KeyCode.A)
		{
			element.isSeletctedFllowMouse = !element.isSeletctedFllowMouse;
			Tools.current  = element.isSeletctedFllowMouse ? Tool.View : Tool.Move;
			SceneView.RepaintAll();
		}
	}


}
