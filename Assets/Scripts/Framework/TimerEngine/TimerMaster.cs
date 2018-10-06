using System;
using System.Threading;
using System.Collections.Generic;
using AW.FSM;

namespace AW.Timer {
    using t_Timer = System.Threading.Timer;


	/// <summary>
	/// 本计时器的最小计时单位是1S
	/// </summary>
	public class TimerMaster : IDeviceState, IGameState {
        //This Timer is the Best one, *****  don't use System.Timers.Timer ****
        private t_Timer threadTimer = null;
        //millisecend
        private const int IntervalPeriod = 1000;

        //最多支持多少个计时任务的数量
        private const int MAX_TIMER_TASK_COUNT = 10000;

        /// 所有当前正在执行的任务，在执行的过程中，不应该被改动，
        /// 所有需要真正执行的地方都放到ActionList
        ///
        private Dictionary<int, TimerTask> TaskDic = null;
        /// <summary>
        /// 执行序列
        /// </summary>
        private Queue<Action> ActionList = null;

        /// <summary>
        /// 等待被删除的Task列表，内容为唯一ID
        /// </summary>
        private List<int> ToBeRemoved = null;

        /// <summary>
        /// 读写的共享锁
        /// </summary>
        private readonly ReaderWriterLockSlim m_readerWriterLock = new ReaderWriterLockSlim();

        //服务器的时间，每一段时间就会同步一次
        private long cachedServerTime;
        //计时模块的计时器
        private long curUtc;

		//OnPause 的时候，记录一下当前的值
		private long UtcWhenPaused = 0;

        //当前的时间有可能会出现暂时为0的情况，因为第一次设定时间会先放入缓存中
        public long curTime {
            get { return curUtc == 0 ? cachedServerTime : curUtc; }
            set { curUtc = value; }
        }

        private int mIndex = 0;

        public TimerMaster() {
            //DueTime to TimeOut.Infinite to prevent the timer from starting
            //Period Specify Timeout.Infinite to disable periodic signaling, callback routine run once.
            threadTimer = new t_Timer(new TimerCallback(ComputeBoundOp), null, Timeout.Infinite, Timeout.Infinite);

            TaskDic = new Dictionary<int, TimerTask>();
            ActionList = new Queue<Action>();
            ToBeRemoved = new List<int>();

            curUtc = 0;
            cachedServerTime = 0;
        }

        //DragonBallTimer status
        private bool _isRun = false;
        public bool IsRunning {
            get { return _isRun;}
            private set {_isRun = value;}
        }

        // we can ignore "state" param
        void ComputeBoundOp (Object state) {
            IsRunning = true;

            if(cachedServerTime > 0) {
                curUtc = cachedServerTime;
                cachedServerTime = 0;
            }

            curUtc ++;

            //申请读锁
            #region require Read lock
            m_readerWriterLock.EnterReadLock();

            if (TaskDic != null) {

                //清空状态
                ActionList.Clear();
                ToBeRemoved.Clear();

                foreach (int TaskId in TaskDic.Keys) {
                    TimerTask task = TaskDic[TaskId];

                    if(task != null) {
                        task.leftTime = task.endTime - curUtc;
                        task.leftTime = task.leftTime <= 0 ? 0 : task.leftTime;

                        if(task.leftTime == 0 && task.endTime != TimerTask.INFINITY) 
                            ToBeRemoved.Add(TaskId);

						if(task.Enabled == false)
							ToBeRemoved.Add(TaskId);

                        if(task.startTime == curUtc) {
                            ActionList.Enqueue(task.handleStart);
                        }

                        if(task.endTime == curUtc) {
                            ActionList.Enqueue(task.handleCompleted);
                        }

                        if(curUtc > task.startTime && (curUtc < task.endTime || task.endTime == TimerTask.INFINITY)) {

                            if(task.frequency == TimerTask.NO_FREUENCY) {
                                // do thing ... 
                            } else {
                                if(task.curFre > 1) task.curFre --;
                                else {
                                    task.curFre = task.frequency;
                                    ActionList.Enqueue(task.handleOnEvent);
                                }
                            }

                        }

                    }
                }
            }
            //释放读锁
            m_readerWriterLock.ExitReadLock();
            #endregion

            //申请写锁
            m_readerWriterLock.EnterWriteLock();
            if(TaskDic != null) {
                foreach(int key in ToBeRemoved) {
                    TaskDic.Remove(key);
                }
            }
            ToBeRemoved.Clear();
            //释放写锁
            m_readerWriterLock.ExitWriteLock();

            // --------- 开始执行 ----------

            ThreadPool.QueueUserWorkItem (
                (actions) => {
                    Action[] WorkList = (actions as Queue<Action>).ToArray();
                    if(WorkList != null) {
                        foreach(Action work in WorkList) {
                            if(work != null) {
                                try {
                                    work();
                                } catch(Exception ex) {
                                    ConsoleEx.DebugLog(ex.Message);
                                }
                            }
                        }
                    }
                }, ActionList
            );

            //开始下一个时间任务
            threadTimer.Change(IntervalPeriod, Timeout.Infinite);
        }

