using UnityEngine;
using System.Collections;

[System.Serializable]
public class UIRectGrid: UISceneElementBase
{
	[SerializeField]
	protected Vector3
		_size = new Vector3 (1.0f, 1.0f, 1.0f);

	public  Vector3 size {
		get{ return _size;}
		set {
			if (value == _size)
				return;
			_gridChanged = true;
			_size = Vector3.Max (value, Vector3.zero);
		}
	}
	
	public class TransformCache
	{
		public Transform _gridTransform;
		public Vector3 oldPosition;
		public Quaternion oldRotation;

		public bool needRecalculation{ get { return !(oldPosition == _gridTransform.position && oldRotation == _gridTransform.rotation); } }
		
		public void Recache ()
		{
			oldPosition = _gridTransform.position;
			oldRotation = _gridTransform.rotation;
		}

		public TransformCache (UIRectGrid grid)
		{
			_gridTransform = grid.transform;
			Recache ();
		}
	}

	protected static float RoundMultiple (float number, float multiple)
	{
		return Mathf.Round (number / multiple) * multiple;
	}

	[SerializeField]
	private bool
		_relativeSize = false;

	public bool relativeSize {
		get{ return _relativeSize;}
		set {
			if (value == _relativeSize)
				return;
			_gridChanged = true;
			_relativeSize = value;
		}
	}

	private TransformCache _transformCache;

	public TransformCache transformCache {
		get {
			if (_transformCache == null)
				_transformCache = new TransformCache (this);
			return _transformCache;
		}
	}

	protected bool _gridChanged = false;

	protected bool hasChanged {
		get {
			if (_gridChanged || transformCache.needRecalculation) {
				_gridChanged = false;
				transformCache.Recache ();
				return true;
			} else {
				return _gridChanged;
			}
		}
	}

	public bool drawOrigin = false;
	protected Vector3[][][] _drawPoints;
	[SerializeField]
	protected Vector3
		_renderFrom = Vector3.zero;

	[SerializeField]
	protected Vector3
		_renderTo = Vector3.one;
	public GFBoolVector3 hideAxis = new GFBoolVector3 ();

	public  Vector3 renderFrom 
	{
		get{ return _renderFrom;}
		set {
			if (value == _renderFrom)
				return;
			_gridChanged = true;
			_renderFrom = Vector3.Min (value, renderTo);
		}
	}
	
	public Color axisColors = Color.red;

	public void Start ()
	{

	}

	public  void DrawGrid ()
	{
		DrawGrid (-size, size);
	}

	public  void DrawGrid (Vector3 from, Vector3 to)
	{

		CalculateDrawPoints (from, to);

		for (int i = 0; i < 3; i++) 
		{
			if (hideAxis [i])
				continue;
			Gizmos.color = axisColors;

			foreach (Vector3[] line in _drawPoints[i]) 
			{
				if (line == null)
					continue;
				Gizmos.DrawLine (line [0], line [1]);
			}
		}


	}

	public  Vector3 renderTo {//
		get{ return _renderTo;}
		set {
			if (value == _renderTo)
				return;
			_gridChanged = true;
			_renderTo = Vector3.Max (value, _renderFrom);
		}
	}
	
	protected Vector3[] units {
		get {
			return new Vector3[3]{Vector3.right, Vector3.up, Vector3.forward};
		}
	}

	public enum GridPlane
	{
		YZ,
		XZ,
		XY}
	;

	[SerializeField]
	private Vector3
		_spacing = Vector3.one;

	public Vector3 spacing {
		get{ return _spacing;}
		set {
			if (value == _spacing)
				return;
			_gridChanged = true;
			_spacing = Vector3.Max (value, 0.1f * Vector3.one);
		}
	}

	public  Vector3 WorldToGrid (Vector3 worldPoint)
	{
		return gwMatrix.inverse.MultiplyPoint3x4 (worldPoint);
	}

	public  Vector3 GridToWorld (Vector3 gridPoint)
	{
		return gwMatrix.MultiplyPoint (gridPoint);
	}

