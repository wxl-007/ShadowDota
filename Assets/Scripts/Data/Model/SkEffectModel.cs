using System;
using System.Collections.Generic;

namespace AW.Data
{
	[Modle(type = DataSource.FromLocal)]
	public class EffectModel : KVModelBase<int, EffectConfigData> {

		public override bool loadFromConfig() {
			return base.load(ConfigType.Effect);
		}

	}

	/// <summary>
	/// 技能的效果
	/// </summary>
	public class EffectConfigData : UniqueBaseData <int> {
		public int ID;
		//名字
		public string Name;
		//描述
		public string Description;
		//1.不可对敌对目标作用
		//2.不可对中立目标作用
		//4.不可对友方目标作用
		//8.使效果目标进入战斗状态
		//16.使效果发起者进入战斗状态
		//32.同步战斗状态
		//64.不可对自己使用
		//128.skillTarget和Dest之间有障碍则无效
		public EffectFlag Flags;
		//0.物理
		//1.法术
		//2.神圣
		//3.反弹
		public SkillTypeClass EffectClass;
		//0.可对所有类型
		//1.不可对建筑
		//2.不可对小兵
		//4.不可对英雄
		//8.不可对召唤物（含分身）
		public TargetSubClass EffectTargetType;
		//0.SkillTarget指定的目标
		//1.SkillTarget周围半径EParam1米内的目标
		//2.朝自身前方发动长为EParam1、宽为EParam2、速度为MissleSpeed的方型冲击
		//3.朝自身前方发动长为size*RParam1、角度为size*RParam2、速度为MissleSpeed的扇形冲击
		public EffectTargetClass EffectTarget;

		//---- 参数----
		public float eParam1;
		public float eParam2;

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
		public NpcStatus EffectTargetStatusReject;

		//和Param一起决定效果的结果，见EffectDefinition表. 主外键的关系
		public EffectOp EffectType;
		//伤害和治疗效果将会增加目标对施法者仇恨值，用以区别肉盾和普通英雄
		public int Hatred;

		/// 
		/// 参数的定义都是Int，其他类型的参数，需要转换
		/// 
		public int Param1;
		public int Param2;
		public int Param3;
		public int Param4;
		public int Param5;
		public int Param6;
		public int Param7;
		public int Param8;
		public int Param9;
		public int Param10;
		public int[] Param11;

		//作用概率,千分比
		//Effect发生概率为 prob / 1000 * 100%
		public int Prob;
		//作用上限
		//作用角色的上限个数，按位置由近及远排序，只对设定数目的有影响，0为无上限
		public int EffectLimit;
		//动作类型
		public int HitAction;

		public override int getKey() {
			return this.ID;
		}
	}


}
