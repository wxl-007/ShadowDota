using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using System.Net;
using System.IO;
using System.IO.Compression;

using System.Security.Cryptography.X509Certificates;

using System.Threading;
using LuaInterface;

public class HttpLuaClient : MonoBehaviour{
	
	protected string uri 	 = null;
	protected string luaname = null;

	private HttpLuaClient intance;

	
    //正常回调函数
    public delegate void ICallback(object result, LuaFunction lf);
    public delegate void IErrorEvent(string sendData, LuaFunction ef, Exception e);

    //发送异常类
    public class IException : IOException {
        public IException(): base(){}
        public IException(string message): base(message){}
        public IException(string message, Exception innerException):base(message, innerException){}
    }

    //同步容器
    private SynchronizationContext syncContext = SynchronizationContext.Current;
    private SynchronizationContext SynchronizationContext{
        get {
            if (syncContext == null){
                syncContext = new SynchronizationContext();
            }
            return syncContext;
        }
        set {
            syncContext = value;
        }
    }

    //异步调用
	internal class AsyncInvokeContext{
        private string      sendData;           //要发送的数据

		private ICallback   callback;           //回调函数
        private IErrorEvent errorCallback;      //系统错误处理

        private LuaFunction lFunction;          //Lua回调函数
        private LuaFunction lerrorFunction;     //Lua错误处理回调函数

		private HttpLuaClient client;
        private SynchronizationContext syncContext;

        public AsyncInvokeContext(HttpLuaClient client, string sendData){
            this.client        = client;
            this.sendData      = sendData;        

			//-----------
			this.callback      = client.LuaCallBack;
			this.errorCallback = client.LuaErrorCallBack;
			this.syncContext   = client.SynchronizationContext;
        }

        public void Invoke(){
            try {
                client.BeginSendAndReceive(client.DoOutput(sendData),new AsyncCallback(SendAndReceiveCallback));
            }
            catch (Exception e) {
                DoError(e);
            }
        }

        //发送结束回调
        private void SendAndReceiveCallback(IAsyncResult asyncResult){
            try {
                string result = client.DoInput(client.EndSendAndReceive(asyncResult));
                syncContext.Post(new SendOrPostCallback(DoCallback), result);
            }
            catch (Exception e) {
                DoError(e);
            }
        }           

        //错误处理回调
        private void DoError(object e) {
            if (errorCallback != null) {
                syncContext.Post(new SendOrPostCallback(DoErrorCallback), e);
            }
        }

        public void setLFunction(LuaFunction lFunction){
            this.lFunction = lFunction;
        }

        public void setLerrorFunction(LuaFunction lerrorFunction){
            this.lerrorFunction = lerrorFunction;
        }

        //错误处理函数
        protected void DoErrorCallback(object e){
            errorCallback(this.sendData, this.lerrorFunction, (Exception) e);
        }

        //发送返回结果
        protected void DoCallback(object revData){
            callback(revData, this.lFunction);
        }
    }

    //发送错误处理
    public void OnError(string msg, Exception e){}
		
	//异步发送，C#中使用
	public bool sendData(string sendData, ICallback callback) {
		//Debug.Log("C#::HttpLuaClient::asyncSend(" + sendData + ", " + callback.ToString() + ")");
		(new AsyncInvokeContext(this, sendData)).Invoke();
		
		return true;
	}

    //同步发送
    public string send(string sendData) {
		//Debug.Log("C#::HttpLuaClient::syncSend(" + sendData + ")");
        string result = DoInput(SendAndReceive(DoOutput(sendData)));
		//Debug.Log("C#::HttpLuaClient::syncSend.Return = " + result);

        return result;
    }
	
	//-------------------------------------------------
	//异步发送
	public bool send(string sendData, LuaFunction lf) {
		AsyncInvokeContext aic = new AsyncInvokeContext(this, sendData);
		aic.setLFunction(lf);

		//Debug.Log("C#::HttpLuaClient::asyncSend(" + sendData + ", " + lf + ")");

		aic.Invoke();
		return true;
	}
	
    //异步发送
	public void send(string sendData, LuaFunction lf, LuaFunction ef) {
        AsyncInvokeContext aic = new AsyncInvokeContext(this, sendData);
        aic.setLFunction(lf);
        aic.setLerrorFunction(ef);

		//Debug.Log("C#::HttpLuaClient::asyncSend(" + sendData + ", " + lf.ToString()  + ", " + ef.ToString() + ")");

        aic.Invoke();
    }

