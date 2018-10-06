using AW.Message;
using AW.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AW.War {
    [Trigger(Cmd = WarMsg_Type.OnCollide)]
    public class TriggerOnCollide : Trigger, ITriggerItem {
        #region ITriggerItem implementation

        public int GetID() {
            return TriggerId;
        }

        public void OnHappen (MsgParam msg, WarServerNpcMgr npcMgr) {
            if(msg != null) {
                ServerNPC castor = npcMgr.GetNPCByUniqueID(msg.Sender);
                ServerNPC target = npcMgr.GetNPCByUniqueID(msg.Receiver);

                List<ServerNPC> itor = new List<ServerNPC>();
                itor.Add(target);

                warMgr.triMgr.trigCastor.cast(castor, itor, cfg);
            }
        }

        public void OnRest () {
            cfg = null;
        }

        #endregion

        /// <summary>
        /// 只执行一次
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        public override bool TickPerFrame () {
            return false;
        }
    }
}