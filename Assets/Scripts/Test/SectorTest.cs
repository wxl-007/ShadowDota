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
	/// 测试NPC前方区域 扇形区域
	/// </summary>
	[RequireComponent(typeof(ServerNPC))]
	public class SectorTest : MonoBehaviour {
		private EffectConfigData efcfg;
		private ServerNPC npc;

		void Awake() {
			efcfg = new EffectConfigData() {
				eParam1 = 3F,
				eParam2 = 90.0F,
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

			Gizmos.color = Color.yellow;
			for(int j = 0; j < len; ++ j) {
				if(j + 1 == len){
					Gizmos.DrawLine(vecs[j], vecs[0]);
				} else {
					Gizmos.DrawLine(vecs[j], vecs[j + 1]);
				}
			}
		}

		private Polygon Select (ServerNPC caster, EffectConfigData efCfg) {
			Transform trans = caster.transform;
			float Y     = trans.localPosition.y;
			//角度转化为弧度
			float angel = Mathf.Deg2Rad * efCfg.eParam2 * 0.5f;
			float radius= efCfg.eParam1;
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

			return polygon;
		}

	}
}
