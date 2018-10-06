using System;

/*
 * We should tell the Task System that what kind of task is going to excute.
 */
public enum TaskType {
    HttpTask = 0x00,
    SocketTask = 0x01,
    TimeSensitiveTask = 0x02,
    ResourceTask = 0x03,
	HttpTaskEx   = 0x04,
}

/* This task should be callback in the background thread or MainThread
 */ 
public enum ThreadType{
    BackGround = 0x00,
    MainThread = 0x01,
}

/*We should ignore the responsz?
*/
public enum TaskResponse {
	Default_Response = 0x00,
	Igonre_Response = 0x01,
	Donot_Send = 0x02,
}

public enum TaskID {
	None,
}

public abstract class BaseTaskAbstract
{
    public TaskType type;
    public ThreadType threadType;
	public TaskResponse respType;

    //A auto-increased unique id
	public TaskID taskId = TaskID.None;

    //No param, No Result value, 
    public Action taskCompeleted;

    /* 
     * this Class is Base class, so subclass must inplement this fuction.
     * we will dispatch to the real controll.
     */
    public abstract void DispatchToRealHandler();

	/*
     * This is going to run the callback routine
     * The code is running under which thread is depands on the ThreadType.
     */ 
	public virtual void handleBackGroundCompleted()
    {
		if (taskCompeleted != null) {
			//We add try {} catch {} is to make sure background thread won't crash
			try {
				taskCompeleted();
			} catch(Exception ex) {
				ConsoleEx.DebugLog(ex.ToString());
			}
		}
    }

	/*
     * This is going to run the callback routine
     * The code is running under which thread is depands on the ThreadType.
     */
	public virtual void handleMainThreadCompleted()
    {
		if (taskCompeleted != null)
			taskCompeleted();
    }

}
