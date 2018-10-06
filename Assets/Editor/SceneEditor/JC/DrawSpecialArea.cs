using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UISpecialArea))] 
public class DrawSpecialArea :DrawSceneElement {

	private string SelectedName;
	private string ShowColliderName;
	UISpecialArea special = null;
	void Start () {

	}


	public  void OnSceneGUI()
	{
		if(special == null)
			special = (UISpecialArea)target;

		base.OnSceneGUI();


		//开始绘制GUI
		Handles.BeginGUI();

		GUILayout.BeginArea(new Rect(SceneView.currentDrawingSceneView.position.width -200 , 100, 200, 200));

		/*转换到面   转换到线*/
		SelectedName = !special.DrawWireOrSurface ? EditorStringConfig.getString(10040):EditorStringConfig.getString(10041);
		
		
		if(GUILayout.Button(SelectedName,GUILayout.Width(100),GUILayout.Height(20)))
		{
			special.DrawWireOrSurface = !special.DrawWireOrSurface;
		}
		
		GUILayout.EndArea();


		Handles.EndGUI();

	}
	
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();
		if(special == null)
			special = (UISpecialArea)target;
	}


//	//校正坐标
//	public override void CorrectionPostion()
//	{
//		if(special == null)
//			special = (UISpecialArea)target;
//
//		if(special != null)
//		{
//			special.transform.position = special.GetCorrectionPostion(test.transform.position);
//			special.isSeletctedFllowMouse = false;
//		}
//	}
//
//	//吸附到地图上
//	public override void AdsorptionToMap()
//	{
//		Vector3 curPos = special.transform.position;
//		curPos.y = special.transform.localScale.y / 2f;
//		special.transform.position = curPos;
//	}

}
