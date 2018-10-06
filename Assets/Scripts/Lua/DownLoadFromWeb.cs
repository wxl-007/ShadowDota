using UnityEngine;
using System.Collections;
using System.Net;
using System.Threading;
public class DownLoadFromWeb
{
	public LuaManager luaM;
	public string luaName;
	public string callback_completed;
	public string callback_progress;
	public string uri;
	public string savePath;

	//从Web上下载资源包(.zip)
	public  void DownLoad(string luaName,string uri, string savePath,string callback_completed,string callback_progress = null)
	{
		this.luaM = LuaManager.Instance;
		this.uri = uri;
		this.savePath = savePath;
		this.luaName = luaName;
		this.callback_completed = callback_completed;
		this.callback_progress = callback_progress;

		//DownLoadTask(this);
		Thread myThread = new Thread(DownLoadTask);
		myThread.Start(this);
	}
	
	public void DownLoadTask(object ParObject)
	{
		DownLoadFromWeb _this = (DownLoadFromWeb)ParObject;
		if(_this != null)
		{
			WebClient client = new WebClient();
			client.DownloadFileAsync(new  System.Uri(_this.uri), _this.savePath);
			//下载完成回调
			  client.DownloadFileCompleted += _this.DownloadCompleted;
			//下载进度条
			//if(!string.IsNullOrEmpty(this.callback_progress))
			  client.DownloadProgressChanged += _this.ProgressChanged;
		}
	}
	
	public  void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
	{
			AsyncTask.QueueOnMainThread
			(
				() => { 
	//				NGUIDebug.Log(e.BytesReceived);
	//				NGUIDebug.Log(e.TotalBytesToReceive);
	//				NGUIDebug.Log(e.ProgressPercentage);
					luaM.CallFunction(this.luaName,this.callback_progress,new object[]{e.BytesReceived,e.TotalBytesToReceive,e.ProgressPercentage});
			    }
			);
	}
	
	public  void DownloadCompleted(object sender,System.ComponentModel.AsyncCompletedEventArgs e)
	{
		    AsyncTask.QueueOnMainThread
			(
					() => { 
				       luaM.CallFunction(this.luaName,this.callback_completed,new object[]{e});
				    }
			);
	}
}
