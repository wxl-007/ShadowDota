using System;
using fastJSON;

public class SocketResponseFactory {
	public static void createResponse(SocketTask task, string json) {
		ConsoleEx.Write( task.relation.respType.ToString() + " is coming back : => " + json);
		BaseResponse response = null;
		if(!string.IsNullOrEmpty(json) && task != null) {
			try {
				response = JSON.Instance.ToObject(json, task.relation.respType) as BaseResponse;

				if(response != null) {
					response.handleResponse();
					//store in the task
					task.response = response;
				}
			} catch(Exception ex) {
				ConsoleEx.DebugLog(ex.ToString());
				task.errorInfo = ex.ToString();
			} 
		}
	}
}


