using System;
using System.Collections.Generic;
using AW.War;

/// 
/// 数据层的各种枚举，可以定义在此
/// 
namespace AW.Data {

	/// <summary>
	/// NPC刷新规则
	/// </summary>
	public enum NPCRefreshRules
	{
		//根据时间刷新
		RefreshByTime,
		//当NPC被使用/销毁时,根据波次刷新
		RefreshByGroup,
	}

	//对战的上中下路
	public enum BATTLE_WAY
	{
		Up 		= 0,	//上路		
		Down 	= 1, 	//中路
		Middle	= 2,	//下路
		None	= 3,    //不分路，比如基地
	}

	/// 
	/// 技能Effect OP的定义
	/// 
	public enum EffectOp {

		/* 攻击
		 * 
			对Target造成一次伤害	"技能伤害=((P(1)/1000)*对应攻击强度+P(2))

			技能暴击率=暴击率+P(4)/1000

			技能暴击伤害提升比=暴击伤害提升比+P(5)/1000

			按照P(7)的方式处理

			if 暴击 then
				技能伤害=暴击伤害提升*技能伤害
				end if

			if 计算护甲 then
				技能伤害=技能伤害*(1-免伤率)*P(3)
				end"	伤害倍数(千分比)	附加伤害值	总伤害提升比	"暴击率CriticalRate
				(千分比)"	"暴击伤害提升比CriticalDamageRate
				(千分比)"	"伤害仇恨比
				(千分比)"	"集合:
				1.不可闪躲
				2.不可致命
				4.不计算护甲减伤
				8.无视护盾吸收伤害"			
		*/
		None       = 0x0,
		//攻击
		Injury     = 0x1,
		//治疗
		Treat      = 0x2,
		//BUff和Dbuff
		DotHot     = 0x3,
		//驱散
		DriveAway  = 0x4,
		//移动目标
		MoveTarget = 0x5,
		//移动到自己子物体
		MoveToChild= 0x6,
		//修改仇恨(ignore)
		HatredValue= 0x7,
		//创建NPC
		CtorNPC    = 0x8,
		//创建炸弹(ignore)
		CreatBomb  = 0x9,
		//复活
		Revival    = 0xA,
		//吸血
		SuckBlood  = 0xB,
		//闪电链 类型
		Chain      = 0xC,
		//抛物线移动 (ignore)
		parabolaMove = 0xD,
		//嘲讽(ignore)
		Taunt      = 0xE,
		//击退类强制移动
		BeatOff    = 0xF,
		//子弹型NPC
		Bullet_NPC = 0x10,
		//护盾
		ProtectionShild = 0x12,
		//变身
		SwitchNpc  = 0x14,
		//和其他NPC属性置换
		ExchangeNpcAttri = 0x15,
		//创建“触发”的触发器
		Trigger    = 0x16,
		//钩子型NPC
		HookNpc    = 0x17,
	}


	/// <summary>
	/// buff的阶段
	/// </summary>
	public enum BuffPhase {
		Start = 0x0,
		Cycle = 0x1,
		End   = 0x2,
		Trigger= 0x03,
	}


	/// <summary>
	/// 触发器选择的枚举
	/// </summary>
	public enum SelectClass {
		Event_C1     = 0x0,
		Event_C2     = 0x1,
		C1_Monster   = 0x2,
		C2_Monster   = 0x3,
		C1_Master    = 0x4,
		C2_Master    = 0x5,
	}

	/// <summary>
	/// 最终受到伤害
	/// </summary>
	[Flags]
	public enum ResistanteClass {
		None       = 0x0,
		// 1.护盾生效
		Protection = 0x1,
		// 2.吸血生效
		Bloody     = 0x2,
		// 4.反弹类
		Rebound    = 0x4,
	}

	public static class ResistanteExtension {
		public static bool check(this ResistanteClass flags, ResistanteClass totest ) {
			return (flags & totest) == totest;
		}

		public static ResistanteClass set(this ResistanteClass flags, ResistanteClass totest) {
			return flags | totest;
		}
	}

	/// <summary>
	/// 0.瞬发(effect直接生效)
	/// 1.引导(effect循环生效)
	/// 2.吟唱(effect吟唱结束，effect生效)
	/// 3.被动(没有effect)
	/// </summary>
	public enum SkType {
		Fleeting = 0x0,
		Booting  = 0x1,
		Chantting= 0x2,
		Passive  = 0x3,
	}

