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

    //吃星星事件触发，也就是当人碰到星星的时候触发
    private void EatStar(Star star)
    {
        //TODO：吃星星事件人物属性处理
        star.owner = this;
        //调用回调
        if (eatStarEvents != null)
        {
            eatStarEvents.Invoke(this, star);
        }
    }

    //吐星星事件触发，也就是星星离开人的时候触发
    private void VomitStar(Star star)
    {
        //TODO：吐星星事件人物属性处理
        star.owner = null;
        //调用回调
        if (vomitStarEvents != null)
        {
            vomitStarEvents.Invoke(this, star);
        }
    }
}