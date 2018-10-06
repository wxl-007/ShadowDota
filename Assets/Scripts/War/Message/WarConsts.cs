using System;

namespace AW.War {
	/// <summary>
	/// 战斗的常量定义
	/// </summary>
	public class WarMsgConsts {

		///
		/// ------------------------------ Note -------------------------------
		/// ------ 因为Req的字符串是反射找的函数，所以请勿随意修改函数名和Req，Rep的常量 ------
		///


		/// <summary>
		/// 加入战斗, JOINReq后面会带有客户端的信息
		/// </summary>
		public const string JOINReq = "Join";
		public const string JOINRep = "Join-OK";

		/// <summary>
		/// 准备开始战斗
		/// </summary>
		public const string ReadyReq = "Ready";
		public const string ReadyRep = "Ready-OK";
		public const string ReadyRepE= "Ready-Error";

		/// <summary>
		/// 进入战斗场景后，UI准备OK
		/// </summary>
		public const string UIReadyReq = "UIReady";
		public const string UIReadyRep = "UIReady-OK";
		public const string UIReadyRepE = "UIReady-Error";

		/// <summary>
		/// 取消准备战斗
		/// </summary>
		public const string NotReadyReq = "NotReady";
		public const string NotReadyRep = "NotReady-OK";
		public const string NotReadyRepE= "NotReady-Error";

		/// <summary>
		/// 退出战斗
		/// </summary>
		public const string QuitReq = "Quit";
		public const string QuitRep = "Quit-OK";
		public const string QuitRepE= "Quit-Error";

		/// <summary>
		/// 释放技能
		/// </summary>
        public const string CastSkReq = "CastSkill";
		public const string CastSkRep = "CaskSkill-OK";

        /// <summary>
        /// 普通攻击
        /// </summary>
        public const string AttackReq = "Attack";
        public const string AttackRep = "Attack-OK";

		/// <summary>
		/// 手动控制的移动
		/// </summary>
		public const string MoveReq = "Move";
		public const string MoveRep = "Move-OK";
		public const string MoveStopReq = "MoveStop";
		public const string MoveStopRep = "MoveStop-OK";
	
		/// <summary>
		/// 切换激活英雄
		/// </summary>
		public const string SwitchReq = "Switch";
		public const string SwitchRep = "Switch-OK";
		public const string SwitchRepE = "Switch-Error";

		/// <summary>
		/// 切换手动和自动
		/// </summary>
		public const string ManualOrAutoReq = "ManualAuto";
		public const string ManualOrAutoRep = "ManualAuto-OK";
		public const string ManualOrAutoRepE = "ManualAuto-Error";
	}

	#region 服务器端 ---> 客户端

	/// <summary>
	/// 告知客户端，服务器端的信息
	/// </summary>
	public class ServerInfo {
		//服务器的IP
		public string IpAddr;
		//服务器的Publisher Port
		public int PubPort;
		//Req <---> Rep
		public int PairPort;
		//Heart beat
		public int HeartBeatPort;
		//服务器端名字
		public string ServerName;
		//服务器端ID
		public string ServerID;

		public ServerInfo() { }

		public ServerInfo( IpcServerReadyMsg serverMsg ) { 
			IpAddr     = serverMsg.IpAddr;
			PubPort    = serverMsg.PubPort;
			PairPort   = serverMsg.PairPort;
			HeartBeatPort = serverMsg.HeartBeatPort;
			ServerName = serverMsg.ServerName;
			ServerID   = serverMsg.ServerID;
		}
	}

	/// <summary>
	/// 同步客户端信息
	/// </summary>
	public class ClientInfoSync {

	}

	#endregion

	#region 客户端 ---> 服务器端

	/// <summary>
	/// 传递给服务器的基类
	/// </summary>
	public class CliID {
		public string ClientID;
	}

	/// <summary>
	/// 加入战斗的信息类
	/// </summary>
	public class JoinInfo : CliID {
		public string ClientName;
		public RoomCharactor Charactor;
	}

	/// <summary>
	/// 准备好
	/// </summary>
	public class ReadyInfo : CliID {

	}

	//UI准备好的信息
	public class UIReadyInfo : CliID {
		public string ClientName;
	}

	//切换英雄
	public class SwitchInfo : CliID {
		public int UniqueID;
		public WarCamp camp;
	}

	//切换自动或手动
	public class ManualOrAuto : CliID {
		public int UniqueID;
		public WarCamp camp;
		public short auto;
	}

    public class CastSkillInfo : CliID
    {
        public int index;
        public float cdTime;
    }

	#endregion
}