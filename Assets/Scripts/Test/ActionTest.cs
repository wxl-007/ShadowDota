using UnityEngine;
using System.Collections;
using System;
using System.Threading;

public class ActionTest : MonoBehaviour {

	internal class Acd {
		public int i = 0;
		string s = "empty";
	}

	internal class Full {
		int[] array ;
		public Acd acd;

		public Full(Action end)  {
			//ThreadPool.QueueUserWorkItem( o => {
				array = new int[3];
				for(int i = 0; i < 3; ++ i) {
					array[i] = i;
				}

				acd = new Acd();
				if(end != null) end();
			//});
		}

	}

	private Full ful;
	// Use this for initialization
	void Start () {
		ful = new Full(testAction);
	}

	void testAction () {
		try {
			Debug.Log(ful.acd.i); 
		} catch(Exception ex) {
			Debug.Log(ex.ToString());
		}

	}

}
