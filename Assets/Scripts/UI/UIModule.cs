using UnityEngine;
using System.Collections;
using UIEventCenter;
using System.Collections.Generic;
//UI模块(排版用)
public class UIModule : MonoBehaviour 
{
	public string UIMoudle;
	public List<UISprite> List_Sprite = new List<UISprite>();
	public List<UILabel> List_Label = new List<UILabel>();
	public List<UIButton> List_Button = new List<UIButton>();
	public List<UISlider> List_Slider = new List<UISlider>();
	public List<UIGrid> List_Grid = new List<UIGrid>();
	public List<UIToggle> List_Toggle = new List<UIToggle>();
	public UIModuleElement[] List_Element;
	public List<GameObject> List_Object = new List<GameObject>();
	public List<Shader> List_ShaderObject = new List<Shader>();

	void Btn_OnClick(GameObject btn)
	{
		EventSender.SendEvent(UIMoudle+"_OnClick",btn);
	}

	void Btn_OnMouseOver(GameObject btn)
	{
		EventSender.SendEvent(UIMoudle+"_OnMouseOver",btn);
	}

	void Btn_OnMouseOut(GameObject btn)
	{
		EventSender.SendEvent(UIMoudle+"_OnMouseOut",btn);
	}

	void Btn_OnPress(GameObject btn)
	{
		EventSender.SendEvent(UIMoudle+"_OnPress",btn);
	}

	void Btn_OnRelease(GameObject btn)
	{
		EventSender.SendEvent(UIMoudle+"_OnRelease",btn);
	}

	void Btn_OnDoubleClick(GameObject btn)
	{
		EventSender.SendEvent(UIMoudle+"_OnDoubleClick",btn);
	}
	
}
