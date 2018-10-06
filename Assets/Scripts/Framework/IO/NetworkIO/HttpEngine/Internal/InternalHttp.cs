using System;

public enum InternalRequestType {
    SHUT_DOWN,
    RESET,
    HOLDING_ON,
    RESUME,
}

/*
 * This is for allen internal framework to charge of HttpEngine.
 */
public class InternalRequest : BaseHttpRequest
{
	public InternalRequestType CommondType;

	public InternalRequest (InternalRequestType type) : base (BaseHttpRequestType.Internal_Control) {
		CommondType = type;
	}
}