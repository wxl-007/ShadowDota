using AW.War;
using AW.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using AW.Framework;
using UnityEngine;

namespace AW.War {

	public class SelectorTools {

		//取得两点间的距离
		public static float GetDistance(Vector3 from, Vector3 to)
		{
			return Vector3.Distance (from, to);
		}

		/// <summary>
		/// 获取两点间距离，如果是from背向的敌人，则默认距离 + 1F
		/// 这样子就将背向的敌人优先级降低.
		/// 只做排序（返回的不是真实的值）
		/// </summary>
		/// <returns>The virtual distance.</returns>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		public static float GetVirtualDistance(Transform from, Transform to) {
			Vector3 forward   = from.forward;
			Vector3 direction = to.position - from.position;

			bool front = Vector3.Dot(forward, direction) > 0;
			float dis = Vector3.SqrMagnitude(from.position - to.position);
			if(!front) dis += 1F;

			return dis;
		}

		//是否在范围内
		public static bool IsInRange(Vector3 from, float range, params Transform[] to)
		{
			float SqrRange = range * range;
			int len = to.Length;
			for (int i = 0; i < len; i++)
			{
				float distance = Vector3.SqrMagnitude(from - to[i].position);
				if (distance <= SqrRange)
					return true;
			}
			return false;
		}

		//是否在范围内
		public static bool IsInRange(Vector3 from, float range, params Vector3[] to)
		{
			float SqrRange = range * range;
			int len = to.Length;
			for (int i = 0; i < len; i++)
			{
				float distance = Vector3.SqrMagnitude (from - to[i]);
				if (distance <= SqrRange)
					return true;
			}
			return false;
		}

		#region 获取特定的NPC列表


		/// <summary>
		/// Gets the npc with in range.
		/// </summary>
		/// <returns>The npc with in range.</returns>
		/// <param name="anchor">起点</param>
		/// <param name="range">范围</param>
		/// <param name="NpcSource">NPC 容器</param>
		/// <param name="capm">阵营</param>
		/// <param name="kind">是否是lifeNpc</param>
		/// <param name="type">如果是LifeNpc的时候，又是什么子类型</param>
		public static List<ServerNPC> GetNpcWithInRange(ServerNPC caster, float range, WarServerNpcMgr NpcSource, CAMP camp, KindOfNPC kind, LifeNPCType type = LifeNPCType.SkTarAll) {
			IEnumerable<ServerNPC> filter = NpcSource.getCampBnpc(camp);

			if(kind == KindOfNPC.Life) {
				filter = filter.Where(n => (n.WhatKindOf() == kind) 
					&& ( type.check(((ServerLifeNpc)n).WhatTypeOf) ));
			} else if (kind == KindOfNPC.NonLife) {
				filter = filter.Where(n => n.WhatKindOf() == kind);
			}

			int count = filter.Count();
			if(count > 0) {
				Vector3 anchor = caster.transform.position;
				float radius = caster.data.configData.radius;
				filter = filter.Where( n=> IsInRange(anchor, n.data.configData.radius + range + radius, n.transform) );
			}

			return filter.ToList();
		}


