using UnityEngine;
using Assets.Script.Monster;

namespace MonsterAIData
{
    struct MAIPlayerData
    {
        readonly Player p;

        public MAIPlayerData(Player player)
        {
            p = player;
            dist = 0;
        }

        public Vector3 Pos => p.transform.position;
        public bool IsBig => p.transform.localScale.y > 1;
        public bool IsAlive => p.rageValue < p.playerData.rageMax;

        public float dist;
    }

    struct MAIMonsterData
    {
        public bool noBig;
        public int targetIndex;
        public float lastCommonAttackTime;
        public float lastDashAttackTime;
        public float lastAreaAttackTime;
    }

    struct MAIActionData
    {
        public MonsterAttackType attackType;
        public MonsterMoveType moveType;
        public Vector3 targetPos;
    }
}