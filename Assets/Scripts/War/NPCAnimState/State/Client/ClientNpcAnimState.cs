
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using fastJSON;
using AW.Data;
using AW.Resources;

namespace AW.War
{
    public class ClientNpcAnimState : MonoBehaviour {

        public int AttackCount = 0;

        #region 管理者
        protected WarClientManager cliMgr;
        #endregion

        #region 组件
        protected Transform cachedTran;
        protected Animator animator;
        protected Transform attackPoint;
        protected ClientNPC cachedNpc;
        public ClientNPC CachedNpc
        {
            set
            {
                cachedNpc = value;
                healthPoint = cachedNpc.data.rtData.totalHp;
                totalPoint = cachedNpc.data.rtData.totalHp;
                isHero = cachedNpc.WhatTypeOf.check(LifeNPCType.Hero);
            }
        }
        protected Renderer[] views;
        protected bool isHero;
        protected CharacterModel modelInfo;
        public Transform getModelPart(ModelDef def)
        {
            return modelInfo.GetModelPart(def);
        }
        #endregion

        #region 状态
        /// <summary>
        /// 动画状态
        /// 服务器npc看不到变化
        /// </summary>
        protected NpcAnimState state = NpcAnimState.Stand;
        public NpcAnimState STATE
        {
            get
            {
                return state;
            }
            set
            { 
                ConsoleEx.DebugLog("ANIM STATE CHANGED FROM:" + state.ToString() + " TO:" + value.ToString(), "red");
                state = value;
            }
        }
        #endregion

        #region 开关和计时器
        protected bool isAnimating = false;
        protected float animationTimer = 0f;
        protected float animationEventTimer = 0f;
        protected bool canTriggerEvent = true;
        protected AnimationMsg curMsg;
        #endregion

        void Awake()
        {
            cachedTran = transform;
            animator = GetComponent<Animator>();
        }

        // Use this for initialization
        public virtual void Start()
        {
//            cachedTran = transform;
//            animator = GetComponent<Animator>();
            attackPoint = transform.FindChild("AttackPoint");
            views = GetComponentsInChildren<Renderer>();
            cliMgr = WarClientManager.Instance;
            modelInfo = new CharacterModel();
            modelInfo.Init(cachedTran);
        }

        // Update is called once per frame
        public virtual void Update()
        {
            if(handler != null)
            {
                handler();
            }
            UpdateUIPosition();
        }

        public void OnNewStateReceived(WarMsgParam param)
        {
            if (animator != null)
            {
                animator.SetBool("isIdle", false);
            }
            switch(param.cmdType)
            {
                case WarMsg_Type.Stand:
                    On_Stand(param);
                    break;
                case WarMsg_Type.Running:
                    On_Run(param);
                    break;
                case WarMsg_Type.Attack:
                    On_Attack(param);
                    break;
                case WarMsg_Type.UseSkill:
                    On_CastSkill(param);
                    break;
                case WarMsg_Type.UseBuff:
                    On_CastBuff(param);
                    break;
                case WarMsg_Type.UseTrigger:
                    On_CastTrigger(param);
                    break;
                case WarMsg_Type.BeKilled:
                    On_Killed(param);
                    break;
                case WarMsg_Type.Respawn:
                    On_Respawn(param);
                    break;
                case WarMsg_Type.Idle:
                    On_Idle(param);
                    break;
                case WarMsg_Type.OnEffect:
                    On_Effect(param);
                    break;
                case WarMsg_Type.OnStatus:
                    OnNpcStatus(param);
                    break;
                case WarMsg_Type.Suffer:
                    On_Suffer(param);
                    break;
                case WarMsg_Type.Destroy:
                    On_Destroy(param);
                    break;
            }
        }

        #region event
        public virtual void OnSwitchRun(bool run)
        {
            animator.SetBool("isRun", run);
        }

