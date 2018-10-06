using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using fastJSON;
using AW.Data;
using AW.Resources;
using System.Linq;
using AW.Framework;

namespace AW.War
{
    public class ServerNpcAnimState : MonoBehaviour
    {
        #region 管理者
        private WarServerManager serMgr;
        #endregion

        #region 组件
        protected Transform cachedTran;
        public ServerLifeNpc cachedNpc;
        public CharacterController cachedCC;
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
                state = value;
            }
        }

        protected bool ableToMove = true;
        public bool canMove
        {
            get
            { 
                ableToMove = STATE != NpcAnimState.Killed && STATE != NpcAnimState.Destroying && !isAttack && !isCastSkill && isConscious && canNextAnim;
                return ableToMove;
            }
        }
        #endregion

        #region Message
        protected IpcNpcAnimMsg animMsg;
        #endregion

        #region 技能
        protected ISkImp[] sk_Imp;
        protected Dictionary<int, List<ISkImp>> sk_Imps = new Dictionary<int, List<ISkImp>>();
        #endregion

        #region 开关和计时器
        protected bool isConscious = true;
        protected bool isAttack;
        protected bool isCastSkill;
        protected bool canNextAnim = true;

        protected bool canTriggerEvent = true;
        protected float animationTimer = 0f;
        protected float animationEventTimer = 0f;
        protected float attackWaitForNext = 0f;
        protected int attackIndex = 0;
        #endregion

        #region 要加载的一些东西
        /// <summary>
        /// 远程法球
        /// </summary>
        protected GameObject fireBall;
        public int curHp;
        public int totHp;
        #endregion

        void Awake()
        {
            animMsg = new IpcNpcAnimMsg();
        }

        // Use this for initialization
        void Start()
        {
            cachedTran = transform;
            cachedCC = GetComponent<CharacterController>();

            fireBall = WarEffectLoader.Load("ServerEffect/ServerFB");
            serMgr = WarServerManager.Instance;
            totHp = cachedNpc.data.rtData.totalHp;
            curHp = cachedNpc.data.rtData.curHp;
        }
	
        // Update is called once per frame
        public virtual void Update()
        {
            if(handler != null)
            {
                handler();
            }
            OnNpcStatus();
            totHp = cachedNpc.data.rtData.totalHp;
            curHp = cachedNpc.data.rtData.curHp;
        }

        public virtual void OnNewStateReceived(WarMsgParam param)
        {
            if (param is WarSrcAnimParam)
            {
                switch (param.cmdType)
                {
                    case WarMsg_Type.ManualInput:
                        On_ManualInput(param);
                        break;
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
                    case WarMsg_Type.Destroy:
                        On_Destroy(param);
                        break;
                }
            }
            else if(param is WarTarAnimParam)
            {
                WarTarAnimParam tarParam = param as WarTarAnimParam;
                OnNewUIStateReceived(tarParam);
            }
        }

        #region ui event
        public virtual void OnNewUIStateReceived(WarTarAnimParam param)
        {
            if(param != null)
            {
                if(param.OP == EffectOp.ExchangeNpcAttri || param.OP == EffectOp.Injury)
                {
                    return;
                }

                SelfDescribed sd = param.described;
                if(sd != null && sd.target == cachedNpc.UniqueID && sd.srcEnd != null)
                {
                    EndResult src = sd.srcEnd;
                    int srcId = sd.src;
                    int target = sd.target;
                    int action = src.param2;
                    int op = src.param3;
                    NpcEffectInfo info = new NpcEffectInfo()
                    {
                        ClientID = DeviceInfo.GUID,
                        from = srcId,
                        to = target,
                        hitAction = action,
                        Op = op,
                    };
                    animMsg.nextAnim = WarMsg_Type.OnEffect.ToString();
                    animMsg.uniqueId = cachedNpc.UniqueID;
                    animMsg.data = JSON.Instance.ToJSON(info);

                    if(serMgr != null)
                    {
                        serMgr.realServer.proxyCli.NPCAnim(animMsg);
                    }
                }
            }
        }

        private NpcStatus statusInLastFrame;
        public virtual void OnNpcStatus()
        {
            if(cachedNpc != null)
            {
                NpcStatus status = cachedNpc.curStatus;
                if(status != statusInLastFrame)
                {
                    NpcStatusInfo info = new NpcStatusInfo();

                    if(status.AnySame(NpcStatus.Slient) && !statusInLastFrame.AnySame(NpcStatus.Slient))
                    {
                        info.addItem(NpcStatus.Slient, true);
                    }
                    else if(!status.AnySame(NpcStatus.Slient) && statusInLastFrame.AnySame(NpcStatus.Slient))
                    {
                        info.addItem(NpcStatus.Slient, false);
                    }

                    if(status.AnySame(NpcStatus.Unconscious) && !statusInLastFrame.AnySame(NpcStatus.Unconscious))
                    {
                        info.addItem(NpcStatus.Unconscious, true);
                        On_Suffer(true);
                    }
                    else if(!status.AnySame(NpcStatus.Unconscious) && statusInLastFrame.AnySame(NpcStatus.Unconscious))
                    {
                        info.addItem(NpcStatus.Unconscious, false);
                        On_Suffer(false);
                    }

                    if(status.AnySame(NpcStatus.Taunt) && !statusInLastFrame.AnySame(NpcStatus.Taunt))
                    {
                        info.addItem(NpcStatus.Taunt, true);
                    }
                    else if(!status.AnySame(NpcStatus.Taunt) && statusInLastFrame.AnySame(NpcStatus.Taunt))
                    {
                        info.addItem(NpcStatus.Taunt, false);
                    }

                    animMsg.nextAnim = WarMsg_Type.OnStatus.ToString();
                    animMsg.uniqueId = cachedNpc.UniqueID;
                    animMsg.data = JSON.Instance.ToJSON(info);

                    if(serMgr != null)
                    {
                        serMgr.realServer.proxyCli.NPCAnim(animMsg);
                    }
                }
                statusInLastFrame = status;
            }
        }
        #endregion

        #region event
        /// <summary>
        /// 攻击的消息存储
        /// </summary>
        protected WarSrcAnimParam atkParam;
        /// <summary>
        /// 技能的消息存储
        /// </summary>
        protected WarSrcAnimParam skParam;

        public virtual void On_ManualInput(WarMsgParam param)
        {
            if (canMove)
            {
                animMsg.nextAnim = WarMsg_Type.Running.ToString();
                animMsg.uniqueId = cachedNpc.UniqueID;
                if(serMgr != null)
                {
                    serMgr.realServer.proxyCli.NPCAnim(animMsg);
                }

                STATE = NpcAnimState.Run;
                SetHandler = RunHandle;

                string msg = param.param as string;
                if (msg != null)
                {
                    NpcAnimInfo info = JSON.Instance.ToObject<NpcAnimInfo>(msg);
                    QuaternionWrap wrap = JSON.Instance.ToObject<QuaternionWrap>(info.data);
                    if (wrap != null)
                    {
                        Quaternion qua = QuaternionWrap.WrapToQuaternion(wrap);
                        cachedTran.rotation = qua;
                    }
                }
            }
        }

        public virtual void On_Stand(WarMsgParam param)
        {
            if(STATE != NpcAnimState.Stand && canMove)
            { 
                STATE = NpcAnimState.Stand;
                SetHandler = StandHandle;
                animMsg.nextAnim = WarMsg_Type.Stand.ToString();
                animMsg.uniqueId = cachedNpc.UniqueID;
                if(serMgr != null)
                {
                    serMgr.realServer.proxyCli.NPCAnim(animMsg);
                }
            }
        }

        public virtual void On_Run(WarMsgParam param)
        {
            if(STATE != NpcAnimState.Run && canMove)
            {
                STATE = NpcAnimState.Run;
                SetHandler = RunHandle;
                animMsg.nextAnim = WarMsg_Type.Running.ToString();
                animMsg.uniqueId = cachedNpc.UniqueID;
                if(serMgr != null)
                {
                    serMgr.realServer.proxyCli.NPCAnim(animMsg);
                }
            }
        }

        public virtual void On_Attack(WarMsgParam param)
        {
            if(STATE != NpcAnimState.Killed && !isAttack && !isCastSkill)
            {
                WarSrcAnimParam srcParam = param as WarSrcAnimParam;
                atkParam = srcParam;
//                Debug.Log(JSON.Instance.ToJSON(atkParam), gameObject);
                if(srcParam != null)
                {
                    int index = cachedNpc.AttIndex;
                    STATE = NpcAnimState.Attack;
                    Effect3DModelConfigData ecd = Core.Data.getIModelConfig<Effect3DModel>().get(srcParam.ShootAction);
                    if (ecd != null)
                    {
                        isAttack = true;
                        canNextAnim = false;
                        canTriggerEvent = true;
                        animationTimer = srcParam.ShootTime;
                        animationEventTimer = srcParam.ShootEventTime;
                        attackWaitForNext = 0.3f;
                        attackIndex = index;
                        SetHandler = AttackHandle;

                        WarTarAnimParam[] targets = atkParam.InjureTar;
                        BNPC npc = null;
                        if (targets != null && targets.Length > 0)
                        {
                            npc = serMgr.npcMgr.GetNPCByUniqueID(targets[0].described.target);
                            if (npc != null)
                            {
                                lastTarget = "Target:" + targets[0].described.target;
                                Vector3 pos = npc.transform.position;
                                pos.y = cachedTran.position.y;
                                cachedTran.LookAt(pos);
                            }
                        }

                        AnimationMsg msg = new AnimationMsg();
                        msg.animationTimer = animationTimer;
                        msg.animationEventTimer = animationEventTimer;
                        msg.index = index;
                        msg.ecd = ecd;
                        msg.animationName = ecd.Anim;
                        msg.targetId = (npc == null) ? 0 : npc.UniqueID;
                        msg.arg1 = srcParam.described.targetEnd.param8;
                        msg.arg2 = srcParam.described.targetEnd.param9;

                        string a_data = JSON.Instance.ToJSON(msg);

                        NpcAnimInfo info = new NpcAnimInfo()
                        {
                            ClientID = DeviceInfo.GUID,
                            nextState = NpcAnimState.Attack,
                            data = a_data,
                        };

                        animMsg.nextAnim = WarMsg_Type.Attack.ToString();
                        animMsg.uniqueId = cachedNpc.UniqueID;
                        animMsg.data = JSON.Instance.ToJSON(info);

                        if(serMgr != null)
                        {
                            serMgr.realServer.proxyCli.NPCAnim(animMsg);
                        }
                    }
                    else
                    {
                        On_Stand(null);
                    }
                }
            }
        }



        public virtual void On_CastSkill(WarMsgParam param)
        {
            if(STATE != NpcAnimState.Killed && !isCastSkill)
            {
//                WarSrcAnimParam srcParam = param as WarSrcAnimParam;
//                int index = param.arg1;
//                List<ISkImp> ops = null;
//                ISkImp sk = null;
//                if (!sk_Imps.TryGetValue(index, out ops))
//                {
//                    ops = new List<ISkImp>();
//                    sk_Imps.Add(index, ops);
//                }
//                sk = ops.Find(imp => imp.SkOp() == srcParam.OP);
//                if (sk == null)
//                {
//                    sk = SkImpFactory.getSkImp(srcParam.OP);
//                    ops.Add(sk);
//                }
//                sk.Reset();
//                sk.InitSk(cachedNpc, param);
//                sk.CastSk();
            }
            if(STATE != NpcAnimState.Killed && !isAttack && !isCastSkill)
            {
                WarSrcAnimParam srcParam = param as WarSrcAnimParam;
                Debug.Log(JSON.Instance.ToJSON(param));
                skParam = srcParam;
                if(srcParam != null)
                {
                    STATE = NpcAnimState.CastSkill;
                    Effect3DModelConfigData ecd = Core.Data.getIModelConfig<Effect3DModel>().get(srcParam.ShootAction);
                    if (ecd != null)
                    {
                        isCastSkill = true;
                        canNextAnim = false;
                        canTriggerEvent = true;
                        animationTimer = srcParam.ShootTime;
                        animationEventTimer = srcParam.ShootEventTime;
                        SetHandler = Skill_Handle;

                        AnimationMsg msg = new AnimationMsg();
                        msg.animationTimer = animationTimer;
                        msg.animationEventTimer = animationEventTimer;
                        msg.index = param.arg1;
                        msg.ecd = ecd;
                        msg.animationName = ecd.Anim;
                        msg.targetId = 0;

                        if(srcParam.described != null && srcParam.OP == EffectOp.CtorNPC)
                        {
                            EndResult result = srcParam.described.srcEnd;
                            CreatNpcDepandency cnd = (CreatNpcDepandency)result.obj;
                            msg.objCrtV = cnd.TargetVector3;
                        }

                        WarTarAnimParam[] targets = skParam.InjureTar;
                        BNPC npc = null;
                        if (targets != null && targets.Length > 0)
                        {
                            msg.targetId = targets[0].described.target;
                            npc = serMgr.npcMgr.GetNPCByUniqueID(targets[0].described.target);
                            if (npc != null)
                            {
                                lastTarget = "Target:" + targets[0].described.target;
                                Vector3 pos = npc.transform.position;
                                pos.y = cachedTran.position.y;
                                cachedTran.LookAt(pos);
                            }
                        }

                        string a_data = JSON.Instance.ToJSON(msg);

                        NpcAnimInfo info = new NpcAnimInfo()
                        {
                            ClientID = DeviceInfo.GUID,
                            nextState = NpcAnimState.CastSkill,
                            data = a_data,
                        };

                        animMsg.nextAnim = WarMsg_Type.UseSkill.ToString();
                        animMsg.uniqueId = cachedNpc.UniqueID;
                        animMsg.data = JSON.Instance.ToJSON(info);

                        if(serMgr != null)
                        {
                            serMgr.realServer.proxyCli.NPCAnim(animMsg);
                        }
                    }
                    else
                    {
                        On_Stand(null);
                    }
                }
            }
        }

        public virtual void On_CastBuff(WarMsgParam param)
        {
            if(STATE != NpcAnimState.Killed)
            {
                WarSrcAnimParam srcParam = param as WarSrcAnimParam;
                int index = param.arg1;
                List<ISkImp> ops = null;
                ISkImp sk = null;
                if (!sk_Imps.TryGetValue(index, out ops))
                {
                    ops = new List<ISkImp>();
                    sk_Imps.Add(index, ops);
                }
                sk = ops.Find(imp => imp.SkOp() == srcParam.OP);
                if (sk == null)
                {
                    sk = SkImpFactory.getSkImp(srcParam.OP);
                    ops.Add(sk);
                }
                sk.Reset();
                sk.InitSk(cachedNpc, param);
                sk.CastSk();
            }
            if(STATE != NpcAnimState.Killed && !isAttack && !isCastSkill)
            {
                WarSrcAnimParam srcParam = param as WarSrcAnimParam;
                skParam = srcParam;
                if(srcParam != null)
                {
                    STATE = NpcAnimState.CastBuff;
                    Effect3DModelConfigData ecd = Core.Data.getIModelConfig<Effect3DModel>().get(srcParam.ShootAction);
                    if (ecd != null)
                    {
                        isCastSkill = true;
                        canNextAnim = false;
                        canTriggerEvent = true;
                        animationTimer = srcParam.ShootTime;
                        animationEventTimer = srcParam.ShootEventTime;
                        SetHandler = Buff_Handle;

                        AnimationMsg msg = new AnimationMsg();
                        msg.animationTimer = animationTimer;
                        msg.animationEventTimer = animationEventTimer;
                        msg.ecd = ecd;
                        msg.animationName = ecd.Anim;
                        msg.targetId = 0;

                        string a_data = JSON.Instance.ToJSON(msg);

                        NpcAnimInfo info = new NpcAnimInfo()
                        {
                            ClientID = DeviceInfo.GUID,
                            nextState = NpcAnimState.CastBuff,
                            data = a_data,
                        };

                        animMsg.nextAnim = WarMsg_Type.UseBuff.ToString();
                        animMsg.uniqueId = cachedNpc.UniqueID;
                        animMsg.data = JSON.Instance.ToJSON(info);

                        if(serMgr != null)
                        {
                            serMgr.realServer.proxyCli.NPCAnim(animMsg);
                        }
                    }
                    else
                    {
                        On_Stand(null);
                    }
                }
            }
        }

        public virtual void On_CastTrigger(WarMsgParam param)
        {
            if(STATE != NpcAnimState.Killed)
            {
                WarSrcAnimParam srcParam = param as WarSrcAnimParam;
                int index = param.arg1;
                List<ISkImp> ops = null;
                ISkImp sk = null;
                if (!sk_Imps.TryGetValue(index, out ops))
                {
                    ops = new List<ISkImp>();
                    sk_Imps.Add(index, ops);
                }
                sk = ops.Find(imp => imp.SkOp() == srcParam.OP);
                if (sk == null)
                {
                    sk = SkImpFactory.getSkImp(srcParam.OP);
                    ops.Add(sk);
                }
                sk.Reset();
                sk.InitSk(cachedNpc, param);
                sk.CastSk();
            }
            if(STATE != NpcAnimState.Killed && !isAttack && !isCastSkill)
            {
                WarSrcAnimParam srcParam = param as WarSrcAnimParam;
                skParam = srcParam;
                if(srcParam != null)
                {
                    STATE = NpcAnimState.CastTrigger;
                    Effect3DModelConfigData ecd = Core.Data.getIModelConfig<Effect3DModel>().get(srcParam.ShootAction);
                    if (ecd != null)
                    {
                        isCastSkill = true;
                        canNextAnim = false;
                        canTriggerEvent = true;
                        animationTimer = srcParam.ShootTime;
                        animationEventTimer = 0.2f;
                        SetHandler = Trigger_Handle;

                        AnimationMsg msg = new AnimationMsg();
                        msg.animationTimer = animationTimer;
                        msg.animationEventTimer = animationEventTimer;
                        msg.ecd = ecd;
                        msg.animationName = ecd.Anim;
                        msg.targetId = 0;

                        string a_data = JSON.Instance.ToJSON(msg);

                        NpcAnimInfo info = new NpcAnimInfo()
                            {
                                ClientID = DeviceInfo.GUID,
                                nextState = NpcAnimState.CastBuff,
                                data = a_data,
                            };

                        animMsg.nextAnim = WarMsg_Type.UseTrigger.ToString();
                        animMsg.uniqueId = cachedNpc.UniqueID;
                        animMsg.data = JSON.Instance.ToJSON(info);

                        if(serMgr != null)
                        {
                            serMgr.realServer.proxyCli.NPCAnim(animMsg);
                        }
                    }
                    else
                    {
                        On_Stand(null);
                    }
                }
            }
        }
        public virtual void On_Killed(WarMsgParam param)
        {
            if(STATE != NpcAnimState.Killed)
            {
                STATE = NpcAnimState.Killed;
                animationTimer = 0f;
                SetHandler = KilledHandle;
                animMsg.nextAnim = WarMsg_Type.BeKilled.ToString();
                animMsg.uniqueId = cachedNpc.UniqueID;
                if(serMgr != null)
                {
                    serMgr.realServer.proxyCli.NPCAnim(animMsg);
                }
                isAttack = false;
                isCastSkill = false;
                cachedNpc.HitAnimReset();
                cachedNpc.curStatus = cachedNpc.curStatus.clear(NpcStatus.InSkill);
            }
        }

        public virtual void On_Respawn(WarMsgParam param)
        {
            if(STATE == NpcAnimState.Killed)
            {
                STATE = NpcAnimState.Respawn;

                cachedTran.position = cachedNpc.spawnPos;
                cachedTran.rotation = cachedNpc.spawnRot;

                cachedNpc.HitAnimReset();
                cachedNpc.SendNpcMoveMsg(true);

                SetHandler = RespawnHandle;
                animMsg.nextAnim = WarMsg_Type.Respawn.ToString();
                animMsg.uniqueId = cachedNpc.UniqueID;
                if(serMgr != null)
                {
                    serMgr.realServer.proxyCli.NPCAnim(animMsg);
                }
                canNextAnim = true;
                On_Stand(null);
            }
        }

        public virtual void On_Idle(WarMsgParam param)
        {
            if (STATE != NpcAnimState.Idle)
            {
                STATE = NpcAnimState.Idle;
                SetHandler = IdleHandle;
                canNextAnim = false;
                animMsg.nextAnim = WarMsg_Type.Idle.ToString();
                animMsg.uniqueId = cachedNpc.UniqueID;
                if(serMgr != null)
                {
                    serMgr.realServer.proxyCli.NPCAnim(animMsg);
                }
            }
        }

        public virtual void On_Destroy(WarMsgParam param)
        {
            STATE = NpcAnimState.Destroying;
            canNextAnim = false;
            SetHandler = null;
            animMsg.nextAnim = WarMsg_Type.Destroy.ToString();
            animMsg.uniqueId = cachedNpc.UniqueID;
            if(serMgr != null)
            {
                serMgr.realServer.proxyCli.NPCAnim(animMsg);
            }
            Destroy(gameObject);
        }

        public virtual void On_Suffer(bool isSuffer)
        {
            if (isSuffer)
            {
                STATE = NpcAnimState.Suffer;
                isConscious = false;
                animMsg.nextAnim = WarMsg_Type.Suffer.ToString();
                animMsg.uniqueId = cachedNpc.UniqueID;
                if (serMgr != null)
                {
                    serMgr.realServer.proxyCli.NPCAnim(animMsg);
                }
            }
            else
            {
                isConscious = true;
                isAttack = false;
                isCastSkill = false;
                canTriggerEvent = true;
                canNextAnim = true;
                cachedNpc.HitAnimReset();
                On_Stand(null);
            }
        }
        #endregion

        #region StateHandle
        public string lastTarget = "";
        public string curHandle = "";
        public string lastHandle = "";
        protected delegate void StateHandle();
        protected StateHandle handler;
        protected StateHandle SetHandler
        {
            set
            { 
                if (handler != null)
                {
                    lastHandle = handler.Method.Name;
                }
                handler = value;
                if (handler != null)
                {
                    curHandle = handler.Method.Name;
                }
            }
        }

        public virtual void StandHandle()
        {

        }

        public virtual void RunHandle()
        {
            if(STATE == NpcAnimState.Run && !cachedNpc.isAuto)
            {
                cachedCC.SimpleMove(cachedTran.rotation * Vector3.forward * cachedNpc.data.rtData.speed);
            }
        }

        public virtual void AttackHandle()
        {
            if(STATE == NpcAnimState.Attack)
            {
                animationTimer -= Time.deltaTime;
                animationEventTimer -= Time.deltaTime;
                if(animationEventTimer <= 0f && canTriggerEvent)
                {
                    canTriggerEvent = false;
                    if (atkParam != null)
                    {
                        WarTarAnimParam[] targets = atkParam.InjureTar;
                        if (targets != null && targets.Length > 0)
                        {
//                            WarServerManager mgr = WarServerManager.Instance;
                            if (cachedNpc.isRengedUnit)
                            {
                                CreateBullet(NpcAnimEffect.Attack_1);
                            }
                            else
                            {
                                for (int i = 0; i < targets.Length; i++)
                                {
                                    if (targets[i] != null)
                                    {
                                        serMgr.npcMgr.SendMessageAsync(cachedNpc.UniqueID, targets[i].described.target, targets[i]);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("atkParam is empty ...");   
                    }
                }
                if(animationTimer < 0)
                {
                    attackWaitForNext -= Time.deltaTime;
                    if(isAttack)
                    {
                        isAttack = false;
                        cachedNpc.HitAnimEnd();
                    }
                    if(attackWaitForNext < 0)
                    {
                        canNextAnim = true;
                        cachedNpc.HitAnimReset();
                        On_Stand(null);
                    }
                }
            }
        }

        public virtual void Skill_Handle()
        {
            if(STATE == NpcAnimState.CastSkill)
            {
                animationTimer -= Time.deltaTime;
                animationEventTimer -= Time.deltaTime;
                if (animationEventTimer <= 0f && canTriggerEvent)
                {
                    canTriggerEvent = false;

                    WarSrcAnimParam srcParam = skParam as WarSrcAnimParam;
                    int index = srcParam.arg1;
                    List<ISkImp> ops = null;
                    ISkImp sk = null;
                    if (!sk_Imps.TryGetValue(index, out ops))
                    {
                        ops = new List<ISkImp>();
                        sk_Imps.Add(index, ops);
                    }
                    sk = ops.Find(imp => imp.SkOp() == srcParam.OP);
                    if (sk == null)
                    {
                        sk = SkImpFactory.getSkImp(srcParam.OP);
                        ops.Add(sk);
                    }
                    sk.Reset();
                    sk.InitSk(cachedNpc, srcParam);
                    sk.CastSk();

                }
                if(animationTimer < 0f)
                {
                    isCastSkill = false;
                    canNextAnim = true;
                    cachedNpc.HitAnimReset();
                    cachedNpc.curStatus = cachedNpc.curStatus.clear(NpcStatus.InSkill);
                    On_Stand(null);
                }
            }
        }

        public virtual void Buff_Handle()
        {
            if (STATE == NpcAnimState.CastBuff)
            {
                animationTimer -= Time.deltaTime;
                animationEventTimer -= Time.deltaTime;
                if (animationEventTimer < 0f && canTriggerEvent)
                {
                    canTriggerEvent = false;
                }
                if(animationTimer < 0f)
                {
                    isCastSkill = false;
                    canNextAnim = true;
                    cachedNpc.HitAnimReset();
                    cachedNpc.curStatus = cachedNpc.curStatus.clear(NpcStatus.InSkill);
                    On_Stand(null);
                }
            }
        }

        public virtual void Trigger_Handle()
        {
            if (STATE == NpcAnimState.CastTrigger)
            {
                animationTimer -= Time.deltaTime;
                animationEventTimer -= Time.deltaTime;
                if (animationEventTimer < 0f && canTriggerEvent)
                {
                    canTriggerEvent = false;
                }
                if(animationTimer < 0f)
                {
                    isCastSkill = false;
                    canNextAnim = true;
                    cachedNpc.HitAnimReset();
                    cachedNpc.curStatus = cachedNpc.curStatus.clear(NpcStatus.InSkill);
                    On_Stand(null);
                }
            }
        }

        public virtual void KilledHandle()
        {
            if(STATE == NpcAnimState.Killed)
            {
                if(!cachedNpc.isHero && cachedNpc.WhatTypeOf != LifeNPCType.Build)
                {
                    animationTimer += Time.deltaTime;
                    if(animationTimer > 2f)
                    {
                        cachedTran.Translate(Vector3.down * Time.deltaTime * 5f);
                        if (animationTimer > 5f)
                        {
                            cachedNpc.SendAnimMsg(WarMsg_Type.Destroy);
                        }
                    }
                }
            }
        }

        public virtual void RespawnHandle()
        {
            if(STATE == NpcAnimState.Respawn)
            {

            }
        }

        public virtual void IdleHandle()
        {
            if(STATE == NpcAnimState.Idle)
            {
                animationTimer += Time.deltaTime;
                if(animationTimer > 1f)
                {
                    canNextAnim = true;
                    On_Stand(null);
                }
            }
        }

        #endregion

        #region bullet
        public virtual void CreateBullet(NpcAnimEffect effect)
        {
            GameObject obj = null;
            switch(effect)
            {
                case NpcAnimEffect.Attack_1_Start:
                case NpcAnimEffect.Attack_2_Start:
                case NpcAnimEffect.Attack_3_Start:

                    break;
                case NpcAnimEffect.Attack_1:
                case NpcAnimEffect.Attack_2:
                case NpcAnimEffect.Attack_3:
                    obj = Instantiate(fireBall, cachedTran.position, cachedTran.rotation) as GameObject;
                    ServerFB sf = obj.GetComponent<ServerFB>();
                    if (sf != null)
                    {
                        obj.transform.parent = serMgr.gameObject.transform;
                        sf.Init(cachedNpc, atkParam);
                    }
                    else
                    {
                        Destroy(obj);   
                    }
                    break;
                case NpcAnimEffect.Skill_1:
                    break;
                case NpcAnimEffect.Skill_1_Start:
                    break;
                case NpcAnimEffect.Skill_2:
                    break;
                case NpcAnimEffect.Skill_2_Start:
                    break;
                case NpcAnimEffect.Skill_3:
                    break;
                case NpcAnimEffect.Skill_3_Start:
                    break;
                case NpcAnimEffect.Skill_4:
                    break;
                case NpcAnimEffect.Skill_4_Start:
                    break;
            }
        }
        #endregion

        void OnDrawGizmos()
        {
            #if DEBUG

            #endif
        }
    }
}
