using UnityEngine;
using UnityEditor;
using AW.War;
using AW.Data;

[CanEditMultipleObjects]
[CustomEditor(typeof(UINPC))] 
public class DrawNPC: DrawSceneElement {

	private string SelectedName;
	private string ShowColliderName;

	private UINPC npc = null;

	void Start () 
	{
	
	}


	public override void OnInspectorGUI () 
	{
		serializedObject.Update();

		UINPC npc = (UINPC)target;
		//npcID;
		if(npc.npcData.npcID == 0) int.TryParse(npc.name,out npc.npcData.npcID); 
		npc.npcData.npcID = EditorGUILayout.IntField("npcID", npc.npcData.npcID) ;

		npc.npcData.CanThrough = EditorGUILayout.Toggle("CanThrough",npc.npcData.CanThrough);

		//CAMP
		//npc.npcData.camp = EditorGUILayout.Popup("Camp",npc.npcData.camp, System.Enum.GetNames( typeof(CAMP)  ));
		GUILayout.BeginHorizontal();
		GUILayout.Label("CAMP");
		string[] EnumNames = System.Enum.GetNames( typeof(CAMP));
		if(EnumNames.Length > 0)
		{
			string campStr = ((CAMP)npc.npcData.camp ).ToString();
			int strIndex = GetArrayIndex<string>(ref EnumNames,ref campStr );
			int EnumIndex = GUILayout.Toolbar(strIndex,  EnumNames);  
			CAMP camp = (CAMP)System.Enum.Parse(typeof(CAMP),EnumNames[EnumIndex]);
			npc.npcData.camp = camp;
		}
		GUILayout.EndHorizontal();
		//isBoss
		npc.npcData.isBoss = System.Convert.ToInt32( EditorGUILayout.Toggle("isBoss", System.Convert.ToBoolean(npc.npcData.isBoss)) );
		//group;
		npc.npcData.group = EditorGUILayout.IntField("group",npc.npcData.group);

		//freshParam1
		//npc.npcData.freshType = EditorGUILayout.IntField("freshType",npc.npcData.freshType);
		//freshParam1
		npc.npcData.freshParam1 = EditorGUILayout.IntField("freshParam1",npc.npcData.freshParam1);
		//freshParam2
		npc.npcData.freshParam2 = EditorGUILayout.IntField("freshParam2",npc.npcData.freshParam2);
		//lifeTime
		npc.npcData.lifeTime = EditorGUILayout.IntField("lifeTime",npc.npcData.lifeTime);
		//AIType
		npc.npcData.AIType = EditorGUILayout.Popup("AIType",npc.npcData.AIType, System.Enum.GetNames( typeof(NPCAIType)  ));
		//buffs
		EditorGUILayout.PropertyField(serializedObject.FindProperty("buffs"),true);

		/*
		public BATTLE_WAY way;
		//建筑的索引
		public int index;
		//是否需要营救
		public bool bSaved;
        */

		//isNPCRefreshPoint
		npc.npcData.isBuliding  = EditorGUILayout.Foldout(npc.npcData.isBuliding , "Buliding_Setting");
		if(npc.npcData.isBuliding)
		{
			++EditorGUI.indentLevel;
			npc.npcData.way = (BATTLE_WAY)EditorGUILayout.EnumPopup("BATTLE_WAY", npc.npcData.way);
			npc.npcData.index = EditorGUILayout.IntField("BulidingIndex",npc.npcData.index);
			npc.npcData.bSaved = EditorGUILayout.Toggle("bSaved",npc.npcData.bSaved);
		    --EditorGUI.indentLevel;
		}





		//isNPCRefreshPoint
		npc.npcData.isNPCRefreshPoint  = EditorGUILayout.Foldout(npc.npcData.isNPCRefreshPoint , "NPCRefreshPoint_Setting");
		if(npc.npcData.isNPCRefreshPoint)
		{
			++EditorGUI.indentLevel;
			npc.npcData.refreshRules = (NPCRefreshRules)EditorGUILayout.EnumPopup("refreshRules", npc.npcData.refreshRules);

			if(npc.npcData.freshParam  == null)
				npc.npcData.freshParam = new NPCRefreshParam();
			/*
				public float starDelayTime;				//多久以后开始刷新
				public float timePerCount;				//没波怪之间的时间间隔
				public float timePerNPC;				//同一波怪，每个npc之间的刷新间隔
				public int freshCount;					//刷怪次数（ -1 认为是无限刷怪）
				public int freshPoolID;					//刷新池的ID
			 */
			EditorGUILayout.LabelField("freshParam {");
			++EditorGUI.indentLevel;
			npc.npcData.freshParam.starDelayTime = EditorGUILayout.FloatField("starDelayTime",   npc.npcData.freshParam.starDelayTime);
			npc.npcData.freshParam.timePerCount  = EditorGUILayout.FloatField("timePerCount",npc.npcData.freshParam.timePerCount);
			npc.npcData.freshParam.timePerNPC    = EditorGUILayout.FloatField("timePerNPC",   npc.npcData.freshParam.timePerNPC);
			npc.npcData.freshParam.freshCount     = EditorGUILayout.IntField("freshCount",   npc.npcData.freshParam.freshCount);
			npc.npcData.freshParam.freshPoolID    = EditorGUILayout.IntField("freshPoolID",   npc.npcData.freshParam.freshPoolID);
			--EditorGUI.indentLevel;
			EditorGUILayout.LabelField("}");

			--EditorGUI.indentLevel;
		}
		else
		{
			if(npc.npcData.freshParam  != null)
				npc.npcData.freshParam = null;
		}


		serializedObject.ApplyModifiedProperties();

		if(GUI.changed)
		{ 
			SceneView.RepaintAll();
		}
	}


	public  void OnSceneGUI()
	{
		if(npc == null)
			npc = (UINPC)target;
		
		base.OnSceneGUI();

		//开始绘制GUI
		Handles.BeginGUI();
		
		GUILayout.BeginArea(new Rect(SceneView.currentDrawingSceneView.position.width -200 , 100, 200, 200));
		
		/*true显示资源细节   false显示合并资源*/
		string SelectedName = !npc.npcSettingData.isMergeResource ? EditorStringConfig.getString(10043):EditorStringConfig.getString(10044);		
		
		if(GUILayout.Button(SelectedName,GUILayout.Width(100),GUILayout.Height(20)))
		{
			npc.npcSettingData.isMergeResource = !npc.npcSettingData.isMergeResource;
			HideFlags flags = npc.npcSettingData.isMergeResource ? HideFlags.HideInHierarchy|HideFlags.HideInInspector : HideFlags.None;
			npc.SetChildrenGameObjectHideFlags(flags);
		}

		GUILayout.EndArea();
		
		
		Handles.EndGUI();
	}

	public int GetArrayIndex<T>(ref T[] array,ref T _value)
	{
		for(int i=0; i<array.Length; i++)
		{
			if( _value.Equals(array[i]) )
				return i;
		}
		return 0;
	}

}