	/// 
	/// 伤害类型
	/// 
	public enum SkillTypeClass {
		// 0.物理类
		Physical = 0x0,
		// 1.法术类
		Magical  = 0x1,
		// 2.神圣类
		Holly    = 0x2,
		// 3.反弹物理类
		Rebound_Phy = 0x3,
		// 4.反弹法术类
		Rebound_Mag = 0x4,
		// 5.反弹神圣类
		Rebound_Hol = 0x5,
	}

	//0.无意义
	//1.不可致命
	//2.不计算护甲减伤
	//4.无视护盾吸收伤害
	[Flags]
	public enum HurtHitClass {
		//Forbid_dodge = 0x0,
		None         = 0x0,
		Forbid_Die   = 0x1,
		Forbid_Armer = 0x2,
		Forbid_ProtectionShield = 0x4,
	}

	public static class HurtHitExtension {
		public static bool check(this HurtHitClass flags, HurtHitClass totest ) {
			return (flags & totest) == totest;
		}

		public static HurtHitClass set(this HurtHitClass flags, HurtHitClass totest) {
			return flags | totest;
		}
	}

	//枚举:
	//1.不可暴击
	//2.不考虑疗效
	public enum TreatClass {
		None            = 0x0,
		Forbid_Critical = 0x1,
		Forbid_BeHeal   = 0x2,
	}

	/// 
	/// 伤害的作用区域
	/// 
	public enum RangeClass {
		// 0.方向
		Direction    = 0x0,
		// 1.AOE
		AOE          = 0x1,
		// 2.单体
		SingleTarget = 0x2,
	}

	/// 
	/// 目标细分类型
	/// 
	[Flags]
	public enum TargetSubClass {
		//0.可对所有类型
		AllTarget         = 0x0,
		//1.不可对建筑
		Forbid_Building   = 0x1,
		//2.不可对小兵
		Forbid_Unit       = 0x2,
		//4.不可对英雄
		Forbid_Hero       = 0x4,
		//8.不可对召唤物（含分身）
		Forbid_Summon     = 0x8,
	}

	public static class TarSubExtension {
		public static bool check(this TargetSubClass flags, TargetSubClass totest ) {
			return (flags & totest) == totest;
		}

		public static TargetSubClass set(this TargetSubClass flags, TargetSubClass totest) {
			return flags | totest;
		}

		public static LifeNPCType toPositive(this TargetSubClass flags) {
			bool ok = flags == TargetSubClass.AllTarget;
			if(ok) return LifeNPCType.SkTarAll;

			//默认所有的都选择出来
			LifeNPCType type = LifeNPCType.SkTarAll;
			//不要建筑物
			ok = flags.check(TargetSubClass.Forbid_Building);
			if(ok) type = type.clear(LifeNPCType.Build);

			//不要小兵
			ok = flags.check(TargetSubClass.Forbid_Unit);
			if(ok) type = type.clear(LifeNPCType.Soldier);

			//不要英雄
			ok = flags.check(TargetSubClass.Forbid_Hero);
			if(ok) type = type.clear(LifeNPCType.Hero);

			//不要召唤物
			ok = flags.check(TargetSubClass.Forbid_Summon);
			if(ok) type = type.clear(LifeNPCType.Summon);

			return type;
		}

	}



	/// 
	/// 目标类型
	/// 
	[Flags]
	public enum TargetClass {
		None        = 0x0,
		//友方
		Friendly    = 0x1,
		//距离最近单位
		Nearest     = 0x2,
		//生命最低单位
		HpLowest    = 0x4,
		//敌方
		Hostile     = 0x8,
		//距离最远单位
		FarAwary    = 0x10,
		//生命最高单位
		HpHighest   = 0x20,
		//只对自己使用
		Self        = 0x40,
	}

	public static class TarExtension {
		public static bool check(this TargetClass flags, TargetClass totest ) {
			return (flags & totest) == totest;
		}

		public static TargetClass set(this TargetClass flags, TargetClass totest) {
			return flags | totest;
		}

		public static bool AnySame(this TargetClass flags, TargetClass totest) {
			return (flags & totest) != 0;
		}
	}

	/// 
	/// 目标权重
	/// 
	public enum TargetWeight {
		// 不区分优先级
		None        = 0x0,
		// 英雄
		Hero        = 0x1,
		// 召唤物（含分身）
		Summon      = 0x2,
		// 建筑
		Building    = 0x4,
		// 小兵
		Unit        = 0x8,
	}

