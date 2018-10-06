using System;
using AW.Data;
using AW.Message;

namespace AW.War {
	/// <summary>
	/// 战斗发送的消息方法
	/// </summary>
	public interface IMsgSendWar {
		/// <summary>
		/// 这是一个同步的发送消息的方法, 倒数第一个参数代表是否匿名,即忽略发起者
		/// 战斗里面的发送消息的方法，如果接受者不存在，会忽略本消息
		/// </summary>
		bool sendMessage(CAMP sender, CAMP receiver, WarCampMsg param, bool anonymous);

		// 按逻辑组的方式发送
		bool sendMessage(int groupSend, int groupRece, WarGroupMsg param, bool anonymous);

		/// <summary>
		/// 这是一个异步的发送消息的方法, 异步回调的结果不准确。但结果是false则必定出错。
		/// 倒数第一个参数代表是否匿名,即忽略发起者
		/// 战斗里面的发送消息的方法，如果接受者不存在，会忽略本消息
		/// </summary>
		bool SendMessageAsync(CAMP sender, CAMP receiver, WarCampMsg param, bool anonymous);

		// 按逻辑组的方式发送
		bool SendMessageAsync(int groupsend, int groupRece, WarGroupMsg param, bool anonymous);
	}

	public class WarMsgParam : MsgParam {
		public WarMsg_Type cmdType;			//战斗消息类型（基类的commond就没用了）
	}


	//根据阵营发送战斗的消息基类
	public class WarCampMsg : WarMsgParam {
		public CAMP SendCamp;
		public CAMP ReceCamp;
		///
		/// 不关心发送者的ID
		/// 
	}

	//根据逻辑组发送战斗的消息基类
	public class WarGroupMsg : WarMsgParam {
		public int SendGroup;
		public int ReceGroup;

		///
		/// 不关心发送者的ID
		/// 
	}

	/// <summary>
	/// 战斗同步消息
	/// </summary>
	public class WarSyncMsg : WarMsgParam {
		public Object obj;
	}

	/// 
	/// 战斗消息类型, 同时也是触发器的类型
	/// 
	public enum WarMsg_Type
	{
		Running = 0x0,	        //跑
		Stand = 0x1,			//站立
		Idle  = 0x2,			//闲置
		Attack= 0x3,			//攻击
		UseSkill = 0x4,		    //放技能
		Respawn = 0x5,		    //重生
		Win = 0x6,				//胜利
		Lose = 0x7,				//失败

        CreateNpc = 0x8,        //创建NPC   
        UseBuff = 0x9,          //使用Buff
        UseTrigger = 0x0A,       //使用Trigger
        Move = 0x0B,             //同步位移

        ManualInput = 0x0C,      //手动操作数据
        Suffer = 0x0D,
        OnEffect = 0x0E,         //技能顶在脑袋上的特效
        OnStatus = 0x10,         //状态顶在脑袋上的特效
        Destroy = 0x11,          //销毁
		// ----- 下面是触发器监视所用的 -------

		//主动碰撞者
		OnCollide   = 0xA0,
		//被动碰撞者
		BeCollide   = 0xA1,
		//主动打的时候
		OnAttacked  = 0xA2,
		//被打的时候
		BeAttacked  = 0xA3,
		//杀死人的时候
		OnKilled    = 0xA4,
		//被杀的时候
		BeKilled    = 0xA5,
		//和敌人连线
		LineEnemy   = 0xA6,
		//和敌人连线
		SeperateEnemy = 0xA7,
		//饥渴
		RmBufIfKilling= 0xA8,
		//反击
		CounterAttack = 0xA9,
	}


	//
	//**************************  技能相关  **************************
	//

	public class EndResult {
		public int param1;
		public int param2;
		public int param3;
		public int param4;
		public int param5;
		public int param6;
		public int param7;
		public float param8;
		public float param9;
		public float param10;

		//目标可能的状态
		//真正目标的状态应该目标自己来决定
		public NpcStatus status;

		public object obj;
	}

	public enum Verb {
		//普通打
		Punch  = 0x0,
		//撞击
		Strike = 0x1,
		//恢复
		Recover= 0x2,
		//吸血
		SuckBloody = 0x3,
		//Buff
		DotHot = 0x4,
		//造物NPC
		Creature = 0x5,
		//闪烁
		Blink   = 0x6,
		//移动
		Move   = 0x7,

		UI_Base  = 0xA5,
		//连线敌人
		LineEnemy = 0xA6,
		//照亮敌人
		LightEnemy = 0xA7,

		UI_Base2 = 0xC5,
		//分离敌人
		SeperateEnemy = 0xC6,
		//取消亮光敌人
		DarkEnemy = 0xC7,

		//Buff特效
		BuffEffect = 0xF0,
	} 

	//自我描述
	public class SelfDescribed {
		//主语
		public int src;
		//宾语
		public int target;
		//谓语
		public Verb act;
		//己方结果
		public EndResult srcEnd;
		//敌方结果
		public EndResult targetEnd;

	}

	/// 
	/// 战斗效果的消息体
	/// 基类的commond没用了
	/// 
	/// 本类用来发送给技能发起者
	/// 
	/// Sender只代表了发送者ID，一定是攻击者的ID
	/// Receiver只代表了接受者ID，一定是Target的ID
	/// 
	public class WarSrcAnimParam : WarAnimParam {
		//技能表中配置的动作
		public int ShootAction;
		//释放时间
		public float ShootTime;
        //事件时间点
        public float ShootEventTime;

		//技能直接掉血的逻辑，则不能有WarTarAnimParam，而是依赖InjureTar来转发
		public WarTarAnimParam[] InjureTar;

	}


	/// <summary>
	/// 基类的commond没用了
	/// 简单和上面区分，这个用于技能承受者
	/// 
	/// </summary>
	public class WarTarAnimParam : WarAnimParam {
		//被击打动作
		public int HitAction;

	}

	/// <summary>
	/// 技能消息的基类
	/// </summary>
	public class WarAnimParam : WarMsgParam {
		public EffectOp OP;
		//对事物的描述
		public SelfDescribed described;
		//调试信息 ******
		//原始的OP类型, 如果为None就是说是触发阶段，有值则表示是承受阶段
		//
		public EffectOp OringinOP = EffectOp.None;
		public int SkillId = -1; 
	}

}
