using System;
using System.Collections.Generic;

public partial class HttpRequestFactory {
	public const int ACTION_LOGIN = 100;
	public const int ACTION_GET_SERVER = 1001;					//快速登录获取服务器列表
	public const int ACTION_THIRD_GET_SERVER = 1002;			//第三方登录获取服务器列表


	public static readonly Dictionary<RequestType, RelationShipReqAndResp> PreDefined = new Dictionary<RequestType, RelationShipReqAndResp>()
	{
		{ RequestType.GET_PARTITION_SERVER,  new RelationShipReqAndResp(RequestType.GET_PARTITION_SERVER, ACTION_GET_SERVER,          typeof(GetPartitionServerResponse)) },
		{ RequestType.THIRD_GET_SERVER,  	 new RelationShipReqAndResp(RequestType.THIRD_GET_SERVER,     ACTION_THIRD_GET_SERVER,    typeof(GetPartitionServerResponse)) },
		{ RequestType.LOGIN_GAME,            new RelationShipReqAndResp(RequestType.LOGIN_GAME,           ACTION_LOGIN,               typeof(LoginResponse))              },
		//TODO : add more here...
	};
}