using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("旋转轴")]
    public Vector3 rotationAxis = new Vector3(0.0f, 1.0f, 0.0f);
    [Header("旋转速度")]
    public float rotationSpeed = 1.0f;

    void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
