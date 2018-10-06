using System;
using System.Collections;
using AW.Event;
using AW.Entity;

/// <summary>
/// 封装的网络处理，ControllerEx发送网络请求都经过的是这个类处理
/// 基于HttpTask的原因是，底层的网络组件依赖的HttpTask
/// 这个HttpTaskEx都不基于Action回调来完成，都是依赖EntityManager
/// 
/// 这里面所有的都不处理异常，异常都在外层捕捉
/// </summary>
public class HttpTaskEx : HttpTask {

	//消息的处理者
	private IDispatch dispatch;
	//发送网络请求的Controller的唯一ID
	public int SenderID;

	public HttpTaskEx (int fromWho, ThreadType threadType, TaskResponse respType = TaskResponse.Default_Response) : base (threadType, respType) {
		SenderID = fromWho;
		type     = TaskType.HttpTaskEx;
		dispatch = Core.EVC;
	}


	/*
	 * This is for creating Common Http Task
	 */ 
	public override void AppendCommonParam (RequestType requestType, BaseRequestParam param) {
		this.request = HttpRequestFactory.createHttpRequestInstance(requestType, param);
		this.relation = HttpRequestFactory.getRelationShip(requestType);
		this.OutFilter.request = requestType;
	}


	/*
	 * This is for creating third party task
	 */
	public override void AppendThirdParam(ThirdPartyRequestType requestType, string param) {
		this.request = new ThirdPartyHttpRequest(requestType, param);
	}

	/*
	 * 现在发送网络消息，都统一走EventCenter
	 */ 
	public override void DispatchToRealHandler() {
		if(dispatch != null)
			dispatch.Dispatch(this);
	}


	public override void handleErrorOcurr() {
		ControllerEx ctrlEx = Core.EntityMgr.getControllerByID(SenderID);
		if(ctrlEx != null) {
			if(!string.IsNullOrEmpty(errorInfo)) {
				ctrlEx.Http_ErrorOccured(request, errorInfo);
			} else {
				if(response == null) 
					response = new ExceptionResponse();
				ExceptionResponse ex = response as ExceptionResponse;
				if(ex == null) ex = new ExceptionResponse();
				ctrlEx.Http_ErrorOccured(request, ex.HttpError);
			}
		} else {
			ConsoleEx.DebugLog("Controller doesn't exist.", ConsoleEx.YELLOW);
		}
	}

	public override void handleBackGroundCompleted() {
		ControllerEx ctrlEx = Core.EntityMgr.getControllerByID(SenderID);
		if(ctrlEx != null) {
			if(response.status != BaseResponse.ERROR)
				ctrlEx.Http_OnReceive_OK(request, response);
			else 
				ctrlEx.Http_OnReceive_Fail(request, response);
		} else {
			ConsoleEx.DebugLog("Controller doesn't exist.", ConsoleEx.YELLOW);
		}
	}

	public override void handleMainThreadCompleted() {
		ControllerEx ctrlEx = Core.EntityMgr.getControllerByID(SenderID);
		if(ctrlEx != null) {
			if(response.status != BaseResponse.ERROR)
				ctrlEx.Http_OnReceive_OK(request, response);
			else 
				ctrlEx.Http_OnReceive_Fail(request, response);
		} else {
			ConsoleEx.DebugLog("Controller doesn't exist.", ConsoleEx.YELLOW);
		}
	}


}
