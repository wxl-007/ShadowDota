using System;
using System.Collections;

public class HttpTask : BaseTaskAbstract {

	/* The code will be excute after we get response
	 */ 
	public Action<BaseHttpRequest, BaseResponse> afterCompleted;

	/* When Error Occured, it will notify user
	 */ 
	public Action<BaseHttpRequest, string> ErrorOccured;

	//this will be fullfilld when create instance
	public BaseHttpRequest request;
	//this will be fullfilled after http response is coming.
	public BaseResponse response;
	//we should know the relationship between Request with BaseBuildingData
	public RelationShipReqAndResp relation;
	//we will store the error Info if Json Parser has ouccerd
	public string errorInfo;

    //发出去的Filter
    public HttpOutFilter OutFilter;
    //尚未使用HttpInFilter
    public HttpInFilter InFilter;

	public HttpTask (ThreadType threadType, TaskResponse respType = TaskResponse.Default_Response) {
		this.type = TaskType.HttpTask;
		this.threadType = threadType;
		this.respType = respType;
        this.OutFilter = new HttpOutFilter();
	}

	/*
	 * This is for creating Common Http Task
	 */ 
	public virtual void AppendCommonParam (RequestType requestType, BaseRequestParam param) {
		this.request = HttpRequestFactory.createHttpRequestInstance(requestType, param);
		this.relation = HttpRequestFactory.getRelationShip(requestType);
		this.OutFilter.request = requestType;
	}

	/*
	 * This is for creating command in the http client
	 */
	public void AppendCmdParam (InternalRequestType requestType) {
		this.request = new InternalRequest(requestType);
	}

	/*
	 * This is for creating third party task
	 */
	public virtual void AppendThirdParam(ThirdPartyRequestType requestType, string param) {
		this.request = new ThirdPartyHttpRequest(requestType, param);
	}

	/// <summary>
	/// 异步方法，必须等待回调
	/// </summary>
	public override void DispatchToRealHandler() {
        Core.NetEng.httpEngine.sendHttpTask(this);
    }

	/// <summary>
	/// 同步的方法，会block当前线程
	/// </summary>
	public void DispatchImmediatly() {
		Core.NetEng.httpEngine.RunImmediatly(this);
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
			afterCompleted(request, response);
		}

	}

	public override void handleMainThreadCompleted() {
		base.handleMainThreadCompleted();
		if(afterCompleted != null)
			afterCompleted(request, response);
	}

    #region 取走ErrorInfo，以便重试的时候能设置为空置
    public string takeErrorOver {
        get {
            string error = errorInfo;
            errorInfo = string.Empty;
            return error;
        }
    }
    #endregion
}
