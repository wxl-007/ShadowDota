using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;
using SkD = AW.War.DelayedSkData;

namespace AW.War {
	public class SkAsyncRunner : MonoBehaviour {
		private const int maxThreads = 8;
		private static int numThreads;

		private static SkAsyncRunner _current;
		private int _count;
		public static SkAsyncRunner Current {
			get {
				Initialize();
				return _current;
			}
		}

		void Awake() {
			_current = this;
			initialized = true;
		}

		private static bool initialized;
		static void Initialize() {
			if (!initialized) {
				if(!Application.isPlaying)
					return;
				initialized = true;
				var g = new GameObject("SkAsyncRunner");

				var gobal = GameObject.FindGameObjectWithTag("Global");
				if(gobal != null) g.transform.parent = gobal.transform;
				_current = g.AddComponent<SkAsyncRunner>();
				DontDestroyOnLoad(g);
			}
		}


		class DelayedSkEf {
			public float time;
			public Action<SkD> action;
			public SkD argu1;
		}

		/*	 Action will be excute laterly
	    */ 
		private List<DelayedSkEf> _delayedsk = new List<DelayedSkEf>();

		private List<DelayedSkEf> _currentDelayedsk = new List<DelayedSkEf>();

		public static void AysncRun(Action<SkD> action, float time, SkD arg1) {
			if(time != 0) {
				lock(Current._delayedsk)
					Current._delayedsk.Add(new DelayedSkEf { time = Time.time + time, action = action, argu1 = arg1});
			}
		}

		List<int> toBeRmSk = new List<int>();

		void Update() {
			lock(_delayedsk) {
				_currentDelayedsk.Clear();

				int count = _delayedsk.Count;
				if(count > 0) {
					for(int i = 0; i < count; ++ i) {
						DelayedSkEf item = _delayedsk[i];
						if(item.time <= Time.time) {
							toBeRmSk.Add(i);
							_currentDelayedsk.Add(item);
						}
					}

					int rmCnt = toBeRmSk.Count;
					if(rmCnt > 0) {
						for (int i = 0; i < rmCnt; i++)
							_delayedsk.RemoveAt(toBeRmSk[i]);
					}

					toBeRmSk.Clear();
				}
			}

			int runCnt = _currentDelayedsk.Count;
			for (int i = 0; i < runCnt; i++) {
				DelayedSkEf deSkEf = _currentDelayedsk[i];
				deSkEf.action(deSkEf.argu1);
			}
		}

	}
}
