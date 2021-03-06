﻿using Assets.Script.Monster;
using UnityEngine;

public class MonsterTest : MonoBehaviour
{

    public Vector3 target;
    public MonsterMoveType moveType;
    public MonsterAttackType attackType;

    // Update is called once per frame
    void Update()
    {
        var monster = gameObject.GetComponent<Monster>();
        monster.MoveTowards(target, moveType);
        if (target.y > 0f)
        {
            monster.Jump();
        }
        monster.AttackBy(attackType);
    }
}
