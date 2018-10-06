using System;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using AW.Framework;

public class HttpClient
{
	public const int TIME_OUT = 10000; // Milliseconds 
	private const int BIT_BUFFER_SIZE = 8192;
	private const int RETRY_TIMES = 2;
    private const int MAX_URL_LENGTH = 1024; //最大URL的长度
	
	//WE HAVE ABILITY TO CHANGE THE BASE URL
	//This url is only for game logical, not for third paty nor download resources.
	private static string baseUrl = null;
	public static string BaseUrl {
		private get { 
			if(string.IsNullOrEmpty(baseUrl)) {
				baseUrl = USER_CENTER;
			}
            
			return baseUrl;
		}
		set {
			baseUrl = value;
		}
	} 

	/// 
	/// 这个值会在游戏一启动的时候就被复制，此值被存放在Engine.cfg配置中
	/// 
	public static string USER_CENTER;

	/// <summary>
	/// 回滚为用户中心的地址
	/// </summary>
	public static void RevertToUserCenter() {
		BaseUrl = USER_CENTER;
	}

	/// 
	/// 判定当前是否 依然是用户中心
	/// 
	public static bool IsStillInUserCenterMode() {
		return BaseUrl == USER_CENTER;
	}

	private const string USERAGENT = "dragonball@redwx";
	// used to build entire input
	private StringBuilder sb; 
	// used on each read operation
	private byte[] buf; 

	private HttpClient ()
	{
		sb = new StringBuilder ();
		buf = new byte[BIT_BUFFER_SIZE];
		ServicePointManager.DefaultConnectionLimit = 20;
	}
	private static HttpClient clientEnd;
	public static HttpClient getInstance ()
	{
		if (clientEnd == null)
			clientEnd = new HttpClient ();
		return clientEnd;
	}

	//游戏的内部逻辑请求
	public string doRequest (HttpRequest req, HttpData_Completeness httpDataCom)
	{
		string param = req.toJson(httpDataCom);
		if (string.IsNullOrEmpty(req.Url))
			return doRequest(param, BaseUrl);
		else
			return doRequest(param, req.Url);
	}

    //直接调用的话，一般用于第三方的网络请求
	//由内部方法调用，用于用户的请求
	public string doRequest (string param, string hostAddress)
	{	

        #if localhost
        if(param.Length > MAX_URL_LENGTH)
            return doRequest(hostAddress, 1, param);
        else
            return doRequest(hostAddress + "&" + param);
        #else
        if(param.Length > MAX_URL_LENGTH)
            return doRequest(hostAddress, 1, param);
        else {
			if(hostAddress.IndexOf('?') != -1){
				return doRequest(hostAddress + "&" + param);
			} else {
				return doRequest(hostAddress + "?" + param);
			}
		}
        #endif
	}
	
	
    /// <summary>
    /// 这是一个直接使用URL向服务器发送请求的方法
    /// </summary>
    public string doRequest (string urlReq, int times = 1, string param = null) {
		bool isExceOcurr = false;

		string strResponse = null;
		if (sb.Length >= 1)
			sb.Remove (0, sb.Length);

        ConsoleEx.Write("Http Request is going out : => " + urlReq);
		// prepare the web page we will be asking for
		HttpWebRequest request = (HttpWebRequest) WebRequest.Create (urlReq);

		request.UserAgent = USERAGENT;
		request.Method = "POST";
		// Set the 'Timeout' property in Milliseconds.
		request.Timeout = TIME_OUT;

        if(!string.IsNullOrEmpty(param)) {
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] buffer = Encoding.Default.GetBytes(param);
            request.ContentLength = buffer.Length;

            Stream dataStream = null;
            try {
                dataStream = request.GetRequestStream();
                dataStream.Write(buffer, 0, buffer.Length);
            } catch(Exception ex) {
                isExceOcurr = true;
                ConsoleEx.DebugLog ( "###### Exception = " + ex.ToString ());
            } finally {
                if(dataStream != null) dataStream.Close();
            }

            if(isExceOcurr) return strResponse;
        }