	/// <summary>
	/// 技能目标的优先级选择
	/// 0.不区分优先级
	/// 1.英雄 > 召唤兽 > 小兵 > 防御塔 > 建筑  （英雄使用）
	/// 2.英雄英雄攻击的目标 > 英雄 > 召唤兽 > 小兵 > 防御塔 > 建筑   （召唤兽使用）
	/// 3.攻击我方英雄的敌方英雄 > 攻击我方英雄的敌方召唤兽 > 攻击我方英雄的敌方小兵 > 小兵 > 召唤兽 > 防御塔 > 英雄 > 建筑  （小兵使用）
	/// 4.攻击我方英雄的敌方英雄 > 攻击我方英雄的敌方召唤兽 > 攻击我方英雄的敌方小兵 > 小兵 > 召唤兽 > 英雄  （防御塔使用）
	/// </summary>
	public enum SkTargetPriority {
		NonePriority     = 0x0,
		PriorityForHero  = 0x1,
		PriorityForSummoner  = 0x2,
		PriorityForMinion= 0x3,
		PriorityForTower = 0x4,
	}

	/// 
	/// Npc的状态
	///0.普通
	///1.昏迷
	///2.恐惧
	///4.媚惑
	///8.睡眠
	///16.嘲讽
	///32.虚无
	///64.沉默
	///128.定身
	///256.减速
	///512.减攻速
	///1024.残废
	///2048.流血
	///4096.中毒
	///8192.燃烧
	///16384.治疗
	///32768.增益
	///65536.物理免疫
	///131072.法术免疫
	//0x40000.技能释放状态
	[Flags]
	public enum NpcStatus {
		None        = 0x0,

		Unconscious = 0x1,
		Frighten    = 0x2,
		Captivate   = 0x4,
		Sleep       = 0x8,
		Taunt       = 0x10,
		Nothingless = 0x20,
		Slient      = 0x40,
		Fixed       = 0x80,
		SlowDown    = 0x100,
		SlowAttack  = 0x200,
		Disabled    = 0x400,
		Bleeding    = 0x800,
		Poison      = 0x1000,
		Fire        = 0x2000,
		Treat       = 0x4000,
		Benifit     = 0x8000,
		PhysicalImmunity = 0x10000,
		MagicImmunity = 0x20000,
		InSkill     = 0x40000,
		TimeFreeze  = 0x80000,

		ForbidMove = Unconscious | Sleep | Fixed,				//不能移动状态
		//可打断引导Buff
		Interrupt  = Unconscious | Frighten | Captivate | Sleep | Taunt | Nothingless | Slient | TimeFreeze,
	}

	/// 
	/// 一律使用这个方法去检测Npc状态
	/// 
	public static class NpcStatusExtension {
		//检测一个或多个，单必须全部满足
		public static bool check( this NpcStatus flags, NpcStatus totest ) {
			return (flags & totest) == totest;
		}

		public static NpcStatus set(this NpcStatus flags, NpcStatus totest) {
			return flags | totest;
		}

		public static NpcStatus clear(this NpcStatus flags, NpcStatus totest) {
			return flags & ~totest;
		}

		public static bool AnySame(this NpcStatus flags, NpcStatus totest) {
			return ((flags & totest) != 0);
		}

		//是离散的状态吗？ 
		//就是说：一次伤害，一次治疗属于离散。但是昏迷属于持续
		public static NpcStatus rmDiscrete(this NpcStatus flags) {
			return flags & ~NpcStatus.Treat;
		}

		//挑选出flags和totest两者不相关的状态
		public static NpcStatus pickUp(this NpcStatus flags, NpcStatus totest) {
			return ((flags | totest) & ~flags);
		}

	}

	/// <summary>
	/// NPC的种类
	/// </summary>
	public enum KindOfNPC {
		NonLife,
		Life,
		Default,
	}

	/// <summary>
	/// 有生命NPC的分类
	/// </summary>
	[Flags]
	public enum LifeNPCType {
		All     = 0x0,				//无效值（默认值）
		Hero    = 0x1,				//英雄
		Build   = 0x2,				//建筑
		Soldier = 0x4,				//小兵或小怪
		Summon  = 0x8,			    //招呼出来的宠物

		/// 
		/// 下面的这些也会加在NpcManger里面，但是SkTarAll并不加入
		/// 也就是说，技能不参与计算下面的枚举。
		/// 
		Model	= 0x10,				//模型
		Prop	= 0x20,				//道具
		FreshPt	= 0x40,				//刷新点

