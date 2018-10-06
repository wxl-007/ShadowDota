using System;
using AW.Data;
using AW.Message;

namespace AW.War {
	/// <summary>
	/// Trigger if npc is on killed.
	/// 被杀死NPC的触发器
	/// </summary>
	[Trigger(Cmd = WarMsg_Type.BeKilled)]
	public class TriggerIfNpcBeKilled : Trigger, ITriggerItem {

		#region ITriggerItem implementation
		public int GetID () {
			return TriggerId;
		}

		/// <summary>
		/// 目前这里会监听战斗结束的消息
		/// 
		/// </summary>
		/// <param name="msg">Message.</param>
		public void OnHappen (MsgParam msg, WarServerNpcMgr npcMgr) {
			//最后检测，其他优先检测
			NeheQiaoModeComplete(msg, npcMgr);
			//数据监测
			StatictisInSkill.instance.analyze(msg, npcMgr);
			//清理逻辑
			clearDeadNpcStatus(msg, npcMgr);

			OnEnd();
		}


		public void OnRest () {
			///
			/// 没有特殊的值需要变量需要清空
			/// 
		}

		#endregion

		/// <summary>
		/// 不需要被触发后，还每一帧继续触发。也就是说这个触发器执行一次
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public override bool TickPerFrame () {
			return false;
		}


		//奈河桥的模式
		public void NeheQiaoModeComplete(MsgParam msg, WarServerNpcMgr npcMgr) {
			NeHeQiaoNpcMgr mgr = npcMgr as NeHeQiaoNpcMgr;
			if(mgr != null) {
				ServerNPC selfBase = mgr.SelfMilitaryBase;
				ServerNPC enemyBase = mgr.EnemyMilitaryBase;

				WarAnimParam warMsg = msg as WarAnimParam;

				if(warMsg != null && warMsg.described != null) {
					int deadNpcId = warMsg.described.srcEnd.param2;

					WarMsg_Type enemyRes = WarMsg_Type.Lose;
					WarMsg_Type selfRes  = WarMsg_Type.Lose;
					if(deadNpcId == selfBase.UniqueID) {
						//我方基地死亡
						enemyRes = WarMsg_Type.Win;
					} else if (deadNpcId == enemyBase.UniqueID) {
						//敌方基地死亡
						selfRes = WarMsg_Type.Win;
					}

					///
					/// ------ 确实是基地死亡了 -----------
					///
					if(enemyRes != WarMsg_Type.Lose || selfRes != WarMsg_Type.Lose) {
						WarCampMsg eParam = new WarCampMsg ();
						eParam.SendCamp = CAMP.Enemy;
						eParam.ReceCamp = CAMP.Enemy;
						eParam.cmdType  = enemyRes;
						mgr.SendMessageAsync (CAMP.Enemy, CAMP.Enemy, eParam, false);

						WarCampMsg sParam = new WarCampMsg ();
						sParam.SendCamp = CAMP.Player;
						sParam.ReceCamp = CAMP.Player;
						sParam.cmdType  = selfRes;
						mgr.SendMessageAsync (CAMP.Player, CAMP.Player, sParam, false);
					}
				}
			}
		}

		//清理逻辑
		void clearDeadNpcStatus(MsgParam msg, WarServerNpcMgr npcMgr) {
			WarAnimParam warMsg = msg as WarAnimParam;
			if(warMsg != null && warMsg.described != null) {
				int deadNpcId = warMsg.described.srcEnd.param2;
				ServerNPC npc = npcMgr.GetNPCByUniqueID(deadNpcId);
				ServerLifeNpc life = npc as ServerLifeNpc;
				if(life != null) {
					life.curStatus = NpcStatus.None;

					///
					/// ---- 解除挂载的Trigger ----
					///
					WarServerManager.Instance.triMgr.RemoveAllTrigger(deadNpcId);

					///
					/// ---- 解除挂载的Buff ----
					/// 
					WarServerManager.Instance.bufMgr.rmAllBuff(deadNpcId);

					//--- 解除仇恨信息 ----
					life.clearHatred();
				}
			}
		}


	}
}
