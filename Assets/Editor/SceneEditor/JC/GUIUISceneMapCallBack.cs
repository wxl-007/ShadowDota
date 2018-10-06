using UnityEngine;
using UnityEditor;
using System;

[InitializeOnLoad]
public class GUIUISceneMapCallBack{
	
	private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;
	private static Texture2D mIcon;
	private static Texture2D MIcon 
	{
		get 
		{
			if (GUIUISceneMapCallBack.mIcon==null)
			{
				GUIUISceneMapCallBack.mIcon = (Texture2D)Resources.Load( "M");
			}
			return GUIUISceneMapCallBack.mIcon;
		}
	}	

	private static Texture2D nIcon;
	private static Texture2D NIcon 
	{
		get 
		{
			if (GUIUISceneMapCallBack.nIcon==null)
			{
				GUIUISceneMapCallBack.nIcon = (Texture2D)Resources.Load( "N");
			}
			return GUIUISceneMapCallBack.nIcon;
		}
	}


	private static Texture2D cIcon;
	private static Texture2D CIcon 
	{
		get 
		{
			if (GUIUISceneMapCallBack.cIcon==null)
			{
				GUIUISceneMapCallBack.cIcon = (Texture2D)Resources.Load( "C");
			}
			return GUIUISceneMapCallBack.cIcon;
		}
	}


	static GUIUISceneMapCallBack()
	{
		EditorApplication.update += Update;
		GUIUISceneMapCallBack.hiearchyItemCallback = new EditorApplication.HierarchyWindowItemCallback(GUIUISceneMapCallBack.DrawHierarchyIcon);
		EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, GUIUISceneMapCallBack.hiearchyItemCallback);
	}
	
	private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
	{
		GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
		if(gameObject != null)
		{
			UISceneElementBase baseElement = gameObject.GetComponent<UISceneElementBase>();
			if (gameObject != null && baseElement != null && gameObject.name != "UIRectGrid")
			{
				Rect rect = new Rect(selectionRect.x + selectionRect.width - 16f, selectionRect.y, 16f, 16f);
				switch(baseElement.settingData.Type)
				{
					case  SceneElementType.MAP:
						GUI.DrawTexture( rect,GUIUISceneMapCallBack.MIcon);
						break;
					case SceneElementType.NPC:
						GUI.DrawTexture( rect,GUIUISceneMapCallBack.NIcon);
						break;
					default:
						GUI.DrawTexture( rect,GUIUISceneMapCallBack.CIcon);
						break;
				}
			}
		}

	}


	static void Update ()
	{
		if( Selection.activeObject != null && Selection.activeObject is GameObject )
		{
			GameObject temp = Selection.activeObject as GameObject;
			UINPC npc = null;
			while((npc = temp.GetComponent<UINPC>()) == null && temp.transform.parent != null)
			{
				temp = temp.transform.parent.gameObject;
			}
			if(temp.transform.parent != null && npc != null && npc.npcSettingData.isMergeResource)
			Selection.activeObject = temp;
		}
	}
		
}
