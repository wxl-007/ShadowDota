using System;
using System.Collections.Generic;
using AW.Data;
using AW.Framework;
using UTran = UnityEngine.Transform;
using UVec3 = UnityEngine.Vector3;
using System.Linq;

namespace AW.War {
	/// <summary>
	/// 自己前方的矩形范围
	/// 这种类型的区域都完全按照Effect的索敌范围（不会再有Skill的索敌）
	/// </summary>
	public class SelfFrontRectangleSelector : IEffectAreaSelector {
		#region IEffectAreaSelector implementation

		/// <param name="targets">没用的</param>
		public List<ServerNPC> Select (ServerNPC caster, List<ServerNPC> targets, EffectConfigData efCfg, WarServerNpcMgr npcMgr) {

			#if DEBUG
			Utils.Assert(caster == null, "Can't find target unless caster isn't null.");
			Utils.Assert(efCfg == null, "Can't find target unless EffectConfigData isn't null.");
			Utils.Assert(npcMgr == null, "Can't find target unless WarServerNpcMgr isn't null.");
			#endif

			//转换为Camp类型，还有一部分的检查不在这里
			//剔除自己的是在Factory里面（即EffectAreaSector）
			CAMP camp = efCfg.Flags.switchTo(caster.Camp);

			UTran trans = caster.transform;
			float Y = trans.localPosition.y;

			float halfWidth = efCfg.eParam2 * 0.5F + caster.data.configData.radius;
			float Height    = efCfg.eParam1 + caster.data.configData.radius;

			UVec3[] localVecs = new UVec3[4];
			localVecs[0] = new UVec3(-halfWidth, Y, 0f);
			localVecs[1] = new UVec3(-halfWidth, Y, Height);
			localVecs[2] = new UVec3(halfWidth, Y, Height);
			localVecs[3] = new UVec3(halfWidth, Y, 0f);


			UVec3[] WorldVecs = new UVec3[4];
			WorldVecs[0] = trans.TransformPoint(localVecs[0]);
			WorldVecs[1] = trans.TransformPoint(localVecs[1]);
			WorldVecs[2] = trans.TransformPoint(localVecs[2]);
			WorldVecs[3] = trans.TransformPoint(localVecs[3]);

			//caster.transform.forward;
			PointF[] vecs = new PointF[4];

			vecs[0] = new PointF() {
				X = WorldVecs[0].x,
				Y = WorldVecs[0].z,
			};

			vecs[1] = new PointF() {
				X = WorldVecs[1].x,
				Y = WorldVecs[1].z,
			};

			vecs[2] = new PointF() {
				X = WorldVecs[2].x,
				Y = WorldVecs[2].z,
			};

			vecs[3] = new PointF() {
				X = WorldVecs[3].x,
				Y = WorldVecs[3].z,
			};

			Polygon rectangle = new Polygon(vecs);

			List<ServerNPC> TargetList = SelectorTools.GetNPCPolygon(rectangle, camp, npcMgr).ToList();
			return TargetList;
		}

		#endregion
	}
}
