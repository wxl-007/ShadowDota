using System;
using AW.Data;
using AW.Framework;
using AW.Message;
using System.Collections.Generic;
using System.Linq;

namespace AW.War {
	///
	/// 将技能施放者和目标的属性ID为<param1>，<param2>，<param3>的值进行置换，
	/// 以<param4>的方式处理置换后数值，置换后目标最低保留最属性大值千分比为<param5>
	///
	[Effect(OP = EffectOp.ExchangeNpcAttri)]
	public class ExchangeNpcAttrEffect : BaseEffect, ICastEffect {

		public ExchangeNpcAttrEffect() : base() { }

		#region ICastEffect implementation

		public void Init (EffectConfigData config, SkillConfigData skillcfg) {
			base.cfg   = config;
			base.skCfg = skillcfg;
		}

		public void Cast (ServerNPC src, IEnumerable<ServerNPC> skTarget, IEnumerable<ServerNPC> target, bool skDirectHurt, List<MsgParam> container) {
			#if DEBUG
			Utils.Assert(cfg == null, "Effect Configure Data is null in ExchangeNpcAttrEffect.");
			#endif

			bool any = target.Any();
			if(any) {

				NPCAttributeModel model = Core.Data.getIModelConfig<NPCAttributeModel>();
				AttrbuteConfig att1 = model.get(cfg.Param1);
				AttrbuteConfig att2 = model.get(cfg.Param2);
				AttrbuteConfig att3 = model.get(cfg.Param3);

				List<AttrbuteConfig> used = new List<AttrbuteConfig>();
				if(att1 != null) used.Add(att1);
				if(att2 != null) used.Add(att2);
				if(att3 != null) used.Add(att3);

				//是否为固定值还是比例值
				bool Fixture = cfg.Param6 == 0;
				//设置可改变下限-- 只有当cfg.Param6 = 1时才有意义
				float factor = cfg.Param5 * Consts.OneThousand;
				//设置可改变下限-- 只有当cfg.Param6 = 0时才有意义
				int Min      = cfg.Param5;

				//运行时的数据，
				ServerNPC tar = target.FirstOrDefault();

				if(tar != null) {

					//统计信息
					WarSrcAnimParam SrcParam = new WarSrcAnimParam() {
						OP = EffectOp.ExchangeNpcAttri,
						OringinOP = EffectOp.ExchangeNpcAttri,
						SkillId = skCfg.ID,
						described = new SelfDescribed() {
							src    = src.UniqueID,
							target = src.UniqueID,
						},
						ShootAction = skCfg.ShootAction,
						ShootTime = skCfg.ShootTime,
                        ShootEventTime = skCfg.ShootEventTime,
					};

					WarTarAnimParam TarParam = new WarTarAnimParam() {
						OP = EffectOp.ExchangeNpcAttri,
						OringinOP = EffectOp.ExchangeNpcAttri,
						SkillId = skCfg.ID,
						described = new SelfDescribed() {
							src    = src.UniqueID,
							target = tar.UniqueID,
						},
						HitAction = cfg.HitAction,
					};


					if(Fixture) {
						FixtureExchange(used, src.data.rtData, tar.data.rtData, Min, SrcParam, TarParam);
					} else {
						PercentExchange(used, src.data, tar.data, factor, SrcParam, TarParam);
					}


					container.Add(SrcParam);
					container.Add(TarParam);

				}
			}

		}

		#endregion

		//固定比例交换
		void FixtureExchange(List<AttrbuteConfig> used, NPCRuntimeData selfRt, NPCRuntimeData tarRt, int Min, WarSrcAnimParam SrcParam, WarTarAnimParam TarParam) {
			int cnt = used.Count;
			if(cnt <= 0) return ;

			for(int i = 0; i < cnt; ++ i) {

				AttrbuteConfig att = used[i];
				//自己的值
				float self_att1_val = selfRt.getFloatValue(att.note);
				//敌人的值
				float tar_att1_val  = tarRt.getFloatValue(att.note);
				//敌人值的下限
				float tar_att1_val_min = (float)Min;

				float final_tar_val  = 0F;

				//如果传递过去的值大于下限，则将自己的值和敌人直接替换
				if(self_att1_val > tar_att1_val_min) {
					final_tar_val = self_att1_val;
				} else {
					//如果传递过去的值小于下限，则将敌人的值给自己，敌人的为最低下限
					final_tar_val = tar_att1_val_min;
				}

				//敌人
				if(att.type == "float")
					tarRt.setValue(att.note, (FloatFog)final_tar_val);
				else 
					tarRt.setValue(att.note, (Int32Fog)final_tar_val);

				//自己 
				if(att.type == "float")
					selfRt.setValue(att.note, (FloatFog)tar_att1_val);
				else 
					selfRt.setValue(att.note, (Int32Fog)tar_att1_val);

				///
				/// 血量的信息需要特别的传递给UI
				///
				if(att.note == "curHp") {
					SrcParam.described.srcEnd = new EndResult() {
						param1 = (int)(tar_att1_val - self_att1_val),
						param2 = TarParam.described.target,
					};

					TarParam.described.targetEnd = new EndResult() {
						param1 = (int)(final_tar_val - tar_att1_val),
						param2 = TarParam.described.target,
					};
				}

			}


		}

		//百分比交换
		void PercentExchange(List<AttrbuteConfig> used, NPCData self, NPCData tar, float Min, WarSrcAnimParam SrcParam, WarTarAnimParam TarParam) {
			int cnt = used.Count;
			if(cnt <= 0) return ;

			for(int i = 0; i < cnt; ++ i) {
				AttrbuteConfig att = used[i];

				NPCRuntimeData selfRt = self.rtData;
				NPCRuntimeData tarRt  = tar.rtData;

				NPCConfigData  selfCfg = self.configData;
				NPCConfigData  tarCfg  = tar.configData;

				float selfCfgVal = selfCfg.getValue<float>(att.note);
				float tarCfgVal  = tarCfg.getValue<float>(att.note);

				float selfRtVal  = selfRt.getFloatValue(att.note);
				float tarRtVal   = tarRt.getFloatValue(att.note);
				//自己的值
				float self_att1_per = selfRtVal / selfCfgVal;
				//敌人的值
				float tar_att1_per  = tarRtVal / tarCfgVal;
				//敌人值的下限
				float tar_att1_per_min = Min;


				float tarVal = 0f;
				//如果传递过去的百分比大于下限，则将自己的值和敌人直接替换
				if(self_att1_per > tar_att1_per_min) {
					tarVal = tarCfgVal * self_att1_per;
				} else {
					tarVal = tarCfgVal * tar_att1_per_min;
				}

				if(att.type == "float")
					tarRt.setValue(att.note, tarVal);
				else 
					tarRt.setValue(att.note, (int)tarVal);

				float selfVal = selfCfgVal * tar_att1_per;
				if(att.type == "float")
					selfRt.setValue(att.note, selfVal);
				else 
					selfRt.setValue(att.note, (int)selfVal);

				///
				/// 血量的信息需要特别的传递给UI
				///
				if(att.note == "curHp") {
					SrcParam.described.srcEnd = new EndResult() {
						param1 = (int)(selfVal - selfRtVal),
						param2 = TarParam.described.target,
					};

					TarParam.described.targetEnd = new EndResult() {
						param1 = (int)(tarVal - tarRtVal),
						param2 = TarParam.described.target,
					};
				}
			}
		}

	}
}
