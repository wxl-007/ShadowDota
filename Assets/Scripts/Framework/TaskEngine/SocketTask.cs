using System;
using System.Collections.Generic;

public class SocketTask : BaseTaskAbstract {

	/* The code will be excute after we get response
	 */ 
	public Action<BaseSocketRequest, BaseResponse> afterCompleted;

	/* When Error Occured, it will notify user
	 */ 
	public Action<BaseSocketRequest, string> ErrorOccured;

	//this will be fullfilld when create instance
	public BaseSocketRequest request;
	//this will be fullfilled after http response is coming.
	public BaseResponse response;
	//we should know the relationship between Request with BaseBuildingData
	public RelationShipReqAndResp relation;
	//we will store the error Info if Json Parser has ouccerd
	public string errorInfo;

	public SocketTask (ThreadType threadType, TaskResponse respType = TaskResponse.Default_Response) {
		this.type = TaskType.SocketTask;
		this.threadType = threadType;
		this.respType = respType;
	}

	/*
	 * This is for creating Common Socket Task
	 */ 
	public void AppendCommonParam (RequestType requestType, BaseRequestParam param) {
		this.request = SocketRequestFactory.createHttpRequestInstance(requestType, param);
		this.relation = SocketRequestFactory.getRelationShip(requestType);
	}

	/*
	 * This is for creating command in the Socket client
	 */
	public void AppendCmdParam (InternalRequestType requestType) {
		this.request = new SockInternalRequest(requestType);
	}

	public override void DispatchToRealHandler() {
		Core.NetEng.SockEngine.sendSocketTask(this);
	}

	public virtual void handleErrorOcurr() {
		if(ErrorOccured != null ) {
			if(!string.IsNullOrEmpty(errorInfo))
				ErrorOccured(request, errorInfo);
			else {
				if(response != null) {
					ExceptionResponse ex = response as ExceptionResponse;
					if(ex != null) 
						ErrorOccured(request, ex.HttpError);
				}
			}

		} 
	}

	public override void handleBackGroundCompleted() {
		base.handleBackGroundCompleted();

		if(afterCompleted != null) {
			try {
				afterCompleted(request, response);
			} catch (Exception ex){
				ConsoleEx.DebugLog(ex.ToString());
			}
		}

	}

	public override void handleMainThreadCompleted() {
		base.handleMainThreadCompleted();
		if(afterCompleted != null)
			afterCompleted(request, response);
	}


	/*	
     * This is internal routine called after task is completed
     * this lives in the background thread.
     * 
     * I will define doWork.
     */ 
	public virtual void handleCompletedInternal(Action<BaseSocketRequest, BaseResponse> doWork) {
		if(doWork != null) {
			doWork(request, response);
		}
	}

}
