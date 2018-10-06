using System.Collections;
using AW.Data;
using AW.War;
using System.Reflection;
using System;

/// <summary>
/// npc配置数据
/// </summary>
[System.Serializable]
public class NPCConfigData : UniqueBaseData<int>
{
	public int ID;
	/// <summary>
	/// 名字
	/// </summary>
	public string name;

	/// <summary>
	/// 初始星级
	/// </summary>
	public int star;

    /// <summary>
    /// 控制脚本
    /// </summary>
    public string controlScript;

	/// <summary>
	/// 模型ID
	/// </summary>
	public int model;

	/// <summary>
	/// NPC类型
	/// </summary>
	public LifeNPCType type;

	//建筑类型
	public BuildNPCType bldType;

	/// <summary>
	/// 是否可以移动
	/// </summary>
	public Moveable moveable;

	/// <summary>
	/// 配音资源ID
	/// </summary>
	public int[] dubbing;

	/// <summary>
	/// 三种技能ID和特殊技能ID
	/// </summary>
	public int[] skill;
	public int specialskill;
	//普通的攻击也算作一种技能
	public int[] normalHit;

	//疗效(0-1)
	public float BeHeal;

	/// <summary>
	/// 生命
	/// </summary>
	public int healthpoint;

	/// <summary>
	/// 物理攻击强度
	/// </summary>
	public int attackpower;

	/// <summary>
	/// 物理防御
	/// </summary>
	public int armorclass;

	/// <summary>
	/// 物理穿透
	/// </summary>
	public float armorpenetration;

	/// <summary>
	/// 法术攻击强度
	/// </summary>
	public int spellpower;

	/// <summary>
	/// 法术防御
	/// </summary>
	public int spellresistance;

	/// <summary>
	/// 法术穿透
	/// </summary>
	public float spellpenetration;

	/// <summary>
	/// 物理免伤
	/// </summary>
	public float attackdamagereduction;

	/// <summary>
	/// 法术免伤
	/// </summary>
	public float spelldamagereduction;

	/// <summary>
	/// 移动速度
	/// </summary>
	public int speed;

	/// <summary>
	/// 暴击率
	/// </summary>
	public float crit;

	/// <summary>
	/// 暴击抵抗
	/// </summary>
	public float critresistance;

	/// <summary>
	/// 暴击伤害加成
	/// </summary>
	public float additionalcrit;

	/// <summary>
	/// 物理吸血
	/// </summary>
	public float attacklifesteal;

	/// <summary>
	/// 法术吸血
	/// </summary>
	public float spelllifesteal;

	/// <summary>
	/// 攻击速度加成
	/// </summary>
	public float attackspeed;

	/// <summary>
	/// 冷却缩减加成
	/// </summary>
	public float additionalreduction;

	/// <summary>
	/// 物理反弹
	/// </summary>
	public float attackdamageback;

	/// <summary>
	/// 法术反弹
	/// </summary>
	public float spelldamageback;

	/// <summary>
	/// 生命回复
	/// </summary>
	public float recovery;

	/// <summary>
	/// 攻击参数
	/// </summary>
	public float attackparam1;
	public float attackparam2;
	public float attackparam3;

	/// <summary>
	/// 普通攻击加成
	/// </summary>
	public float attackadditionaldamage;

	/// 搜敌范围
	/// </summary>
	public float seekRange;

	/// <summary>
	/// 脱战范围
	/// </summary>
	public float fleeRange;

	/// <summary>
	/// 英雄系数
	/// </summary>
	public float factor;

	//半径大小
	public float radius;

    /// <summary>
    /// 是否远程单位
    /// </summary>
    public int ranged;

    //职业
    public Career career;

	public override int getKey() {
		return this.ID;
	}

	//浅拷贝
	public NPCConfigData ShallowCopy() {
		return (NPCConfigData)this.MemberwiseClone();
	}

	#region 反射方式设定或获取配置数据

