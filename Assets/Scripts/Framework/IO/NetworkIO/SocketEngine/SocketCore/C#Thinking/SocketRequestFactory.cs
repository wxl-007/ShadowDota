using System;
using System.Collections.Generic;

public class SocketRequestFactory
{
	//This will be fullfilled after game is launched.
	public static int swInfo;
	//This will be fullfilled after game is launched.
	public static int platformId;

	public const int ACTION_DEFAULT = -1;
	//login in socket  wxl 
	public const int ACTION_LOGIN = 1001;
	public const int ACTION_ATTACKBOSS = 1002;
	public const int ACTION_LOGOUTBOSS = 1003;
	public const int ACTION_GETBLOOD_ATKLIST = 1004;
	public const int ACTION_LOGINBOSS = 1005;

	public const int ACTION_CLOSE = 2001;
	public const int ACTION_GETACTTIME = 1006;
	public const int ACTION_ACTSTATE = 1007;
	public const int ACTION_ACTADDPOWER = 1008;
	public const int ACTION_HONORBUYITEM = 1009;
	public const int ACTION_LOGINFESTIVAL = 1010;
	public const int ACTION_BUYLOTTERY = 1011;
	public const int ACTION_LOGOUTFESTIVAL = 1012;
	public const int ACTION_GETSCORERANKLIST = 1013;

	public const int ACTION_WORLDCHATLOGIN = 1100;
	public const int ACTION_WORLDCHAT = 1102;
    //雷达组队
    public const int ACTION_LOGINGPSWAR = 1201;
    public const int ACTION_GETROOMLIST = 1202;
    public const int ACTION_LOGININROOM = 1203;
    public const int ACTION_LOGGOUTROOM = 1204;
    public const int ACTION_CREATROOM = 1205;
    public const int ACTION_GPSFIGHT = 1206;
    public const int ACTION_FIGHTCOMPLETE = 1207;
    public const int ACTION_LOGOUTGPSWAR = 1208;
    public const int ACTION_SYNCLOCATION = 1209;
	public const int ACTION_SYNCROOM = 1210;

	public static readonly Dictionary<RequestType, RelationShipReqAndResp> PreDefined = new Dictionary<RequestType, RelationShipReqAndResp>()
	{
		//TODO: add more here
	};

	public static SocketRequest createHttpRequestInstance(RequestType type, BaseRequestParam reqParam){
		SocketRequest req = new SocketRequest(type, swInfo, Convert.ToString(platformId));
		if (reqParam != null && Enum.IsDefined(typeof(RequestType), type)) {

			RelationShipReqAndResp preDef = PreDefined[type];
			if (preDef != null) {
				req.setParameter(SocketRequest.ACTION, preDef.requestAction);
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


