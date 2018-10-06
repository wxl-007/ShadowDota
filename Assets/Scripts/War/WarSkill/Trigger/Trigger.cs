using System;
using AW.Data;
using AW.Message;

namespace AW.War {
	/// Triggerd的施法和承受
	/// <summary>
	/// 需要检测的数据源，从OnHappen过来
	/// </summary>
	public class Pair {
		public ServerNPC castor;
		public ServerNPC target;
	}

	//Trigger的类型
	public enum TriggerKind {
		//是逻辑的触发器
		Logical,
		//还是技能的触发器
		Skill,
	}

	/// <summary>
	/// 所有的Trigger实现，也必须基础Trigger
	/// </summary>
	public abstract class Trigger {
		//Trigger的唯一ID
		public int TriggerId;

		//Trigger的配置文件
		protected TriggerConfigData cfg;

		protected ITriggerEnd workOnEnd;

		protected TriggerKind typeOfTrigger {
			get {
				if(cfg == null) return TriggerKind.Logical;
				else return TriggerKind.Skill;
			}
		}

		//挂在哪个NPC身上
		public int HangUpNpcId;

		//挂在那个Buff上
		public int HangUpBuffId;

		/// <summary>
		/// 只有cfg有配置的时候才回去调用OnEnd
		/// </summary>
		protected void OnEnd() {
			if(typeOfTrigger == TriggerKind.Skill) {
				//Get RtBuffData
				RtBufData buf = warMgr.bufMgr.get(HangUpBuffId);
				workOnEnd.IfEnd(buf, this);
			}
		}

		/// <summary>
		/// 初始化Trigger的Num
		/// 如果不替换UniqueId，则可以填-1
		/// </summary>
		/// <param name="triggerId">Trigger identifier.</param>
		public void Init(TriggerConfigData config, int uniqueId) {
			cfg = config;
			if(uniqueId > 0) TriggerId = uniqueId;
			if(cfg != null) {
				switch(config.tEnd) {
				case TriEnd.Recount:
					workOnEnd = null;
					break;
				case TriEnd.Rm_Buff_Trigger:
					workOnEnd = new RmBuffWithTrigger();
					break;
				case TriEnd.Rm_Buff_Layer:
					workOnEnd = null;
					break;
				case TriEnd.Rm_Trigger:
					workOnEnd = new RmTrigger();
					break;
				}
			}
		}

		/// <summary>
		/// 要求每个FixedUpdate都默认执行一次触发器吗？
		/// 也就是说，如果返回True。则这个触发器，也会变为一个不是只执行一次的触发器，而是多拥有了一个检测器的功能
		/// </summary>
		/// <returns><c>true</c>, if per frame was ticked, <c>false</c> otherwise.</returns>
		public abstract bool TickPerFrame();

		protected WarServerManager warMgr {
			get {
				return WarServerManager.Instance;
			}
		}
	}
}
