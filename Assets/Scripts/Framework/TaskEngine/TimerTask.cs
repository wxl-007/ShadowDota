using System;
using System.Collections;

public class TimerTask : BaseTaskAbstract {

	public const int RIGHTNOW = -1;
	public const int INFINITY = -1;
	public const int NO_FREUENCY = -1;

	public readonly long startTime;
	public readonly long endTime;
	public readonly int frequency;

	// following two value only should be read by UI layer. 
	public long leftTime;
	public int curFre;

	// 被计时Engine分配的，不允许修改
	public int _Id;

	// 如果为true，则正常执行，如果为false，则会被删除
	// 这个没有添加线程同步机制，因为没必要
	public bool Enabled;

	/// <summary>
	/// When event begin, this Action will be excuted.
	/// </summary>
	public Action<TimerTask> onEventBegin;

	/// <summary>
	/// When event end, this Action will be excuted.
	/// </summary>
	public Action<TimerTask> onEventEnd;

	/// <summary>
	/// This action will be excuted per frequent. This Action must run in the background.
	/// </summary>
	public Action<TimerTask> onEvent;

	/// <summary>
	/// The thread type
	/// </summary>
	public readonly ThreadType threadTp;

	public TimerTask (long start, long end, int frequency, ThreadType type = ThreadType.BackGround) {
		if(RIGHTNOW == start)
			startTime = Core.TimerEng.curTime;
		else 
			startTime = start;

		endTime  = end;
		this.frequency = frequency;
		leftTime = 0;
		curFre   = frequency;
		threadTp = type;
		Enabled  = true;
	}

	public override void DispatchToRealHandler() {
		Core.TimerEng.dispatchToTimer(this);
	}

    #if Old_Timer
    #region 原始版本的计时器

	public override void handleMainThreadCompleted() {
		try {

			base.handleMainThreadCompleted();
			if(onEventEnd != null)
				onEventEnd(this);

		} catch(Exception ex) {
			ConsoleEx.DebugLog(ex.ToString());
		}
	}

	public override void handleBackGroundCompleted() {
		try {
			base.handleBackGroundCompleted();

			if(onEventEnd != null) 
				onEventEnd(this);
		} catch(Exception ex) {
			ConsoleEx.DebugLog(ex.ToString());
		}

	}


	/// <summary>
	///  ------------------- 下面的方法主要是提供给TimerEngine 使用的 ------------------------
	/// </summary>

	public void handleStart() {
		if(threadTp == ThreadType.BackGround) {
			try {
				if(onEventBegin != null) onEventBegin(this);
			} catch(Exception ex) {
				ConsoleEx.DebugLog(ex.ToString());
			}
		} else {
			AsyncTask.QueueOnMainThread( () => { 
				if( onEventBegin != null ) {
					try{
						onEventBegin(this);
					} catch(Exception ex){
						ConsoleEx.DebugLog(ex.ToString());
					}
				}
			} );
		}
	}

	public void handleCompleted() {
		if(threadTp == ThreadType.BackGround)
			handleBackGroundCompleted();
		else {
			AsyncTask.QueueOnMainThread( () => { handleMainThreadCompleted(); } );
		}
	}


	public void handleOnEvent() {
		try {
			if(onEvent != null) onEvent(this);
		} catch (Exception ex) {
			ConsoleEx.DebugLog(ex.Message);
		}
	}

    #endregion

    #else

    #region 优化计时器的版本

    public override void handleMainThreadCompleted() {
        base.handleMainThreadCompleted();
        if(onEventEnd != null)
            onEventEnd(this);
    }

    public override void handleBackGroundCompleted() {
        base.handleBackGroundCompleted();

        if(onEventEnd != null) 
            onEventEnd(this);
    }


    /// <summary>
    ///  ------------------- 下面的方法主要是提供给TimerEngine 使用的 ------------------------
    /// </summary>

    public void handleStart() {
        if(threadTp == ThreadType.BackGround) {
            if(onEventBegin != null) onEventBegin(this);
        } else {
            AsyncTask.QueueOnMainThread( () => { 
                if( onEventBegin != null ) {
                    onEventBegin(this);
                }
            } );
        }
    }

    public void handleCompleted() {
        if(threadTp == ThreadType.BackGround)
            handleBackGroundCompleted();
        else {
            AsyncTask.QueueOnMainThread( () => { handleMainThreadCompleted(); } );
        }
    }

    public void handleOnEvent() {
        if(onEvent != null) onEvent(this);
    }

    #endregion

    #endif


}
