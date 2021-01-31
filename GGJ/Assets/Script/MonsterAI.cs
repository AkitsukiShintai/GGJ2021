using UnityEngine;
using MonsterAIData;
using Assets.Script.Monster;

public class MonsterAI : MonoBehaviour
{
    Monster m_Monster;
    MAIActionData action;
    MAIMonsterData monster;
    MAIPlayerData[] players;
    public Transform[] stairs;
    public bool isActive;
    Vector3 size;

    // Start is called before the first frame update
    void Start()
    {
        m_Monster = GetComponent<Monster>();
        Player p;
        players = new MAIPlayerData[] {
            new MAIPlayerData(p=Player.FindAnotherPlayer(null)),
            new MAIPlayerData(Player.FindAnotherPlayer(p))
        };
        size = Vector3.Scale(GetComponent<BoxCollider>().size, transform.localScale);
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            Percept();
            Think();
            Act();
        }
    }

    void Percept()
    {
        monster.noBig = true;
        float dist, minDist = float.MaxValue;
        for (int i = 0; i < players.Length; i++)
        {
            ref var p = ref players[i];
            if (p.IsBig)
            {
                monster.noBig = false;
            }
            p.dist = (transform.position - p.Pos).magnitude;
            if (p.IsAlive)
            {
                dist = p.dist * (p.IsBig ? 0.95f : 1);
                if (dist < minDist)
                {
                    minDist = dist;
                    monster.targetIndex = i;
                }
            }
        }
    }

    void Think()
    {
        ref var cfg = ref m_Monster.cfg;
        ref var p = ref players[monster.targetIndex];
        action.moveType = MonsterMoveType.IDLE;
        action.targetPos = p.Pos;
        if (monster.noBig
            && Time.time - monster.lastAreaAttackTime > cfg.areaAttackCoolDown
            && p.dist < cfg.areaAttackRange)
        {
            action.attackType = MonsterAttackType.RANGE;
            monster.lastAreaAttackTime = Time.time;
            action.moveType = MonsterMoveType.WALK;
        }
        else if (p.IsBig
            && Time.time - monster.lastDashAttackTime >= cfg.dashAttackCoolDown
            && p.dist < cfg.dashAttackRange)
        {
            action.attackType = MonsterAttackType.DASH;
            monster.lastDashAttackTime = Time.time;
        }
        else if (Time.time - monster.lastCommonAttackTime >= cfg.commonAttackTime
            && p.dist < cfg.commonAttackRange)
        {
            action.attackType = MonsterAttackType.COMMON;
            monster.lastCommonAttackTime = Time.time;
        }
        else
        {
            action.attackType = MonsterAttackType.IDLE;
            action.moveType = MonsterMoveType.RUN;
        }
    }

    void Act()
    {
        ref var cfg = ref m_Monster.cfg;
        var dir = action.targetPos - transform.position;
        bool jump = false;
        if (dir.y > size.y / 2)
        {
            jump = true;
        }
        else
        {
            foreach (var stair in stairs)
            {
                var v = stair.position - transform.position;
                if (Mathf.Abs(v.x) < size.x && 0 < v.y && v.y < size.y * 2)
                {
                    jump = false;
                    break;
                }
                float speed;
                switch (action.moveType)
                {
                    case MonsterMoveType.IDLE:
                        speed = 0;
                        break;
                    case MonsterMoveType.WALK:
                        speed = cfg.walkSpeed;
                        break;
                    case MonsterMoveType.RUN:
                        speed = cfg.runSpeed;
                        break;
                    default:
                        throw new System.Exception();
                };
                if (Vector3.Dot(dir, v) > 0 && speed * cfg.jumpDuration > Mathf.Abs(v.x))// && v.y < size.y)
                {
                    jump = true;
                }
            }
        }
        if (jump)
        {
            m_Monster.Jump();
        }
        dir.y = 0;
        m_Monster.MoveTowards(dir.normalized, action.moveType);
        if (m_Monster.status.attackType == MonsterAttackType.IDLE)
        {
            m_Monster.AttackBy(action.attackType);
        }
    }
}
