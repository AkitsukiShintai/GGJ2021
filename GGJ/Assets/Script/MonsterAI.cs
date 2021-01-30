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

    // Start is called before the first frame update
    void Start()
    {
        m_Monster = GetComponent<Monster>();
        Player p;
        players = new MAIPlayerData[] {
            new MAIPlayerData(p=Player.FindAnotherPlayer(null)),
            new MAIPlayerData(Player.FindAnotherPlayer(p))
        };
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
        monster.pos = gameObject.transform.position;
        monster.noBig = true;
        float dist, minDist = float.MaxValue;
        for (int i=0; i < players.Length; i++)
        {
            ref var p = ref players[i];
            if (p.isBig)
            {
                monster.noBig = false;
            }
            p.dist = (monster.pos - p.pos).magnitude;
            if (p.isAlive)
            {
                dist = p.dist * (p.isBig ? 0.95f : 1);
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
        action.targetPos = p.pos;
        if (monster.noBig
            && Time.time - monster.lastAreaAttackTime > cfg.areaAttackCoolDown
            && p.dist < cfg.areaAttackRange)
        {
            action.attackType = MonsterAttackType.RANGE;
            monster.lastAreaAttackTime = Time.time;
            action.moveType = MonsterMoveType.WALK;
        }
        else if (p.isBig
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
        var dir = action.targetPos - monster.pos;
        if (dir.y > cfg.height / 4)
        {
            // jump
        }
        else
        {
            foreach (var stair in stairs)
            {
                if (stair.position.y < cfg.height)
                {
                    // jump
                    break;
                }
            }
        }
        dir.y = 0;
        m_Monster.MoveTowards(dir.normalized, action.moveType);
        m_Monster.AttackBy(action.attackType);
    }
}
