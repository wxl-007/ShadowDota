using System;
using System.Collections.Generic;

namespace AW.Data {

	[Modle(type = DataSource.FromLocal)]
	public class SkConditionModel : KVModelBase<int, ConditionConfigure> {
		public override bool loadFromConfig() {
			return base.load(ConfigType.Condition);
		}
	}

	public class ConditionConfigure : UniqueBaseData<int> {
		public int ID;

		/// <summary>
		/// 1 释放条件 2 转换其他技能条件 3 重置本技能(但不重置计数器）
		/// </summary>
		public SkConditionClass ConditionClass;

		/// <summary>
		/// 1.斩首, 绝对血线多少以下技能转换
		/// 2.斩首2, 当前血量是总血量百分比多少以下技能转换
		/// 3.目前不再使用，技能能否释放
		/// 4.存活时间到了，技能转换
		/// 5.计数器到了，技能转换
		/// 6.斩首, 绝对血线多少以下技能重置
		/// 7.只要概率满足，技能转换
		/// </summary>
		public SkConditionType ConditionType;

		//激发的技能ID
		public int TargetSkID;
		//参数1-5
		public int Param1;
		public int Param2;
		public int Param3;
		public int Param4;
		public int Param5;
		//概率
		public int Prop;

		public override int getKey() {
			return this.ID;
		}
	}

}
