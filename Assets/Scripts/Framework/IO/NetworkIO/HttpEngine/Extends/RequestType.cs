/*
 * This is public for 
 */
public enum RequestType
{
	#region HTTP Request 
	GET_PARTITION_SERVER,				//普通登录获取服务器列表
	THIRD_GET_SERVER,					//第三方登录获取服务器列表
	LOGIN_GAME,                         //登陆游戏服务器

	None,
	#endregion

	#region SOCKET Request
	SOCK_LOGIN,
	#endregion

	#region 本地数据
	FIGHT_FULISA,
	#endregion
}