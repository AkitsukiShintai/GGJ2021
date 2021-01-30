using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTargetManager : MonoBehaviour
{
    private GameObject[] m_EnemyArr;
    public List<Transform> m_inRangeEnemys;

    private void Awake()
    {
        m_inRangeEnemys = new List<Transform>();
        m_EnemyArr = GameObject.FindGameObjectsWithTag("Enemy");
    }
    private void Update()
    {
        m_inRangeEnemys.Clear();
        foreach (var item in m_EnemyArr)
        {
            if (IsInView(item.transform.position) && item.gameObject.activeSelf)
            {
                m_inRangeEnemys.Add(item.transform);
            }
        }
    }
    public bool IsInView(Vector3 worldPos)
    {
        Transform camTransform = Camera.main.transform;
        Vector2 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        Vector3 dir = (worldPos - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir);//判断物体是否在相机前面

        if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            return true;
        else
            return false;
    }
}
