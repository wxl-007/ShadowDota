using System;

namespace AW.Test {
	using NUnit.Framework;
	using AW.Framework;
	using UnityEngine;

	/// <summary>
	/// 判定一个点是否在 “多边形” 的内部
	/// </summary>

	[TestFixture]
	public class InAreaTest {
		Polygon polygon;

		[SetUp]
		public void Init() {
			PointF[] vecs = new PointF[8];

			vecs[0] = new PointF() {
				X = 0, Y = 0
			};

			vecs[1] = new PointF() {
				X = 0, Y = 1
			};

			vecs[2] = new PointF() {
				X = 0.3F, Y = 1.7F
			};

			vecs[3] = new PointF() {
				X = 0.5F, Y = 1.8F
			};

			vecs[4] = new PointF() {
				X = 0.95F, Y = 1.2F
			};

			vecs[5] = new PointF() {
				X = 1.1F, Y = 0.9F
			};

			vecs[6] = new PointF() {
				X = 0.75F, Y = 0.5F
			};

			vecs[7] = new PointF() {
				X = 0.43F, Y = 0.17F
			};

			polygon = new Polygon(vecs);


		}

		[Test]
		[Category("Failing Tests")]
		public void OutArea() {
			Vector3 point = new Vector3(0.8F, 0F, 1.8F);
			Assert.IsTrue(polygon.PositionInPolygon(point));
		}


		[Test]
		public void InArea() {
			Vector3 point = new Vector3(0.8F, 0F, 1.2F);
			Assert.IsTrue(polygon.PositionInPolygon(point));
		}

		[Test]
		[Category("Failing Tests")]
		public void OnAreaFail() {
			Vector3 point = new Vector3(0.5F, 0F, 1.8F);
			for(int i = 0; i < 1000; ++ i) {
				Assert.IsTrue(polygon.PositionInPolygon(point));
			}
		}

		[Test]
		public void OnAreaOk() {
			Vector3 point = new Vector3(0.5F, 0F, 1.7999F);
			for(int i = 0; i < 1000; ++ i) {
				Assert.IsTrue(polygon.PositionInPolygon(point));
			}
		}
	}

}
