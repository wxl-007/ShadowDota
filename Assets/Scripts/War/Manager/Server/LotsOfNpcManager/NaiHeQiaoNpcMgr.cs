using System;
using System.Collections.Generic;
using AW.Data;

namespace AW.War {
	/// 
	/// 奈河桥模式--就是双方都有主基地，主基地不存在即失败的战斗模式
	/// 服务器端逻辑
	/// 
	public class NeHeQiaoNpcMgr : WarServerNpcMgr {

		//我方的主基地
		public ServerLifeNpc SelfMilitaryBase;
		//敌方的主基地
		public ServerLifeNpc EnemyMilitaryBase;

		//我方的建筑
		public List<ServerLifeNpc> SelfBuild;
		//敌方的建筑
		public List<ServerLifeNpc> EnemyBuild;

		//己方的泉水
		public ServerNPC SelfSpring;
		//敌方的泉水
		public ServerNPC EnemySpring;
		//中立的泉水
		public ServerNPC NeutralSpring;

		public override void Init () {
			base.Init ();

			SelfMilitaryBase = null;
			EnemyMilitaryBase = null;

			SelfBuild = new List<ServerLifeNpc>();
			EnemyBuild = new List<ServerLifeNpc>();
		}

		/// <summary>
		/// 初始化，采集各种对战所需数据
		/// </summary>
		public override void AnalyzeIfComplete() {
			//我方基地
			List<ServerNPC> npcList = GetNPCListByNum (BASE, CAMP.Player);
			if (npcList != null && npcList.Count > 0)
			{
				SelfMilitaryBase = npcList [0] as ServerLifeNpc;
			}

			//敌方基地
			npcList = GetNPCListByNum (BASE, CAMP.Enemy);
			if (npcList != null && npcList.Count > 0)
			{
				EnemyMilitaryBase = npcList [0] as ServerLifeNpc;
			}

			//我方泉水
			npcList = GetNPCListByNum (SPRINGLIFE, CAMP.Player);
			if (npcList != null && npcList.Count > 0)
			{
				SelfSpring = npcList [0];
			}

			//敌方泉水
			npcList = GetNPCListByNum (SPRINGLIFE,CAMP.Enemy);
			if (npcList != null && npcList.Count > 0)
			{
				EnemySpring = npcList [0];
			}

			//中立泉水
			npcList = GetNPCListByNum (SPRINGLIFE, CAMP.Neutral);
			if (npcList != null && npcList.Count > 0)
			{
				NeutralSpring = npcList [0];
			} 

			//己方的建筑
			SelfBuild = GetLifeNPCByType (LifeNPCType.Build, CAMP.Player);

			//敌方的建筑
			EnemyBuild = GetLifeNPCByType (LifeNPCType.Build, CAMP.Enemy);

		}
	}
}
