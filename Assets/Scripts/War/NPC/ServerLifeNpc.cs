using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AW.Message;
using AW.Data;
using BehaviorDesigner.Runtime;


namespace AW.War {
	/// <summary>
	/// 服务器端的LifeNpc
	/// </summary>
	public class ServerLifeNpc : ServerNPC {
    
		/// <summary>
		/// 技能的控制逻辑
		/// </summary>
		protected SkillCastor npcSkill;

		/// <summary>
        /// 寻路脚本
        /// </summary>
        protected AIPath agent;

        /// <summary>
        /// 寻路脚本
        /// </summary>
        /// <value>The path finding.</value>
        public AIPath pathFinding
        {
            get
            {
                if (agent == null)
                    agent = GetComponent<AIPath>();
                return agent;
            }
        }

        //自动战斗的ai
        protected BehaviorTree mAutoAiTree;
        public BehaviorTree AutoAiTree
        {
            get
            {
                if (mAutoAiTree == null)
                {
                    //ai行为树
                    BehaviorTree[] trees = gameObject.GetComponents<BehaviorTree> ();
                    if (trees != null)
                    {
                        for (int i = 0; i < trees.Length; i++)
                        {
                            if (trees [i].Group == 0)
                            {
                                mAutoAiTree = trees [i];
                            }
                            else if (trees [i].Group == 1)
                            {
                                mManualAiTree = trees [i];
                            }
                        }
                    }
                }
                return mAutoAiTree;
            }
            set
            { 
                mAutoAiTree = value;
            }
        }

        //自动战斗的ai
        protected BehaviorTree mManualAiTree;
        public BehaviorTree ManualAiTree
        {
            get
            {
                if (mManualAiTree == null)
                {
                    //ai行为树
                    BehaviorTree[] trees = gameObject.GetComponents<BehaviorTree> ();
                    if (trees != null)
                    {
                        for (int i = 0; i < trees.Length; i++)
                        {
                            if (trees [i].Group == 0)
                            {
                                mAutoAiTree = trees [i];
                            }
                            else if (trees [i].Group == 1)
                            {
                                mManualAiTree = trees [i];
                            }
                        }
                    }
                }
                return mManualAiTree;
            }
            set
            { 
                mManualAiTree = value;
            }
        }



        //是否活着
        public bool IsAlive
        {
            get { return data.rtData.curHp > 0; }
        }

        /// 
        /// 运行时技能数据
        /// 这个偏向于主动技能
        /// 
        public RtNpcSkillModel runSkMd;

        public LifeNPCType WhatTypeOf {
            get { return data.configData.type; }
        }

		#region 普通攻击
		//默认第一次攻击按钮的点击是有效
		public bool valid = true;
		//当前攻击的第几次 [0,2]
		protected short curIndex;
		public short AttIndex {
			get {
				return curIndex;
			}
		}

		//普通攻击距离
		protected float atkRange = -1;
		public float ATKRange {
			get {
				if(atkRange > 0) return atkRange;
				else {
					RtSkData rtSdk = runSkMd.getAttack(0);
					if(rtSdk != null) { 
						atkRange = rtSdk.skillCfg.Distance; 
					}
					return atkRange;
				}
			}
		}

		//一次击打动画结束
		public void HitAnimEnd() {
			valid = true;
			curIndex = (short)((curIndex + 1) % Consts.NORMAL_ATTACK_TIMES);
		}

		//击打动画重置
		public void HitAnimReset() {
			valid = true;
			curIndex = 0;
		}

		// 攻击
		public void Attack() {
			if(valid) {
				valid    = false;
				List<MsgParam> results = npcSkill.NormalAttack(this, curIndex);
				reportAtk(results);
			}	
		}

		void reportAtk(List<MsgParam> sequnce) {
			int cnt = sequnce.Count;

			///永远Cnt都大于0
			if(cnt > 0) {
				if(outLog)
					ConsoleEx.DebugLog("atk sequnce = " + fastJSON.JSON.Instance.ToJSON(sequnce), ConsoleEx.RED);

				for(int i = 0; i < cnt; ++ i) {
					WarAnimParam warParam = sequnce[i] as WarAnimParam;

					if(warParam.described != null) {
						//自己给自己的消息就直接转过去
						if(broadcast != null) broadcast(warParam);
					}
				}
			} else {
				WarAnimParam param = new WarAnimParam();
				param.Sender = UniqueId;
				param.Receiver = 0;
				param.cmdType = WarMsg_Type.Attack;
				param.OP = EffectOp.Injury;
				//自己给自己的消息就直接转过去
				if(broadcast != null) broadcast(param);
			}

		}

        #region 技能攻击

        public void CastSkill(short index) {
            if(valid) 
            {
                if (index >= 0 && index < Consts.MAX_SKILL_COUNT)
                {
                    npcSkill.Cast (this, index, Report);
                }
            } 

            if(outLog)
                ConsoleEx.DebugWarning (name + " cast skill index ::  " + index);
        }

