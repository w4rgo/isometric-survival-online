using System;
using UnityEngine;

namespace Movement
{
    public class CharacterMovement : MonoBehaviour

    {
        public bool isControllable;

        Animator anim;
        public float speed = 5f;
        private Vector2 movement;
        private Vector3 previousPosition = Vector3.zero;

        // Use this for initialization
        void Start()
        {
            anim = GetComponent<Animator>();
            //GetComponent<Rigidbody2D>().velocity = movement;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (isControllable)
            {
                float inputX = Input.GetAxisRaw("Horizontal");
                float inputY = Input.GetAxisRaw("Vertical");
                movement = new Vector2(inputX, inputY);
                transform.Translate(movement * speed * Time.deltaTime);
                var currentPos = transform.position;

                anim.SetFloat("speedX", movement.x);
                anim.SetFloat("speedY", movement.y);
//                var angleBetween = (Mathf.Atan2(previousPosition.y - currentPos.y, previousPosition.x - currentPos.x) *
//                                    180 /
//                                    Math.PI) + 90;
                previousPosition = currentPos;

//                anim.SetFloat("direction", (float) angleBetween);
            }

        }
    }
}