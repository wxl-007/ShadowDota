using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Collections;
using System.IO;

public sealed class HttpUtil{

		static char [] hexChars = "0123456789abcdef".ToCharArray ();
		const string notEncoded = "!'()*-._";

		static void UrlEncodeChar (char c, Stream result, bool isUnicode) {
			if (c > 255) {
				//FIXME: what happens when there is an internal error?
				if (!isUnicode)
					throw new ArgumentOutOfRangeException ("c", c, "c must be less than 256");
				int idx;
				int i = (int) c;

				result.WriteByte ((byte)'%');
				result.WriteByte ((byte)'u');
				idx = i >> 12;
				result.WriteByte ((byte)hexChars [idx]);
				idx = (i >> 8) & 0x0F;
				result.WriteByte ((byte)hexChars [idx]);
				idx = (i >> 4) & 0x0F;
				result.WriteByte ((byte)hexChars [idx]);
				idx = i & 0x0F;
				result.WriteByte ((byte)hexChars [idx]);
				return;
			}
			
			if (c>' ' && notEncoded.IndexOf (c)!=-1) {
				result.WriteByte ((byte)c);
				return;
			}
			if (c==' ') {
				result.WriteByte ((byte)'+');
				return;
			}
			if (	(c < '0') ||
				(c < 'A' && c > '9') ||
				(c > 'Z' && c < 'a') ||
				(c > 'z')) {
				if (isUnicode && c > 127) {
					result.WriteByte ((byte)'%');
					result.WriteByte ((byte)'u');
					result.WriteByte ((byte)'0');
					result.WriteByte ((byte)'0');
				}
				else
					result.WriteByte ((byte)'%');
				
				int idx = ((int) c) >> 4;
				result.WriteByte ((byte)hexChars [idx]);
				idx = ((int) c) & 0x0F;
				result.WriteByte ((byte)hexChars [idx]);
			}
			else
				result.WriteByte ((byte)c);
		}

	public static byte [] UrlEncodeUnicodeToBytes (string str) {
		if (str == "")
 			return new byte [0];
		MemoryStream result = new MemoryStream (str.Length);
		foreach (char c in str){
			UrlEncodeChar (c, result, true);
		}
		return result.ToArray ();

	}

	public static string UrlEncodeUnicode (string str) {
		if (str == null)
 			return null;
		return Encoding.ASCII.GetString (UrlEncodeUnicodeToBytes (str));
	}


	/// <summary>
	/// 分析 url 字符串中的参数信息
	/// </summary>
	/// <param name="url">输入的 URL</param>
	/// <param name="baseUrl">输出 URL 的基础部分</param>
	/// <param name="nvc">输出分析后得到的 (参数名,参数值) 的集合</param>
	public static void ParseUrl(string url, out string baseUrl, out NameValueCollection nvc) {
		if (url == null)
			throw new ArgumentNullException("url");
		
		nvc = new NameValueCollection();
		baseUrl = "";
		
		if (url == "")
			return;            
		
		int questionMarkIndex = url.IndexOf('?');
		
		if (questionMarkIndex == -1) {
			baseUrl = url;
			return;
		}
		baseUrl = url.Substring(0, questionMarkIndex);
		if (questionMarkIndex == url.Length - 1)
			return;
		string ps = url.Substring(questionMarkIndex + 1);
		
		// 开始分析参数对    
		Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.None);
		MatchCollection mc = re.Matches(ps);
		
		foreach (Match m in mc) {                
			nvc.Add(m.Result("$2"), m.Result("$3"));
		}        
	}

	public static void ParseParameter ( string argus, out NameValueCollection nvc) {
		if (argus == null)
			throw new ArgumentNullException("Parameter");
		// 开始分析参数对    
		Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.None);
		MatchCollection mc = re.Matches(argus);

		nvc = new NameValueCollection();
		foreach (Match m in mc) {                
			nvc.Add(m.Result("$2"), m.Result("$3"));
		}        
	}

}
