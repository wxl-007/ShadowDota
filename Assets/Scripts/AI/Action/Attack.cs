using AW.War;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AW.Message;
using AW.Data;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("战斗")]
	[TaskCategory("Hero")]
	public class Attack : Action
	{
		public SharedLifeNPC target;
        private ServerLifeNpc myHero;
        private AIPath pathFind;

		private WarSrcAnimParam param;

		public override void OnAwake()
		{
            myHero = GetComponent<ServerLifeNpc>();
            pathFind = myHero.pathFinding;
            param = new WarSrcAnimParam ();
		}

		public override TaskStatus OnUpdate()
		{
			//面向target，往死了打
			if (myHero.data.configData.moveable == Moveable.Movable)
			{
				this.transform.LookAt (target.Value.transform);
			}

			if (!myHero.valid)
			{
				return TaskStatus.Success;
			}

			if (pathFind != null && pathFind.enabled)
			{
				pathFind.enabled = false;
			}
				
			myHero.Attack ();

			//如果没有打到人，空打
//			if (msg == null || msg.Count == 0)
//			{
//				if (myHero.outLog)
//				{
//					ConsoleEx.DebugWarning (this.transform.name + "there is no enemy attacked~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
//				}
//				param.Sender = myHero.UniqueID;
//				param.Receiver = 0;
//				param.cmdType = WarMsg_Type.Attack;
//				param.OP = EffectOp.Injury;
//
//				myHero.SendMsg (myHero.UniqueID, param);
//			}
//			else
//			{
//				if (myHero.outLog)
//				{
//					ConsoleEx.DebugWarning (myHero.name + " ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~attck enemy cout :: " + msg.Count);
//				}
//				for (int i = 0; i < msg.Count; i++)
//				{
//					WarSrcAnimParam warParam = msg [i] as WarSrcAnimParam;
//					warParam.cmdType = WarMsg_Type.Attack;
//					myHero.SendMsg (myHero.UniqueID, msg [i]);
//				}
//			}
				
			return TaskStatus.Success;
		}
			

		public override void OnEnd()
		{

		}


		public override void OnDrawGizmos()
		{
			#if UNITY_EDITOR && DEBUG
			var oldColor = UnityEditor.Handles.color;
			UnityEditor.Handles.color = Color.red;
			UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.up, myHero.ATKRange + myHero.data.configData.radius);
			UnityEditor.Handles.color = oldColor;
			#endif
		}
	}
}
