using UnityEngine;
using System.Collections;

//场景元素（建筑物，树，NPC，英雄，暂时都用这个，
//后期某个元素单独扩展的话，请继承此类单写）
public class UISceneElement :UIRectGrid {
	
	//是否自动吸附到地图上
	public bool isAtMap{get;set;}

	//用户选择了使种值元素跟随鼠标移动
	public bool isSeletctedFllowMouse{get;set;}

	public bool isSetTransform{get;set;}

	//是否删除
	public bool isDeleteGameObject{get;set;}
	
	//效正标准
	public virtual Vector3 GetCorrectionPostion(Vector3 pos)
	{
		pos.x = Mathf.RoundToInt(pos.x);
		pos.y = 0;
		pos.z = Mathf.RoundToInt(pos.z);
		return pos;
	}

	//种植(效正坐标)
	public virtual void CorrectionPostion()
	{
		transform.position = GetCorrectionPostion(transform.position);
	}


	//克隆代码
	public virtual void CloneScript(GameObject cloneSource,GameObject cloneObject)
	{

	}

	//吸附地图
	public virtual void AdsorptionToMap()
	{
		Vector3 curPos = transform.position;
		curPos.y = 0;
		transform.position = curPos;
	}

	//绘画函数
	public void OnDrawGizmos()
	{
		base.OnDrawGizmos();
	}

	//更新数据
	public override void UpdateData(){}


	//获得焦点
	public override void OnFocus()
	{
		isSeletctedFllowMouse = false;
	}
	
	//失去焦点
	public override void OnLostFocus()
	{
		isSeletctedFllowMouse = false;
	}

}
