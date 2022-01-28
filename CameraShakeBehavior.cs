using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeBehavior : MonoBehaviour
{

    // Transform du gameobject à shaker
    private Transform transform;
    // Duration du shake
    private static float shakeDuration = 0f;
    // Mesure permettant de controler l'intensité du shake
    private static float shakeMagnitude;
    // Vitesse de fade du Shake
    private float dampingSpeed = 2.25f;
    Vector3 initialPosition;



   
    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }

    void Awake()
    {
        if (transform == null)
        {
            transform = GetComponent(typeof(Transform)) as Transform;
        }


    }

    void OnEnable()
    {
        initialPosition = transform.localPosition;
    }


    public static void TriggerCameraShake(float shakeTime, float shakeForce)
    {
        shakeDuration = shakeTime;
        shakeMagnitude = shakeForce;

    }
}
