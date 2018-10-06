using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiUpdateTest : MonoBehaviour {
	[HideInInspector]
	public Dictionary<int, int> container = new  Dictionary<int, int>();

	public bool isA;
	
	// Update is called once per frame
	void Update () {
		foreach(int v in container.Values) {
			ConsoleEx.DebugLog("i = " + v, isA ? ConsoleEx.YELLOW : ConsoleEx.RED);
		}
	}
}
