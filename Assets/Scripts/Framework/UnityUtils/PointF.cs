namespace AW.Framework {
	using UVec3 = UnityEngine.Vector3;
	/// <summary>
	/// 定义2D的点
	/// </summary>
	public struct PointF {
		public float X;
		public float Y;
	}

	/// <summary>
	/// 自定义的3D的点
	/// </summary>
	public struct Vec3F {
		public float X;
		public float Y;
		public float Z;
	}

	/// <summary>
	/// 写这个自定义的转换是因为：UVec3无法序列化合反序列化
	/// </summary>
	public static class UVec3Extends {

		public static Vec3F toCustomVec3(this UVec3 uvec3) {
			return new Vec3F( ) {
				X = uvec3.x,
				Y = uvec3.y,
				Z = uvec3.z,
			};
		}

		public static UVec3 toUnityVec3(this Vec3F vec3) {
			return new UVec3() {
				x = vec3.X,
				y = vec3.Y,
				z = vec3.Z,
			};
		}

	}

}
