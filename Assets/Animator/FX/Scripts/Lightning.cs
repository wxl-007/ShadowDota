using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour {
	public int numberOfPoint = 10;
	public float length  = 10f;
	private Transform sparksEmitter;

	public float lifeTime = 1f;
	//public Transform targetPos;
	//public Transform startPos;
	void Start()
	{
		LineRenderer lineRenderer = GetComponent<LineRenderer> ();
		lineRenderer.SetVertexCount(numberOfPoint);
		sparksEmitter = transform.Find("Sparks");
		Debug.Log (sparksEmitter);
		}

	void Update () 
	{
		LineRenderer lineRenderer = GetComponent<LineRenderer> ();
		int i = 0;
		numberOfPoint = (int)Mathf.Abs(length/2)+1;
		lineRenderer.SetVertexCount(numberOfPoint);

		Debug.Log (Time.time);
		if (sparksEmitter)
		{
			sparksEmitter.localScale = new Vector3(1, 1, length);
		}

		float interval =  length/ (numberOfPoint-1);
		float pointZ = 0;


		while ( i < numberOfPoint) 
		{  
			Vector3 pos = new Vector3 (0.5f*Random.Range(-1f, 1f), 0.5f*Random.Range(-1f, 1f), pointZ);

			lineRenderer.SetPosition (i, pos);
			i++;
			pointZ =pointZ + interval;
		}
	}
}