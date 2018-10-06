using System;
using System.Text;
using System.Collections.Generic;
using fastJSON;

[Serializable]
public class BaseRequestParam
{
    public BaseRequestParam() {

    }

    /**
     * we use Json library to generate Parameter string
     */
    public virtual string generatePara() {
		return JSON.Instance.ToJSON(this);
    }
}

[Serializable]
public class HttpRequest : BaseHttpRequest
{
    //Default HTTP GET/POST Message format
    public static readonly string ACTION = "act";
    public static readonly string DATA = "data";
    public static readonly string ERRORCODE = "errorCode";
    public static readonly string NO = "no";
    public static readonly string VERSION = "v";
    public static readonly string CRC = "crc";
	public static readonly string UNIQUE_PLATFORM_ID = "upf";
	public static readonly string RESOURCE_VERSION = "rv";
	public static readonly string CACHE = "cache";


    protected RequestType _type;
    protected int _act;
    //store parameters
    protected Dictionary<string, string> _param = new Dictionary<string, string>();
    protected StringBuilder sb = new StringBuilder();
	protected int _Version;
	protected string _PlatformId;
	//store in Memory
	protected BaseRequestParam _paramMem;

    //if we set url to this property, we are going to use this address rather than HttpClient.BaseUrl
    protected string _url;

    public RequestType Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public int Act
    {
        get { return _act; }
    }

    public string Url
    {
        get { return _url; }
		set { _url = value;}
    }

	public BaseRequestParam ParamMem 
	{
		get { return _paramMem; }
	}

	public HttpRequest(RequestType type, int swVersion, string platform, string urladd = "") : base (BaseHttpRequestType.Common_Http_Request)
    {
        _Version = swVersion;
        _url = urladd;
		_PlatformId = platform;
        if (Enum.IsDefined(typeof(RequestType), type))
            _type = type;
        else
            throw new DragonException(DragonException.Exception_Message[DragonException.INVALIDATE_ARGUMENT]);

    }

    public void setParameter(string paraKey, int paravalue)
    {
        _act = paravalue;
        _param.Add(paraKey, Convert.ToString(paravalue));
    }

    public bool setParameter(string paraKey, string paravalue)
    {
        string enValue = null;

        try
        {
            enValue = HttpUtil.UrlEncodeUnicode(paravalue);
        }
        catch (ArgumentOutOfRangeException e)
        {
			Console.Write(e.Message.ToString());

        }

        if (enValue == null)
            return false;
        else
        {
            _param.Add(paraKey, enValue);
            return true;
        }
    }
    /**
     ******************  Append Get/Post Method's parameter ****************
     */


	public void appendPara(BaseRequestParam reqParam) {
		_paramMem = reqParam;

		appendParaBegin();
		appendPara(_paramMem.generatePara());
		appendParaEnd();
	}

	private void appendParaBegin()
    {
        int range = sb.Length;
        if (range > 0)
            sb.Remove(0, range);

        sb.Append("[");
    }

	private void appendPara(string value)
    {
        if (value != null && value != string.Empty)
            sb.Append(value);
    }

	private void appendParaEnd()
    {
        sb.Append("]");
        string finVa = sb.ToString();
        setParameter(DATA, finVa);
    }

    public string toJson(HttpData_Completeness httpDataComm)
    {
        int No = 1;
        if (httpDataComm != null)
            No = httpDataComm.getHttpRequestNo(this);
		int cache = RequestTypeToCache();
        return ACTION + "=" + Convert.ToString(_act) + "&" + NO + "=" + Convert.ToString(No) +
			"&" + VERSION + "=" + Convert.ToString(_Version) +
			"&" + CRC + "=" + HashHttpRequest._________________________(Convert.ToString(_act), Convert.ToString(_Version), sb.ToString(), Convert.ToString(No), _PlatformId) +
			"&" + UNIQUE_PLATFORM_ID + "=" + _PlatformId +
			"&" + RESOURCE_VERSION + "=" + 1 +
			"&" + CACHE + "=" + cache +
            "&" + DATA + "=" + sb.ToString();
    }


	/// <summary>
	/// 返回值为1的时候，服务器会缓存该命令10分钟
	/// </summary>
	/// <returns>The type to cache.</returns>
	int RequestTypeToCache() {
		int cache = 0;
		//TODO : add logical

		return cache;
	}

	public string paramToJson {
		get {
			string param = sb.ToString();
			if (param != null && param.Length >= 2)
			{
				param = param.Substring(1);
				param = param.Substring(0, param.Length - 1);
			}
			return param;
		}
    }
}