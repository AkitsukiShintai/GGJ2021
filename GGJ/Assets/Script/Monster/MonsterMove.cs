using UnityEngine;

namespace Assets.Script.Monster
{
    class MonsterMove
    {
        private Animator animatior;
        private Transform transform;
        private float turnDegPerSec = 120f;

        public MonsterMove(Transform transform, Animator animatior)
        {
            this.animatior = animatior;
            this.transform = transform;
        }

        public void Move(Vector3 dir, float speed)
        {
            var curFaceVec = transform.forward;
            if (transform.forward != dir)
            {
                var deltaDeg = Vector3.Angle(-dir, curFaceVec);
                var precent = (deltaDeg + Time.deltaTime * turnDegPerSec) / 180f;
                var nextDir = Vector3.Slerp(-dir, dir, precent);
                transform.rotation = Quaternion.LookRotation(nextDir);
            }

            transform.position += Vector3.Dot(dir, Vector3.right) * Vector3.right * speed * Time.deltaTime;

        }

    }

}
