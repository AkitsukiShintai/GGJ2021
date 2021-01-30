using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    //现在星星的拥有者
    public Player owner;

    [Tooltip("星星移动速度")]
    public float moveSpeed;

    public bool moving = false;
    private Player target;

    private void Start()
    {
        moving = false;

        Player.eatStarEvents += EatStarCallBack;
        Player.vomitStarEvents += VomitStarCallBack;
    }

    // Update is called once per frame
    private void Update()
    {
        if (moving)
        {
            transform.position += (target.transform.position - transform.position).normalized * moveSpeed;
        }
    }

    private void EatStarCallBack(Player player, Star star)
    {
        if (moving)
        {
            return;
        }
        Player startPlayer = player;
        target = Player.FindAnotherPlayer(player);
        Vector3 moveDir = (target.transform.position - startPlayer.transform.position).normalized;
        transform.position = startPlayer.transform.position + moveDir * 0.5f;
        moving = true;
    }

    private void VomitStarCallBack(Player player, Star star)
    {
        moving = false;
        target = null;
    }
}