        float val = 0f;
        public virtual IEnumerator OnSwitchRun(bool run, bool smooth)
        {
            if (run)
            {
                yield return null;
                val = 1f;
                animator.SetBool("isRun", true);
                OnSwitchAnimatorLayerWeight(1, val);
            }
            else
            {
                val = 1f;
                while(cachedNpc.DisFromNextPos > 0.5f && smooth)
                {
                    yield return new WaitForSeconds(0.01f);
                    val -= 0.2f;
                    if(val >= 0f)
                    {
                        OnSwitchAnimatorLayerWeight(1, val);
                    }
                }
                animator.SetBool("isRun", false);
                OnSwitchAnimatorLayerWeight(1, 0);
            }
        }

        public virtual void On_Stand(WarMsgParam param)
        {
            if(STATE == NpcAnimState.Suffer)
            {
                animator.SetBool("isSuffer", false);
            }
            if(STATE != NpcAnimState.Killed && STATE != NpcAnimState.Stand)
            {
                isAnimating = false;
                STATE = NpcAnimState.Stand;
                SetHandler = StandHandle;
                ResetActiveAnimState();
//                StopCoroutine("OnSwitchRun");
//                StartCoroutine(OnSwitchRun(false, true));
                StopAllCoroutines();
                StartCoroutine(CheckToStand());
            }
        }

        IEnumerator CheckToStand()
        {
            val = 0f;
            while (cachedNpc.DisFromNextPos > 0.1f)
            {
                yield return null;
//                val = cachedNpc.DisFromNextPos;
//                if(val >= 0f && val <= 1f)
//                {
//                    animator.SetLayerWeight(1, val);
//                }
            }
            OnSwitchRun(false);
            OnSwitchAnimatorLayerWeight(1, 0);
        }

        public virtual void On_Run(WarMsgParam param)
        {
            if (STATE != NpcAnimState.Killed)
            {
                STATE = NpcAnimState.Run;
                SetHandler = RunHandle;
                OnSwitchRun(true);
                OnSwitchAnimatorLayerWeight(1, 1);
                StopAllCoroutines();
            }
        }

        public virtual void On_Attack(WarMsgParam param)
        {
            StopAllCoroutines();
            StartCoroutine(CheckForAttack(param));
        }

        IEnumerator CheckForAttack(WarMsgParam param)
        {
            val = 0f;
            if (cachedNpc.movable)
            {
                while (cachedNpc.DisFromNextPos > 0.6f)
                {
                    yield return null;
                    val -= 0.2f;
                    if (val >= 0f && val <= 1f)
                    {
                        animator.SetLayerWeight(1, val);
                    }
                }
            }
            else
            {
                yield return null;
            }

            OnSwitchRun(false);
            OnSwitchAnimatorLayerWeight(1, 0);

            STATE = NpcAnimState.Attack;
            SetHandler = AttackHandle;
            isAnimating = true;
            string m_str = param.param as string;
            NpcAnimInfo info = JSON.Instance.ToObject<NpcAnimInfo>(m_str);
            if(info != null)
            {
                AnimationMsg msg = JSON.Instance.ToObject<AnimationMsg>(info.data);
                if(msg != null)
                {
                    curMsg = msg;
                    int index = msg.index;
                    if(index < AttackCount)
                    {
                        if (!string.IsNullOrEmpty(msg.animationName) && msg.animationName != "[]")
                        {
                            animator.CrossFade(msg.animationName, 0.1f);
                        }
                        animationTimer = msg.animationTimer - 0.02f;
                        animationEventTimer = msg.animationEventTimer;
                        canTriggerEvent = true;
                        if(index == 0)
                        {
                            CreateEffect(NpcAnimEffect.Attack_1_Start);
                        }
                        else if(index == 1)
                        {
                            CreateEffect(NpcAnimEffect.Attack_2_Start);
                        }
                        else if(index  == 2)
                        {
                            CreateEffect(NpcAnimEffect.Attack_3_Start);
                        }
                    }
                }
            }
        }

        public virtual void On_CastSkill(WarMsgParam param)
        {
            Debug.Log(JSON.Instance.ToJSON(param));
            StopAllCoroutines();
            StartCoroutine(CheckToSkill(param));
        }