	public T getValue<T>(string FieldName) {
		if(FieldName == "curHp") FieldName = "healthpoint";

		FieldInfo fi = GetType().GetField(FieldName, BindingFlags.DeclaredOnly | BindingFlags.Public);
		T val = (T)fi.GetValue(this);
		return val;
	}

	#endregion
}

[System.Serializable]
public class NPCRuntimeData {
	///背包ID
	public int id;

	/// <summary>
	/// 配表的ID
	/// </summary>
	public int num;

	/// <summary>
	/// 经验
	/// </summary>
	public Int32Fog exp;

	/// <summary>
	/// 等级
	/// </summary>
	public Int32Fog lv;

	/// <summary>
	/// 当前生命
	/// </summary>
	public Int32Fog curHp;

	//给Linq排序使用
	public int CurHpNested {
		get { return curHp; }
	}

	/// <summary>
	/// 总生命,这是一个恒定值
	/// </summary>
	public Int32Fog totalConstHp;

	/// <summary>
	/// 临时的上限
	/// </summary>
	public Int32Fog totalTempHp;

	//挑选出血量的上限值
	public Int32Fog totalHp {
		get {
			return UnityEngine.Mathf.Max(totalConstHp, totalTempHp);
		}
	}

	//疗效(0-1)
	public FloatFog BeHeal;

	/// <summary>
	/// 仇恨值
	/// </summary>
	public Int32Fog HatredVal;

	/// <summary>
	/// 物理攻击强度
	/// </summary>
	public Int32Fog attackpower;

	/// <summary>
	/// 物理防御
	/// </summary>
	public Int32Fog armorclass;

	/// <summary>
	/// 物理穿透
	/// </summary>
	public FloatFog armorpenetration;

	/// <summary>
	/// 法术攻击强度
	/// </summary>
	public Int32Fog spellpower;

	/// <summary>
	/// 法术防御
	/// </summary>
	public Int32Fog spellresistance;

	/// <summary>
	/// 法术穿透
	/// </summary>
	public FloatFog spellpenetration;

	/// <summary>
	/// 物理免伤
	/// </summary>
	public FloatFog attackdamagereduction;

	/// <summary>
	/// 法术免伤
	/// </summary>
	public FloatFog spelldamagereduction;

	/// <summary>
	/// 移动速度
	/// </summary>
	public Int32Fog speed;

	/// <summary>
	/// 暴击率
	/// </summary>
	public FloatFog crit;

	/// <summary>
	/// 暴击抵抗
	/// </summary>
	public FloatFog critresistance;

	/// <summary>
	/// 暴击伤害加成
	/// </summary>
	public FloatFog additionalcrit;

	/// <summary>
	/// 物理吸血
	/// </summary>
	public FloatFog attacklifesteal;

	/// <summary>
	/// 法术吸血
	/// </summary>
	public FloatFog spelllifesteal;

	/// <summary>
	/// 攻击速度加成
	/// </summary>
	public FloatFog attackspeed;

	/// <summary>
	/// 冷却缩减加成
	/// </summary>
	public FloatFog additionalreduction;

	/// <summary>
	/// 物理反弹
	/// </summary>
	public FloatFog attackdamageback;

	/// <summary>
	/// 法术反弹
	/// </summary>
	public FloatFog spelldamageback;

	/// <summary>
	/// 生命回复
	/// </summary>
	public FloatFog recovery;

	/// <summary>
	/// 普通攻击加成
	/// </summary>
	public FloatFog attackadditionaldamage;

	/// <summary>
	/// 护盾
	/// </summary>
	public FloatFog Protection;

	/// <summary>
	/// 英雄系数
	/// </summary>
	public FloatFog factor;

	/// <summary>
	/// 搜敌范围
	/// </summary>
	public FloatFog seekRange;

	/// <summary>
	/// 脱战范围
	/// </summary>
	public FloatFog fleeRange;

