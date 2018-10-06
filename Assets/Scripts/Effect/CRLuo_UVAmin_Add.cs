using UnityEngine;
using System.Collections;

public class CRLuo_UVAmin_Add : MonoBehaviour
{
	public string _ = "-=<位移材质球UV程序>=-";
	public string __ = "程序开关";
	public bool Use = false;
	public string ___ = "材质球对象";
	public Material myMaterial;

	public string ____ = "UV位移值";
	public float UAdd;
	public float VAdd;

	float UNow;

	float VNow;
	
	void Start(){
		//设置初始值
		UNow = 0;
		VNow = 0;
		//设置材质球的UV位移
		if (myMaterial != null)
		{
			myMaterial.SetTextureOffset("_MainTex", new Vector2(UNow, VNow));
		}
		else
		{

			this.gameObject.renderer.material.SetTextureOffset("_MainTex", new Vector2(UNow, VNow));
		}
	}

	void Update()
	{
		if (Use)
		{
			//设置累加值
			UNow += UAdd * Time.deltaTime;
			VNow += VAdd * Time.deltaTime;

			UNow = UNow % 1;
			VNow = VNow % 1;
			//设置材质球的UV位移
			if (myMaterial != null)
			{
				myMaterial.SetTextureOffset("_MainTex", new Vector2(UNow, VNow));
			}
			else
			{

				this.gameObject.renderer.material.SetTextureOffset("_MainTex", new Vector2(UNow, VNow));
			}
		}
	}
}
