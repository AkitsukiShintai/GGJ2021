using UnityEngine;

namespace Assets.Script.Monster
{
    class MonsterAttack
    {
        Transform transform;
        Animator animator;
        Vector3 commonAttackRange;
        MonsterConfig cfg;

        public MonsterAttack(Transform transform, Animator animator, MonsterConfig cfg)
        {
            this.transform = transform;
            this.animator = animator;
            commonAttackRange = new Vector3(0.5f, 0.25f, 0.5f);
            this.cfg = cfg;
        }

        public void Attack(MonsterAttackType type)
        {
            switch (type)
            {
                case MonsterAttackType.IDLE:
                    break;
                case MonsterAttackType.COMMON:
                    CommonAttack();
                    break;
                case MonsterAttackType.DASH:
                    break;
                case MonsterAttackType.RANGE:
                    RangeAttack();
                    break;
                default:
                    break;
            }
        }

        private void CommonAttack()
        {
            var center = transform.position;
            center.y += commonAttackRange.y;
            center.x += commonAttackRange.x;
            var collisions = Physics.OverlapBox(center, commonAttackRange, transform.rotation, 1 << LayerMask.NameToLayer("Attackable"));

            foreach (var collsion in collisions)
            {
                var playerObj = collsion.gameObject;
                playerObj.GetComponent<Player>().AddRageValue(cfg.commonAttackDmg);
                Debug.Log("BOSS普攻命中" + playerObj.name);
            }

        }

        private void RangeAttack()
        {
            var collisions = Physics.OverlapSphere(transform.position, cfg.areaAttackRange, 1 << LayerMask.NameToLayer("Attackable"));
            foreach (var collsion in collisions)
            {
                var playerObj = collsion.gameObject;
                playerObj.GetComponent<Player>().AddRageValue(cfg.areaAttackDmg);
                Debug.Log("BOSS范围攻击命中" + collsion.gameObject.name);
            }
        }

    }
}
