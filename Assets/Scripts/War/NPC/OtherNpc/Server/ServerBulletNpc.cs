using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AW.Message;
using AW.Data;

namespace AW.War
{
    public class ServerBulletNpc : ServerNPC
    {
        #region 变量，数值，开关
        /// <summary>
        /// param2
        /// 携带的buff
        /// </summary>
        Int32Fog effectID;
        /// <summary>
        /// param3
        /// 速度，由param3除以1000获得
        /// </summary>
        FloatFog speed;
        /// <summary>
        /// param4
        /// 最大位移，由param4除以1000获得
        /// </summary>
        FloatFog maxDis;
        /// <summary>
        /// param5
        /// 消失方式
        /// 0  攻击第一个目标后消失
        /// 1  达到最大距离后消失
        /// </summary>
        Int32Fog disappearType;
        /// <summary>
        /// param6
        /// 物理攻击强度百分比
        /// </summary>
        //int physicsDamage;
        /// <summary>
        /// param7
        /// 法术攻击强度百分比
        /// </summary>
        //int magicDamage;
        /// <summary>
        /// param8
        /// 伤害目标判断
        /// 0  飞行过程持续伤害
        /// 1  只伤害周围目标
        /// 2  只伤害目标
        /// </summary>
        Int32Fog damageType;
        /// <summary>
        /// param9
        /// 伤害半径
        /// </summary>
        //float damageRadius;


        /// <summary>
        /// 是否初始化完毕
        /// </summary>
        bool inited;
        /// <summary>
        /// 自己的Transform
        /// </summary>
        Transform tran;
        /// <summary>
        /// 单帧内的移动距离
        /// </summary>
        FloatFog frameDis;
        #endregion

        #region 目标控制
        WarServerManager wmMgr;
        ServerNPC parent;
        BNPC target;
        int targetIndex;
        List<GameObject> alreadyContacted = new List<GameObject>();
        #endregion

        // Use this for initialization
        public override void Start()
        {
            base.Start();
            SendNpcMoveMsg(true);
        }
	
        // Update is called once per frame
        void Update()
        {
            if(inited)
            {
                Move();
            }
        }

        /// <summary>
        /// Init the specified owner and param.
        /// </summary>
        /// <param name="owner">Owner.</param>
        /// <param name="param">Parameter.</param>
        public override void Init(ServerNPC owner, WarMsgParam param)
        {
            parent = owner;
            tran = transform;
            wmMgr = WarServerManager.Instance;
            Camp = Camp.set(parent.Camp);

            WarSrcAnimParam srcParam = param as WarSrcAnimParam;
            if(srcParam != null)
            {
                SelfDescribed sd = srcParam.described;
                if(sd != null)
                {
                    EndResult result = sd.targetEnd;
                    int effectIndex = result.param2;
                    targetIndex = result.param5;
                    if(targetIndex > 0)
                    {
                        if(wmMgr != null)
                        {
                            target = wmMgr.npcMgr.GetNPCByUniqueID(targetIndex);
                        }
                    }
                    EffectConfigData effect = Core.Data.getIModelConfig<EffectModel>().get(effectIndex);
                    if(effect != null)
                    {
                        effectID = effectIndex;
                        speed = effect.Param3 / 1000f;
                        maxDis = effect.Param4 / 1000;
                        damageType = effect.Param8;
                        disappearType = effect.Param5;
                    }
//                    string msg = string.Format("[effect:{0} speed:{1} maxDis:{2} disappearType:{3} damageType:{4}]", effectID, speed, maxDis, disappearType, damageType);
//                    Debug.Log(msg);
                }
                inited = true;
            }
        }

        #region 行为
        void Move()
        {
            if(target != null && damageType == 2)
            {
                Vector3 dir = tran.position;
                dir.y = target.transform.position.y;
                dir = dir - target.transform.position;
                Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
                tran.rotation = rot;
            }

            frameDis = Time.deltaTime * speed;
            tran.Translate(Vector3.forward * frameDis);

            if(disappearType == 1)
            {
                maxDis -= frameDis;
                if(maxDis <= 0)
                {
                    //TODO:消失
                    inited = false;
                    DestroyMe();
                }
            }
        }

        void DestroyMe()
        {
            IpcDestroyNpcMsg msg = new IpcDestroyNpcMsg();
            msg.id = UniqueId;
            wmMgr.realServer.proxyCli.NpcDestroy(msg);
            Destroy(gameObject);
        }
        #endregion

        /// <summary>
        /// Raises the trigger enter event.
        /// </summary>
        /// <param name="col">Col.</param>
        void OnTriggerEnter(Collider col)
        {
            if (alreadyContacted.Contains(col.gameObject) || !inited)
            {
                return;
            }
            alreadyContacted.Add(col.gameObject);
            ServerNPC npc = col.GetComponent<ServerNPC>();
            if(npc != null)
            {
                WarTarAnimParam param = new WarTarAnimParam();
                param.arg1 = effectID;
                param.arg2 = parent.UniqueID;
                param.OP = EffectOp.Bullet_NPC;
                param.cmdType = WarMsg_Type.OnAttacked;
                param.Sender = parent.UniqueID;
                param.Receiver = npc.UniqueID;
                wmMgr.npcMgr.SendMessageAsync(npc.UniqueID, npc.UniqueID, param);
                if(damageType == 1 || damageType == 2)
                {
                    if(target != null && npc.UniqueID == target.UniqueID && disappearType == 0)
                    {
                        inited = false;
                        DestroyMe();
                    }
                }
            }
        }

        #region override
        public override void OnHandleMessage(MsgParam param)
        {
            base.OnHandleMessage(param);
        }

        public override KindOfNPC WhatKindOf()
        {
            return KindOfNPC.NonLife;
        }
//        public override void OnDestroy()
//        {
//            onEnd = null;
//            parent.removeChild(this);
//            base.OnDestroy();
//        }
        #endregion
    }
}