        IEnumerator CheckToSkill(WarMsgParam param)
        {
            val = 0f;
            while (cachedNpc.DisFromNextPos > 0.6f)
            {
                yield return null;
                val -= 0.2f;
                if(val >= 0f && val <= 1f)
                {
                    animator.SetLayerWeight(1, val);
                }
            }
            OnSwitchRun(false);
            OnSwitchAnimatorLayerWeight(1, 0);

            STATE = NpcAnimState.CastSkill;
            SetHandler = SkillHandle;
            string m_str = param.param as string;
            NpcAnimInfo info = JSON.Instance.ToObject<NpcAnimInfo>(m_str);
            if(info != null)
            {
                AnimationMsg msg = JSON.Instance.ToObject<AnimationMsg>(info.data);
                curMsg = msg;
                int index = msg.index;
                if (!string.IsNullOrEmpty(msg.animationName) && msg.animationName != "[]")
                {
                    animator.CrossFade(msg.animationName, 0.1f);
                }
                animationTimer = msg.animationTimer - 0.02f;
                animationEventTimer = msg.animationEventTimer;
                canTriggerEvent = true;
                if(index == 0)
                {
                    animator.SetBool("isSkill_1", true);
                    CreateEffect(NpcAnimEffect.Skill_1_Start);
                }
                else if(index == 1)
                {
                    animator.SetBool("isSkill_2", true);
                    CreateEffect(NpcAnimEffect.Skill_2_Start);
                }
                else if(index  == 2)
                {
                    animator.SetBool("isSkill_3", true);
                    CreateEffect(NpcAnimEffect.Skill_3_Start);
                }
            }
        }

        public virtual void On_CastBuff(WarMsgParam param)
        {
            StopAllCoroutines();
            StartCoroutine(CheckToBuff(param));
        }

        IEnumerator CheckToBuff(WarMsgParam param)
        {
            val = 0f;
            while (cachedNpc.DisFromNextPos > 0.6f)
            {
                yield return null;
                val -= 0.2f;
                if(val >= 0f && val <= 1f)
                {
                    animator.SetLayerWeight(1, val);
                }
            }
            OnSwitchRun(false);
            OnSwitchAnimatorLayerWeight(1, 0);

            STATE = NpcAnimState.CastBuff;
            SetHandler = BuffHandle;
            isAnimating = true;
            string m_str = param.param as string;
            NpcAnimInfo info = JSON.Instance.ToObject<NpcAnimInfo>(m_str);
            if(info != null)
            {
                AnimationMsg msg = JSON.Instance.ToObject<AnimationMsg>(info.data);
                curMsg = msg;
                if (!string.IsNullOrEmpty(msg.animationName) && msg.animationName != "[]")
                {
                    animator.CrossFade(msg.animationName, 0.1f);
                }
                animationTimer = msg.animationTimer - 0.02f;
                animationEventTimer = msg.animationEventTimer;
                canTriggerEvent = true;
                CreateEffect(NpcAnimEffect.Buff);
            }
        }

        public virtual void On_CastTrigger(WarMsgParam param)
        {
            StopAllCoroutines();
            StartCoroutine(CheckToTrigger(param));
        }

        IEnumerator CheckToTrigger(WarMsgParam param)
        {
            val = 0f;
            while (cachedNpc.DisFromNextPos > 0.6f)
            {
                yield return null;
                val -= 0.2f;
                if(val >= 0f && val <= 1f)
                {
                    animator.SetLayerWeight(1, val);
                }
            }
            OnSwitchRun(false);
            OnSwitchAnimatorLayerWeight(1, 0);

            STATE = NpcAnimState.CastTrigger;
            OnSwitchRun(false);
            OnSwitchAnimatorLayerWeight(1, 0);
            SetHandler = TriggerHandle;
            isAnimating = true;
            string m_str = param.param as string;
            NpcAnimInfo info = JSON.Instance.ToObject<NpcAnimInfo>(m_str);
            if(info != null)
            {
                AnimationMsg msg = JSON.Instance.ToObject<AnimationMsg>(info.data);
                curMsg = msg;
                if (!string.IsNullOrEmpty(msg.animationName) && msg.animationName != "[]")
                {
                    animator.CrossFade(msg.animationName, 0.1f);
                }
                animationTimer = msg.animationTimer - 0.02f;
                animationEventTimer = msg.animationEventTimer;
                canTriggerEvent = true;
                CreateEffect(NpcAnimEffect.Trigger);
            }
        }

