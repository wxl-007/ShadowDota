using UnityEngine;
using System.Collections;
using AW.Framework;
using AW.Data;
using AW.Message;
using AW.Resources;
using BehaviorDesigner.Runtime;

namespace AW.War
{
    public class SkCrtNpc_8 : BaseSkImp, ISkImp 
    {
        public SkCrtNpc_8() : base()
        {

        }

        #region ISkImp

        public void InitSk(ServerNPC npc, WarMsgParam msg)
        {
            base.castor = npc;
            base.param = msg;
        }

        public void CastSk(params Object[] args)
        {
            if(castor != null && param != null)
            {
                WarSrcAnimParam srcParam = param as WarSrcAnimParam;
                if (srcParam != null)
                {
                    SelfDescribed sd = srcParam.described;
                    if(sd != null)
                    {
                        EndResult result = sd.srcEnd;
                        CreatNpcDepandency cnd = (CreatNpcDepandency)result.obj;
                        if(cnd != null && cnd.TargetVector3 != null)
                        {
                            VirtualNpcLoader loader = Core.ResEng.getLoader<VirtualNpcLoader>();

                            GameObject obj = null;
                            Vector3 pos = Vector3.zero;
                            foreach(Vec3F v in cnd.TargetVector3)
                            {
                                pos = v.toUnityVec3();
                                int id = result.param1;
                                obj = loader.LoadNpcObj(id, castor.Camp, pos, Quaternion.LookRotation(Vector3.back));
                                if(obj != null)
                                {
                                    ServerNPC npc = obj.GetComponent<ServerNPC>();
                                    if(npc != null)
                                    {
                                        SendCrtNpcMsg(npc);
                                        castor.addChildNpc(npc);
                                        npc.data = cnd.Source;
                                        npc.Init(castor, srcParam);
                                        if(npc.collider != null)
                                        {
                                            npc.collider.enabled = cnd.IsCollide;
                                        }
                                        if (npc is LifeSummonNPC)
                                        {
                                            AttachAI(npc, result.param2);
                                        }
                                        if(cnd.Buff_IDs != null)
                                        {
                                            for(int i = 0; i < cnd.Buff_IDs.Length; i++)
                                            {
                                                BuffCtorParam bp = new BuffCtorParam();
                                                bp.bufNum = cnd.Buff_IDs[i];
                                                bp.fromNpcId = castor.UniqueID;
                                                bp.toNpcId = npc.UniqueID;
                                                bp.origin = AW.Data.OriginOfBuff.Alone;
                                                bp.duration = Consts.USE_BUFF_CONFIG_DURATION;
                                                bp.initLayer = 1;

                                                WarServerManager.Instance.bufMgr.createBuff(bp);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void AttachAI(ServerNPC npc, int ai)
        {
            ServerLifeNpc lifeNpc = npc as ServerLifeNpc;
            if(lifeNpc == null)
            {
                return;
            }
            lifeNpc.data.btData = new NPCBattleData();
            lifeNpc.data.btData.way = BATTLE_WAY.None;
            AILoader AiLoader = Core.ResEng.getLoader<AILoader>();
            NPCAIType type = (NPCAIType)ai;
            switch(type)
            {
                case NPCAIType.Pathfind_Atk:
                    {
                        BehaviorTree tree = npc.gameObject.GetComponent<BehaviorTree> ();
                        if(tree == null)
                            tree = npc.gameObject.AddComponent<BehaviorTree>();

                        tree.ExternalBehavior = AiLoader.load (AILoader.PATHFIND_ATK);
                        tree.StartWhenEnabled = true;
                        tree.RestartWhenComplete = true;
                        lifeNpc.AutoAiTree = tree;
                    }
                    break;
                case NPCAIType.Simple_PfAtk:
                    {
                        BehaviorTree tree = npc.gameObject.GetComponent<BehaviorTree> ();
                        if(tree == null)
                            tree = npc.gameObject.AddComponent<BehaviorTree>();

                        tree.ExternalBehavior = AiLoader.load (AILoader.SIMPLE_PFATK);
                        tree.StartWhenEnabled = true;
                        tree.RestartWhenComplete = true;
                        lifeNpc.AutoAiTree = tree;
                    }
                    break;
                case NPCAIType.Patrol:
                    {
                        BehaviorTree tree = npc.gameObject.GetComponent<BehaviorTree> ();
                        if(tree == null)
                            tree = npc.gameObject.AddComponent<BehaviorTree>();

                        tree.ExternalBehavior = AiLoader.load (AILoader.NORMAL_ATTACK);
                        tree.StartWhenEnabled = true;
                        tree.RestartWhenComplete = true;
                        lifeNpc.AutoAiTree = tree;
                    }
                    break;
            }
        }

        void SendCrtNpcMsg(ServerNPC npc)
        {
            IpcCreateNpcMsg msg = new IpcCreateNpcMsg();
            msg.npclist = new CrtHero[1];
            msg.npclist[0] = new CrtHero();
            msg.npclist[0].npcID = npc.data.configData.ID;
            msg.npclist[0].uniqueId = npc.UniqueID;
            msg.npclist[0].camp = (int)npc.Camp;
            msg.npclist[0].pos = VectorWrap.ToVector(npc.transform.position);
            msg.npclist[0].rotation = VectorWrap.ToVector(npc.transform.eulerAngles);
            WarServerManager mgr = WarServerManager.Instance;
            if(mgr != null)
            {
                mgr.realServer.proxyCli.CtorNpc(msg);
            }
        }

        public void SkHandle()
        {

        }

        public void Reset()
        {
            param = null;
        }

        public EffectOp SkOp()
        {
            return OP;
        }

        #endregion
    }
}

