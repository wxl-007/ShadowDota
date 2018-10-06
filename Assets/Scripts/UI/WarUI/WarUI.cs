using UnityEngine;
using System.Collections;
using fastJSON;
using AW.Data;

namespace AW.War
{
	public class WarUI : MonoBehaviour
	{

		#region 控制
		private WarClientManager warCliMgr = null;
		protected ProxyServer proxyServer = null;
		#endregion

        #region 组件
        public UIPanel mPanel;
        public UISprite spAuto;
        #endregion

        #region 回调
        public System.Action<int> onHeroSwitch = null;
        #endregion

	    // Use this for initialization
	    void Start()
	    {
            warCliMgr = WarClientManager.Instance;
            if(warCliMgr != null)
            {
                proxyServer = warCliMgr.realCli.proxyServer;
                warCliMgr.realCli.Switch = On_SwitchHero_Ok;
                warCliMgr.realCli.Auto = On_Auto_OK;
                warCliMgr.onCreateHeroFinished = OnCreateHerFinished;
            }
//            StartCoroutine(DelayInitUI());
	    }

        IEnumerator DelayInitUI()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            BattleUIItemBase[] items = GetComponentsInChildren<BattleUIItemBase>();
            if(items != null && items.Length > 0)
            {
                for(int i = 0; i < items.Length; i++)
                {
                    items[i].SetItemController(this);
                }
            }
            warCliMgr.clientTeam.SwitchAutoOrManual(warCliMgr.clientTeam.isAuto);
            spAuto.enabled = warCliMgr.clientTeam.isAuto;
        }
		
