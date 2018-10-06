using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.Data;
using System.Collections.Generic;

namespace AW.AI
{
    [TaskDescription("自己单独作战，对方人数比自己多")]
	[TaskCategory("Hero")]
	public class MoreEnemys : Conditional
	{
        private ServerLifeNpc npc;
        private Transform mTrans;
        private float seekRange;

        private ServerLifeNpc[] enemyList;
        private ServerLifeNpc[] teamList;
        private WarServerCharactor chaPool;
        private List<ServerLifeNpc> towers;

        public override void OnAwake()
        {
            npc = GetComponent<ServerLifeNpc> ();
            mTrans = npc.transform;
            seekRange = npc.data.configData.seekRange * 2.0f;
            chaPool = WarServerManager.Instance.realServer.monitor.CharactorPool;
        }


		public override void OnStart()
		{
            if (npc.Camp == CAMP.Player)
            {
                enemyList = chaPool.GetHeroList(CAMP.Enemy).ToArray();
                teamList = chaPool.GetHeroList(CAMP.Player).ToArray();
            }
            else if (npc.Camp == CAMP.Enemy)
            {
                teamList = chaPool.GetHeroList(CAMP.Enemy).ToArray();
                enemyList  = chaPool.GetHeroList(CAMP.Player).ToArray();
            }
		}

		public override TaskStatus OnUpdate()
		{
            //如果自己单独作战，对面敌人大于1，并且不在塔下，返回成功
            List<ServerLifeNpc> heros = AITools.GetAllNPCInRange(mTrans.position, seekRange, teamList);
            if (heros.Count == 1 && !IsUnderTower())
            {
                List<ServerLifeNpc> enemys = AITools.GetAllNPCInRange(mTrans.position, seekRange, enemyList);
                if (enemys.Count >= 2)
                {
                    npc.data.btData.btStatus = NPCBattle_Status.EscapeToDef;
                    return TaskStatus.Success;
                }
            }
              
            return TaskStatus.Failure;
		}

        bool IsUnderTower()
        {
            towers = WarServerManager.Instance.npcMgr.GetBuildByType(npc.Camp, BuildNPCType.Tower);
            if (towers == null || towers.Count == 0)
                return false;

            ServerLifeNpc nearTower = AITools.GetNeareastNPC(mTrans.position, towers.ToArray());
            if (nearTower != null)
            {
                // 塔后面 seekrange一半的地方以内
                Vector3 targetPos = nearTower.transform.forward * nearTower.data.configData.seekRange * -1;
                if (AITools.IsInRange (mTrans.position, nearTower.data.configData.seekRange * 0.5f, targetPos))
                {
                    return true;
                }
            }

            return false;
        }
	}
}