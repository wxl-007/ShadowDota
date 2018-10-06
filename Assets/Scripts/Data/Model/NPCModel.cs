using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// NPC数据管理类
/// </summary>
namespace AW.Data
{
	[Modle(type = DataSource.FromLocal)]
	public class NPCModel : KVModelBase<int, NPCConfigData>
	{
		public NPCModel() {
		
		}

		public override bool loadFromConfig() {
			return base.load(ConfigType.NPC) && base.load(ConfigType.Hero);
		}

	}

	/// <summary>
	/// NPC数据类
	/// </summary>
	public class NPCData
	{
		public NPCConfigData configData;			//配置数据
		public NPCRuntimeData rtData;				//运行时的数据
		public NPCBattleData btData;				//战斗中的实时数据

		//背包ID
		public int id
		{
			get
			{
				return rtData.id;
			}
		}

		//配置表里的ID
		public int num
		{
			get
			{
				return configData.ID;
			}
		}
	}
}
