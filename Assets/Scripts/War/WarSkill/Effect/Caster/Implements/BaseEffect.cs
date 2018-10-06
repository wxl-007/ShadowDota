using System;
using AW.Data;

namespace AW.War {
	/// <summary>
	/// 各种效果的基类
	/// </summary>
	public class BaseEffect {
		//当前技能效果的配表
		protected EffectConfigData cfg;
		//计算子的控制器
		protected OperatorMgr OpMgr;
		//当前技能的配置
		protected SkillConfigData skCfg;

		protected BaseEffect() {
			OpMgr = OperatorMgr.instance;
		}
	}

}