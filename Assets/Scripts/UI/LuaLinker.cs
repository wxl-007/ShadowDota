using UnityEngine;
using System.Collections;
using UIEventCenter;
using AW.Message;
using AW.Entity ;

//进入GameUI场景时向LUA发送事件
public class LuaLinker : BaseLua
{
	//链接的LUA名字
	public string LuaLink;

	public GameObject[] Node;

	//是否添加一个事件监听器
	public bool AddListener;

	protected override void RunBeforeAwake ()
	{
		//if(AddListener)
			//EventSender.onEvent += OnEvent;
		EventSender.Registere(this);

		LuaName = LuaLink;
		isLuaController = true;
	}




	#region Lua没有网络<临时使用>原来的消息
	public void SendOkMsg()
	{
		TryToLoginParam param = new TryToLoginParam();
		param.commond = TryToLoginParam.SYNC_CONFIG_OK;
		Core.EntityMgr.sendMessage(LogicalType.Login, LogicalType.Login, param, true, MsgRecType.MakeSure);

	}


	public void loginServerFinished()
	{
		Call("loginServerFinished");
	}
	#endregion

	void OnDestroy()
	{
		//EventSender.onEvent -= OnEvent;
		EventSender.Remove(this);
		base.OnDestroy();
	}

}
