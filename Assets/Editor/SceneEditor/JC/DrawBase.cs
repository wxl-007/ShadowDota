using UnityEditor;
using UnityEngine;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(UISceneElementBase),true)] 
public  class DrawBase : Editor 
{
	public Camera SceneCamera;

	public  void OnSceneGUI() 
	{
		UISceneElementBase baseTarget = (UISceneElementBase)target;
		if(baseTarget == null || !baseTarget.enabled)
			return;


		baseTarget.UpdateData();

		SceneCamera = SceneView.currentDrawingSceneView.camera;
		//Vector3 CameraPos = SceneCamera.transform.position;

		if(Event.current.isMouse)
		{
			if(Event.current.type == EventType.MouseDown)
			{
				OnPressAtDrawScene(true);
			}
			else if(Event.current.type == EventType.MouseUp)
			{
				OnPressAtDrawScene(false);
			}
		}
		if(Event.current.isKey)
		{
			if(Event.current.type == EventType.KeyDown)
			{
				OnKeyBoardDown(Event.current.keyCode);
			}
		}

		Handles.BeginGUI();
		GUILayout.BeginArea(new Rect(0, 0, 300, 100));
		//场景摄像机参考
		GUILayout.Label("SceneCamera "+EditorStringConfig.getString(10037)+":"+SceneCamera.transform.position.ToString());	
		GUILayout.Label("SceneCamera "+EditorStringConfig.getString(10038)+":"+SceneCamera.transform.rotation.eulerAngles.ToString());	
		GUILayout.EndArea();
		Handles.EndGUI();
	}

	public virtual void OnPressAtDrawScene(bool isPressed){}
	
	public virtual void OnKeyBoardDown(KeyCode keyCode){}
	
}
