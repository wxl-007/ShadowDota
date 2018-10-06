using UnityEngine;
using System.Collections;
using SceneEditorSttingData;


[ExecuteInEditMode]
public class UISceneElementBase : MonoBehaviour 
{
	//编辑器配置
	public SceneEditorSettingBaseData settingData;

	//更新数据
	public virtual void UpdateData(){}

	//获得焦点
	public virtual void OnFocus(){}

	//失去焦点
	public virtual void OnLostFocus(){}
}
