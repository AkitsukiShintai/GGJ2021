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
    public float jumpForce;
    private int scale = 1;
    private int starCounter = 1;
    private StarTargetManager targetManager;

    private void Awake()
    {
        targetManager = GetComponent<StarTargetManager>();
        jumpForce = playerData.jump;
        if (m_animator == null) { m_animator = GetComponent<Animator>(); }
        if (m_rigidBody == null) { m_rigidBody = GetComponent<Rigidbody>(); }
        Player.eatStarEvents += Player_eatStarEvents;
        Player.vomitStarEvents += Player_vomitStarEvents;
    }

    private void OnDestroy()
    {
        Player.eatStarEvents -= Player_eatStarEvents;
        Player.vomitStarEvents -= Player_vomitStarEvents;
    }

    /// <summary>
    /// 吐星星
    /// </summary>
    /// <param name="player"></param>
    /// <param name="star"></param>
    private void Player_vomitStarEvents(Player player, Star star)
    {
        if (player != gameObject.GetComponent<Player>()) return;
        SetScale(-(int)playerData.amplification);
        GameObject target = null;
        if (targetManager.m_inRangeEnemys.Count > 0)
        {
            int value = UnityEngine.Random.Range(0, 10);
            if (value < 5)
                target = targetManager.m_inRangeEnemys[UnityEngine.Random.Range(0, targetManager.m_inRangeEnemys.Count)].gameObject;
            else
            {
                Player otherPlayer = Player.FindAnotherPlayer(gameObject.GetComponent<Player>());
                target = otherPlayer.gameObject;
            }
        }
        else
        {
            Player otherPlayer = Player.FindAnotherPlayer(gameObject.GetComponent<Player>());
            target = otherPlayer.gameObject;
        }
        star.VomitStar(target);
    }

    /// <summary>
    /// 吃星星
    /// </summary>
    /// <param name="player"></param>
    /// <param name="star"></param>
    private void Player_eatStarEvents(Player player, Star star)
    {
        if (player != gameObject.GetComponent<Player>()) return;
        SetScale((int)playerData.amplification);
    }

    /// <summary>
    /// 设置人物变大缩小
    /// </summary>
    public void SetScale(int value)
    {
        scale += value;
        transform.localScale = Vector3.one * scale;
        if (value > 0)
            jumpForce += scale;
        else
            jumpForce -= scale + 1;
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
        Move(h);
    }

    private void Update()
    {
        if (m_IsDie) return;
        m_animator.SetBool("Grounded", m_isGrounded);
        if (Input.GetKeyDown(playerData.jumpKey) && m_isGrounded)
        {
            m_rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            m_animator.SetTrigger("Jump");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Revive();
        }
    }

    public void Move(float h)
    {
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
        m_IsDie = true;
        m_animator.SetTrigger("Die");
        StartCoroutine(Delay(() =>
        {
            gameObject.SetActive(false);
        }, 1f));
    }

    /// <summary>
    /// 复活
    /// </summary>
    public void Revive()
    {
        m_IsDie = false;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 突进
    /// </summary>
    //public void Fast()
    //{
    //    isFast = true;
    //    StartCoroutine(Delay(() =>
    //    {
    //        isFast = false;
    //        m_animator.SetFloat("MoveSpeed", 0);
    //    }, 0.1f));
    //}

    /// <summary>
    /// 受击
    /// </summary>
    //public void Hit()
    //{
    //    isHit = true;
    //    StartCoroutine(Delay(() =>
    //    {
    //        isHit = false;
    //    }, 0.05f));
    //}
    private IEnumerator Delay(Action action, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        action();

        if (Player.players[0].gameObject.activeSelf == false && Player.players[1].gameObject.activeSelf == false)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level 1");
        }
    }
}