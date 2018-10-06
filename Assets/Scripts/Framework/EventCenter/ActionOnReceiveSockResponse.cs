using System;
using System.Collections.Generic;


namespace AW.Event {
	public class ActionOnReceiveSockResponse {

		public readonly static Dictionary<RequestType, Action<BaseSocketRequest, BaseResponse>> ACTION_LIST = new Dictionary<RequestType, Action<BaseSocketRequest, BaseResponse>>() {
		};

		public static Action<BaseSocketRequest, BaseResponse> getAction(SocketTask task) {
			if(task != null) {
				SocketRequest request = task.request as SocketRequest;
				if(request != null && ACTION_LIST.ContainsKey(request.Type))
					return ACTION_LIST[request.Type];
				else
					return null;
			} 
			else 
				return null;
		}
	}

}