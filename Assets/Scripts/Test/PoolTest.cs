using UnityEngine;
using System.Collections;
using AW.War;
using System.Threading;
using tTimer = System.Threading.Timer;
using SObj = System.Object;

public class PoolTest : MonoBehaviour {

	internal class inner {
		public string inData;
		public int offset;
	}

	private tTimer timer;
	private MsgPool<inner> tPool;
	private int index = 0;
	private bool quit;

	// Use this for initialization
	void Start () {
		index = 0;
		quit  = false;
		tPool = new MsgPool<inner>(worker);
		timer = new tTimer(new TimerCallback(counting), null, Timeout.Infinite, Timeout.Infinite);
		timer.Change(100, Timeout.Infinite);
	}

	void counting(SObj status) {
		inner one = new inner() {
			inData = "all",
			offset = index ++,
		};
		tPool.OnReceive(one);
		if(!quit)
			timer.Change(100, Timeout.Infinite);
	}

	void worker (inner data) {
		ConsoleEx.DebugLog( fastJSON.JSON.Instance.ToJSON(data) );
	}

	void OnApplicationQuit() {
		quit = true;
	}

}
