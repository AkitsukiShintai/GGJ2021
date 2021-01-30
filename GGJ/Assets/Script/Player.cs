using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //吃星星回调
    public delegate void EatStarCallback(Player player, Star star);

    //吐星星回调
    public delegate void VomitStarCallback(Player player, Star star);

    public static event EatStarCallback eatStarEvents = null;

    public static event VomitStarCallback vomitStarEvents = null;

    //获得两个玩家
    private static Player[] players = null;

    //查找另外一个玩家
    public static Player FindAnotherPlayer(Player onePlayer)
    {
        if (players[0] == onePlayer)
        {
            return players[1];
        }
        else
        {
            return players[0];
        }
    }

    //人物属性, 直接通过scriptableObject配置
    public PlayerData playerData;

    private bool m_Raging = false;

    //是否狂暴中
    public bool raging
    {
        get
        {
            return m_Raging;
        }
    }

    private bool m_HaveStar = false;

    //是否拥有星星
    public bool haveStar
    {
        get
        {
            return m_HaveStar;
        }
    }

    public Star testStar;
    private List<Star> m_Stars;

    private float m_RageValue;

    private void Awake()
    {
        if (players == null)
        {
            players = new Player[2];
            players[0] = this;
        }
        else
        {
            players[1] = this;
        }
        m_Stars = new List<Star>(2);
    }

    private void Start()
    {
        m_RageValue = playerData ? playerData.rage : 0;
    }

    private void Update()
    {
        if (playerData == null)
        {
            return;
        }
        if (m_Raging && m_Stars[0])
        {
            //狂暴且有星星，降低狂暴值
            m_RageValue = Mathf.Clamp(m_RageValue - Time.deltaTime * playerData.rageDescentRate, 0, playerData.rageMax);
            TryStopRaging();
        }
        else
        {
            TryRage();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            VomitStar(false);
        }
    }

    //确认玩家可以得到这个星星后发生的事情
    private void GetStar(Star star)
    {
        m_HaveStar = true;
        m_Stars.Add(star);
        star.EatStar(gameObject);
    }

    //确认玩家可以失去这个星星后发生的事情
    private Star LoseStar()
    {
        Star losedStar = m_Stars[0];
        m_Stars.RemoveAt(0);
        if (m_Stars.Count == 0)
        {
            m_HaveStar = false;
        }
        return losedStar;
    }

    //吃星星事件触发，也就是当人碰到星星的时候触发
    private void EatStar(Star star)
    {
        //TODO：吃星星事件人物属性处理
        if (m_Raging)
        {
            if (star.state == Star.StarState.Idle)
            {
                //Idle的星星不能被狂暴人吃
                return;
            }
            else if (star.state == Star.StarState.InBody)
            {
                Debug.LogError("星星在体内为啥会被吃！");
                return;
            }
        }

        //确认完成后获得星星
        GetStar(star);
        //调用回调，GetStar之后星星和对象的关系已经确定
        if (eatStarEvents != null)
        {
            eatStarEvents.Invoke(this, star);
        }
    }

    //吐星星事件触发，也就是星星离开人的时候触发
    private void VomitStar(bool rageVomit)
    {
        //TODO：吐星星事件人物属性处理
        if (!m_HaveStar)
        {
            return;
        }

        Star star = LoseStar();
        if (!rageVomit)
        {
            star.VomitStar(FindAnotherPlayer(this).gameObject);
        }
        else
        {
            Vector3 dir = Random.onUnitSphere;
            dir.z = 0;
            dir = dir.normalized;
            dir.y = Mathf.Abs(dir.y);
            star.VomitStar(dir);
        }
        //调用回调，LoseStar之后星星和对象的关系已经确定
        if (vomitStarEvents != null)
        {
            vomitStarEvents.Invoke(this, star);
        }
    }

    private bool ShouldRage()
    {
        if (m_RageValue >= playerData.rageMax)
        {
            return true;
        }
        return false;
    }

    private void TryRage()
    {
        if (!ShouldRage() || m_Raging)
        {
            return;
        }
        //TODO:变大， 无法控制
        transform.localScale = Vector3.one * playerData.amplification;
        //吐星星
        VomitStar(true);
    }

    //尝试停止狂暴
    private void TryStopRaging()
    {
        if (m_RageValue == 0.0f && m_Raging && m_HaveStar)
        {
            //TODO:变回来
            transform.localScale = Vector3.one;

            m_Raging = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Star"))
        {
            EatStar(other.gameObject.GetComponent<Star>());
            Debug.Log(gameObject.name + "吃星星拉~");
        }
    }

    private void OnColliderEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Star"))
        {
            EatStar(other.gameObject.GetComponent<Star>());
            Debug.Log(gameObject.name + "吃星星拉~");
        }
    }
}