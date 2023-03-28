using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_manager : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 randomPos;
    
    [HideInInspector] public float ShakeDistance;
    [HideInInspector] public float ShakeTime;
    [HideInInspector] public float ShakeDuration;
    [HideInInspector] public bool shakeStart;
    
    void Start() {
        startPos = transform.position;
        randomPos = startPos;
    }
    
    void Update()
    {
        if(ShakeTime < 1) ShakeTime += Time.deltaTime;
        if (ShakeTime < 0.2 && shakeStart) {
            transform.position = startPos + Random.insideUnitSphere * ShakeDistance;
        }
        else {
            transform.position = startPos;
            shakeStart = false;
        }
    }
}