		/// <summary>
		/// 获取特定的KindOfNPC,敌友方,有效状态的, 是建筑物还是什么的  活着的NPC
		/// </summary>
		/// <returns>ServerNPC 列表</returns>
		/// <param name="caster">Caster.</param>
		/// <param name="NpcSource">Npc source.</param>
		/// <param name="kind">KindOfNPC</param>
		/// <param name="target">TargetClass</param>
		/// <param name="TarStatusInCfg">NpcStatus</param>
		/// <param name="type">LifeNPCType.</param>
		public static IEnumerable<ServerNPC> GetNPCValideStatus(ServerNPC caster, WarServerNpcMgr NpcSource, KindOfNPC kind, TargetClass target, 
			NpcStatus TarStatusInCfg, LifeNPCType type = LifeNPCType.SkTarAll) {

			// 临时枚举器
			IEnumerable<ServerNPC> Itor1 = null;

			CAMP camp = CAMP.None;
			if(target.AnySame(TargetClass.Friendly)) camp = caster.Camp;
			else if(target.AnySame(TargetClass.Hostile)) camp = caster.Camp.Hostile();
			else {
				//全阵营
				camp = CAMP.All;
			}

			Itor1 = NpcSource.getCampBnpc(camp);

			if(kind == KindOfNPC.Life) {
				Itor1 = Itor1.Where(n => (n.WhatKindOf() == kind) 
					&& ( type.check(((ServerLifeNpc)n).WhatTypeOf) ));
			} else if (kind == KindOfNPC.NonLife) {
				Itor1 = Itor1.Where(n => n.WhatKindOf() == kind);
			}

			///
			/// --- 筛选出有效状态的NPC ----
			///
			Itor1 = Itor1.Where( n => { 
				ServerLifeNpc lifeTar = n as ServerLifeNpc; 
				if(lifeTar != null) {
					return !lifeTar.curStatus.AnySame(TarStatusInCfg);
				} else {
					return true;
				}
			} );

			return Itor1;
		}

		/// <summary>
		/// 半径内的NPC或者是距离最近（最远）的NPC
		/// </summary>
		public static IEnumerable<ServerNPC> GetNPCInRadius(ServerNPC caster, float range, TargetClass target, IEnumerable<ServerNPC> Itor1) {

			List<ServerNPC> found = new List<ServerNPC>();
			///
			///  枚举器，其实不用返回特定类型，返回枚举器就可以了。（在不需要Copy容器里面的数据时，就是用枚举器）
			/// 
			IEnumerable<ServerNPC> Itor = found.AsEnumerable();

			Vector3 anchor = caster.transform.position;
			Transform trans= caster.transform;

			///
			/// --- 再次根据距离条件筛选出NPC ----
			///
			float radius = caster.data.configData.radius;

			Itor1 = Itor1.Where( n => IsInRange(anchor, n.data.configData.radius + radius + range, n.transform) );

			if(target.AnySame(TargetClass.FarAwary)) {
				ServerNPC farest = Itor1.OrderByDescending( n => GetVirtualDistance (trans, n.transform) ).FirstOrDefault();
				if(farest != null) found.Add(farest);
			} else if(target.AnySame(TargetClass.Nearest)) {
				ServerNPC nearest = Itor1.OrderBy( n => GetVirtualDistance (trans, n.transform) ).FirstOrDefault();
				if(nearest != null) found.Add(nearest);
			} else {
				Itor = Itor1;
			}

			return Itor;
		}

		/// <summary>
		/// 判定最大，最小生命值的NPC
		/// </summary>
		/// <returns>The NPC by hp.</returns>
		public static IEnumerable<ServerNPC> GetNPCByHp(ServerNPC caster, TargetClass target, IEnumerable<ServerNPC> Itor1) {

			List<ServerNPC> found = new List<ServerNPC>();
			///
			///  枚举器，其实不用返回特定类型，返回枚举器就可以了。（在不需要Copy容器里面的数据时，就是用枚举器）
			/// 
			IEnumerable<ServerNPC> Itor = found.AsEnumerable();

			if(target.AnySame(TargetClass.HpLowest)) {
				ServerNPC lowest = Itor1.OrderBy( n => n.data.rtData.CurHpNested ).FirstOrDefault();
				if(lowest != null) found.Add(lowest);
			} else if (target.AnySame(TargetClass.HpHighest)) {
				ServerNPC highest = Itor1.OrderByDescending( n => n.data.rtData.CurHpNested ).FirstOrDefault();
				if(highest != null) found.Add(highest);
			} else {
				Itor = Itor1;
			}

			return Itor;
		}


