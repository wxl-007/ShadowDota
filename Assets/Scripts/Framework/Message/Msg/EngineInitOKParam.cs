using AW.Message;
using AW.War;

public class EngineInitOKParam : MsgParam {
	public const int COMMOAND_ENGINE_OK = 1;
}

//UI控制层参数
public class UIParam : MsgParam {
	public UnityEngine.GameObject btn;
}

/// <summary>
/// 同步完成配表就可以登陆游戏
/// </summary>
public class TryToLoginParam : MsgParam {
	public const int SYNC_CONFIG_OK = 1;
}

/// <summary>
/// 战斗开始的参数
/// </summary>
public class WarStartParam : MsgParam {
	//这里会填写上合适的参数，比如是否为主机等
	public WarInfo warinfo;
}

/// <summary>
/// 战斗场景准备好的时候
/// </summary>
public class WarSceneIsReadyParam : MsgParam {

}
