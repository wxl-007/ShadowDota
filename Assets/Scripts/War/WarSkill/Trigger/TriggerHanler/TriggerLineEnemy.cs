using System;
using AW.Data;
using AW.Message;
using AW.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AW.War {

	/// <summary>
	/// 每一帧都会触发的触发器
	/// 属于子Trigger类型
	/// </summary>
	[Trigger(Cmd = WarMsg_Type.LineEnemy)]
	public class TriggerLineEnemy : PerFrameTrigger, ITriggerItem  {

		//需要被检测的容器
		private List<Pair> checkList = new List<Pair>();

		#region ITriggerItem implementation

		public int GetID () {
			return TriggerId;
		}

		/// <summary>
		/// 触发条件满足，或者说感兴趣的事情发生了。
		/// 但是是不是要真的处理什么逻辑，还不一定
		/// </summary>
		/// <param name="msg">Message</param>
		/// <param name="npcMgr">Npc mgr.</param>
		public void OnHappen (MsgParam msg, WarServerNpcMgr npcMgr) {
			checkList.Clear();
			WarAnimParam warParam = msg as WarAnimParam;
			if(warParam != null && warParam.described != null) {
				SelfDescribed des = warParam.described;
				Pair pair = new Pair();
				pair.castor = npcMgr.GetNPCByUniqueID(des.src);
				pair.target = npcMgr.GetNPCByUniqueID(des.target);

				checkList.Add(pair);
			}
		}

		public void OnRest () {
			cfg = null;
		}

		#endregion

		public override bool BeTriggered () {
			return checkList.Count > 0;
		}

		public override void OnFixedUpdate () {
			Handle();
		}

		#region 检测目标周围

		void Handle() {
			//有Trigger的配置
			if(typeOfTrigger == TriggerKind.Skill) {
				switch(cfg.Condition) {
				case TriCondition.Distance:
					HandleDistance();
					break;
				case TriCondition.HP:
					HandleHp();
					break;
				}
			}
		}

		void HandleDistance() {
			List<ServerNPC> outOfRange = new List<ServerNPC>();
			List<int> Rmed = new List<int>();

			ServerNPC castor = null;
			int cnt = checkList.Count;
			for(int i = 0; i < cnt; ++ i) {
				Pair pair = checkList[i];
				float radius = pair.castor.data.configData.radius + 
					pair.target.data.configData.radius + cfg.Param1 * Consts.OneThousand;
				bool isIn = SelectorTools.IsInRange(pair.castor.transform.position, radius, pair.target.transform.position);
				if(isIn == false) {
					castor = pair.castor;
					outOfRange.Add(pair.target);
					Rmed.Add(i);
				}
			}

			if(outOfRange.Count > 0)  {
				///
				/// 做一个概率上的检测
				///
				IEnumerable<ServerNPC> itor = outOfRange.Where( n => PseudoRandom.getInstance().happen(cfg.Prob) );
				warMgr.triMgr.trigCastor.cast(castor, itor, cfg);
			}

			if(Rmed.Count > 0) {
				int count = Rmed.Count;
				for(int i = 0; i < count; ++ i) {
					checkList.RemoveAt(Rmed[i]);
				}
			}

		}

		void HandleHp() {

		}

		#endregion

	}
}
