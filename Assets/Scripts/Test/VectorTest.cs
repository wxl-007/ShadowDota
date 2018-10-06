using UnityEngine;
using System.Collections;

public class VectorTest : MonoBehaviour {

    public Transform target;
    private Transform tran;

	// Use this for initialization
	void Start () {
        tran = transform;
	}
	
	// Update is called once per frame
	void Update () {
        if(target != null)
        {
            Vector3 dir = tran.position - target.transform.position;
            dir = Vector3.Project(dir, Vector3.forward);
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
            tran.rotation = rot;
        }
        tran.Translate(Vector3.forward * Time.deltaTime * 1f);
	}
}
