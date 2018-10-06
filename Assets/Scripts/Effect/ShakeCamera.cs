using UnityEngine;
using System.Collections;

public class ShakeCamera : MonoBehaviour {
    private Vector3 originPosition;
    private Quaternion originRotation;
    public float shake_decay;
    public float shake_intensity;
    private bool isShaking;

    private float originFieldOfView;

    void Start()
    {
        originRotation = transform.rotation;
        originFieldOfView = camera.fieldOfView;
    }

    void Update (){
        if (shake_intensity > 0)
        {
            transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
            transform.rotation = new Quaternion(
                originRotation.x + Random.Range(-shake_intensity, shake_intensity) * .2f,
                originRotation.y + Random.Range(-shake_intensity, shake_intensity) * .2f,
                originRotation.z,
                originRotation.w);
            shake_intensity -= shake_decay;
        }
        else
        {
            if(isShaking)
            {
                isShaking = false;
                transform.rotation = originRotation;
            }
        }
    }

    public void Shake(float inten = 0.1f, float decay = 0.01f){
        originPosition = transform.position;
        shake_intensity = inten;
        shake_decay = decay;
        isShaking = true;
    }

    public void ShakeNearAndFar()
    {
        camera.fieldOfView = originFieldOfView;
        LeanTween.value(gameObject, originFieldOfView + 2f, originFieldOfView, 0.15f)
            .setOnUpdate(OnUpdate)
            .setRepeat(2)
            .setLoopPingPong()
            .setOnComplete(delegate() {
                camera.fieldOfView = originFieldOfView;
            });
    }

    void OnUpdate(float val1)
    {
        camera.fieldOfView = val1;
    }
}