using UnityEngine;
using System.Collections;
using AW.Resources;

namespace AW.War
{
    public class ClientAnimState_1001 : ClientNpcAnimState {

    	// Use this for initialization
        public override void Start () {
            base.Start();
    	}
    	
    	// Update is called once per frame
        public override void Update () {
            base.Update();
    	}

        public override void CreateEffect(NpcAnimEffect effect)
        {
            GameObject obj = null;
            string src = "";
            switch(effect)
            {
                case NpcAnimEffect.Skill_3_Start:
                    src = curMsg.ecd.Start;
                    if(!string.IsNullOrEmpty(src) && src != "[]")
                    {
                        obj = WarEffectLoader.Load(src);
                        obj = Instantiate(obj, cachedTran.position, cachedTran.rotation) as GameObject;
                        obj.transform.parent = cachedTran;
                        Destroy(obj, 4f);
                    }
                    break;
                case NpcAnimEffect.Trigger:
                    src = curMsg.ecd.Start;
                    if(!string.IsNullOrEmpty(src) && src != "[]")
                    {
                        obj = WarEffectLoader.Load(src);
                        obj = Instantiate(obj, cachedTran.position, cachedTran.rotation) as GameObject;
                        obj.transform.parent = cachedTran;
                        Destroy(obj, 0.4f);
                    }
                    break;
                default:
                    base.CreateEffect(effect);
                    break;
            }
        }
    }
}
