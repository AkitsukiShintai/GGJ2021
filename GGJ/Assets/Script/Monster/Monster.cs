using Assets.Script;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Script.Monster
{

    public enum MonsterAttackType { IDLE, COMMON, DASH, RANGE };

    public enum MonsterMoveType { IDLE, WALK, RUN };


    public class MonsterStatus
    {
        public float rage;
        public MonsterAttackType attackType = MonsterAttackType.IDLE;
        public MonsterMoveType moveType = MonsterMoveType.IDLE;
        public Vector3 moveDir;

        public MonsterStatus(float initRage)
        {
            rage = initRage;
            moveDir = Vector3.left;
        }


    }

    public class Monster : MonoBehaviour
    {

        public MonsterConfig cfg;
        public MonsterStatus status;
        private Queue<KeyValuePair<float, Star>> stars;
        private Animator animator;
        private MonsterMove monsterMove;
        private MonsterAttack monsterAttack;

        void Awake()
        {
            status = new MonsterStatus(cfg.initRage);
            stars = new Queue<KeyValuePair<float, Star>>(2);
            animator = gameObject.GetComponent<Animator>();
            monsterMove = new MonsterMove(transform, animator);
            monsterAttack = new MonsterAttack(transform, animator, cfg.areaAttackRange);
        }

        // public方法设置MonsterStatus, Update检查并更新动画机的状态
        void Update()
        {
            if (CheckVomit()) VomitStar();  // 吐星星
            ChangeRage();
        }

        void FixedUpdate()
        {
            monsterMove.Move(status.moveDir, GetSpeed());
        }


        public float GetSpeed()
        {
            float speed;
            if (status.attackType == MonsterAttackType.DASH)
            {
                speed = cfg.dashAttackMoveSpeed;
            }
            else if (status.moveType == MonsterMoveType.IDLE)
            {
                speed = 0f;
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
            monsterAttack.Attack(type);
        }


        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {

            }
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

        void OnDrawGizmos()
        {
            var commonAttackRange = new Vector3(1.6f, 1f, 1.6f);
            var center = transform.position;
            center.y += 0.5f;
            Gizmos.DrawWireCube(center, commonAttackRange);
            Gizmos.DrawWireSphere(transform.position, cfg.areaAttackRange);

        }
    
    }

}
