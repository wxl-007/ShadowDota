using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

public class AsyncTask : MonoBehaviour {
	private const int maxThreads = 8;
	private static int numThreads;

	private static AsyncTask _current;
	private int _count;
	public static AsyncTask Current {
		get {
			Initialize();
			return _current;
		}
	}

	void Awake()
	{
		_current = this;
		initialized = true;
	}

	private static bool initialized;
	static void Initialize() {
		if (!initialized) {
			if(!Application.isPlaying)
				return;
			initialized = true;
			var g = new GameObject("AsyncTask");

			var gobal = GameObject.FindGameObjectWithTag("Global");
			if(gobal != null) g.transform.parent = gobal.transform;
			_current = g.AddComponent<AsyncTask>();
			DontDestroyOnLoad(g);
		}

	}

	/*	
	 * Action will be excute immediately
	 */
	private List<Action> _actions = new List<Action>();

	#region delayed task
	public struct DelayedQueueItem {
		public float time;
		public Action action;
	}
	/*	 Action will be excute laterly
	 */ 
	private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

	private List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();
	#endregion

	public static void QueueOnMainThread(Action action) {
		QueueOnMainThread(action, 0f);
	}

	public static void QueueOnMainThread(Action action, float time) {
		if(time != 0) {
			lock(Current._delayed)
				Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action});
		}
		else {
			lock (Current._actions)
				Current._actions.Add(action);
		}
	}

    public static void RemoveQueueOnMainThread(Action action) {
        if(action != null) {
            lock(Current._delayed) {
                DelayedQueueItem? toDel =null ;
                foreach(var work in Current._delayed) {
                    if(work.action == action) {
                        toDel = work;
                        break;
                    }
                }

                if(toDel != null) Current._delayed.Remove((DelayedQueueItem)toDel);
            }
        }
    }

	public static void RemoveAllDelayedWork() {
		lock(Current._delayed) {
			Current._delayed.Clear();
		}
	}

	public static void RunAsync(Action a)
	{
		Initialize();
		while(numThreads >= maxThreads)
		{
			Thread.Sleep(1);
		}
		Interlocked.Increment(ref numThreads);
		ThreadPool.QueueUserWorkItem(RunAction, a);
	}

	private static void RunAction(object action)
	{
		try {
			((Action)action)();
		}
		catch { }
		finally {
			Interlocked.Decrement(ref numThreads);
		}
	}

	void OnDisable() {
		if (_current == this) {
			_current = null;
		}
	}

	List<Action> _currentActions = new List<Action>();

	// Update is called once per frame
	void Update() {
		lock (_actions) {
			_currentActions.Clear();
			_currentActions.AddRange(_actions);
			_actions.Clear();
		}

		int count = _currentActions.Count;
		for (int i = 0; i < count; i++)
			_currentActions [i]();

		//------------------------- Split ---------------------------

		lock(_delayed) {
			_currentDelayed.Clear();
			_currentDelayed.AddRange(_delayed.Where(d=>d.time <= Time.time));

			int delayCnt = _currentDelayed.Count;
			for (int i = 0; i < delayCnt; i++)
				_delayed.Remove( _currentDelayed[i]);
		}
			
		int dlyCnt = _currentDelayed.Count;
		for (int i = 0; i < dlyCnt; i++)
			_currentDelayed[i].action();
	}
}