    //--------------------------------------
    private string DoOutput(string sendData){
        return sendData;
    }
    private string DoInput(string recData){
        return recData;
    }   

    //回调Lua函数，回调到Lua 的解析函数
    protected void LuaCallBack(object recData, LuaFunction lFunction){
		//Debug.Log("C#::HttpClient::asyncSend.return(" + recData + ", " + lFunction.ToString() + ")");

		//Debug.Log ("sssss" + this.luaname);

		AsyncTask.QueueOnMainThread(
			() => 
			{
				//Debug.Log ("sssss" + this.luaname);
//			    LuaScriptMgr lua =  LuaManager.Instance.GetLua(this.luaname);
//			    if(lua != null){
//					lua.CallLuaFunction("sendCallBack", new object[]{recData, lFunction});
//				}
				LuaManager.Instance.CallFunction(this.luaname,"sendCallBack", new object[]{recData, lFunction});
		     }
		);

		//Debug.Log("C#::HttpClient::asyncSend.callBack(" + recData + ", " + lFunction.ToString() + ")");
    }        

    //回调Lua函数，回调到Lua 的错误处理函数
    protected void LuaErrorCallBack(string sendData, LuaFunction elFunction, Exception e){
		//Debug.Log("C#::HttpClient::asyncSend.error(" + sendData + ", " + elFunction.ToString() + ", " + e.Message + ")");

		AsyncTask.QueueOnMainThread(
			() => {LuaTools.Invoke("NetInterFace.hprose_callback", "sendCallBack", 10, new object[]{sendData, elFunction, e});}
		);


		//Debug.Log("C#::HttpClient::asyncSend.error(" + sendData + ", " + elFunction.ToString() + e.Message +  ")");
    }

    //################################################################################
    //################################################################################
    //################################################################################
	private static CookieContainer cookieContainer = new CookieContainer();
	private Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	
	private ICredentials credentials 	= null;
	private X509CertificateCollection clientCertificates = null;

	private int  timeout 				= 30000;
	private bool keepAlive 		 		= true;
	private int  keepAliveTimeout 		= 300;
	
	private IWebProxy proxy 	 		= null;
	private string encoding 	 		= "UTF-8";
	private string connectionGroupName 	= null;
	
    //异步调用容器
	private class AsyncContext {
		internal HttpWebRequest  request;
		internal HttpWebResponse response = null;

        //要发送的数据
		internal MemoryStream 	 		data;
		internal AsyncCallback 	 		callback;
		internal System.Threading.Timer timer;

		internal AsyncContext(HttpWebRequest request){
		    this.request = request;
		}
	}
			
    public HttpLuaClient(string luaname, string uri){
		this.uri = uri;
		this.luaname = luaname;
	}
	
	public static HttpLuaClient Create(string luaname, string uri){
		Uri u = new Uri(uri);
		if (u.Scheme != "http" && u.Scheme != "https"){
			throw new Exception("This client doesn't support " + u.Scheme + " scheme.");
		}
		
		return new HttpLuaClient(luaname, uri);
	}
	
	public void SetHeader(string name, string value){
		string nl = name.ToLower();
		
		if (nl != "content-type" && nl != "content-length" && nl != "host") {
			if (value == null) {
				headers.Remove(name);
			}
			else {
				headers[name] = value;
			}
		}
	}
	
