using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UIStars : MonoBehaviour {

	public List<UISprite> list_stars = new List<UISprite>();

	public UIGrid grid;

	[HideInInspector]
	public float cellHeight;

	[HideInInspector]
	public float cellWidth;
	
	public string brightStarName{get;set;}
	
	public string darkStarName{get;set;}

	[HideInInspector]
	public float Scale = 0.41f;

	public void Awake()
	{
		if(string.IsNullOrEmpty(brightStarName)) brightStarName = "Checkpoint-006";
		if(string.IsNullOrEmpty(darkStarName)) darkStarName = "Checkpoint-007";

		if(cellHeight > 0)
		   grid.cellHeight = cellHeight;
		if(cellWidth > 0)
		   grid.cellWidth = cellWidth;

		grid.transform.localScale = new Vector3(Scale,Scale,1f);

		//----test----
		//SetStarts(2);
		//------------
	}
	
	public void SetStarts(int count) 
	{
		int allcount = list_stars.Count;
		if(count < allcount)
		{
			for(int i = 0; i<allcount; i++)
			{
				if(i < count)
				{
				  list_stars[i].spriteName = brightStarName;
				}
				else
				  list_stars[i].spriteName = darkStarName;
			}
		}
	}
	

}
