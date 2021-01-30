using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public enum MonsterAttackType {COMMON, DASH, RANGE};

public enum MonsterMoveType {IDLE, WALK, RUN};


public class MonsterStatus {

    public float rage;
    public MonsterAttackType attackType;
    public MonsterMoveType moveType;
}


public class Monster : MonoBehaviour
{

    public MonsterConfig cfg;
    private MonsterStatus status = new MonsterStatus();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // public方法设置MonsterStatus, Update检查并更新动画机的状态
    void Update()
    {

    }

    public void MoveTo(Vector3 traget, MonsterMoveType type)
    {

    }

    public void AttackBy(MonsterAttackType type)
    {

    }

    void EatStar(Star star)
    {
        // 扣除狂暴值
        // 记录time，等待吐出
    }

    void VomitStar(Star star)
    {
        // 吐出星星，告诉星星往哪里飞
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
