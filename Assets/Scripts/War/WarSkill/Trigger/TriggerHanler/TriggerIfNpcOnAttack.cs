using System;
using AW.Data;
using AW.Message;
using AW.Framework;
using System.Collections.Generic;

namespace AW.War {
	/// <summary>
	/// Trigger if npc is on killed.
	/// 主动打NPC的触发器
	/// </summary>
	[Trigger(Cmd = WarMsg_Type.OnAttacked)]
	public class TriggerIfNpcOnAttack : Trigger, ITriggerItem {
		#region ITriggerItem implementation

		public int GetID () {
			return TriggerId;
		}

		public void OnHappen (MsgParam msg, WarServerNpcMgr npcMgr) {

			WarAnimParam warParam = msg as WarAnimParam;
			if(warParam != null) {

				switch(warParam.OP) {
				case EffectOp.Bullet_NPC:
					BulletNPCCreator(warParam, npcMgr);
					break;
				case EffectOp.HookNpc:
					HookNpcOnAttack(warParam, npcMgr);
					break;
				}

			}

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

		void BulletNPCCreator(WarAnimParam warParam, WarServerNpcMgr npcMgr) {
			ServerNPC caster = npcMgr.GetNPCByUniqueID(warParam.Sender);
			ServerNPC suffer = npcMgr.GetNPCByUniqueID(warParam.Receiver);

			SelfDescribed des = new SelfDescribed() {
				src    = warParam.arg1,
				target = warParam.arg2,
			};

			SufferBulletEffect bulletSuf = warMgr.sufMgr.getImplement<SufferBulletEffect>(EffectOp.Bullet_NPC);
			bulletSuf.Suffer(caster, suffer, des, npcMgr);
		}

		void HookNpcOnAttack(WarAnimParam warParam, WarServerNpcMgr npcMgr) {
			ServerNPC caster = npcMgr.GetNPCByUniqueID(warParam.Sender);
			ServerNPC suffer = npcMgr.GetNPCByUniqueID(warParam.Receiver);

			SufferHookEffect hookSuf = warMgr.sufMgr.getImplement<SufferHookEffect>(EffectOp.HookNpc);
			hookSuf.Suffer(caster, suffer, warParam.described, npcMgr);
		}
	}
}
