using System;
using System.Collections.Generic;
using AW.Message;
using AW.Data;
using System.Linq;

namespace AW.War {
	/// <summary>
	/// Buff的释放器，
	/// </summary>
	public class BuffCastor {
		//buff重定向目标选择器
		private BuffSelector BFSelector;
		/// <summary>
		/// 消息容器
		/// </summary>
		private List<MsgParam> container;
		/// <summary>
		/// 效果释放器
		/// </summary>
		private EffectCastor EfCastor;

		private WarServerNpcMgr msgMgr = null;

		private BuffCastor() {
			container = new List<MsgParam>();
		}

		public static BuffCastor instance {
			get { return GenericSingleton<BuffCastor>.Instance; }
		}

		public void init(WarServerNpcMgr npcMgr) {
			msgMgr = npcMgr;
			BFSelector = new BuffSelector(msgMgr);
			EfCastor = new EffectCastor(msgMgr);
		}

		/// <summary>
		/// 这个函数目前没有作用，
		/// 准备将来用于Buff第一次释放时，需要调用的函数
		/// </summary>
		public void FirstCast(RtBufData rtbf) {

			///
			/// --- 设置NPC的状态 ---
			///

			ServerNPC hang = BFSelector.getHangUp(rtbf);
			ServerLifeNpc life = hang as ServerLifeNpc;
			if(life != null) {
				NpcStatus status = (NpcStatus) Enum.ToObject(typeof(NpcStatus), rtbf.BuffCfg.Status);
				//去除不需要挂载的状态
				status = status.rmDiscrete();
				if(status != NpcStatus.None)
					life.curStatus = life.curStatus.set(status);

				//如果有嘲讽的话
				if(status.AnySame(NpcStatus.Taunt)) {
					life.addHatred(rtbf.CastorNpcID, 1);
				}

			}

			///
			/// ---- 释放技能 ---
			///
			castBuff_Skill(rtbf, BuffPhase.Start);
		}

		/// <summary>
		/// Buff结束时，处理的逻辑
		/// </summary>
		public void EndCast(RtBufData rtbf) {
			///
			/// --- 解除NPC的状态 ---
			///

			ServerNPC hang = BFSelector.getHangUp(rtbf);
			ServerLifeNpc life = hang as ServerLifeNpc;
			if(life != null) {
				NpcStatus status = (NpcStatus) Enum.ToObject(typeof(NpcStatus), rtbf.BuffCfg.Status);
				//去除不需要挂载的状态
				status = status.rmDiscrete();

				///
				/// 如果还有同类型的，其他Buff则不应该清除Buff的状态
				///
				if(status != NpcStatus.None) {
					NpcStatus toCleared = WarServerManager.Instance.bufMgr.SiftOutStatus(status, rtbf.ID, rtbf.HangUpNpcID);
					life.curStatus = life.curStatus.clear(toCleared);
				}
					
				///
				/// 删除嘲讽的目标
				///
				if(status.check(NpcStatus.Taunt)) {
					life.clearSpecHatred(rtbf.CastorNpcID);
				}

			}

			///
			/// ---- 解除挂载的Trigger ----
			///
			int NpcId = hang.UniqueID;
			WarServerManager.Instance.triMgr.RemoveTrigger(rtbf.TriggerID, NpcId);

			/// 
			/// ---- 是否删除NPC (目前不适用） ----
			/// 
			if(life != null) {
				SelfDescribed des = new SelfDescribed() {
					src       = hang.UniqueID,
					target    = hang.UniqueID,
					act       = Verb.Punch,
					srcEnd    = null,
					targetEnd = new EndResult( ) {
						param1 = life.data.rtData.totalHp * 10,
						param2 = 0,
						param3 = 2,
					},
				};

				WarSrcAnimParam warParam = new WarSrcAnimParam() {
					OP = EffectOp.Injury,
					described = des,
				};
			}
	
			///
			/// ---- 释放技能 ---
			///
			castBuff_Skill(rtbf, BuffPhase.End);
		}

