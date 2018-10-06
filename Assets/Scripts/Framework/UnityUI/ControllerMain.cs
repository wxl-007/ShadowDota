using UnityEngine;
using System.Collections;

/*
 * 控制器的主控
 */
public class ControllerMain : MonoBehaviour {

	public static ControllerMain CtrlMain;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
		CtrlMain = this;
	}
	
}
