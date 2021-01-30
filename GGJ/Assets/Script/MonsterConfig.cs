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

        [Tooltip("突进攻击范围")]
        public float dashAttackRange;

        [Tooltip("突进攻击移速")]
        public float dashAttackMoveSpeed;

        [Tooltip("普通攻击速度")]
        public float commonAttackSpeed;

        [Tooltip("走路速度")]
        public float walkSpeed;

        [Tooltip("跑步速度")]
        public float runSpeed;
    }
}