		/// <summary>
		/// 返回Caster为中心，半径为radius的区域内的NPC
		/// 提供给Effect使用
		/// </summary>
		/// <returns>The NPC radius.</returns>
		/// <param name="caster">Caster.</param>
		/// <param name="range">Range.</param>
		/// <param name="NpcSource">Npc source.</param>
		public static IEnumerable<ServerNPC> GetNPCRadius(Vector3 anchor, float radius, CAMP camp, WarServerNpcMgr NpcSource) {
			IEnumerable<ServerNPC> itor = NpcSource.getCampBnpc(camp);
			itor = itor.Where( n => IsInRange(anchor, radius, n.transform) );
			return itor;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>The NPC rectangle.</returns>
		/// <param name="area">Area.</param>
		/// <param name="camp">Camp.</param>
		/// <param name="NpcSource">Npc source.</param>
		public static IEnumerable<ServerNPC> GetNPCPolygon(Polygon area, CAMP camp, WarServerNpcMgr NpcSource) {
			IEnumerable<ServerNPC> filter = NpcSource.getCampBnpc(camp);
			filter = filter.Where( n => area.PositionInPolygon (n.transform.position) );
			return filter;
		}

		#endregion

		#region 给一个阵营的NPC排序
		//排好序的数据
		private static List<List<ServerNPC>> hasPriority = new List<List<ServerNPC>>();
		//此方法仅仅对单体目标有效，多目标的选择不走优先级排序逻辑
		//且不能对自己生效和无优先级
		//排序完了之后，按照“血量或距离”筛选出合适的NPC
		public static ServerNPC GetPrioritiedNpc(ServerNPC caster, TargetClass target, WarServerNpcMgr NpcSource, float range,
			NpcStatus TarStatusInCfg, SkTargetPriority priority, SkPriorityMgr PriorityMgr, LifeNPCType type = LifeNPCType.SkTarAll) {
			///
			/// 获取未排好序的
			///
			IEnumerable<ServerNPC> unPriority = GetNPCValideStatus(caster, NpcSource, KindOfNPC.Life, target, TarStatusInCfg, type);

			///
			/// 过滤掉超出范围的数据
			///
			Vector3 anchor = caster.transform.position;
			Transform trans= caster.transform;

			float radius = caster.data.configData.radius;
			unPriority = unPriority.Where( n => IsInRange(anchor, n.data.configData.radius + radius + range, n.transform) );

			///
			/// 排序
			///
			IPrioritySelect selector = PriorityMgr.getImplement(priority);
			selector.SortByPriority(unPriority, NpcSource, hasPriority);

			///
			/// 依照排好序的顺序，依次查找出一个适合的目标
			/// 查找的方式为：血量和距离
			///
			ServerNPC theOne = null;

			bool Highest = target.AnySame(TargetClass.HpHighest);
			bool Lowest  = target.AnySame(TargetClass.HpLowest);
			bool Farest  = target.AnySame(TargetClass.FarAwary);
			bool Nearest = target.AnySame(TargetClass.Nearest);

			bool ExceptionCheck = false;
			int Condition = 0;
			if(Highest) Condition ++;
			if(Lowest)  Condition ++;
			if(Farest)  Condition ++;
			if(Nearest) Condition ++;

			#if DEBUG
			Utils.Assert(Condition >= 2, "TargetPriority shall set only one Max Or Min condition.");
			#endif

			int count = hasPriority.Count;

			if(Highest || Lowest) {
				
				for(int i = 0; i < count; ++ i) {

					List<ServerNPC> line = hasPriority[i];
					if(line != null) {
						if(Highest)
							theOne = line.OrderByDescending( n => n.data.rtData.CurHpNested ).FirstOrDefault();
						if(Lowest)
							theOne = line.OrderBy( n => n.data.rtData.CurHpNested ).FirstOrDefault();

						if(theOne != null) {
							break;
						}
					}

				}
			}

			if(Farest || Nearest) {

				for(int i = 0; i < count; ++ i) {

					List<ServerNPC> line = hasPriority[i];
					if(line != null) {
						if(Farest)
							theOne = line.OrderByDescending( n => GetVirtualDistance (trans, n.transform) ).FirstOrDefault();
						if(Nearest)
							theOne = line.OrderBy( n => GetVirtualDistance (trans, n.transform) ).FirstOrDefault();

						if(theOne != null) {
							break;
						}
					}

				}

			}

			///
			/// 这种情况是没有排序的情况，只要能选出来第一个就可以
			///
			if(Condition == 0) {
				for(int i = 0; i < count; ++ i) {

					List<ServerNPC> line = hasPriority[i];
					if(line != null && line.Count > 0) {
						theOne = line[0];
						if(theOne != null)
							break;
					}
				}
			}

			return theOne;

		}

		#endregion

		//可以选择单个目标，也可以是多个目标
		//提供给SkillSelecotr使用
		public static IEnumerable<ServerNPC> GetNPCInRange(ServerNPC caster, float range, WarServerNpcMgr NpcSource, KindOfNPC kind, 
			TargetClass target, NpcStatus TarStatusInCfg, LifeNPCType type = LifeNPCType.SkTarAll) {

			List<ServerNPC> found = new List<ServerNPC>();
			///
			///  枚举器，其实不用返回特定类型，返回枚举器就可以了。（在不需要Copy容器里面的数据时，就是用枚举器）
			/// 
			IEnumerable<ServerNPC> Itor = found.AsEnumerable();
			// 临时枚举器
			IEnumerable<ServerNPC> Itor1 = null;

			Vector3 anchor = caster.transform.position;
			Transform trans= caster.transform;

			ServerNPC nearest = null;

			CAMP camp = CAMP.None;
			if(target.AnySame(TargetClass.Friendly)) camp = caster.Camp;
			else if(target.AnySame(TargetClass.Hostile)) camp = caster.Camp.Hostile();

			Itor1 = NpcSource.getCampBnpc(camp);

			if(kind == KindOfNPC.Life) {
				Itor1 = Itor1.Where(n => (n.WhatKindOf() == kind) 
					&& ( type.check(((ServerLifeNpc)n).WhatTypeOf) ));
			} else if (kind == KindOfNPC.NonLife) {
				Itor1 = Itor1.Where(n => n.WhatKindOf() == kind);
			}

			bool any = Itor1 != null ? Itor1.Any() : false;
			if(any) {
				///
				/// --- 筛选出有效状态的NPC ----
				///
				Itor1 = Itor1.Where( n => { 
					ServerLifeNpc lifeTar = n as ServerLifeNpc; 
					if(lifeTar != null) {
						return !lifeTar.curStatus.AnySame(TarStatusInCfg);
					} else {
						return true;
					}
				} );

				///
				/// --- 再次根据距离条件筛选出NPC ----
				///
				float radius = caster.data.configData.radius;
				Itor1 = Itor1.Where( n => IsInRange(anchor, n.data.configData.radius + radius + range, n.transform) );

				if(target.AnySame(TargetClass.FarAwary)) {
					nearest = Itor1.OrderByDescending( n => GetVirtualDistance (trans, n.transform) ).FirstOrDefault();
					if(nearest != null) found.Add(nearest);
				} else if(target.AnySame(TargetClass.Nearest)) {
					nearest = Itor1.OrderBy( n => GetVirtualDistance (trans, n.transform) ).FirstOrDefault();
					if(nearest != null) found.Add(nearest);
				} else {
					Itor = Itor1;
				}


				///
				/// --- 判定最大，最小生命值的NPC -----
				///

				if(target.AnySame(TargetClass.HpLowest)) {
					ServerNPC lowest = Itor1.OrderBy( n => n.data.rtData.CurHpNested ).FirstOrDefault();
					if(lowest != null) found.Add(lowest);
				} else if (target.AnySame(TargetClass.HpHighest)) {
					ServerNPC highest = Itor1.OrderByDescending( n => n.data.rtData.CurHpNested ).FirstOrDefault();
					if(highest != null) found.Add(highest);
				}

			}

			return Itor;
		}


	}
}