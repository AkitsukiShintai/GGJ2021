using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "光源属性.asset", menuName = "GGJ/光源属性")]
public class LightData : ScriptableObject
{
    [Tooltip("视野大小")]
    public float range = 20.0f;

    [Tooltip("视野亮度")]
    public float intensity = 1.0f;

    [Tooltip("消失时间")]
    public float duration = 0.0f;
}
