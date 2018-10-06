using UnityEngine;
using System.Collections;

public class FrameTest : MonoBehaviour {

	FloatFog i = 0.34323f;

	// Use this for initialization
	void Start () {
		FloatFog j = i + 0.3f;
		ConsoleEx.DebugLog(j.ToString());
	}
	
}