	//浅拷贝
	public NPCRuntimeData ShallowCopy() {
		return (NPCRuntimeData)this.MemberwiseClone();
	}

	public NPCRuntimeData(NPCConfigData cfg) {
		defaultInit(cfg);
        c_bf = BindingFlags.Instance | BindingFlags.Public;
    }

	//默认的初始化方式
	void defaultInit(NPCConfigData cfg) {
		this.lv          = 1;
		this.num         = cfg.ID;

		this.curHp       = cfg.healthpoint;
		this.totalConstHp= cfg.healthpoint;
		this.totalTempHp = cfg.healthpoint;

		this.attackpower = cfg.attackpower;
		this.armorclass  = cfg.armorclass;
		this.armorpenetration = cfg.armorpenetration;
		this.spellpower       = cfg.spellpower;
		this.spellresistance  = cfg.spellresistance;
		this.spellpenetration = cfg.spellpenetration;
		this.attackdamagereduction = cfg.attackdamagereduction;
		this.spelldamagereduction = cfg.spelldamagereduction;
		this.speed = cfg.speed;
		this.crit  = cfg.crit;
		this.BeHeal = cfg.BeHeal;
		this.critresistance = cfg.critresistance;
		this.additionalcrit = cfg.additionalcrit;
		this.attacklifesteal = cfg.attacklifesteal;
		this.spelllifesteal = cfg.spelllifesteal;
		this.attackspeed = cfg.attackspeed;
		this.additionalreduction = cfg.additionalreduction;
		this.attackdamageback = cfg.attackdamageback;
		this.spelldamageback = cfg.spelldamageback;
		this.recovery = cfg.recovery;
		this.attackadditionaldamage = cfg.attackadditionaldamage;
		this.Protection = 0f;
		this.factor     = cfg.factor;
		this.seekRange  = cfg.seekRange;
		this.fleeRange  = cfg.fleeRange;

	}

	#region 反射方式设置运行时数据

	public void setValue(string FieldName, object val) {
		FieldInfo fi = GetType().GetField(FieldName, c_bf);
        fi.SetValue(this, val);
	}

	public void addFloatValue(string FieldName, float val) {
		FieldInfo fi = GetType().GetField(FieldName, c_bf);
		float init = (float)fi.GetValue(this);
		init = (val + 1) * init;
		fi.SetValue(this, (FloatFog)init);
	}

	public void addIntegerValue(string FieldName, float val) {
		FieldInfo fi = GetType().GetField(FieldName, c_bf);
		int init = (int)fi.GetValue(this);
		init = (int)((val + 1) * init);
		fi.SetValue(this, (Int32Fog)init);
	}

	public float getFloatValue (string FieldName) {
		FieldInfo fi = GetType().GetField(FieldName, c_bf);
		if(fi.FieldType == typeof(Int32Fog)) {
			Int32Fog val = (Int32Fog)fi.GetValue(this);
			return val.toInt32();
		} else  {
			FloatFog val = (FloatFog)fi.GetValue(this);
			return val;
		}
	}

	private BindingFlags c_bf;

	#endregion
}

/// <summary>
/// NPC战斗中的实时数据
/// </summary>
public class NPCBattleData
{				
	public NPCBattle_Status btStatus;			//战斗中的状态
	public BATTLE_WAY way;						//上中下路
	public int atkerID;							//正在攻击自己的人的ID
	public bool IsInBattle;						//是否在战斗中（或者脱战）
}

public enum NPCBattle_Status
{
	None,
	RestForBlood,				//休整等待回血
	Seeking,					//往目标点移动中
    Fleeing,					//撤退中（被塔攻击的时候）
    Escapeing,                  //往家里逃跑（没血的时候）
    EscapeToDef,                //以少敌多的时候，往塔下跑
}


//职业
public enum Career
{
    Tank,               //肉盾
    Warrior,            //战士
    Magic,              //法师
    Helper,             //辅助
}

