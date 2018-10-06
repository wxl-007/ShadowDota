using System;


/* Some Request is internal purpose,
 * Some Request is network request.
 */ 
public enum BaseSocketRequestType {
	Internal_Control,
	Common_Socket_Request,
}

/*
 * father class of "all kinds of socket Request"
 */ 
public class BaseSocketRequest {
	public BaseSocketRequestType type;

	public BaseSocketRequest (BaseSocketRequestType bsockType) {
		type = bsockType;
	}

	public BaseSocketRequest () { }
}