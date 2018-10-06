using AW.War;
using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.Data;
using AW.Resources;

namespace AW.AI
{
	[TaskDescription("npc刷新")]
	[TaskCategory("Dota")]
	public class CreateSoldier : Action
	{
		//当前刷新波数索引
		public SharedInt curGroupIndex;	

		//当前造兵波数的计数
		public SharedInt curGroupCnt;

		//同一波兵中造兵的索引，正在造第几个兵
		public SharedInt curIndex;
		//下一波兵的个数
		public Repeater npcCnt;

		//要造的小兵的NUM
		private int npcNum;
		//自己身上的npc脚本缓存
		private BNPC myHero;
        private VirtualNpcLoader loader;
		private GameObject mWarPoint;
		private FreshGroupModel freshGroupModel;
		private FreshPoolModel freshPoolModel;
		private AILoader AiLoader;

		private NPCFreshPool curPool;
		private NPCFreshGroup curGroup;

        private IpcCreateNpcMsg crtMsg;
        private IpcNpcHpMsg hpMsg;

		public override void OnAwake() {
            loader = Core.ResEng.getLoader<VirtualNpcLoader>();
            mWarPoint = GameObject.FindGameObjectWithTag("WarServer");
			freshGroupModel = Core.Data.getIModelConfig<FreshGroupModel>();
			freshPoolModel = Core.Data.getIModelConfig<FreshPoolModel>();
			AiLoader = Core.ResEng.getLoader<AILoader>();

			myHero = gameObject.GetComponent<BNPC>();
			curPool = freshPoolModel.GetNPCFreshPool (myHero.dataInScene.freshParam.freshPoolID);

            crtMsg = new IpcCreateNpcMsg();
            crtMsg.npclist = new CrtHero[1];
            crtMsg.npclist[0] = new CrtHero();

            hpMsg = new IpcNpcHpMsg();
		}

		public override TaskStatus OnUpdate()
		{
			curGroup = freshGroupModel.GetFreshGroup (curPool.freshPool[curGroupIndex.Value]);

			if (curGroup != null)
				npcCnt.count.Value = curGroup.freshGroup.Count;
			else
			{
				//空的一波怪
				curIndex.Value = 0;
				curGroupIndex.Value++;
				curGroupCnt.Value++;

				//循环利用刷新池的数据
				if (curGroupIndex.Value >= curPool.freshPool.Count)
				{
					curGroupIndex.Value = 0;
				}

				return TaskStatus.Success;
			}
			if(curIndex.Value < curGroup.freshGroup.Count)
				npcNum = curGroup.freshGroup [curIndex.Value];

			
            ServerNPC newNpc = WarServerManager.Instance.npcMgr.GetNpcFromCache (npcNum);
			if (newNpc != null)
            {
                WarServerManager.Instance.npcMgr.RemoveNpcFromCache (newNpc);
			}
			else
			{
                newNpc = loader.Load (npcNum, myHero.dataInScene.camp, mWarPoint);
			}

            GameObject go = newNpc.gameObject;
            go.SetActive(true);
			go.transform.localPosition = this.transform.localPosition;
			newNpc.data.rtData.curHp = newNpc.data.rtData.totalHp;
			newNpc.Camp = myHero.dataInScene.camp;
			newNpc.data.btData.way = myHero.dataInScene.way;
			newNpc.dataInScene = myHero.dataInScene;
		
            //发送消息通知客户端
            SendCreateNpcMsg(newNpc);
            SendHpMsg(newNpc);

			InitAi (newNpc);
			InitBuff (newNpc);

            if(newNpc is ServerLifeNpc)
			{
                ServerLifeNpc life = newNpc as ServerLifeNpc;
				life.SwitchAutoBattle(true);
			}

			//同一拨，当前造兵索引++
			curIndex.Value++;

			//同一拨，如果造了最后一个兵，当前造兵波数++
			if (curIndex.Value >= curGroup.freshGroup.Count)
			{
				curIndex.Value = 0;
				curGroupIndex.Value++;
				curGroupCnt.Value++;

				//循环利用刷新池的数据
				if (curGroupIndex.Value >= curPool.freshPool.Count)
				{
					curGroupIndex.Value = 0;
				}

				return TaskStatus.Success;
			}
			return TaskStatus.Success;
		}

