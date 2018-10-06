using UnityEngine;
using System.Collections;

public class MoveForward : MonoBehaviour {

    public Vector3 dir = Vector3.zero;
    public float speed = 3f;
    private bool canMove = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (canMove)
        {
            transform.Translate(dir * Time.deltaTime * speed);
        }
	}

    public void Move()
    {
        canMove = true;
    }

    public void DestroyMe()
    {
        canMove = false;
        Destroy(gameObject);
    }
}
