using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AW.Data;
using SceneEditorSttingData;
public class UISceneMap :  UIRectGrid
{
	//该GameObject的资源来源
	public Object resource{get;set;}
	//地图网格
	public GFGrid mapGrid{get;set;}

	public bool isShowMap{get;set;}

	public List<Transform> ElementParents = new List<Transform>();

	public MapInSceneData mapData = new MapInSceneData();

	public MapSettingData mapSettingData = new MapSettingData();

	private Camera _editorCamera;
	public Camera EditorCamera
	{
		get
		{
			if(_editorCamera == null)
			{
				gameObject.SetActive(true);
				GameObject cam = GameObject.Find("EditorCamera");
				if(cam != null)
				{
					_editorCamera = cam.GetComponent<Camera>();
				}
				if(_editorCamera == null)
				{
					GameObject g = new GameObject();
					_editorCamera = g.AddComponent<Camera>();
					_editorCamera.backgroundColor = new Color(0.3f,0.3f,0.3f,1f);
					g.name = "EditorCamera";
					g.transform.parent = transform;
					g.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
				}
			}

			return _editorCamera;
		}
	}


	public void Start() 
	{

	}

	public void InitData()
	{
		RefreshClassification();
		isShowMap = true;
		UpdateData();
		EditorCamera.enabled = false;
	}


	public bool isRefreshing = false;
	//刷新分类
	public void RefreshClassification()
	{
		if(isRefreshing)return;
		isRefreshing = true;

		List<string> names = new List<string>();
		names.AddRange( System.Enum.GetNames(typeof(SceneElementType)) );

		foreach(Transform t in ElementParents)
		{
			if(t != null)
			{
				if(names.Contains(t.gameObject.name))
					names.Remove(t.gameObject.name);
				else
				    DestroyImmediate(t.gameObject);
			}
		}
		RemoveNull(ref ElementParents);

		foreach(string name in names)
		{
			if(name != "MAP")
			{
				GameObject o = new GameObject();
				o.name = name;
				o.transform.parent = transform;
				o.transform.position = Vector3.zero;
				o.transform.localScale = Vector3.one;
				o.transform.localRotation = Quaternion.Euler(Vector3.zero);
				o.hideFlags = HideFlags.NotEditable;
				ElementParents.Add( o.transform );
			}
		}




		isRefreshing = false;
	}


	public void RemoveNull(ref List<Transform> array)
	{
		int find = -1;
		for(int i= 0;i< array.Count ;i++)
		{
			if(array[i] == null)
			{
				find = i;
				break;
			}
		}
		if(find != -1)
		{
			array.RemoveAt(find);
			RemoveNull(ref array);
		}
	}

	public void OnDrawGizmos ()
	{
		if(isShowMap)
		base.OnDrawGizmos();
	}


	//更新数据
	public override void UpdateData()
	{

		//Debug.LogError("UpdateData UISceneMap");
		//pos
		if(mapData.pos == null || mapData.pos.Length < 3)mapData.pos = new float[3];
		Vector3 worldPos = transform.position;
		mapData.pos[0] = Mathf.Floor(worldPos.x);
		mapData.pos[1] = Mathf.Floor(worldPos.y);
		mapData.pos[2] = Mathf.Floor(worldPos.z);
		
		//rotation
		if(mapData.rotation == null || mapData.rotation.Length < 3) mapData.rotation = new float[3];
		Vector3 Rotation = transform.rotation.eulerAngles;
		mapData.rotation[0] = Rotation.x;
		mapData.rotation[1] = Rotation.y;
		mapData.rotation[2] = Rotation.z;
		
		//scale
		if(mapData.scale == null || mapData.scale.Length < 3) mapData.scale = new float[3];
		Vector3 Scale = transform.localScale;
		mapData.scale[0] = Scale.x;
		mapData.scale[1] = Scale.y;
		mapData.scale[2] = Scale.z;

		//****更新配置数据****
		//更新属性
		mapSettingData.Attribute = mapData;

		//更新网格大小
		if(mapSettingData.GridSize == null || mapSettingData.GridSize.Length < 2)
			mapSettingData.GridSize = new float[2];
		mapSettingData.GridSize[0] = renderTo.x;
		mapSettingData.GridSize[1] = renderTo.y;

		//更新网格颜色
		if(mapSettingData.GridColor == null || mapSettingData.GridColor.Length < 4)
			mapSettingData.GridColor = new float[4];
		mapSettingData.GridColor[0] = axisColors.r;
		mapSettingData.GridColor[1] = axisColors.g;
		mapSettingData.GridColor[2] = axisColors.b;
		mapSettingData.GridColor[3] = axisColors.a;

		//转换到基类结构
		settingData = (SceneEditorSettingBaseData)mapSettingData;

	}

}