		private void InitBuff(BNPC npc)
		{
//			//初始化buff
//			if (npc.dataInScene.buffs != null && npc.dataInScene.buffs.Length > 0)
//			{
//				for (int j = 0; j < npc.dataInScene.buffs.Length; j++)
//				{
//					BuffCtorParam buffParam = new BuffCtorParam ();
//					buffParam.bufNum = npc.dataInScene.buffs [j];
//					buffParam.fromNpcId = npc.UniqueID;
//					buffParam.toNpcId = npc.UniqueID;
//					buffParam.origin = OriginOfBuff.Alone;
//					buffParam.initLayer = 1;
//					WarServerManager.Instance.bufMgr.createBuff (buffParam);
//				}
//			}
		}


		//初始化AI
        private void InitAi(ServerNPC npc)
		{
			//			//如果是npc刷新点
            if (npc.data.num == NpcMgr<BNPC>.FRESH_NPC)
			{
				#if NO_SOLDIER
				#else
				BehaviorTree tree = npc.gameObject.GetComponent<BehaviorTree> ();
				if(tree == null)
					tree = npc.gameObject.AddComponent<BehaviorTree>();

				tree.ExternalBehavior = AiLoader.load (AILoader.NPC_FRESH);
				tree.StartWhenEnabled = true;
				tree.RestartWhenComplete = false;
				#endif
			}
			else
			{
				NPCAIType type = (NPCAIType)npc.dataInScene.AIType;
                ServerLifeNpc life = npc as ServerLifeNpc;
				switch (type)
				{
				case NPCAIType.Pathfind_Atk:
					{
						BehaviorTree tree = npc.gameObject.GetComponent<BehaviorTree> ();
						if(tree == null)
							tree = npc.gameObject.AddComponent<BehaviorTree>();

						tree.ExternalBehavior = AiLoader.load (AILoader.PATHFIND_ATK);
						tree.StartWhenEnabled = true;
						tree.RestartWhenComplete = true;

						if (life != null && !life.pathFinding.enabled)
							life.pathFinding.enabled = true;

						if (!tree.enabled)
							tree.enabled = true;
						break;
					}
                case NPCAIType.Simple_PfAtk:
                {
                    BehaviorTree tree = npc.gameObject.GetComponent<BehaviorTree> ();
                    if(tree == null)
                        tree = npc.gameObject.AddComponent<BehaviorTree>();

                    tree.ExternalBehavior = AiLoader.load (AILoader.SIMPLE_PFATK);
                    tree.StartWhenEnabled = true;
                    tree.RestartWhenComplete = true;

                    if (life != null && !life.pathFinding.enabled)
                        life.pathFinding.enabled = true;

                    if (!tree.enabled)
                        tree.enabled = true;
                    break;
                }

				case NPCAIType.Patrol:
					{
						BehaviorTree tree = npc.gameObject.GetComponent<BehaviorTree> ();
						if(tree == null)
							tree = npc.gameObject.AddComponent<BehaviorTree>();

						tree.ExternalBehavior = AiLoader.load (AILoader.NORMAL_ATTACK);
						tree.StartWhenEnabled = true;
						tree.RestartWhenComplete = true;

						if (life != null && !life.pathFinding.enabled)
							life.pathFinding.enabled = true;

						if (!tree.enabled)
							tree.enabled = true;
							
						break;
					}
				}
			}
		}

        //发送创建npc消息
        void SendCreateNpcMsg(ServerNPC newNpc)
        {
            crtMsg.npclist[0].npcID = newNpc.data.configData.ID;
            crtMsg.npclist[0].uniqueId = newNpc.UniqueID;
            crtMsg.npclist[0].camp = (int)newNpc.Camp;
            crtMsg.npclist[0].pos = VectorWrap.ToVector(newNpc.transform.position);
            crtMsg.npclist[0].rotation = VectorWrap.ToVector(newNpc.transform.eulerAngles);
            WarServerManager.Instance.realServer.proxyCli.CtorNpc(crtMsg);
        }

        //发送hp消息
        void SendHpMsg(ServerNPC newNpc)
        {
            hpMsg.srcID = newNpc.UniqueID;
            hpMsg.uniqueId = newNpc.UniqueID;
            hpMsg.deltaHp = newNpc.data.rtData.totalHp;
            hpMsg.isDamage = false;
            WarServerManager.Instance.realServer.proxyCli.NPChp(hpMsg);
        }
	}
}
