using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public enum MonsterAttackType { };

public enum MonsterMoveType { };


public class MonsterStatus {

    public float rage;
    public MonsterAttackType attackType;
    public MonsterMoveType moveType;
    public Vector3 moveTarget;

}


public class Monster : MonoBehaviour
{

    public MonsterConfig cfg;
    private MonsterStatus status;


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
        float ragePerStar = 5.0f;
        status.rage -= ragePerStar;
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