    /*
     * UI controller will pass the end of event timing.
     * 
     * return value : 
     *     1. -1 has ocurred
     *     2. -2 don't begin yet
     *     3. >= 0 remaining time
     */ 
        public const int HAS_OCCURED = -1;
        public const int NOT_BEGIN_YET = -2;
        public long getRemainingOnNonRealTime(long startTime, long endTime) {
            if(endTime <= 0)
                return HAS_OCCURED;
            else {
                if(curUtc > endTime) 
                    return HAS_OCCURED;
                else if (curUtc < startTime) {
                    return NOT_BEGIN_YET;
                } else {
                    return endTime - curUtc;
                }
            }
        }

        /// <summary>
        /// Dispatchs to timer. Multi-thread safe. 不允许空的TimrTask加入进来
        /// </summary>
        /// <param name="task">Task. If task equals Null, we will ignore it.</param>
        public void dispatchToTimer(TimerTask task) {
            if(task != null) {
                m_readerWriterLock.EnterWriteLock();
                if(TaskDic != null) {
                    mIndex = (mIndex + 1) % MAX_TIMER_TASK_COUNT;
                    task._Id = mIndex;
                    TaskDic[mIndex] = task;
                }
                m_readerWriterLock.ExitWriteLock();
            }
        }

		#region 删除Task的各种方法

        //线程安全
        public void deleteTask(TimerTask task) {
            if(task != null) {
                m_readerWriterLock.EnterWriteLock();
                if(TaskDic != null) {
                    if(TaskDic.ContainsKey(task._Id)) {
                        TaskDic.Remove(task._Id);
                    }
                }
                m_readerWriterLock.ExitWriteLock();
            }
        }

		//线程安全
		//added by zhangqiang 
		public void deleteTask(int taskID) 
		{
			m_readerWriterLock.EnterWriteLock();
			if(TaskDic != null)
			{
				if(TaskDic.ContainsKey(taskID))
				{
					TaskDic.Remove(taskID);
				}
			}
			m_readerWriterLock.ExitWriteLock();
		}

        /// <summary>
        /// 清除所有的摸个类型的Timer,线程安全
        /// </summary>
        /// <param name="taskID">Task I.</param>
        public void deleteTask(TaskID taskID) {
            List<int> rm = new List<int>();

            m_readerWriterLock.EnterReadLock();
            if(TaskDic != null) {
                foreach(TimerTask task in TaskDic.Values) {
                    if(task.taskId == taskID) {
                        rm.Add(task._Id);
                    }
                }
            }
            m_readerWriterLock.ExitReadLock();

            m_readerWriterLock.EnterWriteLock();
            if(rm.Count > 0) {
                foreach(int id in rm)
                    TaskDic.Remove(id);
            }
            m_readerWriterLock.ExitWriteLock();
        }