		//技能会选择的目标
		SkTarAll= Build | Soldier | Hero | Summon,
	}

	//建筑类型
	public enum BuildNPCType {
		None    = 0x0,		    //无效值
		Tower   = 0x1,			//塔
		Barrank = 0x2,			//兵营
		Base    = 0x4,			//基地
		Spring  = 0x8, 			//泉水

		LifeBuild = Tower | Barrank | Base,			//有生命的建筑
	}

	public static class BuildingTypeExtension {
		public static bool check( this BuildNPCType flags, BuildNPCType totest ) {
			return (flags & totest) == totest;
		}

		public static BuildNPCType set(this BuildNPCType flags, BuildNPCType totest) {
			return flags | totest;
		}
	}


	/// <summary>
	/// 能否移动
	/// </summary>
	public enum Moveable {
		Movable   = 0x0,
		BeStatic  = 0x1,
	}

	public static class LifeNpcExtension {
		public static bool check( this LifeNPCType flags, LifeNPCType totest ) {
			return (flags & totest) == totest;
		}

		public static LifeNPCType set(this LifeNPCType flags, LifeNPCType totest) {
			return flags | totest;
		}

		public static LifeNPCType clear(this LifeNPCType flags, LifeNPCType totest) {
			return flags & ~totest;
		}

	}

	/// <summary>
	/// 阵营
	/// </summary>
	[Flags]
	public enum CAMP {
		None   = 0x0,	//初始值
		Neutral= 0x1,   //中立
		Player = 0x2,   //玩家
		Enemy  = 0x4,   //敌对
		All    = Neutral | Player | Enemy,
	}

	public static class CAMPExtension {
		public static bool check( this CAMP flags, CAMP totest ) {
			return (flags & totest) == totest;
		}

		public static CAMP set(this CAMP flags, CAMP totest) {
			return flags | totest;
		}

		public static CAMP clear(this CAMP flags, CAMP totest) {
			return flags & ~totest;
		}

		/// <summary>
		/// 设置敌对的阵营
		/// </summary>
		/// <param name="flags">Flags.</param>
		/// <param name="totest">Totest.</param>
		public static CAMP Hostile(this CAMP flags) {
			CAMP hostile = CAMP.All;
			return hostile & ~flags;
		}

	}

	//npc刷新类型
	public enum NPCFreshType
	{
		//初始刷新
		Normal,
		//到达目标点刷新
		ArriveAt,
		//某个怪死亡刷新
		NPSDead,
	}

	/// <summary>
	/// 初始默认的ai类型
	/// </summary>
	public enum NPCAIType
	{
		Idle 			= 0,		//空闲		
		Patrol 			= 1,		//巡逻
		Pathfind_Atk	= 2,		//寻路攻击
        Simple_PfAtk    = 3,        //简单的寻路攻击（对战小兵，剧情AI）
		Wander			= 4,		//无脑徘徊
		Follow_Atk		= 5,		//跟随攻击
	}


	//1.不可对敌对目标作用
	//2.不可对中立目标作用
	//4.不可对友方目标作用
	//8.使效果目标进入战斗状态
	//16.使效果发起者进入战斗状态
	//32.同步战斗状态
	//64.不可对自己使用
	//128.skillTarget和Dest之间有障碍则无效
	[Flags]
	public enum EffectFlag {
		None            = 0x0,
		Forbid_Enemy    = 0x1,
		Forbid_Neutral  = 0x2,
		Forbid_Friendly = 0x4,
		MakeTarget_InWar= 0x8,
		MakeSelf_InWar  = 0x10,
		Sync            = 0x20,
		Forbid_Self     = 0x40,
		Barrie_Invalid  = 0x80,
	}

	public static class EffectFlagExtension {
		public static bool check( this EffectFlag flags, EffectFlag totest ) {
			return (flags & totest) == totest;
		}

		public static EffectFlag set(this EffectFlag flags, EffectFlag totest) {
			return flags | totest;
		}

		public static EffectFlag clear(this EffectFlag flags, EffectFlag totest) {
			return flags & ~totest;
		}

