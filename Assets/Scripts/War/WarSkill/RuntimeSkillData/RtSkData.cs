using System;
using System.Collections.Generic;
using AW.Framework;
using AW.War;

namespace AW.Data {
	/// 
	/// 技能运行时期的数据
	/// 
	public class RtSkData {
		//目前这个是没用的
		public int level;

		//当这个值小于0的时候，技能冷却了
		public float coolDown;

		//能否释放该技能
		public bool canCast {
			get {
				return coolDown <= 0F;
			}
		}

		//技能ID
		public int Num {
			get {
				return skillCfg.ID;
			}
		}

		//是否是有存活时间的技能
		public bool isAliveSk {
			get {
				return skillCfg.IsAlive == (short)1;
			}
		}

		//技能配置
		public SkillConfigData skillCfg = null;
		//技能效果的配置
		public Dictionary<int, EffectConfigData> effectCfgDic = null;

		//引导buff
		public BuffConfigData ChannelBuff;
		//被动技能buff
		public BuffConfigData PassiveBuff;

		//当前技能的索引，based-on zero
		public readonly short pos;

		public RtSkData()
		{
		}

		//Level is ignore now
		//索引值index 如果是人物身上的技能，则填写响应的位置，否则可以填写-1，
		public RtSkData (int skId, short index, int level = 0) {

			SkillModel skMo = Core.Data.getIModelConfig<SkillModel>();
			EffectModel efMo = Core.Data.getIModelConfig<EffectModel>();
			SkBufferModel buMo = Core.Data.getIModelConfig<SkBufferModel>();

			skillCfg = skMo.get(skId);
			Utils.Assert(skillCfg == null, "Can't find skill configure. Skill Id = " + skId);

			effectCfgDic = new Dictionary<int, EffectConfigData>();
			if(skillCfg.EffectID != null) {
				int length = skillCfg.EffectID.Length;
				for(int i = 0; i < length; ++ i) {

					int EffectId = skillCfg.EffectID[i];
					if(EffectId <= 0) continue;

					EffectConfigData ecd = efMo.get(EffectId);
					Utils.Assert(ecd == null, "Can't find effect configure. Effect Id = " + EffectId + ". Skill Id = " + skillCfg.ID);
					effectCfgDic[ecd.ID] = ecd;
				}
			} else {
				ConsoleEx.DebugLog("Can't find skill effect. Skill Id = " + skId, ConsoleEx.YELLOW);
			}

			ChannelBuff = buMo.get(skillCfg.ChannelBuff);
			PassiveBuff = buMo.get(skillCfg.PassiveBuff);

			coolDown = 0F;
			//UI 的索引值
			pos      = index;
		}

		//进入CD
		public virtual void EnterCD () {
			coolDown = skillCfg.BaseCD;
		}

	}

	/// <summary>
	/// 这个是被激活的技能
	/// </summary>
	public class RtFakeSkData : RtSkData {
		//当前的计数器
		public int curCounting;
		//存活时间
		public float aliveDur;
		//哪个npc身上
		public int lifeNpcId;

		public RtFakeSkData (int skId, short index, int level = 0) : base(skId, index, level) { 
			curCounting = 0;
		}

		public override void EnterCD () {
			base.EnterCD();
			curCounting ++;
		}
	}


	/// 
	/// 在战斗中的技能数据类
	/// 
	public class RtNpcSkillModel {
		//哪个NPC身上
		private int NpcId;

		private List<RtSkData> AllSkill = null;

		//普通攻击也算作一种技能
		private List<RtSkData> NormalHit = null;

		//激活技能检查器
		private ConditionCastor ConCastor;

