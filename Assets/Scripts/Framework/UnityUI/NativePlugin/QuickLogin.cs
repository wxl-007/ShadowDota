using System;
using UnityEngine;
using AW.Framework;

public class QuickLogin : IGetUniqueID
{
	private Action<AccountData> m_LoginCallback;
	private AccountData m_AccountData;

	public void getUniqueId (Action<AccountData> onThirdPartyNotify)
	{
		m_LoginCallback = onThirdPartyNotify;
		LoginSuc (DeviceInfo.GUID);
	}

	public void LoginSuc(string strMsg)
	{
		AccountData ad = new AccountData();

		ad.maket = Market.Invalid;
		#if UNITY_IPHONE
		ad.platform = Platform.IOS;
		#elif UNITY_ANDROID
		ad.platform = Platform.Android;
		#else
		ad.platform = Platform.Invalid;
		#endif
		ad.uniqueId = DeviceInfo.GUID;
		ad.lType = LoginType.TYPE_QUICK;
		ad.loginStatus = ThirdLoginState.LoginFinish;
		m_AccountData = ad;

		m_LoginCallback(ad);
	}

	//第三方取消登录
	public void LoginCacel(string strCode)
	{
	}

	//第三方切换账号
	public void SwitchAccount()
	{
	}

	//退出第三方
	public void Quit() {
		Application.Quit();
	}

	//支付
	public void Pay(string strMsg)
	{
	}

	//支付回调
	public void PayResultCallback(string state)
	{
	}

	// 实名注册
	public void RrealNameRegist(string strID)
	{
	}

	//防沉迷查询
	public void AntiAddictionQuery(string strToken, string strID)
	{
	}

	public AccountData GetAccountData()
	{
		return m_AccountData;
	}

	public QuickLogin () { }
}