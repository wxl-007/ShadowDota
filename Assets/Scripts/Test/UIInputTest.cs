using UnityEngine;
using System.Collections;

public class UIInputTest : MonoBehaviour {

	public MultiUpdateTest test;

	public void OnInput() {
		for(int i = 0; i < 10; ++ i) {
			test.container[i] = i;
			ConsoleEx.DebugLog("**************** ", ConsoleEx.RED);
		}
	}

}