	public  Vector3 NearestVertexW (Vector3 fromPoint, bool doDebug = false)
	{
		Vector3 toPoint = WorldToGrid (fromPoint);
		for (int i = 0; i<=2; i++) {
			toPoint [i] = Mathf.Round (toPoint [i]);
		}
		toPoint = GridToWorld (toPoint);
		
		if (doDebug) {
			Gizmos.DrawSphere (GridToWorld (NearestVertexG (fromPoint)), 0.3f);
		}
		return GridToWorld (NearestVertexG (fromPoint));
	}
	
	public  Vector3 NearestFaceW (Vector3 fromPoint, GridPlane thePlane, bool doDebug = false)
	{
		if (doDebug) {
			Vector3 debugCube = spacing;
			debugCube [(int)thePlane] = 0.0f;

			Matrix4x4 oldRotationMatrix = Gizmos.matrix;
			Matrix4x4 newRotationMatrix = Matrix4x4.TRS (GridToWorld (NearestFaceG (fromPoint, thePlane) + 0.5f * Vector3.one - 0.5f * units [(int)thePlane]), transform.rotation, Vector3.one);
			
			Gizmos.matrix = newRotationMatrix;
			Gizmos.DrawCube (Vector3.zero, debugCube);
			Gizmos.matrix = oldRotationMatrix;
		}
		return GridToWorld (NearestFaceG (fromPoint, thePlane) + 0.5f * Vector3.one - 0.5f * units [(int)thePlane]);
	}
	
	public  Vector3 NearestBoxW (Vector3 fromPoint, bool doDebug = false)
	{
		if (doDebug) {
			Matrix4x4 oldRotationMatrix = Gizmos.matrix;
			Matrix4x4 newRotationMatrix = Matrix4x4.TRS (GridToWorld (NearestBoxG (fromPoint) + 0.5f * Vector3.one), transform.rotation, Vector3.one);

			Gizmos.matrix = newRotationMatrix;
			Gizmos.DrawCube (Vector3.zero, spacing);
			Gizmos.matrix = oldRotationMatrix;
		}
		return GridToWorld (NearestBoxG (fromPoint) + 0.5f * Vector3.one);
	}

	public  Vector3 NearestVertexG (Vector3 fromPoint)
	{
		return RoundPoint (WorldToGrid (fromPoint));
	}

	public  Vector3 NearestBoxG (Vector3 fromPoint)
	{
		return RoundPoint (WorldToGrid (fromPoint) - 0.5f * Vector3.one);
	}

	public  Vector3 NearestFaceG (Vector3 fromPoint, GridPlane thePlane)
	{
		return RoundPoint (WorldToGrid (fromPoint) - 0.5f * Vector3.one + 0.5f * units [(int)thePlane]);
	}

	public  Vector3 ReadVertexMatrix (int x, int y, int z, Vector3[,,] vertexMatrix, bool warning = false)
	{
		if (Mathf.Abs (x) > vertexMatrix.GetUpperBound (0) / 2 || Mathf.Abs (y) > vertexMatrix.GetUpperBound (1) / 2 || Mathf.Abs (z) > vertexMatrix.GetUpperBound (2) / 2) 
		{
			if (warning)
				Debug.LogWarning ("coordinates too large for this matrix, will default to " + Vector3.zero);
			return vertexMatrix [(vertexMatrix.GetUpperBound (0) / 2), (vertexMatrix.GetUpperBound (1) / 2), (vertexMatrix.GetUpperBound (2) / 2)];
		}
		return vertexMatrix [(vertexMatrix.GetUpperBound (0) / 2) - x, (vertexMatrix.GetUpperBound (1) / 2) - y, (vertexMatrix.GetUpperBound (2) / 2) - z];
	}



	bool useCustomRenderRange = true;

	public void OnDrawGizmos ()
	{
		if (useCustomRenderRange) 
		{
			DrawGrid (renderFrom, renderTo);
		} 
		else 
		{
			DrawGrid ();
		}
	}

