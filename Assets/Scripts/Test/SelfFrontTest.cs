using UnityEngine;
using UTran = UnityEngine.Transform;
using UVec3 = UnityEngine.Vector3;
using System;
using System.Collections;
using AW.Data;
using AW.War;
using AW.Framework;


namespace AW.Test {

	/// <summary>
	/// 测试NPC前方 矩形区域
	/// </summary>
	[RequireComponent(typeof(ServerNPC))]
	public class SelfFrontTest : MonoBehaviour {

		private EffectConfigData efcfg;
		private ServerNPC npc;
	
		private Polygon Select (ServerNPC caster, EffectConfigData efCfg) {

			#if DEBUG
			Utils.Assert(caster == null, "Can't find target unless caster isn't null.");
			Utils.Assert(efCfg == null, "Can't find target unless EffectConfigData isn't null.");
			#endif

			UTran trans = caster.transform;
			float Y = trans.localPosition.y;

			float halfWidth = efCfg.eParam2 * 0.5F;
			float Height    = efCfg.eParam1;

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

			return rectangle;
		}

		void Awake() {
			efcfg = new EffectConfigData() {
				eParam1 = 2.0F,
				eParam2 = 3.0F,
			};

			npc = GetComponent<ServerNPC>();
		}

		void OnDrawGizmos() {
			Polygon polygon = Select(npc, efcfg);
			float Y = transform.position.y + 0.3F;
			PointF[] points = polygon.Vects;

			int len = points.Length;
			UVec3[] vecs = new UVec3[len];
			for(int i = 0; i < len; ++ i) {
				vecs[i] = new UVec3() {
					x = points[i].X,
					y = Y,
					z = points[i].Y,
				};
			}

			Gizmos.color = Color.red;
			for(int j = 0; j < len; ++ j) {
				if(j + 1 == len){
					Gizmos.DrawLine(vecs[j], vecs[0]);
				} else {
					Gizmos.DrawLine(vecs[j], vecs[j + 1]);
				}
			}

		}

	}
}