using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EditorTrigger : MonoBehaviour {

    private BoxCollider box;
    // Use this for initialization
    void Start () {
        box = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update () {

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, renderer.bounds.size);
    }
}
