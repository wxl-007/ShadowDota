using System;
using System.Threading;
using System.Collections.Generic;

namespace AW.War {
	/// <summary>
	/// CLient---Server之间的消息池
	/// </summary>
	public class MsgPool<T> where T : class {
		private Queue<T> Pool;
		private readonly object _locker = null;
		private bool Loop = true;
		private Thread WorkThread;
		//工作者
		private Action<T> worker = null;

		public MsgPool (Action<T> realWorker) {
			Pool    = new Queue<T>();
			_locker = new object();
			Loop    = true;
			worker  = realWorker;

			WorkThread = new Thread (new ThreadStart (Run));
			WorkThread.Start ();
		}

		public void OnReceive (T msg) {
			lock (_locker) {
				Pool.Enqueue (msg);                   // We must pulse because we're
				Monitor.Pulse (_locker);              // changing a blocking condition.
			}
		}

		public void QuitMsgPool() {
			lock (_locker) {
				Loop = false;
				Pool.Enqueue (null);
				Monitor.Pulse (_locker);
			}
		}

		T getMsg () {
			lock (_locker) {
				while (Pool.Count == 0 && Loop)
					Monitor.Wait (_locker);
				return Pool.Dequeue ();               // This signals our exit.
			}
		}

		void ResetMsgPool() {
			lock (_locker) {
				Pool.Clear();
				Monitor.Wait (_locker);
			}
		}

		void Run () {
			// Keep consuming until told otherwise.
			while (Loop) {
				T msg = getMsg();
				if(msg != null && worker != null) 
					worker(msg);
			}

			ConsoleEx.DebugLog(GetType().ToString() + " MsgPool is Exited.");
		}

	}
}