        /// <summary>
        /// 清除多个类型的TimerTask
        /// </summary>
        /// <param name="IDList">Identifier list.</param>
        public void deleteTask(TaskID[] IDList) {
            List<int> rm = new List<int>();
            m_readerWriterLock.EnterReadLock();

            if(TaskDic != null) {
                foreach(TimerTask task in TaskDic.Values) {
                    foreach(TaskID ti in IDList) {
                        if(task.taskId == ti) {
                            rm.Add(task._Id);
                        }
                    }
                }
            }
            m_readerWriterLock.ExitReadLock();

            m_readerWriterLock.EnterWriteLock();
            if(rm.Count > 0) {
                foreach(int id in rm)
                    TaskDic.Remove(id);
            }
            m_readerWriterLock.ExitWriteLock();
        }


		/// 
		/// 清楚所有的Task
		/// 
		public void deleteAllTask() {
			m_readerWriterLock.EnterWriteLock();
			if(TaskDic != null) {
				TaskDic.Clear();
			}
			m_readerWriterLock.ExitWriteLock();
		}

		#endregion


        public List<long> GetLeftTime(TaskID taskID) {
            List<long> lefttime = new List<long>();

            m_readerWriterLock.EnterReadLock();
            if(TaskDic != null) {
                foreach(TimerTask task in TaskDic.Values) {
                    if(task.taskId == taskID) {
                        lefttime.Add(task.leftTime);
                    }
                }
            }
            m_readerWriterLock.ExitReadLock();

            return lefttime;
        }


       
        public bool checkExist(TaskID taskID) {
            bool found = false;

            m_readerWriterLock.EnterReadLock();
            foreach(TimerTask task in TaskDic.Values) {
                if(task.taskId == taskID) {
                    found = true;
                    break;
                }
            }
            m_readerWriterLock.ExitReadLock();

            return found;
        }

		/// ----------------------------------------------------------------
        /// ----------------------- inherite from interface ----------------
		/// ----------------------------------------------------------------
		public void OnLogin(StateParam<GameState> obj) {
			LoginInfo log = obj.obj as LoginInfo;
			cachedServerTime = (int)log.logUtc;
            if(threadTimer != null && IsRunning == false ) {
                threadTimer.Change(IntervalPeriod, Timeout.Infinite);
            }
        }

		public void OnUnregister(StateParam<GameState> obj) {
			threadTimer.Dispose();
			threadTimer = null;

			deleteAllTask();
			TaskDic = null;
		}

		public void OnDayChanged(StateParam<GameState> obj) { }

		public void OnLevelChanged(StateParam<GameState> obj) { }


		/// 
		/// 暂停游戏的运行
		/// 
		public void OnPaused(StateParam<DeviceState> obj) {
			ConsoleEx.DebugLog("TimerMaster OnPause", ConsoleEx.RED);
			UtcWhenPaused = DateHelper.DateTimeToUnixTimeStamp(DateTime.UtcNow);
		}

		/// 
		/// 恢复游戏的时候, 流逝了多少时间，Timer就补上多少时间
		/// 
		public void OnResume(StateParam<DeviceState> obj) {
			ConsoleEx.DebugLog("TimerMaster OnResume : UtcOnPause = " + UtcWhenPaused, ConsoleEx.RED);
			if(UtcWhenPaused > 0) {
				long UtcOnResume = DateHelper.DateTimeToUnixTimeStamp(DateTime.UtcNow);
				long left = UtcOnResume - UtcWhenPaused;
				if(left > 0) cachedServerTime = curUtc + left;
			}
		}
		public void OnGameLaunched(StateParam<DeviceState> obj) { }
		public void OnQuit(StateParam<DeviceState> obj) { 
			threadTimer.Dispose();
			threadTimer = null;

			deleteAllTask();
			TaskDic = null;
		}
    }
}