using System;
using System.Collections.Generic;
using AW.Data;
using AW.Framework;
using UnityEngine;
using System.Linq;
using UVec3 = UnityEngine.Vector3;

namespace AW.War {

	/// <summary>
	/// 自己前面的扇形区域, 将扇形简化为菱形
	/// </summary>
	public class SelfFrontFanSelector : IEffectAreaSelector {
		#region IEffectAreaSelector implementation

		/// <summary>
		/// 扇形选择区域
		/// </summary>
		/// <param name="caster">Caster.</param>
		/// <param name="targets">Targets 不适用，，可以为Null</param>
		/// <param name="efCfg">Effect的配置</param>
		/// <param name="npcMgr">WarNpcManager</param>
		public List<ServerNPC> Select (ServerNPC caster, List<ServerNPC> targets, EffectConfigData efCfg, WarServerNpcMgr npcMgr) {

			#if DEBUG
			Utils.Assert(caster == null, "Can't find targets unless caster isn't null.");
			Utils.Assert(efCfg == null, "Can't find targets unless EffectConfigData isn't null.");
			Utils.Assert(npcMgr == null, "Can't find targets unless WarServerNpcMgr isn't null.");
			#endif

			//转换为Camp类型，还有一部分的检查不在这里
			//剔除自己的是在Factory里面（即EffectAreaSector）
			CAMP camp = efCfg.Flags.switchTo(caster.Camp);

			Transform trans = caster.transform;
			float Y     = trans.localPosition.y;
			//角度转化为弧度
			float angel = Mathf.Deg2Rad * efCfg.eParam2 * 0.5f;
			float radius= efCfg.eParam1 + caster.data.configData.radius;
			float X     = Mathf.Sin(angel) * radius;
			float Z     = Mathf.Cos(angel) * radius;

			//本地坐标
			UVec3[] localVecs = new UVec3[4];
			localVecs[0] = new UVec3() {
				x = 0F, 
				y = Y,
				z = 0F,
			};

			localVecs[1] = new UVec3() {
				x = -X, 
				y = Y,
				z = Z,
			};

			localVecs[2] = new UVec3() {
				x = 0, 
				y = Y,
				z = radius,
			};

			localVecs[3] = new UVec3() {
				x = X, 
				y = Y,
				z = Z,
			};

			//世界坐标
			UVec3[] worldVecs = new UVec3[4];
			worldVecs[0] = trans.TransformPoint(localVecs[0]);
			worldVecs[1] = trans.TransformPoint(localVecs[1]);
			worldVecs[2] = trans.TransformPoint(localVecs[2]);
			worldVecs[3] = trans.TransformPoint(localVecs[3]);


			//转换为2D坐标
			PointF[] Vecs = new PointF[4];
			Vecs[0] = new PointF() {
				X = worldVecs[0].x,
				Y = worldVecs[0].z
			};
			Vecs[1] = new PointF() {
				X = worldVecs[1].x,
				Y = worldVecs[1].z
			};
			Vecs[2] = new PointF() {
				X = worldVecs[2].x,
				Y = worldVecs[2].z
			};
			Vecs[3] = new PointF() {
				X = worldVecs[3].x,
				Y = worldVecs[3].z
			};

			Polygon polygon = new Polygon(Vecs);
			List<ServerNPC> TargetList = SelectorTools.GetNPCPolygon(polygon, camp, npcMgr).ToList();
			return TargetList;
		}

		#endregion
	}
}
