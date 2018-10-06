using System;

/*
 *  把所有要传给服务器的参数，全部定义在这个类里面。
 * 
 */ 

#region 登陆

[Serializable]
class SockLoginParam : BaseRequestParam {
	public String roleId;
	public String ticket;
    public int zoneId;
	public SockLoginParam() { }

    public SockLoginParam(string rId ,string ti ,int zId) { 
		roleId = rId;
		ticket = ti;
        this.zoneId = zId;
	}
}
#endregion

#region  世界boss

#region 进入世界boss

[Serializable]
class SockLoginBossParam : BaseRequestParam {

	public String roleId;

	public SockLoginBossParam() { }

	public SockLoginBossParam(string rId ) { 
		roleId = rId;
	}
}
#endregion



#region 攻击boss

[Serializable]
class SockAttackBossParam : BaseRequestParam {

	public String roleId;
	public int type;
	public SockAttackBossParam() { }

	public SockAttackBossParam(string rId,int tType ) { 
		roleId = rId;
		type = tType;
	}
}
#endregion


#region 退出世界boss   1003

[Serializable]
class SockLogOutBossParam : BaseRequestParam {

	public String roleId;
	public SockLogOutBossParam() { }


	public SockLogOutBossParam(string rId ) { 
		roleId = rId;
	}
}

#endregion


#region 能量加成   1008

[Serializable]
class SockAddPowerParam : BaseRequestParam {

	public string roleId;
	public int type;   //0 金币 1 钻石
	public SockAddPowerParam() { }


	public SockAddPowerParam(string rId ,int tP){
		roleId = rId;
		type = tP;
	}
}

#endregion

#region 兑换积分商品   1009

[Serializable]
class SockbuyItemParam : BaseRequestParam {

	public string roleId;
	public int goodId;   
	public SockbuyItemParam() { }


	public SockbuyItemParam(string rId ,int godid){
		roleId = rId;
		goodId = godid;
	}
}

#endregion





#endregion


#region 武者的节日    1010--1013

[Serializable]
class SockLoginFestivalParam : BaseRequestParam {

	public String roleId;
	public SockLoginFestivalParam() { }


	public SockLoginFestivalParam(string rId ) { 
		roleId = rId;
	}
}


[Serializable]
class SockBuyLotteryParam : BaseRequestParam {

	public String roleId;
	public int type;
	public SockBuyLotteryParam() { }


	public SockBuyLotteryParam(string rId ,int tType) { 
		roleId = rId;
		type = tType;
	}
}

[Serializable]
class SockLogOutFestivalParam : BaseRequestParam {

	public String roleId;
	public SockLogOutFestivalParam() { }


	public SockLogOutFestivalParam(string rId ) { 
		roleId = rId;
	}
}

[Serializable]
class SockGetScoreRankListParam : BaseRequestParam {

	public SockGetScoreRankListParam() { }

}

#endregion


#region 活动状态

[Serializable]
class SockActivityStateParam : BaseRequestParam {

	public SockActivityStateParam() { }

}

#endregion

#region 登陆Chat

[Serializable]
class SockWorldChatLoginParam : BaseRequestParam 
{
	public string roleId;
	public string roleName;
	public int roleLv;
	public string ticket;
	public int zoneId;
	public long iconId;
	public SockWorldChatLoginParam() { }
	
	public SockWorldChatLoginParam(string rId ,string ti, string na, int lv, int zid, long icon)
	{ 
		roleId = rId;
		roleName = na;
		roleLv = lv;
		ticket = ti;
		zoneId = zid;
		iconId = icon;
	}
}
#endregion

#region 发送聊天内容

[Serializable]
class SockSendWorldChatParam : BaseRequestParam 
{
	public string content;
	public SockSendWorldChatParam() { }
	
	public SockSendWorldChatParam(string con)
	{ 
		content = con;
	}
}
#endregion
#region 雷达组队
/// <summary>
/// 登录雷达房间      1201
/// </summary>
class SockLoginGPSWarParam:BaseRequestParam
{
	public int atk;
	public int def;
    public SockLoginGPSWarParam(int ak, int df)
	{
		this.atk = ak;
		this.def = df;
    }
}
/// <summary>
/// 读取房间列表  1202
/// </summary>
class SockGetRoomListParam:BaseRequestParam{
    public SockGetRoomListParam(){

    }
}
/// <summary>
/// 进入房间        1203
/// </summary>
class SockLoginRoomParam:BaseRequestParam{
    public int roomId;      //房间id
    public SockLoginRoomParam(int tRoomID){
        this.roomId = tRoomID;
    }
}
/// <summary>
/// 离开房间        1204
/// </summary>
class SockLogOutRoomParam:BaseRequestParam{
    public SockLogOutRoomParam(){}
}
/// <summary>
/// 创建房间    1205
/// </summary>
class SockCreatRoomParam:BaseRequestParam{
    public SockCreatRoomParam(){

    }
}

/// <summary>
/// 战斗      1206
/// </summary>
class SockFightParam:BaseRequestParam{
    /// <summary>
    /// 关卡难度  1: 简单 2:普通 3:困难 4:地狱
    /// </summary>
    public int plotLevel;
    public SockFightParam(int tLevel){
        this.plotLevel = tLevel;
    }
}
/// <summary>
/// 战斗结束    1207
/// </summary>
class SockCompleteFightParam:BaseRequestParam{
    public SockCompleteFightParam(){

    }
}
/// <summary>
/// 登出 该活动  1208
/// </summary>
class SockLogOutGPSWarParam:BaseRequestParam{
    public SockLogOutGPSWarParam(){

    }
}

class SockSyncLocationParam:BaseRequestParam{
    public float longitude; //经度
    public float latitude;  //维度
    public SockSyncLocationParam(float tLongitude,float tLatitude){
        this.longitude = tLongitude;
        this.latitude = tLatitude;
    }
}

class SockSyncRoomParam:BaseRequestParam{

	public SockSyncRoomParam(){
	}
}
#endregion