	protected  Vector3[][][] CalculateDrawPoints (Vector3 from, Vector3 to)
	{
		if (!hasChanged && _drawPoints != null && from == renderFrom && to == renderTo) {
			return _drawPoints;
		}
		_drawPoints = new Vector3[3][][];
		
		Vector3 relFrom = relativeSize ? Vector3.Scale (from, spacing) : from; 
		Vector3 relTo =  relativeSize ? Vector3.Scale (to, spacing) : to;
//		relFrom = new Vector3(-0.5f,-0.5f,-0.5f);
//		relTo = new Vector3(0.5f,0.5f,0.5f);
//		relFrom = new Vector3(-1f,-1f,-1f);
//		relTo = new Vector3(0,0,0);
		float[] length = new float[3];
		for (int i = 0; i < 3; i++) 
		{
			length [i] = relTo [i] - relFrom [i];
		}

		int[] amount = new int[3];
		for (int i = 0; i < 3; i++) 
		{
			amount [i] = Mathf.FloorToInt (relTo [i] / spacing [i]) - Mathf.CeilToInt (relFrom [i] / spacing [i]) + 1;
		}

		Vector3[] startPoint = new Vector3[3]
		{
			transform.GFTransformPointFixed (new Vector3 (relTo.x, spacing.y * Mathf.Floor (relTo.y / spacing.y), spacing.z * Mathf.Floor (relTo.z / spacing.z))),
			transform.GFTransformPointFixed (new Vector3 (spacing.x * Mathf.Floor (relTo.x / spacing.x), relTo.y, spacing.z * Mathf.Floor (relTo.z / spacing.z))),
			transform.GFTransformPointFixed (new Vector3 (spacing.x * Mathf.Floor (relTo.x / spacing.x), spacing.y * Mathf.Floor (relTo.y / spacing.y), relTo.z))
		};

		Vector3[] endDirection = new Vector3[3]
		{
			transform.TransformDirection (new Vector3 (-Mathf.Abs (relTo.x - relFrom.x), 0.0f, 0.0f)),
			transform.TransformDirection (new Vector3 (0.0f, -Mathf.Abs (relTo.y - relFrom.y), 0.0f)),
			transform.TransformDirection (new Vector3 (0.0f, 0.0f, -Mathf.Abs (relTo.z - relFrom.z)))
		};

		Vector3[] iterationVector = new Vector3[3]
		{
			transform.TransformDirection (new Vector3 (-spacing.x, 0.0f, 0.0f)),
			transform.TransformDirection (new Vector3 (0.0f, -spacing.y, 0.0f)),
			transform.TransformDirection (new Vector3 (0.0f, 0.0f, -spacing.z))
		};

		for (int i = 0; i < 3; i++) 
		{			
			int idx1 = ((i + 1) % 3);
			int idx2 = ((i + 2) % 3);
			int iterator = 0;
			
			Vector3[][] lineSet = new Vector3[amount [idx1] * amount [idx2]][];
			
			if (relTo [i] - relFrom [i] <= 0.01f) 
			{
				lineSet = new Vector3[0][];
			}
			else 
			{
				for (int j = 0; j < amount[idx1]; ++j) 
				{
					for (int k = 0; k < amount[idx2]; ++k) 
					{
						Vector3[] line = new Vector3[2];
						line [0] = startPoint [i] + j * iterationVector [idx1] + k * iterationVector [idx2];
						line [1] = line [0] + endDirection [i];
						lineSet [iterator] = line;
						iterator++;
					}
				}
			}
			_drawPoints [i] = lineSet;
		}

		if (from != renderFrom || to != renderTo)
			_gridChanged = true;
		return _drawPoints;
	}

	private Matrix4x4 gwMatrix 
	{
		get 
		{
			Matrix4x4 _gwMatrix = new Matrix4x4 ();
			_gwMatrix.SetColumn (0, transform.right * spacing.x);
			_gwMatrix.SetColumn (1, transform.up * spacing.y);
			_gwMatrix.SetColumn (2, transform.forward * spacing.z);
			_gwMatrix.SetColumn (3, transform.position);
			_gwMatrix [15] = 1;

			return _gwMatrix;
		}
	}

	private Vector3 RoundPoint (Vector3 point)
	{
		return RoundPoint (point, Vector3.one);
	}

	private Vector3 RoundPoint (Vector3 point, Vector3 multi)
	{
		for (int i = 0; i < 3; i++) {
			point [i] = RoundMultiple (point [i], multi [i]);
		}
		return point;
	}

	//更新数据
	public override void UpdateData()
	{
		settingData = null;
	}
}