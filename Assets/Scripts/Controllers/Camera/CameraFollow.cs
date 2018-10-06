using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    /// <summary>
    /// Transform
    /// </summary>
    private Transform _tran;

    /// <summary>
    /// The smooth speed.
    /// </summary>
    public float smoothSpeed = 50f;

    /// <summary>
    /// The x dis from target.
    /// </summary>
    private float xDisFromTarget = 0;
    /// <summary>
    /// The y dis from target.
    /// </summary>
    public float yDisFromTarget = 11;
    /// <summary>
    /// The z dis from target.
    /// </summary>
	public float zDisFromTarget = 9;
    /// <summary>
    /// 摄像机的欧拉角
    /// </summary>
    public Vector3 eulerAng = Vector3.zero;

    /// <summary>
    /// 目标
    /// </summary>
    public Transform Target;
    /// <summary>
    /// 设定摄像机跟随目标
    /// </summary>
    /// <param name="target">目标.</param>
    /// <param name="needSmooth">为true是平滑移动到目标，否则直接制定坐标</param>
    public void SetFollowTarget(Transform target, bool needSmooth = true)
    {
        if(Target != target)
        {
            Target = target;
            if (needSmooth)
            {
                SmoothMove();
            }
            else
            {
                Vector3 nextPos = _tran.position;
                nextPos.x = Target.position.x - xDisFromTarget;
                nextPos.y = yDisFromTarget;
                nextPos.z = Target.position.z - zDisFromTarget;
                _tran.position = nextPos;
            }
        }
    }
    /// <summary>
    /// 设定相机目标
    /// </summary>
    /// <value>The set target.</value>
    public Transform SetTarget
    {
        set
        { 
            if(Target != value)
            {
                Target = value;
                SmoothMove();
            }
        }
    }

    /// <summary>
    /// 是否在平滑移动中
    /// </summary>
    private bool isSmoothing = false;

	// Use this for initialization
	void Start () {
        _tran = transform;
        _tran.eulerAngles = eulerAng;
	}
	
	// Update is called once per frame
	void Update () {
        if(Target != null && !isSmoothing)
        {
            Vector3 nextPos = _tran.position;
            nextPos.x = Target.position.x - xDisFromTarget;
            nextPos.y = _tran.position.y;
            nextPos.z = Target.position.z - zDisFromTarget;
            _tran.position = nextPos;
        }
	}

    /// <summary>
    /// 平滑移动至目标
    /// </summary>
    public void SmoothMove()
    {
        if(!isSmoothing)
        {
            isSmoothing = true;
            if (Target != null)
            {
                float dis = Vector3.Distance(_tran.position, Target.position);
                float t = dis / smoothSpeed;
                Vector3 nextPos = Vector3.zero;
                nextPos.x = Target.position.x - xDisFromTarget;
                nextPos.y = _tran.position.y;
                nextPos.z = Target.position.z - zDisFromTarget;
                LeanTween.move(gameObject, nextPos, t).setOnComplete(OnSmoothEnd).setEase(LeanTweenType.linear);
            }
            else
            {
                isSmoothing = false;
            }
        }
    }

    /// <summary>
    /// 平滑结束
    /// </summary>
    public void OnSmoothEnd()
    {
        isSmoothing = false;
    }
}
