﻿using Assets.Script;
using UnityEngine;

public enum MonsterAttackType {IDLE, COMMON, DASH, RANGE};

public enum MonsterMoveType {IDLE, WALK, RUN};


public class MonsterStatus {

    public MonsterStatus(float initRage)
    {
        rage = initRage;
    }

    public float rage;
    public MonsterAttackType attackType = MonsterAttackType.IDLE;
    public MonsterMoveType moveType = MonsterMoveType.IDLE;
}


public class Monster : MonoBehaviour
{

    public MonsterConfig cfg;
    private MonsterStatus status;


    private void Awake()
    {
        status = new MonsterStatus(cfg.initRage); 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // public方法设置MonsterStatus, Update检查并更新动画机的状态
    void Update()
    {

    }

    public void MoveTo(Vector3 target, MonsterMoveType type)
    {

    }

    public void AttackBy(MonsterAttackType type)
    {

    }

    void EatStar(Star star)
    {
        star.EatStar(gameObject);
        // 扣除狂暴值
        // 记录time，等待吐出
    }

    void VomitStar(Star star)
    {
        Vector3 pos = GetStarDropPos();
        star.VomitStar(pos);  // 吐出星星，告诉星星往哪里飞

    }
    
    Vector3 GetStarDropPos()
    {
        return gameObject.transform.position;
    }

    bool CheckDeath()
    {
        return status.rage < cfg.deadRage;
    }

    void Dead()
    {
        // 触发一个死亡事件
        // 处理销毁、死亡动画等
    }
}