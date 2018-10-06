using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AW.Resources;
using AW.Framework;

namespace AW.War
{
    public class ClientAnimState_1004 : ClientNpcAnimState {

        private GameObject skill2_Obj;

    	// Use this for initialization
        public override void Start () {
            base.Start();
    	}
    	
    	// Update is called once per frame
        public override void Update () {
            base.Update();
    	}

        public override void On_Suffer(WarMsgParam param)
        {
            if(skill2_Obj != null)
            {
                Destroy(skill2_Obj);
            }
            base.On_Suffer(param);
        }

        public override void CreateEffect(NpcAnimEffect effect)
        {
            GameObject obj = null;
            string src = "";
            switch(effect)
            {
                case NpcAnimEffect.Skill_2_Start:
                    {
                        src = curMsg.ecd.Start;
                        if (!string.IsNullOrEmpty(src) && src != "[]")
                        {
                            obj = WarEffectLoader.Load(src);
                            if (obj != null)
                            {
                                skill2_Obj = Instantiate(obj, cachedTran.position, cachedTran.rotation) as GameObject;
                                Destroy(skill2_Obj, curMsg.animationTimer);
                            }
                        }
                    }
                    break;
                case NpcAnimEffect.Skill_3_Start:
                    {
                        Debug.Log(fastJSON.JSON.Instance.ToJSON(curMsg));
                        src = curMsg.ecd.Start;
                        if(!string.IsNullOrEmpty(src) && src != "[]")
                        {
                            obj = WarEffectLoader.Load(src);
                            if (obj != null)
                            {
                                List<Vec3F> pos = curMsg.objCrtV;
                                if(pos != null)
                                {
                                    Vector3 p = pos[0].toUnityVec3();
                                    obj = Instantiate(obj, p, Quaternion.identity) as GameObject;
                                    Destroy(obj, 3f);
                                }
                            }
                        }
                    }
                    break;
                default:
                    base.CreateEffect(effect);
                    break;
            }
        }
    }
}