	//同步发送
    protected string SendAndReceive(string data) {
		HttpWebRequest request = GetRequest();
		
		MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));
		Send(ms, request.GetRequestStream());
		
		HttpWebResponse response = (HttpWebResponse)request.GetResponse();
		MemoryStream receive 	 =  Receive(request, response);    
        string result = System.Text.Encoding.UTF8.GetString(receive.ToArray());

		return result;
	}
	
	//异步发送
    protected IAsyncResult BeginSendAndReceive(string data, AsyncCallback callback) {
		HttpWebRequest request = GetRequest();
		AsyncContext context   = new AsyncContext(request);
		
        context.timer 	 = new System.Threading.Timer(new System.Threading.TimerCallback(TimeoutHandler), context, timeout, 0);
		context.data 	 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));
		context.callback = callback;
		
		//开始发送
		return request.BeginGetRequestStream(new AsyncCallback(EndSend), context);
	}
	
	//异步接受
    protected string EndSendAndReceive(IAsyncResult asyncResult) {
		AsyncContext context 	 = (AsyncContext)asyncResult.AsyncState;

		HttpWebRequest  request  = context.request;
		HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);

		context.response  	 = response;
		MemoryStream receive = Receive(request, response);

		if (context.timer != null) {
			context.timer.Dispose();
			context.timer = null;
		}
		
        string result = System.Text.Encoding.UTF8.GetString(receive.ToArray());
		return result;
	}

	//超时处理
	protected void TimeoutHandler(object state) {
		AsyncContext context = (AsyncContext) state;
		try {
			if (context.response == null) {
				if (context.request != null) {
					context.request.Abort();
				}
			}else{
				context.response.Close();
			}

			if (context.timer != null) {
				context.timer.Dispose();
				context.timer = null;
			}
		}catch (Exception){}
	}
        
    //异步发送完毕
    private void EndSend(IAsyncResult asyncResult) {
        AsyncContext context = (AsyncContext) asyncResult.AsyncState;
        Send(context.data, context.request.EndGetRequestStream(asyncResult));       
        context.request.BeginGetResponse(context.callback, context);
    }

    //获取HttpClient
    private HttpWebRequest GetRequest(){
        Uri uri = new Uri(this.uri);
        HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;

        request.Method      = "POST";
        request.Timeout     = timeout;
        request.SendChunked = false;
        request.ContentType = "application/hprose";

        request.Credentials = credentials;
        request.ServicePoint.ConnectionLimit = Int32.MaxValue;

        if (encoding != null) {
            request.Headers.Set("Accept-Encoding", encoding);
        }

        request.ReadWriteTimeout = timeout;
        request.ProtocolVersion  = HttpVersion.Version11;
        if (proxy != null) {
            request.Proxy = proxy;
        }

        request.KeepAlive = keepAlive;
        if (keepAlive){
            request.Headers.Set("Keep-Alive", KeepAliveTimeout.ToString());
        }

        request.ConnectionGroupName    = connectionGroupName;
        if (clientCertificates != null) {
            request.ClientCertificates = clientCertificates;
        }

        foreach (KeyValuePair<string, string> header in headers) {
            request.Headers[header.Key] = header.Value;
        }

        request.CookieContainer = cookieContainer;      
        return request;
    }

    //发送数据
    private void Send(MemoryStream data, Stream ostream) {
        data.WriteTo(ostream);
        ostream.Flush();
        ostream.Close();
    }

    //接受数据
    private MemoryStream Receive(HttpWebRequest request, HttpWebResponse response) {
        Stream istream = response.GetResponseStream();

        string contentEncoding = response.ContentEncoding.ToLower();
        if (contentEncoding.IndexOf("deflate") > -1) {
            istream = new DeflateStream(istream, CompressionMode.Decompress);
        }
        else if (contentEncoding.IndexOf("gzip") > -1) {
            istream = new GZipStream(istream, CompressionMode.Decompress);
        }

        //接受数据
        int len = (int)response.ContentLength;
        MemoryStream data = (len > 0) ? new MemoryStream(len) : new MemoryStream();     
        len = (len > 81920 || len < 0) ? 81920 : len;
        byte[] buffer = new byte[len];

        for (;;) {
            int size = istream.Read(buffer, 0, len);
            if (size == 0){
                break;
            }

            data.Write(buffer, 0, size);
        }

        istream.Close();
        response.Close();

        return data;
    }
        	
	public void setUrl(string url) {
			this.uri = url;
	}

	public string getUrl(){
		return this.uri;
	}

	public string GetHeader(string name) {
			return headers[name];
	}

	public int Timeout{
		get {
				return timeout;
		}
		set {
				timeout = value;
		}
	}

	public ICredentials Credentials{
		get {
				return credentials;
		}
		set {
				credentials = value;
		}
	}

	public bool KeepAlive {
		get {
				return keepAlive;
		}
		set {
				keepAlive = value;
		}
	}

	public int KeepAliveTimeout {
		get {
				return keepAliveTimeout;
		}
		set {
				keepAliveTimeout = value;
		}
	}

	public IWebProxy Proxy{
		get {
				return proxy;
		}
		set {
				proxy = value;
		}
	}

	public string AcceptEncoding{
		get {
				return encoding;
		}
		set {
				encoding = value;
		}
	}

	public string ConnectionGroupName {
		get {
				return connectionGroupName;
		}
		set {
				connectionGroupName = value;
		}
	}

	public X509CertificateCollection ClientCertificates {
		get {
				return clientCertificates;
		}
		set {
				clientCertificates = value;
		}
	}
}