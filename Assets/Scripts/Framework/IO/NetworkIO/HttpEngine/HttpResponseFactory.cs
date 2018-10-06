using System;
using fastJSON;

public static class HttpResponseFactory {

	/// <summary>
	/// 默认的错误字符
	/// </summary>
	public static string InvalidJson = "Invalid Json";

	/// <summary>
	/// task一定不为空，json也一定不为空
	/// 必定保证  1. 当errorInfo为空时，response一定有
	///          2. 当errorInfo不为空时，response则无所谓
	/// </summary>
	public static void createResponse(HttpTask task, string json) {
		ConsoleEx.Write( task.relation.respType.ToString() + " is coming back : => " + json);
		BaseResponse response = null;
		if(!string.IsNullOrEmpty(json)) {
			try {
				response = JSON.Instance.ToObject(json, task.relation.respType) as BaseResponse;

				if(response != null)  {
					response.handleResponse();
					//store in the task
					task.response = response;
				} else {
					task.errorInfo = InvalidJson;
				}
			} catch(Exception ex) {
				ConsoleEx.DebugLog(ex.ToString());
				task.errorInfo = ex.ToString();
			} 
		} else  {
			task.errorInfo = InvalidJson;
		}
	}
}
