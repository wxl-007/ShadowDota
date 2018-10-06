using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIModuleElement : MonoBehaviour 
{
	public List<UISprite> m_SpriteList = new List<UISprite>();
	public List<UILabel> m_LabelList = new List<UILabel>();
	public List<UIButton> m_ButtonList = new List<UIButton>();
	public List<GameObject> m_ObjectList = new List<GameObject>();
	public List<TweenPosition> m_tweenPosition = new List<TweenPosition>();
	public List<TweenScale> m_tweenScale = new List<TweenScale>();
	public List<TweenAlpha> m_tweenAlpha = new List<TweenAlpha>();
	public List<UIModuleElement> m_UIModuleElementList = new List<UIModuleElement>();
}
