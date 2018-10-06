using UnityEngine;
using System.Collections;

namespace AW.Test {

	public class AsyncTest : MonoBehaviour {

		void Start() {
			Invoke("doTest", 5F);
		}

		void doTest() {
			Logical log = new Logical();
			log.doWork();
		}


		class Logical {
			public int i = 0;

			float[] delay = new float[] {
				1F, 3F, 5F, 6F, 10F, 20F 
			};

			public void doWork() {
				for(int j = 0; j < 6; ++ j) {
					///
					/// ---- 结论是： 如果不保存参数，则这里一直输出6 
					///
					AsyncTask.QueueOnMainThread( () => { print(j); } , delay[j]);

					///
					/// ---- 下面写法，才能输出 0, 1, 2, 3, 4, 5 ----
					///
					// AsyncTask.AysncRun(print, delay[i], j);
				}
			}

			void print(int val) {
				Debug.Log("value = " + val.ToString());
			}
		}

	}

}