using System;
using System.Collections.Generic;
using System.Threading;

public class DragonBallTimer {
	//This Timer is the Best one, *****  don't use System.Timers.Timer ****
	private System.Threading.Timer threadTimer = null;

	//millisecend
	private const int IntervalPeriod = 1000;
	private const int Capacity = 30;

	private Thread_Safe_Linkedlist<TimerTask> taskList = null;
	//This is the Temp list which contains TimerTask.
	private List<TimerTask> tobeDelete = null;

    //服务器的时间，每一段时间就会同步一次
    private long cachedServerTime;
    //计时模块的计时器
	private long curUtc;

    //当前的时间有可能会出现暂时为0的情况，因为第一次设定时间会先放入缓存中
	public long curTime {
        get { return curUtc == 0 ? cachedServerTime : curUtc ; }
		set { curUtc = value; }
	}

	public DragonBallTimer() {
		//DueTime to TimeOut.Infinite to prevent the timer from starting
		//Period Specify Timeout.Infinite to disable periodic signaling, callback routine run once.
		threadTimer = new System.Threading.Timer(new TimerCallback(ComputeBoundOp), null, Timeout.Infinite, Timeout.Infinite);
		taskList = new Thread_Safe_Linkedlist<TimerTask>();
		tobeDelete = new List<TimerTask>();

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

		try {
			IsRunning = true;

			if(cachedServerTime > 0) {
				curUtc = cachedServerTime;
				cachedServerTime = 0;
			}

			curUtc ++;

			if (taskList != null) {

				tobeDelete.Clear();

				foreach (TimerTask task in taskList) {
					if(task != null) {

						task.leftTime = task.endTime - curUtc;
						task.leftTime = task.leftTime <= 0 ? 0 : task.leftTime;

						if(task.leftTime == 0 && task.endTime != TimerTask.INFINITY) 
							tobeDelete.Add(task);

						if(task.startTime == curUtc) {
							task.handleStart();
						}

						if(task.endTime == curUtc) {
							task.handleCompleted();
						}

						if(curUtc > task.startTime && (curUtc < task.endTime || task.endTime == TimerTask.INFINITY)) {

							if(task.frequency == TimerTask.NO_FREUENCY) {
								// do thing ... 
							} else {
								if(task.curFre > 1) task.curFre --;
								else {
									task.curFre = task.frequency;
									try {
										if(task.onEvent != null) task.onEvent(task);
									} catch(Exception ex) {
										ConsoleEx.DebugLog(ex.ToString());
									}

								}
							}

						}

					}
				}

				try {
					if(tobeDelete.Count > 0) {
						foreach(TimerTask task in tobeDelete) {
							taskList.Remove(task);
						}
					}
				} catch(Exception ex) {
					ConsoleEx.DebugLog(ex.ToString());
				}

			}
		} catch (Exception ex) {
			ConsoleEx.DebugLog(ex.ToString());
		} finally {
			threadTimer.Change(IntervalPeriod, Timeout.Infinite);
		}

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
	/// Dispatchs to timer. Multi-thread safe.
	/// </summary>
	/// <param name="task">Task. If task equals Null, we will ignore it.</param>
	public void dispatchToTimer(TimerTask task) {
		if(task != null && taskList != null) {
			taskList.Add(task);
		}
	}

    //线程安全
	public void deleteTask(TimerTask task)
	{
		if(task != null && taskList != null) {
			taskList.Remove(task);
		}
	}

    /// <summary>
    /// 清除所有的摸个类型的Timer,线程安全
    /// </summary>
    /// <param name="taskID">Task I.</param>
    public void deleteTask(TaskID taskID) {
        List<TimerTask> clear = new List<TimerTask>();
        if(taskList != null) {
            foreach(TimerTask task in taskList) {
                if(task.taskId == taskID) {
                    clear.Add(task);
				}
			}

            taskList.Remove(clear);
		}
	}

	public List<long> GetLeftTime(TaskID taskID)
	{
		List<long> lefttime = new List<long>();
		if(taskList != null) 
		{
			foreach(TimerTask task in taskList)
			{
				if(task.taskId == taskID) 
				{
					lefttime.Add(task.leftTime);
				}
            }
		}
		return lefttime;
     }

        
        /// <summary>
        /// 清除多个类型的TimerTask
    /// </summary>
    /// <param name="IDList">Identifier list.</param>
    public void deleteTask(TaskID[] IDList) {
        List<TimerTask> clear = new List<TimerTask>();
        if(taskList != null) {
            foreach(TimerTask task in taskList) {
                foreach(TaskID ti in IDList) {
                    if(task.taskId == ti) {
                        clear.Add(task);
                        break;
                    }
                }
            }

            taskList.Remove(clear);
        }
    }


	public bool checkExist(TaskID taskID) {
		bool found = false;
		if(taskList != null) {
			foreach(TimerTask task in this.taskList) {
				if(task.taskId == taskID) {
					found = true;
					break;
				}
			}
		}
		return found;
	}

	// ----------------------- inherite from interface ----------------
	public void Dispose() {
		threadTimer.Dispose();
		threadTimer = null;

		taskList.Clear();
		taskList = null;
	}

	//after login response is returned.
	//this should be called
    public void OnLogin(long logUtc) {
        cachedServerTime = logUtc;

        if(threadTimer != null && IsRunning == false) {
			threadTimer.Change(IntervalPeriod, Timeout.Infinite);
		}
	}

}
