using System;
using AW.Data;
using AW.Message;
using AW.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AW.War {
	[Effect(OP = EffectOp.HookNpc)]
	public class SufferHookEffect : ISufferEffect {

		private EffectSelector efSelector = null;

		public void Suffer (ServerNPC caster, ServerNPC sufferer, SelfDescribed des, WarServerNpcMgr npcMgr) {
			int CasterId = caster.UniqueID;
			int SufferId = sufferer.UniqueID;
			int idx = des.targetEnd.param2;
			bool finalTar = des.targetEnd.param3 == SufferId;
			bool maxDis  = des.targetEnd.param4 == 1;
			bool returnback = des.targetEnd.param5 == 1;
			int effectId = des.targetEnd.param1;

			HandleOnAttack(CasterId, SufferId, idx, finalTar, maxDis, returnback, effectId, npcMgr);
		}

		/// <summary>
		/// 处理主动打击其他NPC的时候，触发的事情
		/// </summary>
		/// <param name="CasterId">施法者ID</param>
		/// <param name="SufferId">撞击者ID</param>
		/// <param name="idx">几个被撞击者</param>
		/// <param name="finalTar">是否最终目标</param>
		/// <param name="maxDis">是否最大距离</param>
		/// <param name="returnback">返回原处</param>
		/// <param name="EffectId">效果ID</param>
		/// <param name="npcMgr">Npc mgr.</param>
		void HandleOnAttack(int CasterId, int SufferId, int idx, bool finalTar, bool maxDis, bool returnback, int EffectId, WarServerNpcMgr npcMgr) {

			ServerNPC caster = npcMgr.GetNPCByUniqueID(CasterId);
			ServerNPC suffer = npcMgr.GetNPCByUniqueID(SufferId);

			///
			/// -------- 获取Effect的配置 -------
			///
			EffectModel efModel = Core.Data.getIModelConfig<EffectModel>();
			EffectConfigData efCfg = efModel.get(EffectId);
			Utils.Assert(efCfg == null, "Can't find Effect Configure. effect ID = " + EffectId);

			if(efSelector == null) efSelector = EffectSufferShared.get(npcMgr);

			///
			/// 先做技能
			///
			IEnumerable<ServerNPC> filter = efSelector.Select(caster, new List<ServerNPC>{ suffer }, efCfg);
			if(filter.Any()) {
				//消失的时候，触发可能的位移
				HookNpcDisappearType disappearType = (HookNpcDisappearType) Enum.ToObject(typeof(HookNpcDisappearType), efCfg.Param2);
				//HookNpcDmgType hookDmgType = (HookNpcDmgType) Enum.ToObject(typeof(HookNpcDmgType), efCfg.Param8);
				HookNpcMoveType moveType   = (HookNpcMoveType) Enum.ToObject(typeof(HookNpcMoveType), efCfg.Param5);

				///
				/// 一定会有伤害，之后，先判定是否消失，如果消失，则再次判定是否移动
				///

				///
				/// ------- 开始运算BulletOp --------
				///
				OperatorMgr OpMgr = OperatorMgr.instance;
				BulletNpcOp op    = OpMgr.getImplement<BulletNpcOp>(EffectOp.Bullet_NPC);

				Dmg dmg = op.toTargetDmg(caster.data, suffer.data, efCfg);
				int moveDirection = 0;
				bool dis = Disappear(disappearType, idx, finalTar, maxDis, returnback);
				if(dis) {
					moveDirection = (int)moveType;
				}

				SelfDescribed des = record(CasterId, SufferId, dmg, dis, moveDirection);
				WarTarAnimParam param = new WarTarAnimParam(){
					OP = EffectOp.Injury,
					OringinOP = EffectOp.HookNpc,
					described = des,
				};
				///发送消息
				npcMgr.SendMessageAsync(CasterId, SufferId, param);
			}
		}

		//判定是否要消失
		bool Disappear(HookNpcDisappearType disappearType, int idx, bool finalTar, bool maxDis, bool returnback) {
			bool disApp = false;

			switch(disappearType) {
			case HookNpcDisappearType.DisWhenFinalTar:
				disApp = finalTar;
				break;
			case HookNpcDisappearType.DisWhenFirstAtked:
				disApp = idx == 1;
				break;
			case HookNpcDisappearType.DisWhenMaxDistance:
				disApp = maxDis;
				break;
			case HookNpcDisappearType.DisWhenReturnback:
				disApp = returnback;
				break;
			} 

			return disApp;
		}

		SelfDescribed record (int CasterId, int SufferId, Dmg damage, bool dis, int moveDirection) {
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
					param5 = dis ? 1 : 0,
					param6 = moveDirection,
				}
			};

			return des;
		}
	}
}
