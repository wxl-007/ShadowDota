using UVec3 = UnityEngine.Vector3;

namespace AW.Framework {
	/// <summary>
	///      Represents a geometric polygon made up of any number of sides, defined by <see cref="PointF"/> structures
	///      between those points.
	///      按照逆时针的顺序把点设定好
	/// </summary>
	public class Polygon
	{
		private readonly PointF[] _vertices;
		public PointF[] Vects {
			get {
				return _vertices;
			}
		}

		/// <summary>
		///     Creates a new instance of the <see cref="Polygon"/> class with the specified vertices.
		/// </summary>
		/// <param name="vertices">
		///     An array of <see cref="PointF"/> structures representing the points between the sides of the polygon.
		/// </param>
		public Polygon(PointF[] vertices)
		{
			_vertices = vertices;
		}

		/// <summary>
		///     Determines if the specified <see cref="PointF"/> if within this polygon.
		/// </summary>
		/// <remarks>
		///     This algorithm is extremely fast, which makes it appropriate for use in brute force algorithms.
		/// </remarks>
		/// <param name="point">
		///     The point containing the x,y coordinates to check.
		/// </param>
		/// <returns>
		///     <c>true</c> if the point is within the polygon, otherwise <c>false</c>
		/// </returns>
		public bool PointInPolygon(PointF point) {
			int len = _vertices.Length;
			var j = len - 1;
			var oddNodes = false;

			for (var i = 0; i < len; i++)
			{
				if (_vertices[i].Y < point.Y && _vertices[j].Y >= point.Y ||
					_vertices[j].Y < point.Y && _vertices[i].Y >= point.Y)
				{
					if (_vertices[i].X +
						(point.Y - _vertices[i].Y)/(_vertices[j].Y - _vertices[i].Y)*(_vertices[j].X - _vertices[i].X) < point.X)
					{
						oddNodes = !oddNodes;
					}
				}
				j = i;
			}

			return oddNodes;
		}

		/// <summary>
		/// 忽略Y axis
		/// </summary>
		/// <returns><c>true</c>, if in polygon was positioned, <c>false</c> otherwise.</returns>
		/// <param name="pos">Position.</param>
		public bool PositionInPolygon (UVec3 pos) {
			PointF p = new PointF() {
				X = pos.x,
				Y = pos.z,
			};
			return PointInPolygon(p);
		}

	}
}