		/// <summary>
		/// 转换为Camp类型
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="Flags">Flags.</param>
		public static CAMP switchTo(this EffectFlag Flags, CAMP self) {
			CAMP camp = CAMP.All;
			bool ck = Flags.check(EffectFlag.Forbid_Enemy);
			if(ck) {
				CAMP enemy = self.Hostile();
				camp = camp.clear(enemy);
			}

			ck = Flags.check(EffectFlag.Forbid_Friendly); 
			if(ck) {
				camp = camp.clear(self);
			}

			ck = Flags.check(EffectFlag.Forbid_Neutral);
			if(ck) {
				camp = camp.clear(CAMP.Neutral);
			}

			return camp;
		}

	}

	//0.SkillTarget指定的目标
	//1.SkillTarget周围半径EParam1米内的目标
	//2.朝自身前方发动长为EParam1、宽为EParam2、速度为MissleSpeed的方型冲击
	//3.朝自身前方发动长为size*RParam1、角度为size*RParam2、速度为MissleSpeed的扇形冲击
	//4.SkillTarget 和Skill的发动者 一起作为目标
	public enum EffectTargetClass {
		SkillTarget = 0x0,
		SkillTarget_Radius = 0x1,
		Self_Front_Rectangle = 0x2,
		Self_Front_Fan = 0x3,
		Self_And_Target= 0xA,
	}

	/// <summary>
	/// Buff的检测发起者，Buff的检测依附着
	/// </summary>
	public enum BuffChosen {
		Add_To_Adhere = 0x0,
		Add_To_Caster = 0x1,
	}

	/// <summary>
	/// buff的起源
	/// </summary>
	public enum OriginOfBuff {
		//独立的
		Alone = 0x0,
		//被技能带出来
		BornWithSkill = 0x1,
	}

	/// <summary>
	/// 0.瞬发
	/// 1.引导
	/// 2.被动
	/// </summary>
	public enum BufType {
		Fleeting = 0x0,
		Booting  = 0x1,
		Passive  = 0x2,
	}

	/// <summary>
	/// buff的结束方式
	/// </summary>
	public enum EndOfBuff {
		//不删除NPC
		Remain = 0x0,
		//删除NPC
		Killed = 0x1,
	}

	/// <summary>
	/// 生或死
	/// </summary>
	[Flags]
	public enum LiveOrDie {
		Live = 0x1,
		Die  = 0x2,
		All  = Live | Die,
	}


	/// <summary>
	/// Bullet NPC的伤害类型，对应到EffectDefinition的Param8
	/// 0.飞行过程对所有目标伤害
	/// 1.只伤害最终目标周围
	/// 2.只伤害最终目标
	/// </summary>
	public enum BulletHurtType {
		Keep_Dmg            = 0x0, 
		Final_Target_Radius = 0x1,
		Final_Target        = 0x2,
	}

	/// <summary>
	/// DotHot的Effect，对应Param3
	/// </summary>
	/// 0.定时
	/// 1.增加时长
	/// 2.不做处理
	public enum DotHotDurType {
		Fixture = 0x0,
		Increase= 0x1,
		Leaveit = 0x2,
	}

	/// <summary>
	/// 选择：
	/// 0.目标位置
	/// 1.施法者前方
	/// 2.施法者周围
	/// </summary>
	public enum CtorNpcPos {
		Target  = 0x0,
		Castor_Forward = 0x1,
		Castor_Surround= 0x2,
	}

	/// <summary>
	/// 数据源是哪里
	/// 0.读取NPC参数
	/// 1.继承创建者初始参数
	/// 2.继承创建者当前参数
	/// </summary>
	public enum CtorNpcSource {
		NPC_Table = 0x0,
		NPC_Castor_Init = 0x1,
		NPC_Castro_Cur  = 0x2,
	}

	/// <summary>
	/// 选择属性：
	/// 0.不修改属性
	/// 1.最大生命和当前生命值
	/// 2.基础物理和法术强度
	/// 4.物理免伤
	/// 8.法术免伤
	/// </summary>
	[Flags]
	public enum CtorNpcAttri {
		None = 0x0,
		MaxAndCur_Hp = 0x1,
		PhysicalAndMagical = 0x2,
		Physical_Resistant = 0x4,
		Magical_Resistant  = 0x8,
	}

	public static class CtorNpcExtension {
		public static bool check(this CtorNpcAttri flags, CtorNpcAttri totest ) {
			return (flags & totest) == totest;
		}

		public static CtorNpcAttri set(this CtorNpcAttri flags, CtorNpcAttri totest) {
			return flags | totest;
		}

