using System;
using AW.Data;
using AW.Message;
using System.Collections.Generic;
using AW.Framework;
using UVec3 = UnityEngine.Vector3;

namespace AW.War {

	[Effect(OP = EffectOp.CtorNPC)]
	public class CtorNpcEffect : BaseEffect, ICastEffect  {

		public CtorNpcEffect() : base() { }

		#region ICastEffect implementation

		public void Init (EffectConfigData config, SkillConfigData skillcfg) {
			base.cfg   = config;
			base.skCfg = skillcfg;
		}

		/// <summary>
		/// 释放技能效果
		/// </summary>
		/// <param name="src">效果发起者</param>
		/// <param name="skTarget">技能选择的目标, 无意义</param>
		/// <param name="target">技能先选择目标后，再次Effect选择后的目标</param>
		/// <param name="skDirectHurt">是否是技能的直接伤害, 这个没有伤害</param>
		/// <param name="container">所有数据展现的容器，本次Effect施法的效果会依次加入</param>
		public void Cast (ServerNPC src, IEnumerable<ServerNPC> skTarget, IEnumerable<ServerNPC> target, bool skDirectHurt, List<MsgParam> container) {

			#if DEBUG
			Utils.Assert(skCfg == null, "Skill Configure is null in BulletNpcEffect routine.");
			#endif

			WarSrcAnimParam SrcParam = new WarSrcAnimParam() {
				OP = EffectOp.CtorNPC,
				ShootAction = skCfg.ShootAction,
				ShootTime   = skCfg.ShootTime,
                ShootEventTime = skCfg.ShootEventTime,
			};

			int CtorNpcId  = cfg.Param1;
			float LifeTime = cfg.Param2 * Consts.OneThousand;

			CtorNpcPos pos = (CtorNpcPos) Enum.ToObject(typeof(CtorNpcPos), cfg.Param3);

			//碰撞
			bool Collide   = cfg.Param4 == 1;
			int AI_ID      = cfg.Param5;

			int param_1   = cfg.Param8;
			int param_2   = cfg.Param10;

			//会有的Buff
			int[] BuffIDList = cfg.Param11;

			//数据源
			CtorNpcSource datasrc = (CtorNpcSource) Enum.ToObject(typeof(CtorNpcSource), cfg.Param6);

			//数据源的修正
			CtorNpcAttri  attr_1 = (CtorNpcAttri) Enum.ToObject(typeof(CtorNpcAttri), cfg.Param7);
			CtorNpcAttri  attr_2 = (CtorNpcAttri) Enum.ToObject(typeof(CtorNpcAttri), cfg.Param9);

			///
			/// 获取数据源
			///
			NPCData data = new NPCData();
			NPCConfigData cfgD = null;
			NPCRuntimeData rtD = null;

			if(datasrc == CtorNpcSource.NPC_Table) {
				NPCModel model = Core.Data.getIModelConfig<NPCModel>();
				cfgD = model.get(CtorNpcId);
				Utils.Assert(cfgD == null, "Can't find NPC configure. NPC ID = " + CtorNpcId);
			} else if(datasrc == CtorNpcSource.NPC_Castro_Cur) {
				cfgD = src.data.configData;
				rtD  = src.data.rtData.ShallowCopy();
			} else if(datasrc == CtorNpcSource.NPC_Castor_Init) {
				cfgD = src.data.configData;
			}

			rtD = rtD ?? new NPCRuntimeData(cfgD);

			List<String> changed_1 = attr_1.SwitchTo();
			List<String> changed_2 = attr_2.SwitchTo();

			int cnt = changed_1.Count;
			if(cnt > 0) {
                for (int i = 0; i < cnt; ++i)
                    rtD.setValue(changed_1[i], (Int32Fog)param_1);
			} 

			cnt = changed_2.Count;
			if(cnt > 0) {
				for(int i = 0; i < cnt; ++ i)
                    rtD.setValue(changed_2[i], (Int32Fog)param_2);
			}

			data.configData = cfgD;
			data.rtData     = rtD;

			///
			/// 获取目标位置
			///
			List<UVec3> TargetVec3 = new List<UVec3>();
			if(pos == CtorNpcPos.Castor_Forward) {
				TargetVec3.Add( src.transform.forward );
			} else if(pos == CtorNpcPos.Target) {

				foreach(ServerNPC t in target) {
					TargetVec3.Add( t.transform.position );
				}

			} else if(pos == CtorNpcPos.Castor_Surround) {
				TargetVec3.Add( src.transform.position );
			}


			///
			/// ------ 获取统计信息 ------
			///
			SelfDescribed des = new SelfDescribed() {
				src     = src.UniqueID,
				target  = src.UniqueID,
				act     = Verb.Creature,
				srcEnd  = new EndResult {
					param1 = CtorNpcId,
					param2 = AI_ID,
					param8 = LifeTime,
				},
				targetEnd = null,
			};

            des.srcEnd.obj = ctorToShowcase(Collide, data, BuffIDList, TargetVec3, src);
			SrcParam.SkillId    = skCfg.ID;
			SrcParam.described  = des;

			container.Add(SrcParam);
		}

		#endregion

        CreatNpcDepandency ctorToShowcase(bool Collide, NPCData data, int[] BuffIDList, List<UVec3> TargetVec3, ServerNPC src = null) {
			CreatNpcDepandency depandency = new CreatNpcDepandency() {
				IsCollide     = Collide,
				Source        = data,
				Buff_IDs      = BuffIDList,
			};

            int Vec3Cnt = TargetVec3.Count;
            depandency.TargetVector3 = new List<Vec3F>();
            if (Vec3Cnt > 0)
            {
                for (int i = 0; i < Vec3Cnt; ++i)
                {
                    depandency.TargetVector3.Add(TargetVec3[i].toCustomVec3());
                }
            }
            else
            {
                depandency.TargetVector3.Add(src.transform.position.toCustomVec3());
            }

			return depandency;
		}
	}

	//给UI的依赖数据
	public class CreatNpcDepandency {
		//NPC的出生位置
		public List<Vec3F> TargetVector3;
		//是否有碰撞
		public bool IsCollide;
		//出身的数据
		public NPCData Source;
		//Buff的ID列表
		public int[] Buff_IDs;
	}

}
