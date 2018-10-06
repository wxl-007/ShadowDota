using System;
using AW.Framework;
/*
 *  把所有要传给服务器的参数，全部定义在这个类里面。
 * 
 */
#region 第三方登录获取服务器列表
[Serializable]
class ThirdGetServerParam : BaseRequestParam {

	public string account;			//唯一号
	public string session;			//部分平台为null
	public int os;					//系统
	public int maket;				//平台市场（360， 腾讯，91···）
	public string extension;        //扩展字段-目前用来黑桃的渠道ID

	public ThirdGetServerParam () {  }
	public ThirdGetServerParam (AccountData ad) {
		account = ad.uniqueId;
		session = ad.session;
		os = (int)ad.platform;
		maket = (int)ad.maket;
		extension = ad.extension;
	}
}
#endregion

#region 获取服务器列表
[Serializable]
class PartitionServerParam : BaseRequestParam {
	private const int FLAT_IOS = 1;
	private const int FLAT_ANDROID = 2;
	private const int FLAT_WP = 3;
	private const int FLAT_DEFAULT = FLAT_IOS;

	public int appv;
	public string account;
	public int flat;

	public PartitionServerParam () { }

	public PartitionServerParam (int appversion, string acc) {
		#if UNITY_IPHONE
		flat = FLAT_IOS;
		#elif UNITY_ANDROID
		flat = FLAT_ANDROID;
		#elif UNITY_WP8
		flat = FLAT_WP;
		#else
		flat = FLAT_DEFAULT;
		#endif
		appv = appversion;
		account = acc;
	}
}
#endregion

#region 获取登陆信息
[Serializable]
class LoginParam : BaseRequestParam {
	//账户id
	public string accid;

	public string token;

	//选中服务器id
	public string sid;
	//1(快速上线)|2(账户上线)
	public short type;

	public LoginParam() { }

	public LoginParam(AccountData ad, string loginToken, string serverId) { 
		accid = ad.uniqueId;
		token = loginToken;
		type = 2;
		sid = serverId;
	}
}
#endregion