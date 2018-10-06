using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using fastJSON;
using System.IO;

[System.Serializable]
public class SceneElementEnumData
{
	public string EnumName;
	public bool isHaveColor;
	public float colorR;
	public float colorG;
	public float colorB;
	public float colorA;
	public SceneElementEnumData()
	{
		EnumName = "";
		colorR = colorG = colorB = colorA = 255f;
	}
	public Color GetColor()
	{
		return new Color(colorR/255f,colorG/255f,colorB/255f,colorA/255f);
	}
	public void SetColor(Color color)
	{
		colorR = Mathf.RoundToInt(255f*color.r);
		colorG =	Mathf.RoundToInt(255f*color.g);
		colorB =	Mathf.RoundToInt(255f*color.b);
		colorA =	Mathf.RoundToInt(255f*color.a);
	}
}

public class SceneEditorGridWindow : EditorWindow 
{
	[SerializeField]
	public List<SceneElementEnumData> enumDatas;

	public void Init()
	{
		enumDatas =new List<SceneElementEnumData>();
		/*固定不变的不可编辑的
				MAP = 0,
				NPC = 1,
				BUILDING = 2,
				TREE = 3,
		*/
		SceneElementEnumData data = null;

		string[] EnumNames = System.Enum.GetNames(typeof(SceneElementType));
		for(int i= 0;i<EnumNames.Length;i++)
		{
			data = new SceneElementEnumData();
			data.colorR = SceneEditorEnum.colorArray[i][0];
			data.colorG = SceneEditorEnum.colorArray[i][1];
			data.colorB = SceneEditorEnum.colorArray[i][2];
			data.colorA = SceneEditorEnum.colorArray[i][3];
			data.EnumName = EnumNames[i];
			data.isHaveColor = SceneEditorEnum.isHaveColor[i];
			enumDatas.Add(data);
		}
		data = new SceneElementEnumData();
		enumDatas.Add(data);
	}

	
	bool isHave = true;
	void OnGUI () 
	{
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("+",GUILayout.Width(50),GUILayout.Height(20)))
		{
			enumDatas.Add(new SceneElementEnumData());
		}
		GUILayout.Space(20f);
		if(GUILayout.Button("-",GUILayout.Width(50),GUILayout.Height(20)))
		{
			int index = enumDatas.Count -1;
			if(index > 3)
				enumDatas.RemoveAt(index);
		}
		EditorGUILayout.EndHorizontal();


		isHave = EditorGUILayout.Foldout(isHave, "SceneElementType");

		if(isHave)
		{
			++EditorGUI.indentLevel;
			//bool isControlsEnable = false;
			for(int i= 0;i< enumDatas.Count;i++)
			{
				SceneElementEnumData data = enumDatas[i];
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(20);

				EditorGUI.BeginDisabledGroup(i < 4);

				GUILayout.Label("Element"+i.ToString(),GUILayout.Width(60));
				data.EnumName = GUILayout.TextField(data.EnumName,GUILayout.Width(150));
				data.isHaveColor = GUILayout.Toggle(data.isHaveColor,"");
				if(data.isHaveColor)
				{
					Color color = EditorGUILayout.ColorField(data.GetColor());
					data.SetColor(color);
					GUILayout.Label("["+  data.colorR.ToString() +"," +data.colorG.ToString() +","+data.colorB.ToString()+"," +data.colorA.ToString()+"]");
				}

				EditorGUI.EndDisabledGroup();

				GUILayout.Space(100f);
				EditorGUILayout.EndHorizontal();
			}
			--EditorGUI.indentLevel;
		}





		if(GUILayout.Button("生成代码",GUILayout.Width(150),GUILayout.Height(40)))
		{
			string ColorCode = "   public static float[][] colorArray = new float[][]\n   {\n";
			string EnumString = "public enum SceneElementType\n{\n";
			string isHaveColdeString = "   public static bool[] isHaveColor = new bool[]{";
			int i = 0;
			foreach(SceneElementEnumData data in enumDatas)
			{
				if(!string.IsNullOrEmpty(data.EnumName)) 
				{
					string colorString = "      new float[]{"+data.colorR.ToString()+","+data.colorG.ToString()+","+data.colorB.ToString()+","+data.colorA.ToString()+"},\n";
					ColorCode+=colorString;			
					EnumString+="      "+data.EnumName+",\n";
					isHaveColdeString+=  data.isHaveColor.ToString().ToLower()+",";
					i++;
				}
			}
			ColorCode+="   };\n\n";
			EnumString+="};\n\n";
			isHaveColdeString+="};\n\n";
			//代码生成
			SceneEditorTools.AutoGenerateEnumCode(EnumString,ColorCode,isHaveColdeString);
		}
	}

}
