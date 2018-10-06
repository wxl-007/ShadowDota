using UnityEngine;
using System.Collections;
using AW.Resources;

namespace AW.War
{
    public class ClientAnimState_1003 : ClientNpcAnimState {

    	// Use this for initialization
        public override void Start () {
            base.Start();
    	}
    	
    	// Update is called once per frame
        public override void Update () {
            base.Update();
    	}

        public override void On_Killed(WarMsgParam param)
        {
            animator.SetLayerWeight(2, 0);
            base.On_Killed(param);
        }

        public override void On_Respawn(WarMsgParam param)
        {
            animator.SetLayerWeight(2, 1);
            base.On_Respawn(param);
        }

        public override void CreateEffect(NpcAnimEffect effect)
        {
            GameObject obj = null;
            string src = "";
            switch(effect)
            {
                case NpcAnimEffect.Skill_1:

                    break;
                case NpcAnimEffect.Skill_2:
                    src = curMsg.ecd.Middle;
                    if(!string.IsNullOrEmpty(src) && src != "[]")
                    {
                        obj = WarEffectLoader.Load(src);
                        obj = Instantiate(obj, cachedTran.position, cachedTran.rotation) as GameObject;
                        Destroy(obj, 3f);
                    }
                    break;
                case NpcAnimEffect.Skill_3:

                    break;
                default:
                    base.CreateEffect(effect);
                    break;
            }
        }
    }
}
