using System;
using AW.War;
using System.Collections.Generic;

namespace AW.Data {

	/// <summary>
	/// 战斗的触发器，实际上目前也就是War在使用
	/// </summary>

	[Modle(type = DataSource.FromLocal)]
	public class SkTriggerModel : KVModelBase <int, TriggerConfigData> {
		public override bool loadFromConfig() {
			return base.load(ConfigType.Trigger);
		}
	}

	/// <summary>
	/// 触发器的配置数据
	/// </summary>
	public class TriggerConfigData : UniqueBaseData <int> {
		public int ID;
		public string Name;
		public string Description;
		//是否感兴趣的条件判定条件
		public WarMsg_Type tEvent;
		//触发的条件
		public TriCondition Condition;
		public int Param1;
		public int Param2;
		public int Param3;
		public int Param4;
		//触发概率
		public int Prob;
		//达到该值后触发
		public int Count;
		//触发时所施放的技能ID
		public int SkillID;
		//触发技能时，发起者的选择方式
		public SelectClass Caster;
		//触发技能时，目标的选择方式
		public SelectClass Target;
		//结束处理方式
		public TriEnd tEnd;

		public override int getKey() {
			return this.ID;
		}

	}


}

