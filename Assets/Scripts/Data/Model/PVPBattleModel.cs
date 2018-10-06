using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// pvp配置模块
/// </summary>
namespace AW.Data
{
	[Modle(type = DataSource.FromLocal)]
	public class PVPBattleModel : KVModelBase <int, ChapterConfigData>
	{
		public PVPBattleModel()
		{
		}

		/*************************************实现接口方法******************************/
		public override bool loadFromConfig() 
		{
			return base.load (ConfigType.PVPBattle);
		}
		/*****************************************************************************/

		/// <summary>
		/// 得到pvp的配置信息
		/// </summary>
		/// <returns>The PVP config.</returns>
		public Dictionary<int, ChapterConfigData> GetPVPConfig()
		{
			return instance;
		}

		/// <summary>
		/// 得到pvp的配置信息
		/// </summary>
		/// <returns>The PVP config.</returns>
		public ChapterConfigData GetPVPConfig(int id) {
			ChapterConfigData cfgData = null;
			if (instance.TryGetValue (id, out cfgData))
				return cfgData;

			ConsoleEx.DebugLog (" not find  pvp config data   " + id);
			return null;
		}

	}
}
