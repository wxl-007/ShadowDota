using UnityEngine;
using System.Collections;
using AW.Data;
using SceneEditorSttingData;
public enum SpecialAreaType
{
	SpecialArea0,
	SpecialArea1,
	SpecialArea2,
}

public class UISpecialArea : UISceneElement
{

	public  SpecialAreaInSceneData areaData = new SpecialAreaInSceneData();

	public SpecialAreaSettingData areaSettingData = new SpecialAreaSettingData();

	//false: 线  true: 面
	public bool DrawWireOrSurface {get;set;}

	[HideInInspector][SerializeField] public int SpecialAreaIndex = 0;

	private Shader _useShader;
	public Shader useShader
	{
		get
		{
			if(_useShader == null)
				_useShader = Resources.Load("UnPack/SpecialArea/Shader 0") as Shader;
			return _useShader;
		}
		set
		{ 
			useShader = value; 
		}
	}

	public Material _useMaterial;
	public Material useMaterial
	{
		get
		{
			if(_useMaterial == null)
				_useMaterial = new Material( useShader );
			return _useMaterial;
		}
		set
		{
			_useMaterial = value;
		}
	}
	
	private Color drawColor 
	{
		get
		{
			int typeIndex = (int)areaSettingData.Type;
			if( SceneEditorEnum.isHaveColor[typeIndex])
			{
				float r =  SceneEditorEnum.colorArray[typeIndex][0] == 0? 0 : SceneEditorEnum.colorArray[typeIndex][0] / 255f;
				float g = SceneEditorEnum.colorArray[typeIndex][1] == 0? 0 : SceneEditorEnum.colorArray[typeIndex][1] / 255f;
				float b = SceneEditorEnum.colorArray[typeIndex][2] == 0? 0 : SceneEditorEnum.colorArray[typeIndex][2] / 255f;
				float a = SceneEditorEnum.colorArray[typeIndex][3] == 0? 0 : SceneEditorEnum.colorArray[typeIndex][3] / 255f;
				return new Color( r,g,b,a );
			}
			return  Color.white;
		}
	}

	void Start () 
	{
		InitData();
	}


	public void InitData()
	{
		boxCollider.enabled = true;
		boxCollider.hideFlags = HideFlags.HideInHierarchy;
		render.material = useMaterial;
		
		Grid.axisColors = drawColor;
		render.enabled = DrawWireOrSurface;

		this.isAtMap = areaSettingData.isAtMap;
		this.isSeletctedFllowMouse = areaSettingData.isSeletctedFllowMouse;
		this.isSetTransform = areaSettingData.isSetTransform;

		UpdateData();
	}


	private BoxCollider _box;
	BoxCollider boxCollider
	{
		get
		{
			if(_box == null)
				_box = this.GetComponent<BoxCollider>();
			if(_box == null)
			{
				_box = this.gameObject.AddComponent<BoxCollider>();
				_box.center = Vector3.zero;
			}
			return _box;
		}
	}



	//线框
	public UIRectGrid _grid;
	public UIRectGrid Grid
	{
		get
		{
			if(_grid == null)
			{
				UIRectGrid[] grids = transform.GetComponentsInChildren<UIRectGrid>();
				foreach(UIRectGrid data in grids)
				{
					if(data.gameObject.name == "UIRectGrid")
					{
						_grid = data;
						return _grid;
					}
				}
			}
			return _grid;
		}
	}

	public MeshRenderer _render;
	public MeshRenderer render
	{
		get
		{
			if(_render == null)
				_render = GetComponent<MeshRenderer>();
			return _render;
		}
	}

	public void OnDrawGizmos()
	{
		Grid.axisColors = drawColor;

		Color temp = drawColor;
		temp.a = !DrawWireOrSurface ? 0 :temp.a;
		render.material = useMaterial;
		render.sharedMaterial.color = temp;	

		if(!render.enabled)render.enabled = true;

	
		Vector3 localScale = transform.localScale;

		Grid.spacing = localScale;
		Grid.renderTo = localScale;
		Grid.transform.localScale = Vector3.one;
	}

	public override Vector3 GetCorrectionPostion(Vector3 pos)
	{
		pos.x = Mathf.RoundToInt(pos.x);
		pos.y = transform.localScale.y / 2f;
		pos.z = Mathf.RoundToInt(pos.z);
		return pos;
	}

	//更新数据
	public override void UpdateData()
	{
		//Debug.LogError("UpdateData UISpecialArea");
		areaData.Type = areaSettingData.Type;
		//pos
		if(areaData.pos == null || areaData.pos.Length < 3)
			areaData.pos = new float[3];
		Vector3 worldPos = transform.position;
		areaData.pos[0] = Mathf.Floor(worldPos.x);
		areaData.pos[1] = Mathf.Floor(worldPos.y);
		areaData.pos[2] = Mathf.Floor(worldPos.z);
		
		//rotation
		if(areaData.rotation == null || areaData.rotation.Length < 3) areaData.rotation = new float[3];
		Vector3 Rotation = transform.rotation.eulerAngles;
		areaData.rotation[0] = Rotation.x;
		areaData.rotation[1] = Rotation.y;
		areaData.rotation[2] = Rotation.z;
		
		//scale
		if(areaData.scale == null || areaData.scale.Length < 3) areaData.scale = new float[3];
		Vector3 Scale = transform.localScale;
		areaData.scale[0] = Scale.x;
		areaData.scale[1] = Scale.y;
		areaData.scale[2] = Scale.z;


		//更新配置数据
		//更新属性
		areaSettingData.Attribute = areaData;
		areaSettingData.isAtMap = this.isAtMap;
		areaSettingData.isSeletctedFllowMouse = this.isSeletctedFllowMouse;
		areaSettingData.isSetTransform = this.isSetTransform;

		//转换到基类结构
		settingData = (SceneEditorSettingBaseData)areaSettingData;
	}

	//克隆代码
	public override void CloneScript(GameObject cloneSource,GameObject cloneObject)
	{
		UISpecialArea oldareaScript = cloneSource.GetComponent<UISpecialArea>();
		UISpecialArea newareaScript =cloneObject.GetComponent<UISpecialArea>();
		
		newareaScript.areaData = oldareaScript.areaData;
		newareaScript.areaSettingData = oldareaScript.areaSettingData;
		newareaScript.InitData();

	}

	//吸附地图
	public override void AdsorptionToMap()
	{
		Vector3 curPos = transform.position;
		curPos.y = transform.localScale.y / 2f;
		transform.position = curPos;
	}

	//不效正坐标
	public override void CorrectionPostion()
	{

	}
}
