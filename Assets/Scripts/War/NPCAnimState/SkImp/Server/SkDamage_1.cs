using UnityEngine;
using System.Collections;
using AW.Data;
using AW.Message;

namespace AW.War
{
    public class SkDamage_1 : BaseSkImp, ISkImp 
    {
        public SkDamage_1() : base()
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
                    WarTarAnimParam[] targets = srcParam.InjureTar;
                    if (targets != null && targets.Length > 0)
                    {
                        WarServerManager mgr = WarServerManager.Instance;
                        for (int i = 0; i < targets.Length; i++)
                        {
                            if (targets[i] != null)
                            {
                                mgr.npcMgr.SendMessageAsync(castor.UniqueID, targets[i].described.target, targets[i]);
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
