using UnityEngine;

namespace Assets.Script.Monster
{
    class MonsterMove
    {
        private Animator animatior;
        private Transform transform;
        private MonsterConfig cfg;
        private bool isJumping;

        public bool IsJumping { get => isJumping; }

        public MonsterMove(Transform transform, Animator animatior, MonsterConfig cfg)
        {
            this.animatior = animatior;
            this.transform = transform;
            this.cfg = cfg;
        }

        public void Move(Vector3 dir, float speed)
        {
            var curFaceVec = transform.forward;
            if (transform.forward != dir)
            {
                var deltaDeg = Vector3.Angle(-dir, curFaceVec);
                var precent = (deltaDeg + Time.deltaTime * cfg.turnDegPerSec) / 180f;
                var nextDir = Vector3.Slerp(-dir, dir, precent);
                if (nextDir != Vector3.zero) transform.rotation = Quaternion.LookRotation(nextDir);
            }

            if (speed != 0f)
            {
                var pos = transform.position + Vector3.Dot(dir, Vector3.right) * Vector3.right * speed * Time.deltaTime;
                pos.z = 0f;
                transform.position = pos;
                animatior.SetBool("IsMoving", true);
            }
            else
            {
                animatior.SetBool("IsMoving", false);
            }

            if (isJumping)
            {
                transform.position += transform.up * cfg.jumpSpeed * Time.deltaTime;
            }
        }
        

        public void StartJump()
        {
            isJumping = true;
        }

        public void EndJump()
        {
            isJumping = false;
        }

    }

}
