using UnityEngine;
using System.Collections;
using AW.War;
using AW.Data;

namespace AW.War
{
    public class SkillChooseItem : BattleUIItemBase
    {

        public UISprite sprite;
        public int index;
        public UISprite cd;
        public UILabel cdTime;

        private float cdValue;
        private float baseCd;

        private ClientNPC cachedNpc;
        private NpcSkillAttr attr = null;

        // Use this for initialization
        void Start()
        {
	
        }

        void FixedUpdate()
        {
            if(attr != null)
            {
                if (attr.isInCd && attr.baseCd != 0)
                {
                    cd.fillAmount = attr.cdValue / attr.baseCd;
                    if(!cdTime.enabled)
                    {
                        cdTime.enabled = true;
                    }
                    cdTime.text = attr.cdValue.ToString("00");
                }
                else
                {
                    cd.fillAmount = 0f;
                    cdTime.enabled = false;
                }
            }
        }

        public override void SetItemController(WarUI root)
        {
            rootUI = root;
            root.onHeroSwitch += OnHeroSelected;
        }

        public void OnHeroSelected(int id)
        {
            Debug.Log("switch to npc : " + id);
            WarClientManager mgr = WarClientManager.Instance;
            if (mgr != null)
            {
                cachedNpc = mgr.npcMgr.GetNpc(id);
                if(cachedNpc != null)
                {
                    string name = cachedNpc.data.configData.model + "_skill_" + index;
                    sprite.spriteName = name;
                    attr = cachedNpc.GetSkillAttr(index - 1);
                }
            }
        }

    }
}
