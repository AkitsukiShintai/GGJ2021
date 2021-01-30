using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "人物属性.asset", menuName = "GGJ/人物属性")]
public class PlayerData : ScriptableObject
{
    [Tooltip("暴躁值")]
    public float rage;

    [Tooltip("暴躁值下降速率")]
    public float rageDescentRate;

    [Tooltip("暴躁值峰值")]
    public float rageMax;

    [Tooltip("移速")]
    public float moveSpeed;

    [Tooltip("跳跃力")]
    public float jump;

    [Tooltip("变大值")]
    public float amplification;

    [Tooltip("视野值")]
    public float vision;

    [Tooltip("视野叠加触发值")]
    public float visionOverlap;

    [Tooltip("视野扩大值")]
    public float visionAmplification;

    [Tooltip("跳跃按键")]
    public KeyCode jumpKey;

    [Tooltip("突进按键")]
    public KeyCode fastKey;

    [Tooltip("范围攻击按键")]
    public KeyCode rangeAttackKey;

    [Tooltip("普通攻击案件")]
    public KeyCode attackKey;
}