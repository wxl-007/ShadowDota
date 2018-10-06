using UnityEngine;
using System.Collections;
using  AW.Data;
using AW.War;
using SceneEditorSttingData;
public class UINPC : UISceneElement
{
	//NPC的所有配置信息(编译环境下)
	public NPCInSceneData npcData = new NPCInSceneData();
	public NPCSettingData npcSettingData = new NPCSettingData();

	public int[] buffs;

	//public int[] RefreshPool;

	void Start () 
	{
		InitData();
	}

	public void InitData()
	{
		this.isAtMap = npcSettingData.isAtMap;
		this.isSeletctedFllowMouse = npcSettingData.isSeletctedFllowMouse;
		this.isSetTransform = npcSettingData.isSetTransform;
		this.buffs = npcData.buffs;
//		this.RefreshPool = npcData.RefreshPool;
		HideFlags flags = npcSettingData.isMergeResource ? HideFlags.HideInHierarchy|HideFlags.HideInInspector : HideFlags.None;
		this.SetChildrenGameObjectHideFlags(flags);
		UpdateData();
	}


	public void OnDrawGizmos()
	{
		//Gizmos.DrawIcon(transform.position + Vector3.forward, "Hero.jpg", true);
	}

	public void OnDrawGizmosSelected()
	{
		switch((CAMP)npcData.camp)
		{
		case CAMP.Neutral://中立
			Gizmos.color = Color.yellow;
			break;
		case CAMP.Enemy://天灾
			Gizmos.color = Color.red;
			break;
		case CAMP.Player://
			Gizmos.color = Color.green;
			break;
		case CAMP.All:
			Gizmos.color = Color.black;
			break;
		default:
			Gizmos.color = Color.white;
			break;
		}
		Gizmos.DrawCube(GetCorrectionPostion(transform.position),new Vector3(2f,0.01f,2f));
	}


	//更新数据
	public override void UpdateData()
	{
		//gameObject.SetActiveRecursively(false);
		//Debug.LogError("UpdateData NINPC");
		npcData.buffs = this.buffs;
//		npcData.RefreshPool = this.RefreshPool;
		//pos
		if(npcData.pos == null || npcData.pos.Length < 3)npcData.pos = new float[3];
		Vector3 worldPos = transform.position;
		npcData.pos[0] = Mathf.Floor(worldPos.x);
		npcData.pos[1] = Mathf.Floor(worldPos.y);
		npcData.pos[2] = Mathf.Floor(worldPos.z);

		//rotation
		if(npcData.rotation == null || npcData.rotation.Length < 3) npcData.rotation = new float[3];
		Vector3 Rotation = transform.rotation.eulerAngles;
		npcData.rotation[0] = Rotation.x;
		npcData.rotation[1] = Rotation.y;
		npcData.rotation[2] = Rotation.z;

		//scale
		if(npcData.scale == null || npcData.scale.Length < 3) npcData.scale = new float[3];
		Vector3 Scale = transform.localScale;
		npcData.scale[0] = Scale.x;
		npcData.scale[1] = Scale.y;
		npcData.scale[2] = Scale.z;


		//更新配置数据
		//更新属性
		npcSettingData.Attribute = npcData;
		npcSettingData.isAtMap = this.isAtMap;
		npcSettingData.isSeletctedFllowMouse = this.isSeletctedFllowMouse;
		npcSettingData.isSetTransform = this.isSetTransform;

		//转换到基类结构
		settingData = (SceneEditorSettingBaseData)npcSettingData;
	}

	//克隆代码
	public override void CloneScript(GameObject cloneSource,GameObject cloneObject)
	{
		UINPC oldareaScript = cloneSource.GetComponent<UINPC>();
		UINPC newareaScript =cloneObject.GetComponent<UINPC>();

		newareaScript.npcData = oldareaScript.npcData;
		newareaScript.npcSettingData = oldareaScript.npcSettingData;
		newareaScript.InitData();
	}

	//设置子物体的显示状态
	public void SetChildrenGameObjectHideFlags(HideFlags flags)
	{
		Transform[] children =  GetComponentsInChildren<Transform>();
		int length = children.Length;
		if(length > 1)
		{
			for(int i=1;i<length;i++)
				children[i].hideFlags = flags;
		}
	}

}
