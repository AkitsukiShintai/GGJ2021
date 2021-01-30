using Assets.Script;
using System.Collections.Generic;
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
    public Vector3 moveDir;
}


public class Monster : MonoBehaviour {

    public MonsterConfig cfg;
    public MonsterStatus status;
    private Queue<KeyValuePair<float, Star>> stars;
    private Animator animatior;
    private float turnPercent = 5f;


    private void Awake()
    {
        status = new MonsterStatus(cfg.initRage);
        stars = new Queue<KeyValuePair<float, Star>>(2);
        //animatior = gameObject.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // public方法设置MonsterStatus, Update检查并更新动画机的状态
    void Update()
    {
        if (CheckVomit()) VomitStar();  // 吐星星
        ChangeRage();
        Attack();

    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (transform.forward != status.moveDir)
        {
            status.moveDir = Vector3.Slerp(transform.forward, status.moveDir, Time.deltaTime * turnPercent);
            transform.rotation = Quaternion.LookRotation(status.moveDir);
        }

        if (status.moveType != MonsterMoveType.IDLE)
        {
            transform.position += Vector3.Dot(status.moveDir, Vector3.right) * Vector3.right * GetSpeed() * Time.deltaTime;
        }
    }

    void Attack()
    {

    }


    public float GetSpeed()
    {
        float speed;
        if (status.attackType == MonsterAttackType.DASH)
        {
            speed = cfg.dashAttackMoveSpeed;
        }
        else if (status.moveType == MonsterMoveType.RUN)
        {
            speed = cfg.runSpeed;
        }
        else
        {
            speed = cfg.walkSpeed;
        }
        return speed;
    }


    public void MoveTo(Vector3 target, MonsterMoveType type)
    {
        status.moveType = type;
        status.moveDir = target;
    }


    public void AttackBy(MonsterAttackType type)
    {
        status.attackType = type;
    }

    void EatStar(Star star)
    {
        star.EatStar(gameObject);
        // 记录time，等待吐出
        stars.Enqueue(new KeyValuePair<float, Star>(Time.time, star));
    }


    void VomitStar()
    {
        Vector3 pos = GetStarDropPos();
        var star = stars.Dequeue().Value;
        star.VomitStar(pos);  // 吐出星星，告诉星星往哪里飞
    }
    
    Vector3 GetStarDropPos()
    {
        return gameObject.transform.position;
    }

    bool CheckVomit()
    {
        if (stars.Count > 0)
        {
            var getTime = stars.Peek().Key;
            return getTime - Time.time > cfg.holdStarTime;
        }
        return false;
    }


    void ChangeRage()
    {
        float rageRate = 0f;
        foreach (var timeAndStar in stars)
        {
            var star = timeAndStar.Value;
            rageRate -= 10f;
        }
        status.rage += rageRate * Time.deltaTime;
        if (CheckDeath()) Dead();
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
