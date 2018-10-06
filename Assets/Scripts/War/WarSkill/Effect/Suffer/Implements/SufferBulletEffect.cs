using System;
using AW.Message;
using AW.Data;
using System.Collections.Generic;
using AW.Framework;
using System.Linq;

namespace AW.War {
	[Effect(OP = EffectOp.Bullet_NPC)]
	public class SufferBulletEffect : ISufferEffect {

		private EffectSelector efSelector = null;

		#region ISufferEffect implementation

		/// <summary>
		/// Suffer the specified caster, sufferer, des and npcMgr.
		/// </summary>
		/// <param name="caster">Caster.</param>
		/// <param name="sufferer">Sufferer.</param>
		/// <param name="des">SelfDescribed src = arg1, target = arg2 </param>
		/// <param name="npcMgr">Npc mgr.</param>
		public void Suffer (ServerNPC caster, ServerNPC sufferer, SelfDescribed des, WarServerNpcMgr npcMgr) {
			WarMsgParam warMsg = new WarMsgParam();
			warMsg.Sender   = caster.UniqueID;
			warMsg.Receiver = sufferer.UniqueID;
			warMsg.arg1     = des.src;
			warMsg.arg2     = des.target;
			HandleOnAttack(warMsg, npcMgr);
		}

		/// <summary>
		/// 处理主动打击其他NPC的时候，触发的事情
		/// </summary>
		void HandleOnAttack(MsgParam msg, WarServerNpcMgr npcMgr) {
			WarMsgParam warMsg = msg as WarMsgParam;
			if(warMsg != null) {

				if(efSelector == null) efSelector = EffectSufferShared.get(npcMgr);

				int CasterId = warMsg.Sender;
				int SufferId = warMsg.Receiver;
				int EffectId = warMsg.arg1;
				int FinalId  = warMsg.arg2;
				//是否是最终目标
				bool IsfinalTarget = SufferId == FinalId;
				///
				/// -------- 获取Effect的配置 -------
				///
				EffectModel efModel = Core.Data.getIModelConfig<EffectModel>();
				EffectConfigData efCfg = efModel.get(EffectId);
				Utils.Assert(efCfg == null, "Can't find Effect Configure. effect ID = " + EffectId);
				//半径
				float radius = efCfg.Param9 * Consts.oneHundred;
	
				ServerNPC caster = npcMgr.GetNPCByUniqueID(CasterId);
				ServerNPC suffer = npcMgr.GetNPCByUniqueID(SufferId);

				///
				/// ----------- 先坐第一步的选择和解析 ------------
				///
				IEnumerable<ServerNPC> filter = efSelector.Select(caster, new List<ServerNPC>{ suffer }, efCfg);
				if(filter.Count() > 0) {
					BulletHurtType HurtType = (BulletHurtType) Enum.ToObject(typeof(BulletHurtType), efCfg.Param8);
					List<ServerNPC> targets = selectTarget(HurtType, suffer, IsfinalTarget, npcMgr, radius);

					int tarCnt = targets.Count;
					if(tarCnt > 0) {

						///
						/// ------- 开始运算BulletOp --------
						///
						OperatorMgr OpMgr = OperatorMgr.instance;
						BulletNpcOp op    = OpMgr.getImplement<BulletNpcOp>(EffectOp.Bullet_NPC);

						for(int i = 0; i < tarCnt; ++ i) {
							ServerNPC target = targets[i];
							//alive ?
							if(target.data.rtData.curHp > 0) {
								Dmg damage = op.toTargetDmg(caster.data, target.data, efCfg);
								SelfDescribed des = record(CasterId, SufferId, damage, i);

								WarTarAnimParam param = new WarTarAnimParam(){
									OP = EffectOp.Injury,
									OringinOP = EffectOp.Bullet_NPC,
									described = des,
								};
								///发送消息
								npcMgr.SendMessageAsync(CasterId, SufferId, param);
							}
						}

					}

				}

			}
		}

		//选择目标
		List<ServerNPC> selectTarget(BulletHurtType HurtType, ServerNPC suffer, bool isFinal, WarServerNpcMgr npcMgr, float radius) {
			List<ServerNPC> target = new List<ServerNPC>();

			switch(HurtType) {
			/// 伤害所有碰见的敌人
			case BulletHurtType.Keep_Dmg:
				target.Add(suffer);
				break;
				/// 伤害最终的敌人
			case BulletHurtType.Final_Target_Radius:
				if(isFinal) {
					/// --- 默认对敌人的各种类型友方产生攻击 ---
					///
					IEnumerable<ServerNPC> itor = SelectorTools.GetNPCInRange(suffer, radius, npcMgr, KindOfNPC.Life, TargetClass.Friendly, NpcStatus.None);
					target.AddRange(itor);
				}
				break;
				/// 伤害最终的敌人
			case BulletHurtType.Final_Target:
				if(isFinal) {
					target.Add(suffer);
				}
				break;
			}

			return target;
		}

		SelfDescribed record (int CasterId, int SufferId, Dmg damage, int i) {
			//统计数据
			SelfDescribed des = new SelfDescribed() {
				src    = CasterId,
				target = SufferId,
				act    = Verb.Strike,
				srcEnd = null,
				targetEnd = new EndResult() {
					param1 = (int)damage.dmgValue,
					param2 = damage.isCritical ? 1 : 0,
					param3 = (int)damage.dmgType,
					param4 = (int)damage.hitCls,
				}
			};

			return des;
		}

		#endregion

	}
}

