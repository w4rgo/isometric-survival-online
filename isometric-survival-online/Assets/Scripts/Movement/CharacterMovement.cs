using System;
using UnityEngine;

namespace Movement
{
    public class CharacterMovement : MonoBehaviour

    {
        public bool isControllable;

        private PlayerAnimators anim;
        public float speed = 5f;
        private Vector2 movement;
        private Vector3 previousPosition = Vector3.zero;
        private Vector3 currentPos = Vector3.zero;
        private Vector3 lastDirection = Vector3.zero;

        public event Action InitiatedMelee = delegate { };
        public event Action<Vector2> OnMeleeAttack = delegate {  };

        void Start()
        {
            anim = GetComponent<PlayerAnimators>();
            previousPosition = transform.position;
        }

        private void Update()
        {
            if (isControllable)
            {
                var isMelee = Input.GetKeyDown(KeyCode.Space);
                if (isMelee)
                {
                    InitiatedMelee();
                    MeleeAttackAnimation();
                }
            }
        }

        void FixedUpdate()
        {
            currentPos = transform.position;

            if (isControllable)
            {
                Move();
            }
            if (currentPos != previousPosition)
            {
                RunningAnimation();
            }
            else
            {
                IdleAnimation();
            }

            previousPosition = currentPos;
        }

        private void Move()
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");
            movement = new Vector2(inputX, inputY);
            transform.Translate(movement * speed * Time.deltaTime);
        }

        private void RunningAnimation()
        {
            var heading = currentPos - previousPosition;
            var distance = heading.magnitude;
            var direction = heading / distance;
            anim.SetFloatToAnimators("speedX", direction.x);
            anim.SetFloatToAnimators("speedY", direction.y);
            lastDirection = direction;
        }

        private void IdleAnimation()
        {
            anim.SetFloatToAnimators("speedX", 0f);
            anim.SetFloatToAnimators("speedY", 0f);
            anim.SetFloatToAnimators("lastSpeedX", lastDirection.x);
            anim.SetFloatToAnimators("lastSpeedY", lastDirection.y);
        }

        public void MeleeAttackAnimation()
        {
            OnMeleeAttack(lastDirection);
            anim.SetFloatToAnimators("lastSpeedX", lastDirection.x);
            anim.SetFloatToAnimators("lastSpeedY", lastDirection.y);
            anim.SetTriggerToAnimators("melee_attack");
            anim.SetTriggerToAnimators("melee_attack");
        }

        public void MeleeHit()
        {
            Debug.Log("Triggering death");
            anim.SetFloatToAnimators("lastSpeedX", lastDirection.x);
            anim.SetFloatToAnimators("lastSpeedY", lastDirection.y);
            anim.SetTriggerToAnimators("death");
        }
    }
}