        public virtual void On_Killed(WarMsgParam param)
        {
            ClearUIObj();
            STATE = NpcAnimState.Killed;
            if (health != null)
            {
                health.value = 0f;
                health.gameObject.SetActive(false);
            }
            if(heroHealth != null)
            {
                heroHealth.value = 0f;
            }
            OnSwitchRun(false);
            OnSwitchAnimatorLayerWeight(1, 0);
            SetHandler = KilledHandle;
            animator.CrossFade("Die", 0f);
            animator.SetBool("isSuffer", false);
            animator.SetBool("isDead", true);
            animator.SetBool("isIdle", false);
            isAnimating = false;
        }

        public virtual void On_Respawn(WarMsgParam param)
        {
            STATE = NpcAnimState.Respawn;
            SetHandler = RespawnHandle;
            if (health != null)
            {
                health.value = 1f;
                if (isHero)
                {
                    health.gameObject.SetActive(true);
                }
            }
            if(heroHealth != null)
            {
                heroHealth.value = 1f;
            }
            healthPoint = totalPoint;
            animator.SetBool("isDead", false);
        }

        public virtual void On_Idle(WarMsgParam param)
        {
            STATE = NpcAnimState.Idle;
            animator.SetBool("isIdle", true);
            animator.CrossFade("Idle", 0f);
        }

        public virtual void On_Destroy(WarMsgParam param)
        {
            Destroy(gameObject);
        }

        public virtual void On_Suffer(WarMsgParam param)
        {
            STATE = NpcAnimState.Suffer;
            ResetActiveAnimState();
            OnSwitchRun(false);
            OnSwitchAnimatorLayerWeight(1, 0);
            animator.SetBool("isSuffer", true);
            animator.CrossFade("Suffer", 0f);
        }

        public virtual void OnSwitchAnimatorLayerWeight(int layer, float weight)
        {
            if(animator.layerCount > layer)
            {
                animator.SetLayerWeight(layer, weight);
            }
        }

        public virtual void ClearUIObj()
        {
            foreach(KeyValuePair<string, GameObject> kv in uiEffectCache)
            {
                {
                    Destroy(kv.Value);
                }
            }
            uiEffectCache.Clear();
            foreach(KeyValuePair<NpcStatus, GameObject> kv in statusCache)
            {
                kv.Value.SetActive(false);
            }
        }