        void Report(MsgParam result) {
            if(outLog)
                ConsoleEx.DebugLog("Msg is sending now : = " + fastJSON.JSON.Instance.ToJSON(result));
            valid = false;
            WarAnimParam warParam = result as WarAnimParam;
            warParam.cmdType = WarMsg_Type.UseSkill;

            if(warParam.described != null) {
                SelfDescribed decribed = warParam.described;
                //如果不是自己则发送消息出去
                if(decribed.target != UniqueId)
                    npcMgr.SendMessageAsync(UniqueId, decribed.target, result);
                else {

                    //否则，自己给自己的消息就直接转过去
                    WarMsgParam param = result as WarMsgParam;
                    if(param != null && broadcast != null) broadcast(param);
                }
            }

            skMsg.index = result.arg1;
            skMsg.uniqueID = UniqueId;
            skMsg.baseCD = runSkMd.getRuntimeSkill((short)skMsg.index).skillCfg.BaseCD;
            WarServerManager.Instance.realServer.proxyCli.NpcSkillCD(skMsg);
        }

        #endregion

		// 攻击目标的选择
		// 是远目标
		public IEnumerable<ServerNPC> FindAtkTarget(Sight sight = Sight.FarSight) {
			return npcSkill.FindAtkTargets(this, curIndex, sight);
		}

		#endregion


        /// <summary>
        /// npc当前的状态
        /// </summary>
        public NpcStatus curStatus;

        private WarServerManager warServerMgr;
        private WarServerNpcMgr npcMgr;
        private WarServerCharactor chaPool;

        private IpcNpcAnimMsg animMsg;
        private IpcNpcHpMsg hpMsg;

        private IpcSkillMsg skMsg;

        private ServerNpcAnimState animState;
        public ServerNpcAnimState mAnimState
        {
            get
            {
                return animState;
            }
        }

		public override void Awake() {
			base.Awake();

            warServerMgr = WarServerManager.Instance;
            npcMgr = warServerMgr.npcMgr;
            chaPool = warServerMgr.realServer.monitor.CharactorPool;

            animMsg = new IpcNpcAnimMsg();
            hpMsg = new IpcNpcHpMsg();
            skMsg = new IpcSkillMsg();

            animState = gameObject.AddComponent<ServerNpcAnimState>();
            if(animState != null)
            {
                broadcast = animState.OnNewStateReceived;
                animState.cachedNpc = this;
            }
        }

		public override void Start()
        {
			base.Start();
			//普通攻击
			valid    = true;
			curIndex = 0;
			npcSkill = warServerMgr.npcSkill;
        }

        //战斗结束
        protected void BattleOver()
        {
            if (pathFinding != null)
                pathFinding.enabled = false;
            if (AutoAiTree != null)
                AutoAiTree.enabled = false;
        }

//        void FixedUpdate()
//        {
//            SendNpcMoveMsg();
//        }


        public bool isAuto = true;

        public bool isHero
        {
            get
            { 
                return WhatTypeOf.check(LifeNPCType.Hero);
            }
        }
        /// 切换自动战斗和手动战斗
        /// </summary>
        /// <param name="bAuto">If set to <c>true</c> b auto.</param>
        public void SwitchAutoBattle(bool bAuto)
        {
            if (bAuto)
            {
                if (!AutoAiTree.enabled)
                    AutoAiTree.enabled = true;
                AutoAiTree.EnableBehavior();
            }
            else
            {
                pathFinding.enabled = false;
                AutoAiTree.enabled = false;
                AutoAiTree.DisableBehavior();

//              //播放站立消息 
                SendAnimMsg(WarMsg_Type.Stand);
            }
            isAuto = bAuto;
        }
            
        public override void OnHandleMessage (MsgParam param) {
            base.OnHandleMessage (param);


            if (param is WarTarAnimParam)
            {
                WarTarAnimParam effParam = param as WarTarAnimParam;
                switch (effParam.OP)
                {
                    case EffectOp.Injury:
                        BeInjured (effParam);
                        break;

                    case EffectOp.Treat:
                        BeTreated (effParam);
                        break;
                    case EffectOp.ExchangeNpcAttri:
                        break;
                }
            }
            else if(param is WarSrcAnimParam)
            {
                WarSrcAnimParam src = param as WarSrcAnimParam;
                if(src.OP == EffectOp.ExchangeNpcAttri)
                {
                    Debug.Log("换血了");
               
                }
            }
            else if (param is WarMsgParam)
            {
                WarMsgParam warParam = param as WarMsgParam;

                switch (warParam.cmdType)
                {
                    case WarMsg_Type.Win:
                    case WarMsg_Type.Lose:
                        BattleOver ();
                        break;
                }
            }
        }

