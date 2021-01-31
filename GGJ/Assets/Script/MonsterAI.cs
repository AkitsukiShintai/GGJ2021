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
        monster.targetIndex = -1;
        float dist, minDist = float.MaxValue;
        for (int i = 0; i < players.Length; i++)
        {
            ref var p = ref players[i];
            if (!p.IsAlive)
                continue;
            if (p.IsBig)
                monster.noBig = false;
            p.dist = Mathf.Abs((transform.position - p.Pos).x);
            dist = p.dist * (p.IsBig ? 0.95f : 1);
            if (dist < minDist)
            {
                minDist = dist;
                monster.targetIndex = i;
            }
        }
    }

    void Think()
    {
        action.moveType = MonsterMoveType.IDLE;
        action.attackType = MonsterAttackType.IDLE;
        if (monster.targetIndex == -1)
            return;
        ref var cfg = ref m_Monster.cfg;
        ref var p = ref players[monster.targetIndex];
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
            action.moveType = MonsterMoveType.RUN;
    }

    void Act()
    {
        ref var cfg = ref m_Monster.cfg;
        var u = action.targetPos - transform.position;
        bool jump = false;
        if (u.y > size.y / 2 && JumpDist > Mathf.Abs(u.x))
            jump = true;
        else
            foreach (var stair in stairs)
            {
                var v = stair.position - transform.position;
                if (Mathf.Abs(v.x) < size.x && 0 < v.y && v.y < size.y * 2)
                {
                    jump = false;
                    break;
                }
                if (Vector3.Dot(u, v) > 0 && JumpDist > Mathf.Abs(v.x))
                    jump = true;
            }
        if (jump)
            m_Monster.Jump();
        m_Monster.MoveTowards(u, action.moveType);
        if (m_Monster.status.attackType == MonsterAttackType.IDLE)
            m_Monster.AttackBy(action.attackType);
    }

    float JumpDist
    {
        get
        {
            ref var cfg = ref m_Monster.cfg;
            switch (action.moveType)
            {
                case MonsterMoveType.IDLE:
                    return 0;
                case MonsterMoveType.WALK:
                    return cfg.walkSpeed * cfg.jumpDuration;
                case MonsterMoveType.RUN:
                    return cfg.runSpeed * cfg.jumpDuration;
                default:
                    throw new System.Exception();
            }
        }
    }
}
