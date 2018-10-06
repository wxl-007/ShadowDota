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
	public class UseSkill : Action
	{
		public SharedLifeNPC target;
		public SharedRtSkillData curSkill;
        private ServerLifeNpc myHero;
        private AIPath pathFind;
		private short skillIdx = -1;

		public override void OnAwake()
		{
            myHero = GetComponent<ServerLifeNpc>();
            pathFind = myHero.pathFinding;
		}

		public override void OnStart()
		{
			for (short i = 0; i < 4; i++)
			{
				RtSkData sk = myHero.runSkMd.getRuntimeSkill (i);
				if (sk != null && sk.skillCfg.ID == curSkill.Value.skillCfg.ID)
				{
					skillIdx = i;
					break;
				}
			}

			if (skillIdx == -1)
				ConsoleEx.DebugError (myHero.name + "  not find skill idx ::  " + curSkill.Value.skillCfg.Name);
		}

		public override TaskStatus OnUpdate()
		{
			#if USE_SKILL
			// 如果不是正在释放技能
			if (myHero.curStatus != NpcStatus.InSkill)
			{
				//如果当前技能可以对目标使用
                if (WarServerManager.Instance.npcSkill.canCast (myHero, target.Value, curSkill.Value))
				{
					curSkill.Value = null;
					myHero.CastSkill (skillIdx);
				}
			}
			#endif
			return TaskStatus.Failure;
		}
	}
}
