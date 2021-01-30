using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum StepType
{
    Move, Hide
}
/// <summary>
/// 台阶脚本
/// </summary>
public class Steps : MonoBehaviour
{
    public StepType m_StepType = StepType.Hide;
    public Vector3 m_StartPos;
    public Vector3 m_EndPos;
    public float m_MoveSpeed;
    public float m_DealyHideTime;

    private bool isStartHide = false;

    private void Awake()
    {
        if (m_StepType == StepType.Move)
        {
            transform.localPosition = m_StartPos;
        }
    }
    private void Update()
    {
        if (m_StepType == StepType.Move)
        {
            float distance = Mathf.PingPong(Time.time * m_MoveSpeed, Vector3.Distance(m_StartPos, m_EndPos));
            transform.localPosition = new Vector3(distance, transform.position.y, transform.position.z);
        }
        else
        {
            if (isStartHide)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 6);
                if (transform.localScale.x < 0.01f || transform.localScale.y < 0.01f || transform.localScale.z < 0.01f)
                {
                    transform.localScale = Vector3.zero;
                    isStartHide = false;
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (m_StepType == StepType.Hide && collision.collider.CompareTag("Player"))
        {
            ContactPoint[] contactPoints = collision.contacts;
            for (int i = 0; i < contactPoints.Length; i++)
            {
                if (Vector3.Dot(contactPoints[i].normal, Vector3.up) < -0.5f)
                {
                    StartCoroutine(Delay(() =>
                    {
                        isStartHide = true;
                    }, m_DealyHideTime));
                }
            }
        }
    }
    private IEnumerator Delay(Action action, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        action();
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Steps))]
public class StepsEditor : Editor
{
    private SerializedObject obj;
    private SerializedProperty startPos;
    private SerializedProperty endPos;
    private SerializedProperty moveSpeed;
    private SerializedProperty dealyHideTime;

    void OnEnable()
    {
        obj = new SerializedObject(target);
        startPos = obj.FindProperty("m_StartPos");
        endPos = obj.FindProperty("m_EndPos");
        moveSpeed = obj.FindProperty("m_MoveSpeed");
        dealyHideTime = obj.FindProperty("m_DealyHideTime"); 
    }
    public override void OnInspectorGUI()
    {
        Steps step = (Steps)target;
        step.m_StepType = (StepType)EditorGUILayout.EnumPopup("台阶类型", step.m_StepType);
        if (step.m_StepType == StepType.Move)
        {
            step.m_MoveSpeed = moveSpeed.floatValue;
            EditorGUILayout.PropertyField(startPos);
            EditorGUILayout.PropertyField(endPos);
            EditorGUILayout.PropertyField(moveSpeed);
            if (GUILayout.Button("记录起始点"))
            {
                startPos.vector3Value = step.transform.localPosition;
                step.m_StartPos = step.transform.localPosition;
            }
            if (GUILayout.Button("记录结束点"))
            {
                endPos.vector3Value = step.transform.localPosition;
                step.m_EndPos = step.transform.localPosition;
            }
        }
        else
        {
            EditorGUILayout.PropertyField(dealyHideTime);
            step.m_DealyHideTime = dealyHideTime.floatValue;
        }
    }
}
#endif