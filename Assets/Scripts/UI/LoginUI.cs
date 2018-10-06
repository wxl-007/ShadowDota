using UnityEngine;
using System.Collections;
using AW.Entity;
using AW.Data;
using AW.Message;

public class LoginUI : MonoBehaviour {

	public void EnterWar() 
	{
		///
		/// 进入战后界面之前，必须先处理好Server和Client握手协议
		///
		WarStartParam param = new WarStartParam();
		Core.EntityMgr.sendMessage(LogicalType.Anonymous, LogicalType.War, param, true, MsgRecType.MakeSure);
	}

	public void EnterUI() 
	{
		UnityUtils.JumpToScene(Core.GameFSM, SceneName.GameUIScene);
	}

}
