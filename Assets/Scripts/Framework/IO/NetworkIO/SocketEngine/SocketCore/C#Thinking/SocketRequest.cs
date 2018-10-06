using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

/*
 * This is for allen internal framework to charge of SockertEngine.
 */
public class SockInternalRequest : BaseSocketRequest
{
	public InternalRequestType CommondType;

	public SockInternalRequest (InternalRequestType type) : base (BaseSocketRequestType.Internal_Control) {
		CommondType = type;
	}
}



/// <summary>
/// Socket request. In fact, it should serialize to binary array.
/// Not now, we define as HttpRequest
/// </summary>

[Serializable]
public class SocketRequest : BaseSocketRequest {

	public static readonly string ACTION = "\"act\"";
	public static readonly string DATA = "\"data\"";
	public static readonly string NO = "\"no\"";
	public static readonly string VERSION = "\"v\"";
	public static readonly string CRC = "\"crc\"";
	public static readonly string UNIQUE_PLATFORM_ID = "\"upf\"";
	public static readonly string RESOURCE_VERSION = "\"rv\"";
	public static readonly string ACTION_R = "act";

	protected RequestType _type;
	protected int _act;
	//store parameters
	protected Dictionary<string, string> _param = new Dictionary<string, string>();
	protected StringBuilder sb = new StringBuilder();
	protected int _Version;
	protected string _PlatformId;
	//store in Memory
	protected BaseRequestParam _paramMem;

	public RequestType Type
	{
		get { return _type; }
		set { _type = value; }
	}

	public int Act
	{
		get { return _act; }
	}

	public BaseRequestParam ParamMem 
	{
		get { return _paramMem; }
	}

	public SocketRequest(RequestType type, int swVersion, string platform) : base ( BaseSocketRequestType.Common_Socket_Request )
	{
		_Version = swVersion;
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

		//sb.Append("[");      wxl   
	}

	private void appendPara(string value)
	{
		if (value != null && value != string.Empty)
			sb.Append(value);
	}

	private void appendParaEnd()
	{
		//  	sb.Append("]");    wxl 
		string finVa = sb.ToString();
		setParameter(DATA, finVa);
	}

	/// <summary>
	/// 必须以/r/n结束
	/// </summary>
	/// <returns>The json.</returns>
	/*	public string toJson()
	{
		int No = 1;

		return ACTION + "=" + Convert.ToString(_act) + "&" + NO + "=" + Convert.ToString(No) +
			"&" + VERSION + "=" + Convert.ToString(_Version) +
			"&" + CRC + "=" + HashHttpRequest._________________________(Convert.ToString(_act), Convert.ToString(_Version), sb.ToString(), Convert.ToString(No), _PlatformId) +
			"&" + UNIQUE_PLATFORM_ID + "=" + _PlatformId +
			"&" + RESOURCE_VERSION + "=" + 1 +
			"&" + DATA + "=" + sb.ToString() + "\r\n";
	}
*/
	/// <summary>
	/// Tos the json.   wxl   use      socket 协议 格式
	/// </summary>
	/// <returns>The json.</returns>
	public string toJson()
	{
		int No = 1;
		string Message =
			"{" + ACTION + ":" + Convert.ToString(_act) +","+ NO + ":" + Convert.ToString(No) +","+
			VERSION + ":" + "\""+ Convert.ToString(_Version) + "\"" + "," +
			CRC + ":\"" + HashHttpRequest._________________________(Convert.ToString(_act), Convert.ToString(_Version), sb.ToString(), Convert.ToString(No), _PlatformId) + "\","+
			UNIQUE_PLATFORM_ID + ":" + _PlatformId +","+
			RESOURCE_VERSION + ":" + 1 +","+
			DATA + ":" + sb.ToString() + "}" +  "\r\n"; 
		//	UnityEngine.Debug.Log (" MSG  =  "  + Message);


		return Message;
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


