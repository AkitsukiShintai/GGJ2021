using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    Left, Right
}

public class PlayerMove : MonoBehaviour
{
    public PlayerType m_PlayerType = PlayerType.Left;
    public PlayerData playerData;

    [SerializeField] private float m_turnSpeed = 200;

    private Animator m_animator = null;
    private Rigidbody m_rigidBody = null;

    private float m_currentH = 0;
    private Vector3 m_currentDirection = Vector3.zero;

    /// <summary>
    /// 插值
    /// </summary>
    private readonly float m_interpolation = 10;

    private bool m_isGrounded;
    private bool m_IsDie;

    /// <summary>
    /// 突进
    /// </summary>
    private bool isFast = false;

    private bool isHit = false;
    private List<Collider> m_collisions = new List<Collider>();

    private void Awake()
    {
        playerData.jump = 4;
        if (m_animator == null) { m_animator = GetComponent<Animator>(); }
        if (m_rigidBody == null) { m_rigidBody = GetComponent<Rigidbody>(); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(Vector3.up, contactPoints[i].normal) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

    private void FixedUpdate()
    {
        if (m_IsDie) return;
        //if (isFast)
        //{
        //    m_animator.SetFloat("MoveSpeed", 1);
        //    transform.position += transform.forward * 20 * Time.deltaTime;
        //    Vector3 pos = transform.position; pos.z = 0; transform.position = pos;
        //    return;
        //}
        //if (isHit)
        //{
        //    transform.position -= transform.forward * 20 * Time.deltaTime;
        //    Vector3 pos = transform.position; pos.z = 0; transform.position = pos;
        //    return;
        //}
        MoveUpdate();
    }

    private void Update()
    {
        if (m_IsDie) return;
        m_animator.SetBool("Grounded", m_isGrounded);
        if (Input.GetKeyDown(playerData.jumpKey) && m_isGrounded)
        {
            m_rigidBody.AddForce(Vector3.up * playerData.jump, ForceMode.Impulse);
            m_animator.SetTrigger("Jump");
        }
        //if (Input.GetKeyDown(playerData.fastKey)) Fast();
        //if (Input.GetKeyDown(playerData.attackKey)) NormalAttack();
        //if (Input.GetKeyDown(playerData.rangeAttackKey)) { RangeAttack(); SetScale(3); }
    }

    private void MoveUpdate()
    {
        float h = 0;
        switch (m_PlayerType)
        {
            case PlayerType.Left:
                h = Input.GetAxis("LeftHorizontal");
                break;

            case PlayerType.Right:
                h = Input.GetAxis("RightHorizontal");
                break;

            default:
                break;
        }

        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = Vector3.right * m_currentH;
        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if (direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * playerData.moveSpeed * Time.deltaTime;
            Vector3 pos = transform.position; pos.z = 0; transform.position = pos;
            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }
    }

    public void Die()
    {
        gameObject.SetActive(false);
        m_IsDie = true;
        m_animator.SetTrigger("Die");
    }

    /// <summary>
    /// 设置人物变大缩小
    /// </summary>
    public void SetScale(int value)
    {
        if (transform.localScale.x == value) return;
        transform.localScale = Vector3.one * value;
        playerData.jump *= value;
    }

    /// <summary>
    /// 突进
    /// </summary>
    public void Fast()
    {
        isFast = true;
        StartCoroutine(Delay(() =>
        {
            isFast = false;
            m_animator.SetFloat("MoveSpeed", 0);
        }, 0.1f));
    }

    private void RangeAttack()
    {
        //TODO播放攻击动画，生成攻击特效
    }

    private bool CircleAttack(Transform attacked, Transform skillPosition, float radius)
    {
        float distance = Vector3.Distance(attacked.position, skillPosition.position);
        if (distance < radius)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 普通攻击
    /// </summary>
    public void NormalAttack()
    {
    }

    /// <summary>
    /// 受击
    /// </summary>
    public void Hit()
    {
        isHit = true;
        StartCoroutine(Delay(() =>
        {
            isHit = false;
        }, 0.05f));
    }

    private IEnumerator Delay(Action action, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        action();
    }
}