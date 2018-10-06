using System;

public class HttpDownloadTask : HttpTask {

	public Action<string> DownloadStart;
	public Action<long,long> Report;

	public long TotalSize {
		get;
		private set;
	}

	public HttpDownloadTask (ThreadType threadType, TaskResponse respType = TaskResponse.Default_Response) : base(threadType, respType) {

	}

	/*	
	 * This is for creating download task
	 */ 
	public void AppendDownloadParam(string URL, string fn, string path, string md5, long size) {
		TotalSize = size;
		this.request = new HttpDownloadRequest(URL, fn, path, md5);
	}


	public void handleStart() {
		HttpDownloadRequest mRequest = this.request as HttpDownloadRequest;
		if(threadType == ThreadType.BackGround) {
			if(mRequest != null && DownloadStart != null)
				DownloadStart(mRequest.url);
		} else {
			AsyncTask.QueueOnMainThread( () => { if(mRequest != null && DownloadStart != null ) DownloadStart(mRequest.url);} );
		}
	}

	public void handleReport(long cur) {
		if(threadType == ThreadType.BackGround) {
			if(Report != null)
				Report(cur, TotalSize);
		} else {
			AsyncTask.QueueOnMainThread( () => { if(Report != null) Report(cur, TotalSize);} );
		}
	}


}


