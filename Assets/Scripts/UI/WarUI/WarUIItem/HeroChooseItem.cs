using UnityEngine;
using System.Collections;
using AW.Data;
using System.Collections.Generic;
using AW.War;

namespace AW.War
{
    public class HeroChooseItem : BattleUIItemBase
    {

        public UISprite head;
        public int index;
        public Transform fist;
        public UIProgressBar health;

        protected ClientNPC cachedNpc;

        // Use this for initialization
        void Start()
        {
        
        }

        public override void SetItemController(WarUI root)
        {
            rootUI = root;
            root.onHeroSwitch += OnHeroSelected;
            WarClientManager mgr = WarClientManager.Instance;
            if(mgr != null)
            {
                ClientNPC npc = mgr.clientTeam.get(index);
                if (npc != null)
                {
                    cachedNpc = npc;
                    npc.animState.HeroHealthBar = health;
                    string name = "head_" + npc.data.configData.model;
                    head.spriteName = name;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void OnHeroSelected(int id)
        {
            UISprite sp = health.foregroundWidget as UISprite;
            if (cachedNpc != null)
            {
                if (id == cachedNpc.UniqueID)
                {
                    Vector3 pos = fist.transform.localPosition;
                    pos.y = transform.localPosition.y + 40;
                    fist.transform.localPosition = pos;
                    sp.spriteName = "battle-022";
                }
                else
                {
                    sp.spriteName = "battle-021";
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
//            LifeNPC npc = WarManager.Instance.npcMgr.GetMember(index);
//            if (npc != null)
//            {
//                npc.playHero.UIHealthBar = health;
//            }
//            if (id == index)
//            {
//                Vector3 pos = fist.transform.localPosition;
//                pos.y = transform.localPosition.y + 40;
//                fist.transform.localPosition = pos;
//                sp.spriteName = "battle-022";
//            }
//            else
//            {
//                sp.spriteName = "battle-021";
//            }
        }

        public void SwitchOk(string msg)
        {
            Debug.Log(gameObject.name + ":::" + msg);
        }

        void Init(int id)
        {
            string name = "head_" + id;
            head.spriteName = name;
        }
    }
}