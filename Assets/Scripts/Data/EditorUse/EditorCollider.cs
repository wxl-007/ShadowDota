using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EditorCollider : MonoBehaviour {
    private BoxCollider box;
    public bool isGround;
	// Use this for initialization
	void Start () {
        box = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, renderer.bounds.size);
    }
}
