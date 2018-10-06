using System;

/* Some Request is internal purpose,
 * Some Request is network request.
 */ 
public enum BaseHttpRequestType {
	Internal_Control,
	Common_Http_Request,
	Third_Party_Request,
	Download_Http_Request,
}

/*
 * father class of "all kinds of http Request"
 */ 
public class BaseHttpRequest {
	public BaseHttpRequestType baseType;

	public BaseHttpRequest(BaseHttpRequestType mType) { 
		baseType = mType;
	}

	public BaseHttpRequest() { }
}