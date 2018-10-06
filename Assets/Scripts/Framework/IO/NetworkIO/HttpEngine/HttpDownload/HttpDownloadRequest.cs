using System;
using System.IO;
using AW.Framework;

public class HttpDownloadRequest : BaseHttpRequest {

	public string url;

	//The path where the config file will be saved.
	public string whereToBeSaved;

	//md5sum to be checked.
	public string md5sum;

	public HttpDownloadRequest (string URL, string fn, string path, string md5) : base (BaseHttpRequestType.Download_Http_Request) {
		Utils.Assert(string.IsNullOrEmpty(URL), "Download Url is Null or Empty.");
		Utils.Assert(string.IsNullOrEmpty(fn), "Download FileName is Null or Empty.");
		Utils.Assert(string.IsNullOrEmpty(path), "Download locate Path is Null or Empty.");
		Utils.Assert(string.IsNullOrEmpty(md5), "MD5sum of Resource is Null or Empty.");

		url = URL;
		whereToBeSaved = Path.Combine(path, fn);
		md5sum = md5;
	}

}