		//更多的buff结束处理
		public void EndCastStep2(RtBufData rtBf) {
			container.Clear();
			///
			/// 通知UI，去除Buff的UI显示
			///
			int action = rtBf.BuffCfg.BuffAction;
			if(action > 0) {

				///分析BuffAction的信息
				SelfDescribed des = new SelfDescribed() {
					src      = rtBf.CastorNpcID,
					target   = rtBf.HangUpNpcID,
					act      = Verb.BuffEffect,
					targetEnd= null,
					srcEnd   = new EndResult() {
						param2 = action,
						param3 = 1,
					},
				};

				WarTarAnimParam warUI = new WarTarAnimParam() {
					OP   = EffectOp.DotHot,
					HitAction = action,
					described = des,
				};
				container.Add(warUI);

				dispatchMsg(container, null);
			}

		}

		/// <summary>
		/// 仅仅是计算出序列
		/// 目前这个函数能做到的就是每次BUff的Tick（包括第一次）都调用cast
		/// </summary>
		/// <param name="rtbf">Rtbf.</param>
		public void cast(RtBufData rtbf) {

			///
			/// --- 设置NPC的状态 ---
			///

			ServerNPC hang = BFSelector.getHangUp(rtbf);
			ServerLifeNpc life = hang as ServerLifeNpc;
			if(life != null) {
				NpcStatus status = (NpcStatus) Enum.ToObject(typeof(NpcStatus), rtbf.BuffCfg.Status);
				//去除不需要挂载的状态
				status = status.rmDiscrete();
				if(status != NpcStatus.None)
					life.curStatus = life.curStatus.set(status);

				//如果有昏迷的话
				if(status.AnySame(NpcStatus.Taunt)) {
					life.addHatred(rtbf.CastorNpcID, 1);
				}
			}

			///
			/// ---- 释放技能 ---
			///
			castBuff_Skill(rtbf, BuffPhase.Cycle);
		}

		void castBuff_Skill(RtBufData rtbf, BuffPhase phase) {
			container.Clear();

			RtSkData skill = null;
			switch(phase) {
			case BuffPhase.Start:
				skill = rtbf.OnStartSkill;
				break;
			case BuffPhase.End:
				skill = rtbf.OnEndSkill;
				break;
			case BuffPhase.Cycle:
				skill = rtbf.onCycleskill;
				break;
			}

			if(skill != null) {
				ServerNPC castor = BFSelector.locateCastor(rtbf);
				ServerNPC target = BFSelector.locateTarget(rtbf);

				List<ServerNPC> Targets = new List<ServerNPC>();
				Targets.Add(target);
				IEnumerable<ServerNPC> itor = Targets.AsEnumerable<ServerNPC>();

				EfCastor.Cast(castor, itor, skill, container, false);

				dispatchMsg(container, skill);
			}
		}

		/// <summary>
		/// 派发出去消息
		/// </summary>
		/// <param name="outMsg">Out message.</param>
		void dispatchMsg(List<MsgParam> outMsg, RtSkData skill) {

			if(outMsg != null && outMsg.Count > 0) {

				int count = outMsg.Count;

				for(int i = 0; i < count; ++ i) {
					MsgParam msg = outMsg[i];
					WarAnimParam warMsg = msg as WarAnimParam;
                    warMsg.cmdType = WarMsg_Type.UseBuff;
					warMsg.SkillId = skill == null ? -1 : skill.Num;

					if(warMsg != null && warMsg.described != null) {
						SelfDescribed des = warMsg.described;
						#if DEBUG
						ConsoleEx.DebugLog("Buff Msg is going out : " + fastJSON.JSON.Instance.ToJSON(des), ConsoleEx.YELLOW);
						#endif
						msgMgr.SendMessageAsync(des.src, des.target, warMsg);
					}
				}

			}
		}

	}
}