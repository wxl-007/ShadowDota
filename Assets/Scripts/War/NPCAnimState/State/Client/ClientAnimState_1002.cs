using UnityEngine;
using System.Collections;
using AW.Resources;

namespace AW.War
{
    public class ClientAnimState_1002 : ClientNpcAnimState {

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
                case NpcAnimEffect.Skill_1:
                    src = curMsg.ecd.Middle;
                    if(!string.IsNullOrEmpty(src) && src != "[]")
                    {
                        obj = WarEffectLoader.Load(src);
                        if (obj != null)
                        {
                            obj = Instantiate(obj, attackPoint.position, cachedTran.rotation) as GameObject;
                            SkillEffectBase seb = obj.GetComponent<SkillEffectBase>();
                            if (seb != null)
                            {
                                seb.LifeTime(1f);
                                seb.EmitEffect(null, null, false);
                            }
                            else
                            {
                                Destroy(seb, 1f);
                            }
                        }
                    }
                    break;
                case NpcAnimEffect.Skill_2:
                    src = curMsg.ecd.Middle;
                    if(!string.IsNullOrEmpty(src) && src != "[]")
                    {
                        obj = WarEffectLoader.Load(src);
                        if(obj != null)
                        {
                            Vector3 pos = Vector3.zero;
                            ClientNPC npc = cliMgr.npcMgr.GetNpc(curMsg.targetId);
                            if (npc != null)
                            {
                                pos = npc.CachedTran.position;
                            }
                            else
                            {
                                pos = cachedTran.position + cachedTran.rotation * Vector3.forward * 5f;
                            }
                            obj = Instantiate(obj, pos, cachedTran.rotation) as GameObject;
                            Destroy(obj, 3f);
                        }
                    }
                    break;
                case NpcAnimEffect.Skill_3:
                    src = curMsg.ecd.Middle;
                    if(!string.IsNullOrEmpty(src) && src != "[]")
                    {
                        obj = WarEffectLoader.Load(src);
                        if(obj != null)
                        {
                            obj = Instantiate(obj, cachedTran.position, cachedTran.rotation) as GameObject;
                            if(obj != null)
                            {
                                ClientNPC npc = cliMgr.npcMgr.GetNpc(curMsg.targetId);
                                SkillEffectBase seb = obj.GetComponent<SkillEffectBase>();
                                if(npc != null && seb != null)
                                {
                                    Vector3 dir = npc.transform.position;
                                    dir.y = cachedTran.position.y;
                                    dir = dir - cachedTran.position;
                                    Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
                                    obj.transform.rotation = rot;
                                    seb.EmitEffect(cachedNpc, npc, false);
                                }
                                Destroy(obj, 0.2f);
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
