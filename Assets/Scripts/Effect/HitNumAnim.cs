using UnityEngine;
using System.Collections;

public class HitNumAnim : MonoBehaviour {

    public bool canMove;

    private float timer;

    public float lifeTime = 1.3f;

    public float scaleUpTime = 0.15f;
    public float scaleDownTime = 0.15f;

    public Vector3 dir = Vector3.zero;

    public float speedX = 0f;
    public float aSpeedX = 0f;
    public float speedY = 0f;
    public float aSpeedY = 0f;

    private Transform tran;

	// Use this for initialization
	void Start () {
        tran = transform;
        Scale();
      
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if(timer > scaleUpTime * 2f && !canMove)
        {
            canMove = true;
        }
        if(timer > lifeTime)
        {
            Destroy(gameObject);
        }
        if(canMove)
        {
            Move();
        }
	}

    void Scale()
    {
        LeanTween.scale(gameObject, Vector3.one * 3f, scaleUpTime).setRepeat(2).setLoopPingPong().setEase(LeanTweenType.easeInOutCubic);
    }

    void Move()
    {
        speedX += (Time.deltaTime * aSpeedX);
        speedY += (Time.deltaTime * aSpeedY);
        tran.Translate(speedX * Time.deltaTime, speedY * Time.deltaTime, 0f);
    }

    void Reset()
    {
        timer = 0f;
        canMove = false;
        scaleUpTime = 0.15f;
        scaleDownTime = 0.15f;
    }
}
