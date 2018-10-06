using System;

public enum ThirdPartyRequestType {
	XingYun,
	CMS,
}

public class ThirdPartyHttpRequest : BaseHttpRequest {
	private ThirdPartyRequestType ThirdType;
	private string RequestURL;

	public ThirdPartyHttpRequest (ThirdPartyRequestType type, string url) : base (BaseHttpRequestType.Third_Party_Request) {
		ThirdType = type;
		RequestURL = url;
	}

	public string RequestUrl {
		get { return RequestURL; }
	}

	public ThirdPartyRequestType ReqType {
		get { return ThirdType; }
	}
}