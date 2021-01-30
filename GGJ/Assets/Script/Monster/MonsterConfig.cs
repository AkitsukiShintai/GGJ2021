using UnityEngine;

namespace Assets.Script
{

    [CreateAssetMenu(fileName = "怪物属性.asset", menuName = "GGJ/怪物属性")]
    public class MonsterConfig : ScriptableObject
    {

        [Tooltip("暴躁初始值")]
        public float initRage;

        [Tooltip("暴躁死亡值")]
        public float deadRage;

        [Tooltip("普通攻击范围")]
        public float commonAttackRange;

        [Tooltip("普通攻击速度")]
        public float commonAttackSpeed;

        public float commonAttackTime
        {
            // TODO: 性能问题
            get => 1 / commonAttackSpeed;
        }

        [Tooltip("普通攻击伤害")]
        public float commonAttackDmg;

        [Tooltip("突进攻击范围")]
        public float dashAttackRange;

        [Tooltip("突进攻击移速")]
        public float dashAttackMoveSpeed;

        [Tooltip("突进攻击CD")]
        public float dashAttackCoolDown;

        [Tooltip("突进攻击伤害")]
        public float dashAttackDmg;

        [Tooltip("范围攻击范围")]
        public float areaAttackRange;

        [Tooltip("范围攻击CD")]
        public float areaAttackCoolDown;

        [Tooltip("范围攻击伤害")]
        public float areaAttackDmg;

        [Tooltip("走路速度")]
        public float walkSpeed;

        [Tooltip("跑步速度")]
        public float runSpeed;

        [Tooltip("星星停留时间")]
        public float holdStarTime;
    }
}
