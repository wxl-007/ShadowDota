using System;
using AW.Data;
using System.Collections.Generic;
using AW.Message;
using AW.Framework;

namespace AW.War {
	using UVec3 = UnityEngine.Vector3;
	/// <summary>
	/// 将目标模型替换成<param1>的NPC，位置不变，并增加<param2>的AI，以<param3>的方式获取NPC属性，
	/// 以<param4>的方式对操作方式进行修改，持续时间为<param5>，
	/// 修改<param6>的属性值为<param7>的千分比，修改<param8>的属性值为<param9>的千分比
	/// </summary>

	[Effect(OP=EffectOp.SwitchNpc)]
	public class SwitchNpcEffect : BaseEffect, ICastEffect {
		public SwitchNpcEffect() : base() { }

		#region ICastEffect implementation
		public void Init(EffectConfigData config, SkillConfigData skillcfg) {
			cfg   = config;
			skCfg = skillcfg;
		}

		/// <summary>
		/// 释放技能效果
		/// </summary>
		/// <param name="src">效果发起者</param>
		/// <param name="skTarget">技能选择的目标</param>
		/// <param name="target">技能先选择目标后，再次Effect选择后的目标</param>
		/// <param name="skDirectHurt">是否是技能的直接伤害</param>
		/// <param name="container">所有数据展现的容器，本次Effect施法的效果会依次加入</param>
		public void Cast (ServerNPC src, IEnumerable<ServerNPC> skTarget, IEnumerable<ServerNPC> target, bool skDirectHurt, List<MsgParam> container) {
			#if DEBUG
			Utils.Assert(cfg == null, "Effect Configure is null in SwitchNpcEffect.");
			#endif

			int SwitchToId = cfg.Param1;
			UVec3 Locale   = src.transform.position;
			NPCAIType AI   = (NPCAIType) Enum.ToObject(typeof(NPCAIType), cfg.Param2);

			//数据源
			CtorNpcSource datasrc = (CtorNpcSource) Enum.ToObject(typeof(CtorNpcSource), cfg.Param3);
			int chargeOf   = cfg.Param4;
			float duration = cfg.Param5 * Consts.OneThousand;

			//参数10，是否继承普通攻击
			bool inheritNorAtk = cfg.Param10 == 0;

			NPCConfigData npcCfg = null;
			NPCRuntimeData npcRt = null;

			//获取参数配置
			NPCModel model = Core.Data.getIModelConfig<NPCModel>();
			switch(datasrc) {
			case CtorNpcSource.NPC_Castor_Init:
				npcCfg = src.data.configData.ShallowCopy();
				break;
			
			case CtorNpcSource.NPC_Castro_Cur:
				npcCfg = src.data.configData.ShallowCopy();
				npcRt = src.data.rtData.ShallowCopy();
				break;

			case CtorNpcSource.NPC_Table:
				npcCfg = model.get(SwitchToId);
				Utils.Assert(npcCfg == null, "Can't find NPC configure. NPC ID = " + SwitchToId);
				break;
			}

			if(inheritNorAtk == false) {
				NPCConfigData swtNpcCfg = model.get(SwitchToId);
				Utils.Assert(swtNpcCfg == null, "Can't find NPC configure. NPC ID = " + SwitchToId);
				npcCfg.ID = SwitchToId;
				npcCfg.normalHit = swtNpcCfg.normalHit;
			}

			npcRt = npcRt ?? new NPCRuntimeData(npcCfg);

			//额外的属性修改
			NPCAttributeModel AttModel = Core.Data.getIModelConfig<NPCAttributeModel>();
			AttrbuteConfig att1 = AttModel.get(cfg.Param6);
			AttrbuteConfig att2 = AttModel.get(cfg.Param8);

			//参数
			float param1 = cfg.Param7 * Consts.OneThousand;
			float param2 = cfg.Param9 * Consts.OneThousand;

			//修正属性的值
			if(att1 != null) {
				if(att1.type == "int") {
					npcRt.addIntegerValue(att1.note, param1);
				} else if(att1.type == "float") {
					npcRt.addFloatValue(att1.note, param1);
				}
			}

			if(att2 != null) {
				if(att2.type == "int") {
					npcRt.addIntegerValue(att2.note, param2);
				} else if(att2.type == "float") {
					npcRt.addFloatValue(att2.note, param2);
				}
			}

			//Buff的列表
			int[] BuffIds = cfg.Param11;

			///
			/// 统计信息
			///
			WarSrcAnimParam SrcParam = new WarSrcAnimParam() {
				OP = EffectOp.SwitchNpc,
                ShootAction = skCfg.ShootAction,
                ShootEventTime = skCfg.ShootEventTime,
				ShootTime   = skCfg.ShootTime,
				described   = new SelfDescribed() {
					src    = src.UniqueID,
					target = src.UniqueID,
					act     = Verb.Creature,
					srcEnd  = new EndResult {
						param1 = SwitchToId,
						param2 = (int)AI,
						param3 = chargeOf,
						param8 = duration,
					},
					targetEnd = null,
				},

				SkillId   = skCfg.ID,
				OringinOP = EffectOp.SwitchNpc,
			};

			SrcParam.described.srcEnd.obj = ctorToShowcase(npcRt, npcCfg, BuffIds, Locale);

			container.Add(SrcParam);
		}

		#endregion


		CreatNpcDepandency ctorToShowcase(NPCRuntimeData rt, NPCConfigData cfg, int[] BuffIDList, UVec3 TargetVec3) {
			NPCData data = new NPCData() {
				configData = cfg,
				rtData     = rt,
			};

			//有碰撞
			bool Collide = true;
			CreatNpcDepandency depandency = new CreatNpcDepandency() {
				IsCollide     = Collide,
				Source        = data,
				Buff_IDs      = BuffIDList,
				TargetVector3 = new List<Vec3F>(),
			};
		
			depandency.TargetVector3.Add(TargetVec3.toCustomVec3());

			return depandency;
		}
	}
}
