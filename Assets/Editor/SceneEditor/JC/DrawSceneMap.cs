using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(UISceneMap))] 
//绘画出场景元素的特性
public class DrawSceneMap : DrawGridEditor 
{
	void OnEnable()
	{

	}

	public UISceneMap uiMap;


	//bool isfllowCamera = false; 

	public  void OnSceneGUI()
	{
		base.OnSceneGUI();
		if(uiMap == null)
			uiMap = (UISceneMap) target;

		Handles.BeginGUI();

		//规定GUI显示区域
		GUILayout.BeginArea(new Rect(100, 70, 200, 100));

		/*正在编辑*/
		GUILayout.Label(EditorStringConfig.getString(10030)+":"+uiMap.gameObject.name);	

		//GFGrid gfGrid = SceneEditorSettings.mapGrid;

		/*显示网格*/
		uiMap.isShowMap = GUILayout.Toggle(uiMap.isShowMap,EditorStringConfig.getString(10005));

		/*允许复活*/
		uiMap.mapData.canReborn = System.Convert.ToInt32(GUILayout.Toggle(System.Convert.ToBoolean(uiMap.mapData.canReborn),EditorStringConfig.getString(10039)  ));

		GUILayout.EndArea();
		Handles.EndGUI();
	}


	public override void OnInspectorGUI () 
	{
		if(uiMap == null)
			uiMap = (UISceneMap) target;

		uiMap.mapData.canReborn = System.Convert.ToInt32(GUILayout.Toggle(System.Convert.ToBoolean(uiMap.mapData.canReborn),"canReborn") );


		if(GUI.changed)
			SceneView.RepaintAll();

	}


	//鼠标按下true,弹起是false
//	public override void OnPressAtDrawScene(bool isPressed)
//	{
//	}

}