		public static List<string> SwitchTo(this CtorNpcAttri flags) {
			List<string> switchData = new List<string>();
			bool found = flags.check(CtorNpcAttri.MaxAndCur_Hp);
			if(found) {
				switchData.Add("totalConstHp");
				switchData.Add("curHp");
			}

			found = flags.check(CtorNpcAttri.PhysicalAndMagical);
			if(found) {
				switchData.Add("attackpower");
				switchData.Add("spellpower");
			}

			found = flags.check(CtorNpcAttri.Physical_Resistant);
			if(found) {
				switchData.Add("armorclass");
			}

			found = flags.check(CtorNpcAttri.Magical_Resistant); 
			if(found) {
				switchData.Add("spellresistance");
			}

			return switchData;
		}
	}

	//触发器的条件
	[Flags]
	public enum TriCondition {
		Distance = 0x1,
		HP       = 0x2,
		Buff     = 0x4,
	}

	//0.删除源Buff和Trigger
	//1.不删除源Buff但删除Trigger
	//2.重置计数器
	//3.删除1层源Buff(如果源Buff的stacks>1,则减1层),且重置计数器
	public enum TriEnd {
		Rm_Buff_Trigger = 0x0,
		Rm_Trigger      = 0x1,
		Recount         = 0x2,
		Rm_Buff_Layer   = 0x3,
	}

	//远近的视野
	public enum Sight {
		//使用NPC的SeekRange
		FarSight,
		//使用技能的Distance
		NearSight,
	}


	/// <summary>
	/// 1 释放条件 2 转换其他技能条件 3 重置本技能(但不重置计数器）
	/// </summary>
	public enum SkConditionClass {
		CheckCast = 0x1,
		SwithOtherSkill = 0x2,
		ResetSkill = 0x3,
	}

	/// <summary>
	/// 1.斩首, 绝对血线多少以下技能转换
	/// 2.斩首2, 当前血量是总血量百分比多少以下技能转换
	/// 3.目前不再使用，技能能否释放
	/// 4.存活时间到了，技能转换
	/// 5.计数器到了，技能转换
	/// 6.斩首, 绝对血线多少以下技能重置
	/// 7.只要概率满足，技能转换
	/// </summary>
	public enum SkConditionType {
		BeHead    = 0x1,
		BeHead2   = 0x2,
		CheckCast = 0x3,
		TimeOut   = 0x4,
		Counting  = 0x5,
		BeHeadReset = 0x6,
		Pro       = 0x7,
		BeHead2Reset = 0x8,
	}

	/// <summary>
	/// 移动到自己子物体的条件
	/// 0.无条件位移(只存在单一子物体使用)
	/// 1.距离自身最远
	/// 2.攻击的目标生命百分比最低
	/// </summary>
	public enum MvToChildCon {
		None     = 0x0,
		Nearest  = 0x1,
		Farest   = 0x2,
		HpLowest = 0x3,
		HpHighest= 0x4,
	}

	/// <summary>
	/// 移动到自己子物体后
	/// 0.删除子物体
	/// 1.不删除子物体
	/// </summary>
	public enum MvToChildAlive {
		RmChild    = 0x0,
		LeaveAlone = 0x1,
	}

	/// <summary>
	/// 钩子类型NPC的伤害类型
	/// 0.没有伤害
	/// 1.全程造成伤害
	/// 2.去程造成伤害
	/// 3.返程造成伤害
	/// </summary>
	public enum HookNpcDmgType {
		None_Dmg   = 0x0,
		Always_Dmg = 0x1,
		Forth_Dmg  = 0x2,
		Back_Dmg   = 0x3,
	}

	/// <summary>
	/// 钩子类型NPC的消失类型
	/// 0.攻击第一个目标后消失
	/// 1.攻击最终目标后消失
	/// 2.到达最大距离后消失
	/// 3.返回施法者位置后消失
	/// </summary>
	public enum HookNpcDisappearType {
		DisWhenFirstAtked = 0x0,
		DisWhenFinalTar   = 0x1,
		DisWhenMaxDistance= 0x2,
		DisWhenReturnback = 0x3,
	}

	/// <summary>
	/// 钩子NPC的唯一类型
	/// 0.不位移
	/// 1.目标位移到创建者位置
	/// 2.创建者位移到目标位置
	/// </summary>
	public enum HookNpcMoveType {
		None_Move = 0x0,
		ToCastor  = 0x1,
		ToTarget  = 0x2,
	}

}