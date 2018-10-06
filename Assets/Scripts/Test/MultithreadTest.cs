using UnityEngine;
using System.Collections;

namespace AW.Test {
	using System.Threading;

	public class MultithreadTest : MonoBehaviour {
		public static MultithreadTest instance;

		[HideInInspector]
		public int i;
		// Use this for initialization
		void Start () {
			instance = this;

			i = 3;

			NewThread nt = new NewThread();
			nt.start();
		}

	}


	public class NewThread {

		public NewThread() {

		}

		public void start() {
			ThreadPool.QueueUserWorkItem( wc => {
				Thread.Sleep(100);
				ConsoleEx.DebugLog(MultithreadTest.instance.i.ToString()); 

				MultithreadTest.instance.i = 5;
				ConsoleEx.DebugLog(MultithreadTest.instance.i.ToString()); 
			});
		}

	}

}