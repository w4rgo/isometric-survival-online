using System;
using UnityEngine;

namespace Movement
{
    public class CharacterMovement : MonoBehaviour

    {

        Animator anim;
        public float speed = 5f;
        private Vector2 movement;
        private Vector3 previousPosition = Vector3.zero;

        // Use this for initialization
        void Start () {
            anim = GetComponent<Animator>();
            //GetComponent<Rigidbody2D>().velocity = movement;
        }

        // Update is called once per frame
        void FixedUpdate () {
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");
            movement = new Vector2(inputX, inputY);
            transform.Translate(movement * speed * Time.deltaTime);
            var currentPos = transform.position;

            //Diagonals
            anim.SetFloat("speedX",movement.x);
            anim.SetFloat("speedY",movement.y);

//            if (inputX != 0 && inputY != 0) {
//                if (movement.y == 1 && movement.x == -1)
//                {
//                    anim.SetTrigger("move_up_left");
//                }
//
//                if (movement.y == 1 && movement.x == 1)
//                {
//                    anim.SetTrigger("move_up_right");
//                }
//
//                if (movement.y == -1 && movement.x == -1)
//                {
//                    anim.SetTrigger("move_down_left");
//                }
//
//                if (movement.y == -1 && movement.x == 1)
//                {
//                    anim.SetTrigger("move_down_right");
//                }
//            }
//
//            else
//            {
//
//                //left/right/up/down
//                if (movement.x == -1)
//                {
//                    anim.SetTrigger("move_left");
//                }
//
//                if (movement.x == 1)
//                {
//                    anim.SetTrigger("move_right");
//                }
//
//
//                if (movement.y == 1)
//                {
//                    anim.SetTrigger("move_up");
//                }
//
//
//                if (movement.y == -1)
//                {
//                    anim.SetTrigger("move_down");
//                }
//            }





            var angleBetween = (Mathf.Atan2(previousPosition.y - currentPos.y, previousPosition.x - currentPos.x) * 180 /
                               Math.PI)+90;
            previousPosition = currentPos;

            anim.SetFloat("direction", (float) angleBetween);
        }

    }
}