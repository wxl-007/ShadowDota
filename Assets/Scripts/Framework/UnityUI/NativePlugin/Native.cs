using UnityEngine;
using System.Collections;
using AW.Framework;
using System.Runtime.InteropServices;

/// <summary>
/// 这个游戏已启动就会存在的，而且不会释放
/// </summary>
public class Native : MonoBehaviour  {

	// --------------------- 所有第三方SDK调用的方法 --------------------- 
	public const string LOGIN_FUN = "startLogin";						//登录(必须接)
	public const string SWITCH_ACCOUNT_FUN = "SwitchAccount";			//切换账号(必须接)
	public const string PAY_FUN = "startPay";							//支付(必须接)
	public const string QUIT_FUN = "Quit";								//退出(必须接)
	public const string FANGCHENMI_FUN = "AntiAddictionQuery";			//防沉迷查询(必须接)
	public const string REAL_NAME_REGISTE = "RealNameRegister";			//实名注册(必须接)
	public const string BBS_FUN = "";									//游戏论坛（可选）
	public const string CUSTOM_FUN = "";								//客服中心（可选）
	public const string LOGOUT_FUN = "logoutSpade";                     //登出SDK游戏

	//安卓native方法
	public const string START_GPS = "StartGPS";							//获取gps位置

	// --------------------- 所有第三方SDK调用的方法 --------------------- 


	private static Native _instance;
	public static Native mInstace
	{
		get
		{
			return _instance;
		}
	}

	//第三方
	public IGetUniqueID m_thridParty;

	void Awake() {
		_instance = this;
		DontDestroyOnLoad(gameObject);
		m_thridParty = new GetUniqueIDFactory().createInstance();
	}
		
	//登录第三方成功
	void LoginThridPartySuc(string strAutoCode)
	{
		ConsoleEx.DebugLog ("Receive ::LoginThridPartySuc");
		m_thridParty.LoginSuc (strAutoCode);
	}

	//取消登录
	void LoginCacel(string strAutoCode)
	{
		ConsoleEx.DebugLog ("Receive :: Login third party failure");
		m_thridParty.LoginCacel (strAutoCode);
	}

	void PayResultCallBack(string state)
	{
		ConsoleEx.DebugLog ("Receive ::PayResultCallBack：： " + state);
		m_thridParty.PayResultCallback (state);
	}

	/// <summary>
	/// 登出第三方SDK
	/// </summary>
	void LogoutThridParty(string code) {
		ConsoleEx.DebugLog ("Receive ::LogoutThridPart.");
		m_thridParty.SwitchAccount();
	}

	/// <summary>
	/// 登出游戏
	/// </summary>
	void QuitGame(string code) {
		ConsoleEx.DebugLog ("Receive ::QuitGame.");
		m_thridParty.Quit();
	}
}