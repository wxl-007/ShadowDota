using UnityEngine;
using System.Collections;
using UnityEditor;
//本类存放了用户对场景编译器的设置
public class SceneEditorSettings 
{
	//是否打开了鼠标跟随功能
	public static bool isOpenFllowMouse
	{
		get{ return EditorPrefs.GetBool("isOpenFllowMouse");}
		set{ EditorPrefs.SetBool("isOpenFllowMouse",value); }
	}

	//是否打开了网格编辑功能
	public static bool isEditorMapGrid
	{
		get{ return EditorPrefs.GetBool("isEditorMapGrid");}
		set{ EditorPrefs.SetBool("isEditorMapGrid",value); }
	}

	//是否打开了种怪的功能
	public static bool isOpenFarming
	{
		get{ return EditorPrefs.GetBool("isOpenFarming");}
		set{ EditorPrefs.SetBool("isOpenFarming",value); }
	}

	//当前的地图
	public static GameObject currMap
	{
		get{ return Get<GameObject>("currMap",null); }
		set{ Set("currMap",value);}
	}

	//当前的地图网格
	public static GFGrid mapGrid
	{
		get{return Get<GFGrid>("mapGrid",null);}
		set{Set("mapGrid", value);}
	}

	//当前选中的种值类型
	public static int farmingTypeIndex
	{
		get{ return EditorPrefs.GetInt("farmingTypeIndex");}
		set{ EditorPrefs.SetInt("farmingTypeIndex",value); }
	}

	//获取种植物文件夹Object
	public static Object GetPlantFileObject(SceneElementType type)
	{
		return Get<Object>("SceneElementType_"+type.ToString(),null);
	}
	//设置种植物文件夹Object
	public static void SetPlantFileObject(SceneElementType type,Object _object)
	{
		Set("SceneElementType_"+type.ToString() ,_object);
	}

	//场景资源文件夹
	public static Object SceneSourceFileObject
	{
		get{ return Get<Object>("SceneSourceFileObject",null); }
		set{ Set("SceneSourceFileObject" ,value); }
	}


	//种植物出生位置(世界位置)X
	public static int BronPostionX
	{
		get{ return EditorPrefs.GetInt("BronPostionX"); } 
		set{ EditorPrefs.SetInt("BronPostionX",value); }
	}
	//种植物出生位置(世界位置)Y
	public static int BronPostionY
	{
		get{ return EditorPrefs.GetInt("BronPostionY"); } 
		set{ EditorPrefs.SetInt("BronPostionY",value); }
	}
	////种植物出生位置(世界位置)Z
	public static int BronPostionZ
	{
		get{ return EditorPrefs.GetInt("BronPostionZ"); } 
		set{ EditorPrefs.SetInt("BronPostionZ",value); }
	}

	static public void Set (string name, Object obj)
	{
		if (obj == null)
		{
			EditorPrefs.DeleteKey(name);
		}
		else
		{
			if (obj != null)
			{
				string path = AssetDatabase.GetAssetPath(obj);
				
				if (!string.IsNullOrEmpty(path))
				{
					EditorPrefs.SetString(name, path);
				}
				else
				{
					EditorPrefs.SetString(name, obj.GetInstanceID().ToString());
				}
			}
			else EditorPrefs.DeleteKey(name);
		}
	}

	static public T Get<T> (string name, T defaultValue) where T : Object
	{
		string path = EditorPrefs.GetString(name);
		if (string.IsNullOrEmpty(path)) return null;
		
		T retVal = NGUIEditorTools.LoadAsset<T>(path);
		
		if (retVal == null)
		{
			int id;
			if (int.TryParse(path, out id))
				return EditorUtility.InstanceIDToObject(id) as T;
		}
		return retVal;
	}

}
