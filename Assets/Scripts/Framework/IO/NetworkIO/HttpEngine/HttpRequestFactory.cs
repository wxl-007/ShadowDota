using System;
using System.Collections.Generic;

public sealed class RelationShipReqAndResp {
	public Type respType;
	public RequestType requetType;
	public int requestAction;

	public RelationShipReqAndResp (RequestType rqType, int rqAction, Type respType) {
		this.requetType = rqType;
		this.requestAction = rqAction;
		this.respType = respType;
	}
}

public partial class HttpRequestFactory {

    //This will be fullfilled after game is launched.
    public static int swInfo;
	//This will be fullfilled after game is launched.
	public static int platformId;

    public const int ACTION_DEFAULT = -1;

	public static HttpRequest createHttpRequestInstance(RequestType type, BaseRequestParam reqParam, string urlAddress = ""){
		HttpRequest req = new HttpRequest(type, swInfo, Convert.ToString(platformId), urlAddress);
		if (reqParam != null && Enum.IsDefined(typeof(RequestType), type)) {

            RelationShipReqAndResp preDef = PreDefined[type];
            
            if (preDef != null) {
				req.setParameter(HttpRequest.ACTION, preDef.requestAction);
                req.appendPara(reqParam);
            }
            else {
                throw new DragonException("Dictionary PreDefined is not defined.");
            }
        } else {
            throw new DragonException(DragonException.Exception_Message[DragonException.INVALIDATE_ARGUMENT]);
        }
        return req;
    }

	public static RelationShipReqAndResp getRelationShip(RequestType type) {
		return PreDefined[type];
	}


}  