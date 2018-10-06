using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 关卡配置模块
/// </summary>
namespace AW.Data
{
	[Modle(type = DataSource.FromLocal)]
	public class ChapterModel : KVModelBase<int, ChapterConfigData> {
	
		public ChapterModel() {

		}

		/*************************************实现接口方法******************************/
		public override bool loadFromConfig() {
			return base.load(ConfigType.PVEChapter);
		}
		/*****************************************************************************/
	}


	/// <summary>
	/// 关卡配置数据
	/// </summary>
	public class ChapterConfigData : UniqueBaseData <int>
	{
		public int ID;
		/// <summary>
		/// 关卡名字
		/// </summary>
		public string name;

		/// <summary>
		/// 关卡描述
		/// </summary>
		public string desp;

		/// <summary>
		/// 关卡类型
		/// </summary>
		public int type;

		/// <summary>
		/// 章ID
		/// </summary>
		public int chapter;

		/// <summary>
		/// 小关ID
		/// </summary>
		public int section;

		/// <summary>
		/// 前置关卡ID
		/// </summary>
		public int front;

		/// <summary>
		/// 关卡配置ID
		/// </summary>
		public int scene_config;

		/// <summary>
		/// 消耗体力
		/// </summary>
		public int cost;

		/// <summary>
		/// 扫荡消耗体力
		/// </summary>
		public int saodang_cost;

		/// <summary>
		/// 每日次数限制
		/// </summary>
		public int time_Limit;

		/// <summary>
		/// 解锁等级
		/// </summary>
		public int unlock_lv;

		/// <summary>
		/// 对话类型
		/// </summary>
		public int dialogue_type;

		/// <summary>
		/// 对话内容
		/// </summary>
		public string dialogue;
	
		/// <summary>
		/// 场景资源ID
		/// </summary>
		public int scene;


		/// <summary>
		/// 获得经验
		/// </summary>
		public int exp;

		/// <summary>
		/// 获得金币
		/// </summary>
		public int coin;

		/// <summary>
		/// 可能获得
		/// </summary>
		public string[] rewards;

		/// <summary>
		/// 难度参数
		/// </summary>
		public float diff_param;
	
		public override int getKey() {
			return this.ID;
		}
	}
}
