using System;

public class BaseResponse
{

	public static short ERROR = 0x02;
	public int act;
	public short status;
	public int errorCode;
    //这是一个同步的状态（只有在一部分的网络响应的时候才有值）
    //public loginInfo sync;
	
	//这是任务相关,如果这个值>0说明有任务task完成
	public int[] task;
	
	
	protected string _Json;

	/*	*  check if this response has error **/
	public virtual bool hasError ()
	{
		if (errorCode == ERROR) 
			return Consts.FAILURE;
		else 
			return Consts.OK;
	}

	// return error code
	public int getErrorCode ()
	{
		return errorCode;
	}

	/**
	 *  we must implements this method to do your job here!
	 *  Return Value : false means wait for user's operation
	 *  			   true means run following codes
	 * 
	 */ 
	public virtual void handleResponse () {

	}


}