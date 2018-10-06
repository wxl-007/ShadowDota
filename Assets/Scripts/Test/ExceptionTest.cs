using System;
using UnityEngine;
using System.Collections;


/// <summary>
/// 用来测试：如果函数里面有错误，外测try有没有用
/// </summary>
public class ExceptionTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Worker worker = new Worker();
		try {
			worker.doWork();
		} catch(NullReferenceException ex) {
			ConsoleEx.DebugLog(ex.ToString());
		} 
	}


	class Sample {
		public int j = 0;
	}


	class Worker {
		public int i = 0;
		public Sample sample = null;

		public void doWork() {
			int all = i + sample.j;
			ConsoleEx.DebugLog(all.ToString());
		}
	}
}
