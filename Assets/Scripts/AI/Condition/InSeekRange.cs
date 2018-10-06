using UnityEngine;
using AW.War;
using AW.Data;
using System.Linq;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Returns success when enermy in seek range")]
	[TaskCategory("Hero")]
	public class InSeekRange : Conditional
	{
		//锁定的目标敌人
		public SharedLifeNPC target;

		//敌方阵营的英雄
        public  List<ServerLifeNpc> enermys;

		//寻敌范围
		public float seekRange;
        private ServerLifeNpc hero;

		private bool InitFinish = false;
		CAMP hostile;
		private Transform mTrans;

		public override void OnAwake()
		{
            enermys = new List<ServerLifeNpc> ();
            hero = GetComponent<ServerLifeNpc>();
			mTrans = this.transform;
			seekRange = hero.data.configData.seekRange + hero.data.configData.radius;
			//得到对方阵营的英雄
			hostile = hero.Camp.Hostile ();
		}

		void InitData()
		{
            if (!WarServerManager.Instance.bInit)
			{
//				ConsoleEx.DebugWarning ("数据初始化未完成");
				return;
			}
				
			enermys.Clear ();
			enermys = WarServerManager.Instance.npcMgr.GetLifeNPCListByCamp (hostile);
			InitFinish = true;
		}

		public override TaskStatus OnUpdate()
		{
			if (!InitFinish)
			{
				InitData ();
				return TaskStatus.Running;
			}

			InitData ();

			if (enermys == null || enermys.Count == 0)
			{
				return TaskStatus.Failure;
			}

			//如果范围内有敌方敌人,
            List<ServerLifeNpc> list = AITools.GetAllNPCInRange (mTrans.position, seekRange, enermys.ToArray ());
			if (list != null && list.Count > 0)
			{
                ServerLifeNpc newTarget = null;

				//判断有没有攻击目标
                IEnumerable<ServerNPC> enemyTargets = hero.FindAtkTarget(Sight.NearSight);
                if (enemyTargets != null && enemyTargets.Count<ServerNPC> () > 0)
				{
                    newTarget = enemyTargets.ElementAt<ServerNPC> (0) as ServerLifeNpc;
				}
				else
				{

					enemyTargets = hero.FindAtkTarget ();
                    if (enemyTargets != null && enemyTargets.Count<ServerNPC> () > 0)
					{
                        newTarget = enemyTargets.ElementAt<ServerNPC> (0) as ServerLifeNpc;
					}
				}
					
				if (newTarget != null && newTarget.IsAlive)
				{
					//如果目标是建筑
					if (newTarget.data.configData.type == LifeNPCType.Build)
					{
						//如果攻击目标是基地，判断三路是否有一路已经被破了，如果有一路被破了，才能打基地
						if (newTarget.data.configData.bldType == BuildNPCType.Base)
						{
							bool canAtk = false;
							for (int i = 0; i < 3; i++)
							{
								//get all the tower on the way
                                List<ServerLifeNpc> bldWay = WarServerManager.Instance.npcMgr.GetBuildByWay (newTarget.Camp, (BATTLE_WAY) i, false);
								if (bldWay != null && bldWay.Count > 0)
								{
                                    List<ServerLifeNpc> aliveBld = WarServerManager.Instance.npcMgr.GetBuildByWay (newTarget.Camp, (BATTLE_WAY) i, true);
									if(aliveBld != null && aliveBld.Count == 0)
									{
										canAtk = true;
										break;
									}
								}
							}

							if(canAtk)
							{
								target.Value = newTarget;
								return TaskStatus.Success;
							}
						}
						//如果不是基地，判断前一个是否还活着，如果
						else
						{
							// 得到当前这一路的所有活着的建筑
                            List<ServerLifeNpc> bldWay = WarServerManager.Instance.npcMgr.GetBuildByWay (newTarget.Camp, newTarget.dataInScene.way, true);
							if (bldWay != null && bldWay.Count > 0)
							{
								//检查活着的建筑里，有没有前一个建筑
								bool canAtk = true;
								for (int i = 0; i < bldWay.Count; i++)
								{
									if (bldWay [i].dataInScene.index == newTarget.dataInScene.index - 1)
									{
										canAtk = false;
										break;
									}
								}
								if (canAtk)
								{
									target.Value = newTarget;
									return TaskStatus.Success;
								}
							}
						}
					}
					else
					{
						target.Value = newTarget;
						return TaskStatus.Success;
					}
				}
			}

			return TaskStatus.Failure;
		}

		public override void OnEnd()
		{

		}

		public override void OnReset()
		{
			InitFinish = false;
			target.Value = null;
			enermys.Clear ();
		}

		public override void OnDrawGizmos()
		{
			#if UNITY_EDITOR && DEBUG
			var oldColor = UnityEditor.Handles.color;

            if(hero != null)
			{
                UnityEditor.Handles.color = Color.cyan;
                UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.up, seekRange);

				UnityEditor.Handles.color = Color.yellow;
				UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.up, hero.data.configData.radius);

                UnityEditor.Handles.color = Color.red;
                UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.up, hero.ATKRange);
			}

			UnityEditor.Handles.color = oldColor;
			#endif
		}
	}
}