        //收到伤害
        protected void BeInjured(WarTarAnimParam param)
        {
            if (param == null)
            {
                ConsoleEx.DebugWarning ("BeInjured :: BeInjured :: param is null");
                return;
            }


            if (param.described != null )
            {
                SufferInjureEffect sufInjury = warServerMgr.sufMgr.getImplement<SufferInjureEffect>(EffectOp.Injury);
                SelfDescribed des = param.described ;

                //记录攻击自己的人的ID
                data.btData.atkerID = des.src;

                if (des.target == UniqueId && IsAlive)
                {
                    ServerNPC caster = npcMgr.GetNPCByUniqueID (des.src);
                    sufInjury.Suffer(caster, this, des, npcMgr);
                    Dmg oneDmg = sufInjury.getHandled;

                    hpMsg.uniqueId = UniqueID;
                    hpMsg.deltaHp = (int)oneDmg.dmgValue;
                    hpMsg.curHp = data.rtData.curHp;
                    hpMsg.totalHp = data.rtData.totalHp;
                    hpMsg.isDamage = true;
                    hpMsg.srcID = des.src;
                    warServerMgr.realServer.proxyCli.NPChp(hpMsg);

//                    if (caster.UniqueID == npcMgr.ActiveHero.UniqueId)
//                    {
//                        if (OnHitByActiveHero != null)
//                        {
//                            Vector3 posIn2D = Vector3.zero;
//                            Vector3 posIn3D = transform.position;
//                            posIn3D.y += 3.5f;
//                            wmMgr.GetUIPos_Ref3DPos (posIn3D, ref posIn2D);
//                            OnHitByActiveHero ((int)oneDmg.dmgValue, true, oneDmg.isCritical, posIn2D);
//                        }
//                    }

                    //检查自己是否被杀
                    if (data.rtData.curHp <= 0)
                    {
                        BeKillded ();
                    }
                }
            }
        }

        //处理被杀消息
        void BeKillded()
        {
            if (pathFinding != null)
                pathFinding.enabled = false;

            AutoAiTree.DisableBehavior ();

            if (ManualAiTree != null)
                ManualAiTree.DisableBehavior ();

//            //给自己发消息，播放死亡动画
//            WarSrcAnimParam deadParam = new WarSrcAnimParam ();
//            deadParam.cmdType = WarMsg_Type.BeKilled;
//            deadParam.Sender = UniqueID;
//            deadParam.Receiver = UniqueID;
//            SendMsg (UniqueID, deadParam);
            SendAnimMsg(WarMsg_Type.BeKilled);

            //如果是敌方或者己方英雄挂了，重生
            if (chaPool.IsTeamHero(UniqueID))
            {
                Invoke ("Respawn", 15);
            }
        }

        //重生
        void Respawn()
        {
            data.rtData.curHp = data.rtData.totalHp;
            data.btData.atkerID = 0;
            data.btData.btStatus = NPCBattle_Status.None;
            data.btData.IsInBattle = false;

            if (!chaPool.IsHeroActive(this))
            {
                SwitchAutoBattle (true);
            } 
            else
            {
                SwitchAutoBattle (isAuto);
            }

            SendAnimMsg(WarMsg_Type.Respawn);

            gameObject.SetActive (true);
        }


        //治疗消息
        protected void BeTreated(WarTarAnimParam param)
        {
            if (param == null)
            {
                ConsoleEx.DebugWarning ("BeTreated :: BeTreated :: param is null");
                return;
            }

            if (param.described != null)
            {
                SufferTreatEffect treatInjury = warServerMgr.sufMgr.getImplement<SufferTreatEffect>(EffectOp.Treat);

                SelfDescribed des = param.described ;
                ServerNPC caster = npcMgr.GetNPCByUniqueID (des.src);
                if (des.target == UniqueId)
                {
                    treatInjury.Suffer (caster, this, des, npcMgr);
                    Treat oneTreat = treatInjury.getHandled;

                    if (oneTreat.treatValue > 0)
                    {
                        hpMsg.uniqueId = UniqueID;
                        hpMsg.deltaHp = (int)oneTreat.treatValue;
                        hpMsg.curHp = data.rtData.curHp;
                        hpMsg.totalHp = data.rtData.totalHp;
                        hpMsg.isDamage = false;
                        hpMsg.srcID = des.src;
                        warServerMgr.realServer.proxyCli.NPChp(hpMsg);
                    }

//                    //回血UI
//                    caster = npcMgr.GetNPCByUniqueID (des.target);
//                    if (caster.UniqueID == warServerMgr.teamMgr.selfActiveHero.UniqueId && data.rtData.curHp < data.rtData.totalHp)
//                    {
//                        if (OnHitByActiveHero != null)
//                        {
//                            if (wmMgr != null)
//                            {
//                                Vector3 posIn2D = Vector3.zero;
//                                Vector3 posIn3D = transform.position;
//                                posIn3D.y += 3.5f;
//                                wmMgr.GetUIPos_Ref3DPos (posIn3D, ref posIn2D);
//                                OnHitByActiveHero ((int)oneTreat.treatValue, false, oneTreat.isCritical, posIn2D);
//                            }
//                        }
//                    }
                }
            }
        }
	}
}
