using System;
using AW.Data;
using AW.War;

namespace AW.War {
	/// 
	/// 所有的技能效果都必须有这个特性
	/// 当满足OP的，特定的Effect类就会被调用起来执行
	/// 
	[AttributeUsage(AttributeTargets.Class)]
	public class EffectAttribute : Attribute {
		public EffectOp OP { get; set; }
	}


	/// <summary>
	/// 所有的触发器都必须有这个特性，
	/// 当满足条件的Cmd时，触发器就会工作
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)] 
	public class TriggerAttribute : Attribute {
		public WarMsg_Type Cmd { get; set; }
	}


	/// <summary>
	/// 所有的激活技能的条件，都必须有这个特性，当条件满足的时候，合适的条件判定器就会被找到
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)] 
	public class ConditionAttribute : Attribute {
		public SkConditionType Con { get; set; }
	}

	///
	/// 此特性用来决定技能Skill的单目标技能优先级的判定规则
	/// 
	[AttributeUsage(AttributeTargets.Class)]
	public class SkPriorityAttribute: Attribute {
		public SkTargetPriority priority { get; set; }
	}

}

