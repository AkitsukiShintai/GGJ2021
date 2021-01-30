using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public enum StarState
    {
        /// <summary>
        /// 如果星星是自己飞出来的那么处于idle状态(狂暴的人或者怪物扔出来)
        /// </summary>
        Idle,

        /// <summary>
        /// 正常人扔出来处于飞行状态，给另一个对象
        /// </summary>
        Flying,

        /// <summary>
        /// 被某个对象持有
        /// </summary>
        InBody
    }

    //现在星星的拥有者
    public GameObject owner;

    [Tooltip("星星移动速度")]
    public float moveSpeed;

    private StarState m_State = StarState.Idle;

    public StarState state
    {
        get
        {
            return m_State;
        }
        set
        {
            m_State = value;
        }
    }

    private Transform target;

    private void Start()
    {
        m_State = StarState.Idle;
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_State == StarState.Flying)
        {
            transform.position += (target.transform.position - transform.position).normalized * moveSpeed;
        }
    }

    /// <summary>
    /// 玩家吃星星
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="star">The star.</param>
    public void EatStar(GameObject player)
    {
        if (m_State == StarState.InBody)
        {
            return;
        }
        owner = player.gameObject;
        target = null;
        m_State = StarState.InBody;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 玩家吐星星
    /// </summary>
    public void VomitStar(GameObject target)
    {
        if (m_State != StarState.InBody)
        {
            return;
        }
        gameObject.SetActive(true);
        this.target = target.transform;
        Vector3 moveDir = (target.transform.position - owner.transform.position).normalized;
        transform.position = owner.transform.position + moveDir * 0.5f;
        m_State = StarState.Flying;
        owner = null;
    }

    public void VomitStar(Vector3 position)
    {
        if (m_State != StarState.InBody)
        {
            return;
        }
        m_State = StarState.Idle;
        owner = null;
    }
}