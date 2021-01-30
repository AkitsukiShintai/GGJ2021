using UnityEngine;

namespace Assets.Script.Monster
{
    class MonsterAttack
    {
        Transform transform;
        Animator animator;
        Vector3 commonAttackRange;
        float rangeAttackRadius;

        public MonsterAttack(Transform transform, Animator animator, float rangeAttackRadius)
        {
            this.transform = transform;
            this.animator = animator;
            commonAttackRange = new Vector3(0.8f, 0.5f, 0.8f);
            this.rangeAttackRadius = rangeAttackRadius;
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
                Debug.Log("BOSS普攻命中" + collsion.gameObject.name);
            }

        }

        private void RangeAttack()
        {
            var collisions = Physics.OverlapSphere(transform.position, rangeAttackRadius, 1 << LayerMask.NameToLayer("Attackable"));
            foreach (var collsion in collisions)
            {
                Debug.Log("BOSS范围攻击命中" + collsion.gameObject.name);
            }
        }

    }
}
