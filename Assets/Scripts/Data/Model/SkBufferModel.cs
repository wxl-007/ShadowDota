using System;
using System.Collections.Generic;

namespace AW.Data {

	/// <summary>
	/// 技能的Buff配置信息，目前是没有技能升级这一说
	/// </summary>

	[Modle(type = DataSource.FromLocal)]
	public class SkBufferModel : KVModelBase<int, BuffConfigData> {

		public SkBufferModel() {

		}

		public override bool loadFromConfig() {
			return base.load(ConfigType.Buff);
		}

	}

	public class BuffConfigData : UniqueBaseData <int> {
		public int ID;
		public string Name;
		public string Description;
		public int BuffIcon;

		//buff的类型
		public BufType BuffType;

		//1.恐惧
		//2.媚惑
		//4.睡眠
		//8.定身
		//16.减速
		//32.减攻速
		//64.残废
		//128.流血
		//256.中毒
		//512.燃烧
		//1024.虚无
		//2048.昏迷
		//4096.嘲讽
		//8192.沉默
		//16384.魔法免疫
		//32768.物理免疫"
		public NpcStatus Status;
		//0.物理
		//1.法术
		//2.神圣"
		public SkillTypeClass BuffClass;
		//buff的分组，主要用来判定互斥，只要人物身上挂载了相同的group，则必须互斥
		public int BuffGroup;
		//buff调用的动作组
		public int BuffAction;
		//叠加层数
		public int Stacks;
		//持续时间（注意，单位ms）
		public float Duration;
		//触发器模版ID，以冒号“:”分隔的TriggerID列表
		public int[] TriggerIDList;
		//触发器的结束方式
		public EndOfBuff KillTarget;

		//---------- skill触发 ----------
		public int ScriptStart;
		public int ScriptEnd;
		public int ScriptCycle;

		//循环时间（注意，单位ms）
		public float EffectCycle;

		//技能发起者
		//0.依附者
		//1.施放者"
		public BuffChosen Caster;
		//技能目标
		//0.依附者
		//1.施放者"
		public BuffChosen Target;
	
		//第一次循环延迟时间（单位ms）
		public float DelayTime;

		//人物属性的改变
		public int Attr1;
		public int A1Param1;
		public int A1Param2;
		public int Attr2;
		public int A2Param1;
		public int A2Param2;
		public int Attr3;
		public int A3Param1;
		public int A3Param2;

		public int Attr4;
		public int A4Param1;
		public int A4Param2;

		public int Attr5;
		public int A5Param1;
		public int A5Param2;

		public int Attr6;
		public int A6Param1;
		public int A6Param2;

		public int Attr7;
		public int A7Param1;
		public int A7Param2;

		public int Attr8;
		public int A8Param1;
		public int A8Param2;

		public override int getKey() {
			return this.ID;
		}
	}

}