		/// 
		/// 根据NPC的ID来构造技能数据模型
		/// 
		public RtNpcSkillModel(int NpcNum, int NpcId) {
			this.NpcId = NpcId;

			AllSkill = new List<RtSkData>();
			NormalHit = new List<RtSkData>();

			NPCModel model = Core.Data.getIModelConfig<NPCModel>();
			NPCConfigData npc = model.get(NpcNum);

			Utils.Assert(npc == null, "Can't find NPC configure. Npc Num = " + NpcNum);

			RtSkData sk = null;
			int count = npc.skill.Length;
			for(int i = 0; i < count; ++ i) {
				int skillId = npc.skill[i];
				if(skillId != 0) {
					sk = new RtSkData(skillId, (short)i);
					AllSkill.Add(sk);
				} else {
					AllSkill.Add(null);
				}
			}

			if(npc.specialskill != 0) {
				sk = new RtSkData(npc.specialskill, 3);
				AllSkill.Add(sk);
			} else {
				AllSkill.Add(null);
			}

			///普通的攻击其实也是技能
			if(npc.normalHit != null) {
				foreach(int skillId in npc.normalHit) {
					if(skillId != 0) {
						sk = new RtSkData(skillId, -1);
						NormalHit.Add(sk);
					} else {
						NormalHit.Add(null);
					}
				}
			} 

			ConCastor = ConditionCastor.instance;
		}

		/// <summary>
		/// 激活其他技能
		/// </summary>
		/// <returns>The to skill.</returns>
		/// <param name="pos">Position.</param>
		public RtSkData switchToSkill(short pos, int skillId, bool isRest) {

			SkillModel skMo = Core.Data.getIModelConfig<SkillModel>();
			SkillConfigData skillCfg = skMo.get(skillId);

			//被激活的技能
			RtSkData InciteSk = null;
			if(skillCfg.IsAlive == 1) {
				InciteSk = new RtFakeSkData(skillId, pos);
				((RtFakeSkData)InciteSk).lifeNpcId = this.NpcId;
				///
				/// 判定是否是重置的技能
				///
				int curCnt = -1;
				if(isRest) {
					RtFakeSkData oldSk = AllSkill[pos] as RtFakeSkData;
					curCnt = oldSk.curCounting;
					((RtFakeSkData)InciteSk).curCounting = curCnt;
				}

				ConsoleEx.DebugLog("--incide RtFakeSkData -", ConsoleEx.RED);
			} else {
				InciteSk = new RtSkData(skillId, pos);
				ConsoleEx.DebugLog("--incide RtSkData -", ConsoleEx.RED);
			}

			if(InciteSk == null) ConsoleEx.DebugLog("Can't find Skill. Skill ID = " + skillId);
			else {
				//设置技能CD
				InciteSk.coolDown = InciteSk.skillCfg.BaseCD;
				AllSkill[pos] = InciteSk;
			}

			return InciteSk;
		}

		///
		/// 获取运行时的技能数据
		/// Pos Zero based.
		/// if pos = 4, means Specal skill
		public RtSkData getRuntimeSkill(short pos) {
			Utils.Assert(pos >= Consts.MAX_SKILL_COUNT || pos < 0, "Out of Range. Only 4 skills per hero. Pos = " + pos) ; 
			return AllSkill[pos];
		}

		/// <summary>
		/// 返回普通攻击的技能
		/// </summary>
		/// <value>The get attack.</value>
		public RtSkData getAttack(short pos) {
			Utils.Assert(pos >= 3 || pos < 0, "Out of Range. Only 3 phases per hero when trying normal attack. Pos = " + pos) ; 
			return NormalHit[pos];
		}

		/// 
		/// 调用CD
		/// 
		public void Update(float deltatime) {
			int len = AllSkill.Count;
			for(int i = 0; i < len; ++ i) {
				RtSkData sk = AllSkill[i];
				if(sk != null && sk.canCast == false) {
					sk.coolDown -= deltatime;
				}

				//记录中间技能的时间
				if(sk != null && sk.isAliveSk) {
					RtFakeSkData fakeSk = (RtFakeSkData)sk;
					fakeSk.aliveDur += deltatime;
					ConCastor.EnterIncite(fakeSk);
				}

			}
		}
	}

}
