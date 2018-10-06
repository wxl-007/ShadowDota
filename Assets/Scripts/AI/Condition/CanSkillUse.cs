using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.AI;
using AW.Data;
using System.Collections.Generic;

namespace AW.AI
{
	[TaskDescription("英雄技能是否可以使用")]
	[TaskCategory("Hero")]
	public class CanSkillUse : Conditional
	{
		public SharedLifeNPC target;			//技能使用目标
		public SharedRtSkillData rtSkill;		//当前选中的技能
        private ServerLifeNpc npc;
		private List<RtSkData> skillList;
		private SkillCastor npcSkill;

		public override void OnAwake()
		{
            npc = GetComponent<ServerLifeNpc> ();
			npcSkill = WarServerManager.Instance.npcSkill;
			skillList = new List<RtSkData> ();
			for (short i = 0; i < Consts.MAX_SKILL_COUNT; i++)
			{
				skillList.Add( npc.runSkMd.getRuntimeSkill (i));
			}

			rtSkill.Value = null;
		}

		public override TaskStatus OnUpdate()
		{
            #if USE_SKILL
			for (int i = 0; i < skillList.Count; i++)
			{
				//如果技能cd好了，并且技能可以对当前target使用，返回true
				if (skillList[i] != null && skillList [i].canCast && npcSkill.IsValidTarget(npc, target.Value, skillList[i]))
				{
					rtSkill.Value = skillList [i];
					return TaskStatus.Success;
				}
			}
            #endif
			return TaskStatus.Failure;
		}

		public override void OnDrawGizmos()
		{
			#if UNITY_EDITOR && DEBUG
			var oldColor = UnityEditor.Handles.color;
			UnityEditor.Handles.color = Color.red;

			if(npc != null && rtSkill.Value != null && target.Value != null)
			{
				UnityEditor.Handles.DrawWireDisc(npc.transform.position, npc.transform.up, npc.data.configData.radius + rtSkill.Value.skillCfg.Distance + target.Value.data.configData.radius);
			}

			UnityEditor.Handles.color = oldColor;
			#endif
		}

	}
}