using UnityEngine;
using System.Collections;
using AW.Data;
using AW.Message;
using AW.Resources;

namespace AW.War
{
    public class SkBulletNpc_16 : BaseSkImp, ISkImp 
    {
        protected GameObject bulletNpc;

        public SkBulletNpc_16() : base()
        {

        }

        #region ISkImp

        public void InitSk(ServerNPC npc, WarMsgParam msg)
        {
            base.castor = npc;
            base.param = msg;
            bulletNpc = WarEffectLoader.Load("ServerEffect/ServerBulletNpc");
        }

        public void CastSk(params Object[] args)
        {
            if(castor != null && param != null)
            {
                WarSrcAnimParam srcParam = param as WarSrcAnimParam;
                if(srcParam != null)
                {
                    SelfDescribed sd = srcParam.described;
                    if (sd != null)
                    {
                        EndResult result = sd.targetEnd;
                        if(result != null)
                        {
                            int modelId = result.param3;
                            VirtualNpcLoader loader = Core.ResEng.getLoader<VirtualNpcLoader>();

                            Transform tran = castor.transform;
                            Vector3 pos = tran.position + tran.rotation * Vector3.forward;
                            pos.y = 0.1f;

                            GameObject obj = loader.LoadBulletNpc(modelId, castor.Camp, pos, tran.rotation);
                            Physics.IgnoreCollision(castor.collider, obj.collider);

                            if (obj != null)
                            {
                                ServerBulletNpc npc = obj.GetComponent<ServerBulletNpc>();
                                {
                                    SendCrtBulletMsg(npc);
                                    castor.addChildNpc(npc);
                                    npc.Init(castor, param);
                                }
                            }
                        }
                    }


                }
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

        void SendCrtBulletMsg(ServerNPC npc)
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
    }
}

