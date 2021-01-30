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
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    //确认玩家可以得到这个星星后发生的事情
    private void GetStar(Star star)
    {
        star.PlayerEatStar(this);
        m_HaveStar = true;
    }

    //确认玩家可以失去这个星星后发生的事情
    private void LoseStar(Star star)
    {
        star.PlayerVomitStar(this);
        m_HaveStar = false;
    }

    //吃星星事件触发，也就是当人碰到星星的时候触发
    private void EatStar(Star star)
    {
        if (m_HaveStar)
        {
            Debug.LogError(gameObject.name + "我已经有星星了为啥还要吃！");
            return;
        }
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
    private void VomitStar(Star star)
    {
        //TODO：吐星星事件人物属性处理
        if (!m_HaveStar)
        {
            return;
        }

        LoseStar(star);

        //调用回调，LoseStar之后星星和对象的关系已经确定
        if (vomitStarEvents != null)
        {
            vomitStarEvents.Invoke(this, star);
        }
    }

    private bool ShouldRage()
    {
        if (playerData.rage >= playerData.rageMax)
        {
            return true;
        }
        return false;
    }

    private void Rage()
    {
        if (!ShouldRage() || m_Raging)
        {
            return;
        }
        //TODO:变大， 无法控制
        transform.localScale = Vector3.one * playerData.amplification;
    }
}