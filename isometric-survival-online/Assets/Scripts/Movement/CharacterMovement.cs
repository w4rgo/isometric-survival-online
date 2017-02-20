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

        public event Action InitiatedMelee;


        // Use this for initialization
        void Start()
        {
            anim = GetComponent<PlayerAnimators>();
            previousPosition = transform.position;
            //GetComponent<Rigidbody2D>().velocity = movement;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            currentPos = transform.position;

            if (isControllable)
            {
                var isMelee = Input.GetKey(KeyCode.Space) ;
                if (isMelee)
                {
                    InitiatedMelee();
                    MeleeAttack();
                }

                float inputX = Input.GetAxisRaw("Horizontal");
                float inputY = Input.GetAxisRaw("Vertical");
                movement = new Vector2(inputX, inputY);
                transform.Translate(movement * speed * Time.deltaTime);
            }



            Debug.Log("movement cp: " + currentPos + " pp:" + previousPosition );
            if (currentPos != previousPosition )
            {
                var heading = currentPos - previousPosition;
                var distance = heading.magnitude;
                var direction = heading / distance;
                Debug.Log("changed pos: ");
                anim.SetFloatToAnimators("speedX", direction.x);
                anim.SetFloatToAnimators("speedY", direction.y);
                lastDirection = direction;

            }
            else
            {
                anim.SetFloatToAnimators("speedX", 0f);
                anim.SetFloatToAnimators("speedY", 0f);
                anim.SetFloatToAnimators("lastSpeedX", lastDirection.x);
                anim.SetFloatToAnimators("lastSpeedY", lastDirection.y);

            }

            previousPosition = currentPos;
        }

        public void MeleeAttack()
        {

//                anim.SetFloatToAnimators("speedX", lastDirection.x);
//                anim.SetFloatToAnimators("speedY", lastDirection.y);
                anim.SetFloatToAnimators("lastSpeedX", lastDirection.x);
                anim.SetFloatToAnimators("lastSpeedY", lastDirection.y);
                anim.SetTriggerToAnimators("melee_attack");


        }
    }
}