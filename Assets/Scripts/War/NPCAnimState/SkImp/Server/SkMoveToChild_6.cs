using UnityEngine;
using System.Collections;
using AW.Data;
using AW.Message;

namespace AW.War
{
    public class SkMoveToChild_6 : BaseSkImp, ISkImp 
    {
        public SkMoveToChild_6() : base()
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
                if(srcParam != null)
                {
                    SelfDescribed sd = srcParam.described;
                    if(sd != null)
                    {
                        EndResult result = sd.srcEnd;
//                        float x = result.param8;
//                        float z = result.param9;
                        Vector3 pos = Vector3.zero;
//                        castor.transform.position = pos;
//                        castor.SendNpcMoveMsg(true);
                        if(castor is ServerLifeNpc)
                        {
                            ServerLifeNpc sCastor = castor as ServerLifeNpc;
                            sCastor.HitAnimReset();
                        }
                        int clearFlag = result.param1;
                        if (clearFlag == 0)
                        {
                            ServerNPC npc = castor.getOneChildNpc(result.param2);
                            if (npc != null)
                            {
                                pos = npc.transform.position;
                                pos.y = 0.1f;
                                castor.transform.position = pos;
                                castor.SendNpcMoveMsg(true);
                                castor.removeChild(npc);
                                WarServerManager mgr = WarServerManager.Instance;
                                IpcDestroyNpcMsg msg = new IpcDestroyNpcMsg();
                                msg.id = npc.UniqueID;
                                mgr.realServer.proxyCli.NpcDestroy(msg);
                                UnityEngine.GameObject.Destroy(npc.gameObject);
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
    }
}
