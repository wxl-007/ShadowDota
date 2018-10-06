using System;
using Framework;
using fastJSON;
using System.Collections.Generic;

/*
 * 
 * 所有的服务器响应格式的定义都在这里。
 * 
 */ 

#region 获取服务器分区的信息

[Serializable]
public class Server {
    //1 = 新服 2=火爆的服务器 3=满服 4=停服 5=合服 6=推荐服
	public const int STATUS_NEW = 1;
	public const int STATUS_HOT = 2;
	public const int STATUS_FULL = 3;
	public const int STATUS_STOP = 4;
	public const int STATUS_COMBINE = 5;
    public const int STATUS_RECOMMAND = 6;

	public int sid;
	public int id;
	public string name;
	public int status;
	public string url;
	//计费服务器地址
	public string payUrl;

    //聊天地址
	public string chat_ip;
    //活动地址
	public string active_ip;
    //端口号
    public int chat_port;
    public int active_port;

	public Server() { 

    }
}

[Serializable]
public class PartitionServer {
	//All servers
	public Server[] sv;
	//last表示最近登陆过的服务器id
	public int last;

	/// 
	/// ---------- 公告的内容和标题 ------------
	/// 
	public string noticeContent;
	public string noticeTitle;

	//第三方token（快速登录没用）
	public string platToken;

	//第三方唯一ID（快速登录没用）
	public string platId;

	//游戏登陆token
	public string token;

	public PartitionServer() { }
}

[Serializable]
public class GetPartitionServerResponse : BaseResponse
{
	public PartitionServer data;

	public GetPartitionServerResponse () { }
}



[Serializable]
public class ThirdGetServerResponse : BaseResponse
{
	public PartitionServer data;

	public ThirdGetServerResponse () { }
}


#endregion

#region 获取资源更新
[Serializable]
public class UpdateDetails {
	public string fn;
	//资源类型 配置文件和资源文件
	public short type;
	public long size;
	public string md5;
}

[Serializable]
public class ResourcesUpdateInfo {
	//下载地址, 完整的下载地址是: url + file.fn
	public string url;
	public UpdateDetails[] file;
}

[Serializable]
public class ResourceResponse : BaseResponse {
	public ResourcesUpdateInfo data;

	public ResourceResponse() { }
}

#endregion


#region 登陆

//玩家的基本信息
[Serializable]
public class PlayerInfo {
	//角色ID
	public int id;
	//账号ID
	public int accountId;
	//
	public string sessionId;

	public string name;
	//引导完成进度 -1标识未开启新手引导，0标识完成了新手引导
	public int guide;
	//当前等级
	public int lv;
	//Vip等级
	public short vip;
	//金币
	public int coin;
	//钻石
	public int stone;
	//精力值
	public int jl;
	//精力+1点的秒数
	public long jldur;
	//下一个精力+1的秒数
	public long jldurfull;
	//体力值
	public int tl;
	//体力+1点的秒数
	public long tldur;
	//下一个体力+1的秒数
	public long tldurfull;
	//当前使用的队伍
	public int team;
	//战功值
	public int glory;

	//神龙祭坛奥义凹槽购买状态 1为已经购买 0为还没购买
	public int[] aislt;

	//当前的经验值
	public int exp;
	//服务器的当前时间
	public long systime;
	//创建时间
	public long createTime;
	//免战结束时间
	public long shiled;
	/// <summary>
	/// masgn代表今日领取签到奖励标识 0 未领取 1 领取一次 2 领取两次
	/// </summary>
	//	public int masgn;
	//头像ID
	public int headID;
	//祝福值
	public int happy;
}

public class loginInfo {
	//玩家的基本信息
	public PlayerInfo user;

}

[Serializable]
public class LoginResponse : BaseResponse {
	public loginInfo data;

	public LoginResponse() { }
}

#endregion