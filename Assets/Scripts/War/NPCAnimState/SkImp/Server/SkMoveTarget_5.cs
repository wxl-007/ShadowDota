using UnityEngine;
using System.Collections;
using AW.Data;
using AW.Message;

namespace AW.War
{
    public class SkMoveTarget_5 : BaseSkImp, ISkImp 
    {
        public SkMoveTarget_5() : base()
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
                        EndResult result = sd.targetEnd;
                        if (result != null)
                        {
                            WarServerManager mgr = WarServerManager.Instance;
                            int target = result.param3;
                            ServerNPC npc = mgr.npcMgr.GetNPCByUniqueID(target);
                            if(npc != null)
                            {
                                Vector3 dir = npc.transform.position;
                                dir.y = castor.transform.position.y;
                                dir = dir - castor.transform.position;
                                Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
                                castor.transform.rotation = rot;
                                LeanTween.cancel(castor.gameObject);
                                LeanTween.move(castor.gameObject, npc.transform.position, 0.2f);
                                castor.SendNpcMoveMsg(true);
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
