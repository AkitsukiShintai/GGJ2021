using System;
using System.Collections;
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
        private float lastTouchDmgTime = 0f;

        private void Awake()
        {
            status = new MonsterStatus(cfg.initRage);
            stars = new Queue<KeyValuePair<float, Star>>(2);
            animator = gameObject.GetComponent<Animator>();
            monsterMove = new MonsterMove(transform, animator, cfg);
            monsterAttack = new MonsterAttack(transform, animator, cfg);
        }

        private void Update()
        {
            if (CheckVomit()) VomitStar();  // 吐星星
            ChangeRage();
        }

        private void FixedUpdate()
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

        public void MoveTowards(Vector3 direction, MonsterMoveType type)
        {
            status.moveType = type;
            status.moveDir = new Vector3(direction.x, 0f, 0f).normalized;
        }

        public void Jump()
        {
            var isGrounded = Physics.Raycast(transform.position + new Vector3(0f, 0.8f, 0f), Vector3.down, 0.9f);
            if (isGrounded && !monsterMove.IsJumping)
            {
                monsterMove.StartJump();
                StartCoroutine(Delay(() =>
                {
                    monsterMove.EndJump();
                }, cfg.jumpDuration));
            }
        }

        public void AttackBy(MonsterAttackType type)
        {
            if (type == MonsterAttackType.DASH)
            {
                status.attackType = type;
                StartCoroutine(Delay(() =>
                {
                    status.attackType = MonsterAttackType.IDLE;
                }, cfg.dashAttackRange / cfg.dashAttackMoveSpeed));
            }
            monsterAttack.Attack(type);
        }

        private IEnumerator Delay(Action action, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            action();
        }

        private void OnCollisionStay(Collision collision)
        {
            var playerObj = collision.gameObject;
            if (playerObj.CompareTag("Player"))
            {
                TryTouchAttack(playerObj.GetComponent<Player>());
            }
        }

        private void TryTouchAttack(Player player)
        {
            float curTime = Time.time;
            if (curTime >= lastTouchDmgTime + cfg.commonAttackTime)
            {
                lastTouchDmgTime = curTime;
                Debug.Log("BOSS 触摸 攻击" + player.name);
                player.AddRageValue(cfg.commonAttackDmg);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var playerObj = collision.gameObject;
            if (playerObj.CompareTag("Player"))
            {
                if (status.attackType == MonsterAttackType.DASH)
                {
                    Debug.Log("BOSS Dash 攻击" + playerObj.name);
                    playerObj.GetComponent<Player>().AddRageValue(cfg.dashAttackDmg);
                }
                else
                {
                    TryTouchAttack(playerObj.GetComponent<Player>());
                }
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

        private void EatStar(Star star)
        {
            star.EatStar(gameObject);
            // 记录time，等待吐出
            stars.Enqueue(new KeyValuePair<float, Star>(Time.time, star));
        }

        private void VomitStar()
        {
            Vector3 pos = GetStarDropPos();
            var star = stars.Dequeue().Value;
            star.VomitStar(pos);  // 吐出星星，告诉星星往哪里飞
        }

        private Vector3 GetStarDropPos()
        {
            return gameObject.transform.position;
        }

        private bool CheckVomit()
        {
            if (stars.Count > 0)
            {
                var getTime = stars.Peek().Key;
                return getTime - Time.time > cfg.holdStarTime;
            }
            return false;
        }

        private void ChangeRage()
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

        public float GetRage()
        {
            return status.rage;
        }

        private bool CheckDeath()
        {
            return status.rage < cfg.deadRage;
        }

        private void Dead()
        {
            // 触发一个死亡事件
            gameObject.SetActive(true);
        }

        private void OnDrawGizmos()
        {
            var commonAttackRange = new Vector3(1f, 0.5f, 1f);
            var center = transform.position;
            center.y += commonAttackRange.y / 2f;
            Gizmos.DrawWireCube(center, commonAttackRange);
            Gizmos.DrawWireSphere(transform.position, cfg.areaAttackRange);
        }
    }
}