        private Dictionary<string, GameObject> uiEffectCache = new Dictionary<string, GameObject>();
        public virtual void On_Effect(WarMsgParam param)
        {
            if(param != null)
            {
                string m_str = param.param as string;
                NpcEffectInfo info = JSON.Instance.ToObject<NpcEffectInfo>(m_str);
                if(info != null && info.to == cachedNpc.UniqueID)
                {
                    Effect3DModelConfigData ecd = Core.Data.getIModelConfig<Effect3DModel>().get(info.hitAction);
                    if (ecd != null)
                    {
                        GameObject obj = null;
                        string key = info.from + "_" + ecd.Start;
                        Debug.Log(key, gameObject);
                        if (info.Op == 0)
                        {
                            obj = WarEffectLoader.Load(ecd.Start);
                            if (obj != null)
                            {
                                obj = Instantiate(obj, cachedTran.position, cachedTran.rotation) as GameObject;
                                obj.transform.parent = cachedTran;
                                SkillEffectBase seb = obj.GetComponent<SkillEffectBase>();
                                ClientNPC srcNpc = cliMgr.npcMgr.GetNpc(info.from);
                                if (seb != null)
                                {
                                    seb.EmitEffect(srcNpc, cachedNpc, false);
                                }
                                if (!uiEffectCache.ContainsKey(key))
                                {
                                    uiEffectCache.Add(key, obj);
                                }
                            }
                            else
                            {
                                Debug.Log(string.Format("Fail to load effect obj with name:{0} from:{1} shootAction:{2}", ecd.Start, info.from));
                            }
                        }
                        else
                        {
                            if(uiEffectCache.TryGetValue(key, out obj))
                            {
                                uiEffectCache.Remove(key);
                                Destroy(obj);
                                obj = null;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 通用的一些特效
        /// </summary>
        public static GameObject Effect_ChaoFeng;
        public static GameObject Effect_Unconscious;
        public static GameObject Effect_Silence;
        private Dictionary<NpcStatus, GameObject> statusCache = new Dictionary<NpcStatus, GameObject>();
        public virtual void OnNpcStatus(WarMsgParam param)
        {
            if(param != null)
            {
                string m_str = param.param as string;
                NpcStatusInfo info = JSON.Instance.ToObject<NpcStatusInfo>(m_str);
                if(info != null)
                {
                    List<StatusInfoItem> list = info.items;
                    if(list != null)
                    {
                        GameObject obj = null;
                        foreach(StatusInfoItem si in list)
                        {
                            if(!statusCache.TryGetValue(si.status, out obj))
                            {
                                obj = ClientNpcAnimState.getStatusObj(si.status);
                                if(obj != null)
                                {
                                    obj = Instantiate(obj) as GameObject;
                                    if (obj != null)
                                    {
                                        SkillEffectBase seb = obj.GetComponent<SkillEffectBase>();
                                        if (seb != null)
                                        {
                                            seb.EmitEffect(null, cachedNpc, true);
                                        }
                                    }
                                    statusCache.Add(si.status, obj);
                                }
                            }
                            obj.SetActive(si.isAdd);
                        }
                    }
                }
            }
        }
        #endregion

        #region StateHandle
        protected delegate void StateHandle();
        protected StateHandle handler;
        protected StateHandle SetHandler
        {
            set
            { 
                handler = value;
            }
        }

        public virtual void StandHandle()
        {

        }

        public virtual void RunHandle()
        {
            if(STATE == NpcAnimState.Run)
            {

            }
        }

        public virtual void AttackHandle()
        {
            if(STATE == NpcAnimState.Attack)
            {
                animationTimer -= Time.deltaTime;
                animationEventTimer -= Time.deltaTime;
                if(animationEventTimer < 0 && canTriggerEvent)
                {
                    canTriggerEvent = false;
                    int index = curMsg.index;
                    if(index == 0)
                    {
                        CreateEffect(NpcAnimEffect.Attack_1);
                    }
                    else if(index == 1)
                    {
                        CreateEffect(NpcAnimEffect.Attack_2);
                    }
                    else if(index == 2)
                    {
                        if(cliMgr != null && cliMgr.clientTeam.activeNpc.UniqueID == cachedNpc.UniqueID && curMsg.targetId != 0)
                        {
                            cliMgr.ShakeMainCamera();
                        }
                        CreateEffect(NpcAnimEffect.Attack_3);
                    }
                }
                if(animationTimer < 0)
                {
                    isAnimating = false;
                    On_Stand(null);
                }
            }
        }

        public virtual void SkillHandle()
        {
            if(STATE == NpcAnimState.CastSkill)
            {
                animationTimer -= Time.deltaTime;
                animationEventTimer -= Time.deltaTime;
                if(animationEventTimer < 0 && canTriggerEvent)
                {
                    canTriggerEvent = false;
                    int index = curMsg.index;
                    if(index == 0)
                    {
                        CreateEffect(NpcAnimEffect.Skill_1);
                    }
                    else if(index == 1)
                    {
                        CreateEffect(NpcAnimEffect.Skill_2);
                    }
                    else if(index == 2)
                    {
                        CreateEffect(NpcAnimEffect.Skill_3);
                    }
                }
                if(animationTimer < 0)
                {
                    isAnimating = false;
                }
            }
        }

        public virtual void BuffHandle()
        {
            if(STATE == NpcAnimState.CastBuff)
            {
                animationTimer -= Time.deltaTime;
                animationEventTimer -= Time.deltaTime;
                if(animationEventTimer < 0 && canTriggerEvent)
                {
                    canTriggerEvent = false;
                }
                if(animationTimer < 0)
                {
                    isAnimating = false;
                }
            }
        }

        public virtual void TriggerHandle()
        {
            if(STATE == NpcAnimState.CastTrigger)
            {
                animationTimer -= Time.deltaTime;
                animationEventTimer -= Time.deltaTime;
                if(animationEventTimer < 0 && canTriggerEvent)
                {
                    canTriggerEvent = false;
                }
                if(animationTimer < 0)
                {
                    isAnimating = false;
                }
            }
        }

        public virtual void KilledHandle()
        {
            
        }

        public virtual void RespawnHandle()
        {

        }

        #endregion

        #region 清理状态
        public virtual void ResetActiveAnimState()
        {
//            Debug.Log("\\******((((((- . -))))))******/");
            animator.SetBool("isAttack_1", false);
            animator.SetBool("isAttack_2", false);
            animator.SetBool("isAttack_3", false);
            animator.SetBool("isSkill_1", false);
            animator.SetBool("isSkill_2", false);
            animator.SetBool("isSkill_3", false);
        }
        #endregion

        #region Effect
        public virtual void CreateEffect(NpcAnimEffect effect)
        {
            GameObject obj = null;
            string src = "";
            switch(effect)
            {
                case NpcAnimEffect.Attack_1_Start:
                case NpcAnimEffect.Attack_2_Start:
                case NpcAnimEffect.Attack_3_Start:
                    src = curMsg.ecd.Start;
                    if (!string.IsNullOrEmpty(src) && !(src == "[]"))
                    {
                        obj = WarEffectLoader.Load(src);
                        obj = Instantiate(obj, cachedTran.position, cachedTran.rotation) as GameObject;
                        Destroy(obj, 2f);
                    }
                break;
                case NpcAnimEffect.Attack_1:
                case NpcAnimEffect.Attack_2:
                case NpcAnimEffect.Attack_3:
                    src = curMsg.ecd.Middle;
                    if (!string.IsNullOrEmpty(src) && !(src == "[]"))
                    {
                        obj = WarEffectLoader.Load(src);
                        obj = Instantiate(obj, attackPoint.position, cachedTran.rotation) as GameObject;
                        ClientFB fb = obj.GetComponent<ClientFB>();
                        if (fb != null)
                        {
                            fb.Init(curMsg);
                        }
                        else
                        {
                            Destroy(obj, 2f);
                        }
                    }
                    break;
                case NpcAnimEffect.Skill_1:
                    break;
                case NpcAnimEffect.Skill_1_Start:
                    src = curMsg.ecd.Start;
                    if(!string.IsNullOrEmpty(src) && src != "[]")
                    {
                        obj = WarEffectLoader.Load(src);
                        obj = Instantiate(obj, cachedTran.position, cachedTran.rotation) as GameObject;
                        Destroy(obj, 5f);
                    }
                    break;
                case NpcAnimEffect.Skill_2:
                    break;
                case NpcAnimEffect.Skill_2_Start:
                    src = curMsg.ecd.Start;
                    if(!string.IsNullOrEmpty(src) && src != "[]")
                    {
                        obj = WarEffectLoader.Load(src);
                        obj = Instantiate(obj, cachedTran.position, cachedTran.rotation) as GameObject;
                        Destroy(obj, 5f);
                    }
                    break;
                case NpcAnimEffect.Skill_3:
                    break;
                case NpcAnimEffect.Skill_3_Start:
                    src = curMsg.ecd.Start;
                    if(!string.IsNullOrEmpty(src) && src != "[]")
                    {
                        obj = WarEffectLoader.Load(src);
                        obj = Instantiate(obj, cachedTran.position, cachedTran.rotation) as GameObject;
                        Destroy(obj, 5f);
                    }
                    break;
                case NpcAnimEffect.Skill_4:
                    break;
                case NpcAnimEffect.Skill_4_Start:
                    break;
            }
        }
        #endregion

        #region 3D元素
        protected virtual bool isVisible()
        {
            if(views.Length > 0)
            {
                for(int i = 0; i < views.Length; i++)
                {
                    if(views[i].isVisible)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region 2D元素
        /// <summary>
        /// 跟随角色的血条
        /// </summary>
        protected UIProgressBar health;
        public UIProgressBar HealthBar
        {
            get{ return health;}
            set
            {
                health = value;
            }
        }

        /// <summary>
        /// 己方英雄在右侧的血条
        /// </summary>
        protected UIProgressBar heroHealth;
        public UIProgressBar HeroHealthBar
        {
            get{ return heroHealth;}
            set
            {
                heroHealth = value;
            }
        }

        int healthPoint;
        int totalPoint;
        public System.Action<int, bool, bool, Vector3> HitNum = null;
        public virtual void UpdateHealthValue(IpcNpcHpMsg msg)
        {
            if(health != null && msg != null && cliMgr != null)
            {
                bool isDamage = msg.isDamage;
                int val = msg.deltaHp;
                healthPoint = msg.curHp;
                if (msg.srcID == cliMgr.clientTeam.activeNpc.UniqueID || cliMgr.clientTeam.activeNpc.UniqueID == cachedNpc.UniqueID)
                {
                    if (HitNum != null)
                    {
                        Vector3 pos = cachedTran.position;
                        pos.y += 4f;
                        pos = cliMgr.GetUIPosRef3DPos(pos);
                        HitNum(val, isDamage, false, pos);
                    }
                }
                if(!isHero)
                {
                    if(!health.gameObject.activeInHierarchy && healthPoint < totalPoint && healthPoint > 0)
                    {
                        health.gameObject.SetActive(true);
                    }
                    if(health.gameObject.activeInHierarchy && healthPoint >= totalPoint)
                    {
                        health.gameObject.SetActive(false);
                    }
                }
                if(healthPoint > 0)
                {
                    float healthVal = healthPoint / (totalPoint * 1.0f);
                    health.value = healthVal;
                    if(heroHealth != null)
                    {
                        heroHealth.value = healthVal;
                    }
                }
            }
        }

        private Vector3 pos;
        private bool lastInVisible = true;
        public virtual void UpdateUIPosition()
        {
            if (health != null && cliMgr != null)
            {
                if (isVisible())
                {
                    pos = cachedTran.position;
                    pos.y += 5.5f;
                    pos = cliMgr.GetUIPosRef3DPos(pos);
                    if (lastInVisible || cliMgr.IsSwitch)
                    {
                        health.transform.position = pos;
                        lastInVisible = false;
                        return;
                    }
                    else
                    {
                        health.transform.position = Vector3.Lerp(health.transform.position, pos, Time.deltaTime * 10f);
                    }
                }
                else
                {
                    health.transform.position = Vector3.up * 1000;
                    lastInVisible = true;
                }
            }
            else
            {
                health.gameObject.SetActive(false);
                lastInVisible = true;
            }
        }
        #endregion

        public static GameObject getStatusObj(NpcStatus status)
        {
            switch(status)
            {
                case NpcStatus.Taunt:
                    if (Effect_ChaoFeng == null)
                    {
                        Effect_ChaoFeng = WarEffectLoader.Load("gongyong/ChaoFeng");
                    }
                    return Effect_ChaoFeng;
                case NpcStatus.Slient:
                    if (Effect_Silence == null)
                    {
                        Effect_Silence = WarEffectLoader.Load("gongyong/Silence");
                    }
                    return Effect_Silence;
                case NpcStatus.Unconscious:
                    if (Effect_Unconscious == null)
                    {
                        Effect_Unconscious = WarEffectLoader.Load("gongyong/Vertigo");
                    }
                    return Effect_Unconscious;
            }
            return null;
        }

        void OnDrawGizmos()
        {
            #if DEBUG

            #endif
        }
    }
}