		// execute the request 
		// This request will throw a WebException if it reaches the timeout limit before it is able to fetch the resource.
		Stream resStream = null;
		HttpWebResponse response = null;
		try {
			response = (HttpWebResponse)request.GetResponse ();

			// we will read data via the response stream
			resStream = response.GetResponseStream ();

			string tempString = null;
			int count = 0;

			do {
				// fill the buffer with data
				count = resStream.Read (buf, 0, buf.Length);

				// make sure we read some data
				if (count != 0) {
					// translate from bytes to ASCII text
					tempString = Encoding.ASCII.GetString (buf, 0, count);

					// continue building the string
					sb.Append (tempString);
				}
			} while (count > 0); // any more data to read?

		} catch (WebException ex) {
			isExceOcurr = true;
			ConsoleEx.DebugLog( "###### WebException = " + ex.ToString () + "\nex.Message = " + ex.Message + "\nEx.status = " + ex.Status.ToString());
		} catch (System.Exception ex){
			isExceOcurr = true;
			ConsoleEx.DebugLog ( "###### Exception = " + ex.ToString ());
		} finally {
			if (resStream != null) {resStream.Close ();	resStream = null;}
			if(response != null) {response.Close (); response = null;}
			if(request != null) {request.Abort(); request = null;}

			if(!isExceOcurr) {
				strResponse = sb.ToString ();
				if( !string.IsNullOrEmpty(strResponse) && strResponse.Length > 3) {
					if(strResponse.StartsWith("???")) 
						strResponse = strResponse.Substring(3);
				}

				System.GC.Collect();
			} else {
				System.GC.Collect();

				if(times < RETRY_TIMES) {
					//If exception is ocurr, we try again after a few second.
					Thread.Sleep(500);
					strResponse = doRequest(urlReq, ++ times, param);
				}
			}


		}

		return strResponse;
	}

	public bool doDownload (HttpDownloadTask task) {
		Utils.Assert(task == null, "Download Resources Task in Null.");

		HttpDownloadRequest DownReq = task.request as HttpDownloadRequest;

		string urlDown = DownReq.url, whereToBeSaved = DownReq.whereToBeSaved;

		bool isExceOcurr = false;

		ConsoleEx.Write("Http Download is going out : => " + urlDown);
		task.handleStart();

		HttpWebRequest request = (HttpWebRequest) WebRequest.Create (urlDown);

		request.Method = "GET";
		// Set the 'Timeout' property in Milliseconds.
		request.Timeout = TIME_OUT;
		// execute the request 
		// This request will throw a WebException if it reaches the timeout limit before it is able to fetch the resource.
		Stream resStream = null;
		HttpWebResponse response = null;
		FileStream fs = null;
		try {
			response = (HttpWebResponse)request.GetResponse ();

			// we will read data via the response stream
			resStream = response.GetResponseStream ();
			// we will open the file stream whatever 
			fs = File.Create(whereToBeSaved);

			int count = 0;
			long curPrgress = 0;

			do {
				// fill the buffer with data
				count = resStream.Read (buf, 0, buf.Length);
				curPrgress += count;
				task.handleReport(curPrgress);
				// make sure we read some data
				if (count > 0) {
					fs.Write(buf, 0, count);
				}
			} while (count > 0); // any more data to read?

		} catch (WebException ex) {
			isExceOcurr = true;
			ConsoleEx.DebugLog( "###### WebException = " + ex.ToString ());
		} catch (System.Exception ex){
			isExceOcurr = true;
			ConsoleEx.DebugLog ( "###### Exception = " + ex.ToString ());
		} finally {
			if (resStream != null) {resStream.Close ();	resStream = null;}
			if(response != null) {response.Close (); response = null;}
			if(request != null) {request.Abort(); request = null;}
			if(fs != null) {fs.Flush(); fs.Close(); fs = null;}

			System.GC.Collect();
		}


		return isExceOcurr;
	}

}
