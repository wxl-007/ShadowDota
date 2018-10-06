using System;
using System.Collections.Generic;

namespace AW.Data {

	/// <summary>
	/// 技能的配置信息，目前是没有技能升级这一说
	/// </summary>

	[Modle(type = DataSource.FromLocal)]
	public class SkillModel : KVModelBase<int, SkillConfigData> {
		public override bool loadFromConfig() {
			return base.load(ConfigType.Skill);
		}
	}

	/// <summary>
	/// 技能的配置数据
	/// </summary>
	public class SkillConfigData : UniqueBaseData <int> {
		public int ID;
		//技能名
		public string Name;
		//描述
		public string Description;
		//技能图标信息
		public int Icon;
		//技能类型
		//
		// 0.物理类
		// 1.法术类
		// 2.神圣类
		//
		public SkillTypeClass SkillClass;
		//冷却(单位s)
		public float BaseCD;
		//弹道的基础飞行速度
		//速度单位为(厘米/秒)，如果没有弹道，则弹道速度为0
		public float MissileSpeed;
		//施法距离(单位米)
		public float Distance;
		//技能优先级
		//只在智能施法时生效，根据血量施放对应优先级最高技
		public short DamageType;

		/// <summary>
		/// 技能的类型
		/// </summary>
		public SkType SkillType;

		//范围类型
		// 0.方向
		// 1.AOE
		// 2.单体
		public RangeClass RangeType;
		//选择目标类型
		//
		//0.可对所有类型
		//1.不可对建筑
		//2.不可对小兵
		//4.不可对英雄
		//8.不可对召唤物（含分身）
		public TargetSubClass TargetType;

		//目标选择条件组合
		//0.只对自己使用
		//1.默认友方
		//2.默认距离最近单位
		//4.默认生命最低单位
		//8.敌方
		//16.默认距离最远单位
		//32.默认生命最高单位
		public TargetClass SkillTarget;
		//施放者状态判断
		//0.可在任何状态下使用
		//1.不可在恐惧状态下使用
		//2.不可在媚惑状态下使用
		//4.不可在睡眠状态下使用
		//8.不可在定身状态下使用
		//16.不可在减速状态下使用
		//32.不可在减攻速状态下使用
		//64.不可在残废状态下使用
		//128.不可在流血状态下使用
		//256.不可在中毒状态下使用
		//512.不可在燃烧状态下使用
		//1024.不可在虚无状态下使用
		//2048.不可在昏迷状态下使用
		//4096.不可在嘲讽状态下使用
		//8192.不可在沉默状态下使用
		//16384.不可在魔法免疫状态下使用
		//32768.不可在物理免疫状态下使用
		public NpcStatus CasterStatusReject;
		//目标状态判断
		//0.可在任何状态下使用
		//1.不可在恐惧状态下使用
		//2.不可在媚惑状态下使用
		//4.不可在睡眠状态下使用
		//8.不可在定身状态下使用
		//16.不可在减速状态下使用
		//32.不可在减攻速状态下使用
		//64.不可在残废状态下使用
		//128.不可在流血状态下使用
		//256.不可在中毒状态下使用
		//512.不可在燃烧状态下使用
		//1024.不可在虚无状态下使用
		//2048.不可在昏迷状态下使用
		//4096.不可在嘲讽状态下使用
		//8192.不可在沉默状态下使用
		//16384.不可在魔法免疫状态下使用
		//32768.不可在物理免疫状态下使用
		public NpcStatus TargetStatusReject;
		//其他施放条件判定
		public int Condition;
		//施放时间单位s
		public float ShootTime;
        //事件时间点
        public float ShootEventTime;
		/// <summary>
		/// 目标 0不排序，1排序
		/// 排序:英雄，召唤物，建筑，小兵
		/// </summary>
		public SkTargetPriority TargetPriority;
		//施放动作
		public int ShootAction;
		//引导时间单位s
		public float ChannelTime;
		//引导动作
		public string ChannelAction;
		//引导BUFF
		public int ChannelBuff;
		//被动BuffID
		public int PassiveBuff;
		//效果ID
		public int[] EffectID;
		//延迟effect时间单位s
		public float[] EffectDelayTime;
		//激发行为
		public int[] Incite;
		//0没有存活时间 1有存活时间（换言之就是一个中间技能）
		public short IsAlive;
		//哪个EffectID发生的时候，发动这个条件检测
		public int InciteEffectID;

		//makesure不会出错
		public EffInSk getEffect(int index) {
			EffInSk result = new EffInSk() {
				effid = EffectID.Value<int>(index),
				effdelay = EffectDelayTime.Value<float>(index),
			};
			return result;
		}

		public override int getKey() {
			return this.ID;
		}

	}


	/// <summary>
	/// Effect在技能表里的信息
	/// </summary>
	public struct EffInSk {
		public int effid;
		public float effdelay;
	}

}