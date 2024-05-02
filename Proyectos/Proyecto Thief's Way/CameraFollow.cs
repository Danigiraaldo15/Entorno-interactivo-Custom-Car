using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing = 5f;
    public static CameraFollow Instance;
    Vector3 offset;

    bool follow = false;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void StartFollow()
    {
        offset = new Vector3(0, 5, -5);
        follow = true;
    }

    void Update()
    {
        if (target && follow)
        {
            Vector3 targetCameraPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetCameraPos, smoothing * Time.deltaTime);
        }
    }
}
