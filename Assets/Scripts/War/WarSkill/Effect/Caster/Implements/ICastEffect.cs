using System;
using AW.Message;
using System.Collections.Generic;
using AW.Data;

namespace AW.War {
	/// 
	/// 释放技能效果的接口，所有技能的效果都必须实现此接口
	/// 另外，还必须拥有EffectAttribute特性
	/// 
	public interface ICastEffect  {
		/// <summary>
		/// 初始化技能效果
		/// </summary>
		/// <param name="config">Config.</param>
		void Init(EffectConfigData config, SkillConfigData skillcfg);
		/// <summary>
		/// 释放技能效果
		/// </summary>
		/// <param name="src">效果发起者</param>
		/// <param name="skTarget">技能选择的目标</param>
		/// <param name="target">技能先选择目标后，再次Effect选择后的目标</param>
		/// <param name="skDirectHurt">是否是技能的直接伤害</param>
		/// <param name="container">所有数据展现的容器，本次Effect施法的效果会依次加入</param>
		void Cast(ServerNPC src, IEnumerable<ServerNPC> skTarget, IEnumerable<ServerNPC> target, bool skDirectHurt, List<MsgParam> container);
	}

}
