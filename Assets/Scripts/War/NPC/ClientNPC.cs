using System;
using System.Collections.Generic;
using UnityEngine;
using AW.Message;
using AW.Data;
using BehaviorDesigner.Runtime;
using AW.War;

namespace AW.War {

    public class NpcSkillAttr
    {
        public int npcId;
        public int skillIndex;
        public bool isInCd;
        public float cdValue;
        public float baseCd;
    }

	//客户端的NPC基类
	public class ClientNPC : BNPC {

        public ClientNpcAnimState animState = null;

        public LifeNPCType WhatTypeOf {
            get { return data.configData.type; }
        }

		protected Transform tran;
		public Transform CachedTran
		{
			get{ return tran;}
		}

        public bool movable = false;

		/// <summary>
		/// 当前血量
		/// </summary>
		protected FloatFog CurHp;
		/// <summary>
		/// 最大血量
		/// </summary>
		protected FloatFog MaxHp;

        //是否开始同步位置
        public bool IsStartMove;
        //移动的目标位置
        public Vector3 nextPos;
        //旋转的目标角度
        public Quaternion nextRotate;

        public Dictionary<int, NpcSkillAttr> skillAttrDic = new Dictionary<int, NpcSkillAttr>();


		// Use this for initialization
        public virtual void Start () {
			tran = transform;
            skillAttrDic.Add(0, new NpcSkillAttr());
            skillAttrDic.Add(1, new NpcSkillAttr());
            skillAttrDic.Add(2, new NpcSkillAttr());
            skillAttrDic.Add(3, new NpcSkillAttr());
		}
            
        private void NpcMove(WarMsgParam param)
        {
            IpcNpcMoveMsg msg = param.param as IpcNpcMoveMsg;

            if (msg.forceMove)
            {
                tran.position = VectorWrap.ToVector3(msg.pos);
                tran.rotation = QuaternionWrap.ToQuaternion(msg.rotation);
                nextPos = tran.position;
                nextRotate = tran.rotation;
            }
            else
            {
                IsStartMove = true;
                movable = true;
                nextPos = VectorWrap.ToVector3(msg.pos);
                nextRotate = QuaternionWrap.ToQuaternion(msg.rotation);
            }
        } 

        private float disFromNextPos = 0f;
        public float DisFromNextPos
        {
            get
            { 
                disFromNextPos = Vector3.Distance(CachedTran.position, nextPos);
                return disFromNextPos;
            }
        }

        public void UpdateHp(IpcNpcHpMsg msg)
        {
            if(animState != null)
            {
                animState.UpdateHealthValue(msg);
            }
        }

        /// <summary>
        /// 客户端cd计算
        /// </summary>
        float skCdVal = 0f;
        protected void RunSkCd()
        {
            skCdVal = 0f;
            foreach(KeyValuePair<int, NpcSkillAttr> kv in skillAttrDic)
            {
                if(kv.Value.isInCd)
                {
                    skCdVal = kv.Value.cdValue;
                    skCdVal -= Time.deltaTime;
                    if(skCdVal <= 0f)
                    {
                        kv.Value.isInCd = false;
                        continue;
                    }
                    kv.Value.cdValue = skCdVal;
                }
            }
        }

        public virtual void FixedUpdate()
        {

            RunSkCd();

            if (IsStartMove)
            {
                Vector3 curPos = tran.position;
                tran.position = Vector3.Lerp(curPos, nextPos, Time.deltaTime * 10f);

                Quaternion curRotate = tran.rotation;
                tran.rotation = Quaternion.Slerp(curRotate, nextRotate, Time.deltaTime * 10f);
            }
        }

        public override void OnHandleMessage (MsgParam param) {
            base.OnHandleMessage(param);

            if (param is WarMsgParam)
            {
                WarMsgParam warParam = param as WarMsgParam;
                switch (warParam.cmdType)
                {
                    case WarMsg_Type.Move:
                        NpcMove(warParam);
                        break;
                    default:
                        if(broadcast != null)
                        {
                            broadcast(warParam);
                        }
                        break;
                }
            }
        }


        public override KindOfNPC WhatKindOf()
        {
            return base.WhatKindOf();
        }

        #region 技能冷却数据
        public void AddSkillAttr(int index, NpcSkillAttr atr)
        {
            NpcSkillAttr attr = null;
            if (skillAttrDic.TryGetValue(index, out attr))
            {
                attr.baseCd = atr.baseCd;
                attr.cdValue = atr.baseCd;
                attr.isInCd = true;
            }
            else
            {
                NpcSkillAttr attr2 = new NpcSkillAttr()
                {
                    baseCd = atr.baseCd,
                    cdValue = atr.baseCd,
                    isInCd = true,
                    skillIndex = index,
                    npcId = UniqueId,
                };
                skillAttrDic.Add(index, attr2);
            }
        }

        public NpcSkillAttr GetSkillAttr(int index)
        {
            NpcSkillAttr attr = null;
            if(skillAttrDic.TryGetValue(index, out attr))
            {
                return attr;
            }
            return null;
        }
        #endregion
	}
}
