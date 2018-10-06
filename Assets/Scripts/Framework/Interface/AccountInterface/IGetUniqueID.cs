using System;

namespace AW.Framework {

	public enum Platform {
		Invalid = -1,
		Android = 1,
		IOS = 2,
		WP = 3,
	}

	/// <summary>
	/// to Which Market.
	/// </summary>
	public enum Market {
		Invalid = -1,
		APP_STORE = 1,
		QI_HOO = 2,
		Spade  = 5,
	}

	public enum ThirdLoginState
	{
		Invalid = -1,			//无效
		Opened = 0,				//打开第三方登录
		LoginFinish = 1,		//第三方登录成功
		CancelLogin = 2,		//取消第三方登录
	}

	public enum LoginType {
		TYPE_QUICK  = 0x01,      //不走任何账号系统，只在本地存储一个唯一ID
		TYPE_THIRDPARTY = 0x02,  //进过第三方账号系统
	}

	/// <summary>
	/// Third Party Account or Generate by framework
	/// </summary>
	public class AccountData {
		public string uniqueId;			//唯一ID
		public string session;			//session（部分平台可能没用）
		public string token;			
		public Platform platform;		//ios   android   winphone
		public Market maket;
		public LoginType lType;
		public string payCallback;			//支付回调地址
		public ThirdLoginState loginStatus;		//登录状态

		//扩展字段-Spade用来黑桃字段
		public string extension;
	}

	/// <summary>
	/// Get Unique Id - From Third Party or Generate by framework
	/// </summary>
	public interface IGetUniqueID {
	
		//实例化sdk，调用sdk登录方法
		void getUniqueId(Action<AccountData> onThirdPartyNotify);

		//登录第三方成功,得到第三方认证码
		void LoginSuc(string strID);

		//第三方取消登录
		void LoginCacel(string strCode);

		//第三方切换账号
		void SwitchAccount();

		//退出第三方
		void Quit();

		//支付
		void Pay(string strMsg);

		//支付结果回调
		void PayResultCallback(string state);

		// 实名注册
		void RrealNameRegist(string strID);

		//防沉迷查询
		void AntiAddictionQuery(string strToken, string strID);

		AccountData GetAccountData();
	}
}



