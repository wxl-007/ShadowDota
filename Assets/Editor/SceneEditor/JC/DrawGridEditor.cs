using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(UIRectGrid))]
public  class DrawGridEditor :  DrawBase
{
	protected UIRectGrid grid;
	protected bool showDrawSettings;



	void OnEnable () 
	{
		grid = target as UIRectGrid;
		showDrawSettings = EditorPrefs.HasKey("GFGridShowDraw") ? EditorPrefs.GetBool("GFGridShowDraw") : true;
	}
	
	void OnDisable()
	{
		EditorPrefs.SetBool("GFGridShowDraw", showDrawSettings);
	}

	public override void OnInspectorGUI () 
	{
		ColourFields();
		SpacingFields();
		DrawRenderFields();		
		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}

	protected void ColourFields () 
	{		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Axis Colors");		
		grid = target as UIRectGrid;
		grid.axisColors = EditorGUILayout.ColorField(grid.axisColors);
		EditorGUILayout.EndHorizontal();
	}
	
	protected void DrawRenderFields () 
	{
		grid.renderFrom = EditorGUILayout.Vector3Field("Render From", grid.renderFrom);
		grid.renderTo = EditorGUILayout.Vector3Field("Render To", grid.renderTo);
	}

	protected  void SpacingFields ()
	{
		UIRectGrid rGrid = (UIRectGrid)target;
		rGrid.spacing = EditorGUILayout.Vector3Field("Spacing", rGrid.spacing);
	}
}
