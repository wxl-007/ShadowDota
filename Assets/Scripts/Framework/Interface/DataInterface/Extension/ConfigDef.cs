using System.Collections.Generic;

namespace AW.Data {
	#region 配置文件的定义，决定如何读取（通常是设定读取的位置）
	public enum ConfigType {
		SensitiveData,
		SoundConfig,
		NPC,						//npc
		Hero,						//英雄
		PVEChapter,					//关卡章节
		PVPBattle,					//pvp对战配置
		Skill,                      //技能信息
		Effect,                     //技能效果的信息
		Buff,                       //技能Buff
		Trigger,                    //技能触发器
		Condition,                  //技能激发条件
		RefreshPool,				//npc刷新池
		RefreshGroup,				//NPC刷新组
		NPCAttribute,               //NPC属性索引
        Effect3DModel,              //技能施加的特效模型
	}
	#endregion

	public static class Config {
		public static Dictionary<ConfigType, HowToRead> LocalConfigs = new Dictionary<ConfigType, HowToRead>() { 
			//TODO : add "The way how to read local configure data"
			{ ConfigType.SensitiveData, new HowToRead(ConfigType.SensitiveData,  "Config/SensitiveData.cfg",    typeof(BaseData)) }, 
			{ ConfigType.NPC,           new HowToRead(ConfigType.NPC,            "Config/NPC.cfg",				typeof(NPCConfigData)) }, 
			{ ConfigType.Hero,          new HowToRead(ConfigType.Hero,           "Config/Hero.cfg",				typeof(NPCConfigData)) }, 
			{ ConfigType.PVEChapter,    new HowToRead(ConfigType.PVEChapter,     "Config/Chapter.cfg",			typeof(ChapterConfigData)) },
			{ ConfigType.PVPBattle,     new HowToRead(ConfigType.PVPBattle,      "Config/PVPBattle.cfg",		typeof(ChapterConfigData)) }, 
			{ ConfigType.Skill,         new HowToRead(ConfigType.Skill,          "Config/Skill.cfg",			typeof(SkillConfigData)) }, 
			{ ConfigType.Effect,        new HowToRead(ConfigType.Effect,         "Config/Effect.cfg",			typeof(EffectConfigData)) }, 
			{ ConfigType.Buff,          new HowToRead(ConfigType.Buff,           "Config/Buff.cfg",			    typeof(BuffConfigData)) }, 
			{ ConfigType.Trigger,       new HowToRead(ConfigType.Trigger,        "Config/Trigger.cfg",			typeof(TriggerConfigData)) }, 
			{ ConfigType.Condition,     new HowToRead(ConfigType.Condition,      "Config/SkCondition.cfg",		typeof(ConditionConfigure))},
			{ ConfigType.NPCAttribute,  new HowToRead(ConfigType.NPCAttribute,   "Config/Attrbute.cfg",	        typeof(AttrbuteConfig)) }, 
			{ ConfigType.RefreshPool,   new HowToRead(ConfigType.RefreshPool,    "Config/NPCFreshPool.multi.cfg",			typeof(NPCFreshPool)) }, 
			{ ConfigType.RefreshGroup,  new HowToRead(ConfigType.RefreshGroup,   "Config/NPCFreshGroup.multi.cfg",			typeof(NPCFreshGroup)) }, 
            { ConfigType.Effect3DModel, new HowToRead(ConfigType.Effect3DModel,  "Config/EffectModel.multi.cfg",            typeof(Effect3DModelConfigData))},

		};
	}
}