	    // Update is called once per frame
	    void Update()
	    {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                On_Attack_Clicked();
            }
            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                On_Skill0_Clicked();
            }
            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                On_Skill1_Clicked();
            }
            if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                On_Skill2_Clicked();
            }
	    }

		#region Mono回调
		void OnEnable()
		{
            WarClientNpcMgr mgr = WarClientManager.Instance.npcMgr;
            if(mgr != null)
            {
                mgr.createUIForNpc = CreateUIForNpc;
            }
			EasyJoystick.On_JoystickTouchStart += On_JoystickStart;
			EasyJoystick.On_JoystickMove += On_JoystickMove;    
			EasyJoystick.On_JoystickTouchUp += On_JoystickMoveEnd;
		}

        public void OnCreateHerFinished()
        {
            BattleUIItemBase[] items = GetComponentsInChildren<BattleUIItemBase>();
            if(items != null && items.Length > 0)
            {
                for(int i = 0; i < items.Length; i++)
                {
                    items[i].SetItemController(this);
                }
            }
            warCliMgr.clientTeam.SwitchAutoOrManual(warCliMgr.clientTeam.isAuto);
            spAuto.enabled = warCliMgr.clientTeam.isAuto;

            if(onHeroSwitch != null)
            {
                onHeroSwitch(warCliMgr.clientTeam.activeNpc.UniqueID);
            }
        }

		void OnDisable()
		{
            WarClientNpcMgr mgr = WarClientManager.Instance.npcMgr;
            if(mgr != null)
            {
                mgr.createUIForNpc = null;
            }
			EasyJoystick.On_JoystickTouchStart -= On_JoystickStart;
			EasyJoystick.On_JoystickMove -= On_JoystickMove;    
			EasyJoystick.On_JoystickTouchUp -= On_JoystickMoveEnd;
		}
		#endregion

		#region 摇杆
		/// <summary>
		/// 摇杆的引用
		/// </summary>
		public EasyJoystick joystick;
        /// <summary>
        /// 每次摇杆计算获得的四元数
        /// </summary>
        private Quaternion qua;
        /// <summary>
        /// 封装后的四元数
        /// </summary>
        private QuaternionWrap wrap;

		void On_JoystickStart(MovingJoystick move)
		{

		}

		void On_JoystickMove(MovingJoystick move)
		{
			float angle = move.Axis2Angle(true); 
            qua = Quaternion.Euler(new Vector3(0, angle, 0));
            wrap = QuaternionWrap.QuaternionToWrap(qua);

            string plainText = JSON.Instance.ToJSON(wrap);
            NpcAnimInfo info = new NpcAnimInfo(){ 
                ClientID = DeviceInfo.GUID,
                nextState = NpcAnimState.ManualInput,
                ui = new WarUIInfo()
                    {
                        ClientID = DeviceInfo.GUID,
                        uniqueId = warCliMgr.clientTeam.activeNpc.UniqueID,
                        camp = WarCamp2Camp.toWarCamp(warCliMgr.clientTeam.activeNpc.Camp),
                    },
                data = plainText,
            };

            plainText = JSON.Instance.ToJSON(info);

            proxyServer.Move(plainText);

            if (warCliMgr.clientTeam.isAuto)
            {
                warCliMgr.clientTeam.SwitchAutoOrManual(false);
            }
		}

		void On_JoystickMoveEnd (MovingJoystick move)
		{
            string plainText = "";
            NpcAnimInfo info = new NpcAnimInfo(){ 
                ClientID = DeviceInfo.GUID,
                nextState = NpcAnimState.Stand,
                ui = new WarUIInfo()
                    {
                        ClientID = DeviceInfo.GUID,
                        uniqueId = warCliMgr.clientTeam.activeNpc.UniqueID,
                        camp = WarCamp2Camp.toWarCamp(warCliMgr.clientTeam.activeNpc.Camp),
                    },
                data = plainText,
            };
            plainText = JSON.Instance.ToJSON(info);
            proxyServer.MoveStop(plainText);
		}
		#endregion

        #region Button
        public void On_Attack_Clicked()
        {
            string plainText = "";
            NpcAnimInfo info = new NpcAnimInfo()
            { 
                ClientID = DeviceInfo.GUID,
                nextState = NpcAnimState.Attack,
                ui = new WarUIInfo()
                    {
                        ClientID = DeviceInfo.GUID,
                        uniqueId = warCliMgr.clientTeam.activeNpc.UniqueID,
                        camp = WarCamp2Camp.toWarCamp(warCliMgr.clientTeam.activeNpc.Camp),
                    },
                data = "",
            };
            plainText = JSON.Instance.ToJSON(info);
            proxyServer.Attack(plainText);
            if (warCliMgr.clientTeam.isAuto)
            {
                warCliMgr.clientTeam.SwitchAutoOrManual(false);
            }
        }

        /// <summary>
        /// 主动技能1
        /// </summary>
        public void On_Skill0_Clicked()
        {
            string plainText = "";
            NpcAnimInfo info = new NpcAnimInfo()
            { 
                ClientID = DeviceInfo.GUID,
                nextState = NpcAnimState.CastSkill,
                index = 0,
                ui = new WarUIInfo()
                    {
                        ClientID = DeviceInfo.GUID,
                        uniqueId = warCliMgr.clientTeam.activeNpc.UniqueID,
                        camp = WarCamp2Camp.toWarCamp(warCliMgr.clientTeam.activeNpc.Camp),
                    },
                data = "",
            };
            plainText = JSON.Instance.ToJSON(info);
            proxyServer.CastSkill(plainText);
            if (warCliMgr.clientTeam.isAuto)
            {
                warCliMgr.clientTeam.SwitchAutoOrManual(false);
            }
        }

        /// <summary>
        /// 主动技能2
        /// </summary>
        public void On_Skill1_Clicked()
        {
            string plainText = "";
            NpcAnimInfo info = new NpcAnimInfo()
            { 
                ClientID = DeviceInfo.GUID,
                nextState = NpcAnimState.CastSkill,
                index = 1,
                ui = new WarUIInfo()
                    {
                        ClientID = DeviceInfo.GUID,
                        uniqueId = warCliMgr.clientTeam.activeNpc.UniqueID,
                        camp = WarCamp2Camp.toWarCamp(warCliMgr.clientTeam.activeNpc.Camp),
                    },
                data = "",
            };
            plainText = JSON.Instance.ToJSON(info);
            proxyServer.CastSkill(plainText);
            if (warCliMgr.clientTeam.isAuto)
            {
                warCliMgr.clientTeam.SwitchAutoOrManual(false);
            }
        }

        /// <summary>
        /// 主动技能3
        /// </summary>
        public void On_Skill2_Clicked()
        {
            string plainText = "";
            NpcAnimInfo info = new NpcAnimInfo()
            { 
                ClientID = DeviceInfo.GUID,
                nextState = NpcAnimState.CastSkill,
                index = 2,
                ui = new WarUIInfo()
                    {
                        ClientID = DeviceInfo.GUID,
                        uniqueId = warCliMgr.clientTeam.activeNpc.UniqueID,
                        camp = WarCamp2Camp.toWarCamp(warCliMgr.clientTeam.activeNpc.Camp),
                    },
                data = "",
            };
            plainText = JSON.Instance.ToJSON(info);
            proxyServer.CastSkill(plainText);
            if (warCliMgr.clientTeam.isAuto)
            {
                warCliMgr.clientTeam.SwitchAutoOrManual(false);
            }
        }

        public void On_Auto_Clicked()
        {
            bool isAuto = warCliMgr.clientTeam.isAuto;
            warCliMgr.clientTeam.SwitchAutoOrManual(!isAuto);
        }

        public void On_Auto_OK(string msg)
        {
            if(msg == "0")
            {
                warCliMgr.clientTeam.SwitchManualOrAutoSuc(false);
            }
            else if(msg == "1")
            {
                warCliMgr.clientTeam.SwitchManualOrAutoSuc(true);
            }
            spAuto.enabled = warCliMgr.clientTeam.isAuto;
        }

        public void On_SwitchHero_Clicked(int index)
        {
            warCliMgr.clientTeam.SwitchActive(index);
        }

        public void On_SwitchHero_Ok(string msg)
        {
            int id = 0;
            if(int.TryParse(msg, out id))
            {
                ClientNPC npc = warCliMgr.npcMgr.GetNpc(id);
                if(npc != null)
                {
                    warCliMgr.clientTeam.SwitchActiveHeroSuc(npc);
                    warCliMgr.SwitchHero(npc);
                }
                if(onHeroSwitch != null)
                {
                    onHeroSwitch(id);
                }
            }
        }
        #endregion

        #region 血条控制
        public GameObject HealthRoot;
        /// <summary>
        /// 英雄的血条
        /// </summary>
        public GameObject HeroHealthObj;
        /// <summary>
        /// 其它单位的血条
        /// </summary>
        public GameObject UnitHealthObj;
        //TODO 是否要加建筑的血条

        public void CreateUIForNpc(ClientNPC npc)
        {
            if(npc != null)
            {
                GameObject obj = null;
                bool isHero = npc.WhatTypeOf.check(LifeNPCType.Hero);
                if (isHero)
                {
                    obj = NGUITools.AddChild(HealthRoot, HeroHealthObj);
                }
                else
                {
                    obj = NGUITools.AddChild(HealthRoot, UnitHealthObj);
                }
                obj.SetActive(true);
                UIProgressBar health = obj.GetComponent<UIProgressBar>();
                UISprite sp = health.foregroundWidget as UISprite;

                ClientNpcAnimState cna = npc.animState;

                if(cna != null)
                {
                    cna.HealthBar = health;
                    cna.HitNum = GenerateHitNumber;
                }

                if (sp != null)
                {
                    if (isHero)
                    {
                        if (npc.Camp == CAMP.Player)
                        {
                            sp.spriteName = "battle-021";
                        }
                        else if (npc.Camp == CAMP.Enemy)
                        {
                            sp.spriteName = "battle-020";
                        }
                        else
                        {
                            sp.spriteName = "battle-020";
                        }
                    }
                    else
                    {
                        if (npc.Camp == CAMP.Player)
                        {
                            sp.spriteName = "battle-043";
                        }
                        else
                        {
                            sp.spriteName = "battle-044";
                        }
                    }
                }

                if(!isHero)
                {
                    obj.SetActive(false);
                }
            }
        }
        #endregion

        #region 打击字
        /// <summary>
        /// 红色打击字
        /// </summary>
        public UILabel redHit;
        /// <summary>
        /// 绿色打击字
        /// </summary>
        public UILabel greenHit;

        public void GenerateHitNumber(int delta, bool isDamage, bool isCritical, Vector3 pos)
        {
            if(delta <= 0)
            {
                return;
            }
            GameObject obj = null;
            if (isDamage)
            {
                obj = NGUITools.AddChild(mPanel.gameObject, redHit.gameObject);

            }
            else
            {
                obj = NGUITools.AddChild(mPanel.gameObject, greenHit.gameObject);
            }
            obj.transform.position = pos;
            if (isCritical)
            {
                obj.transform.localScale *= 2f;
            }
            UILabel l = obj.GetComponent<UILabel>();
            l.text = delta.ToString();
            obj.SetActive(true);
        }
        #endregion
	}
}