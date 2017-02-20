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

        private Vector3 lastDirection = Vector3.zero;
        // Use this for initialization
        void Start()
        {
            anim = GetComponent<PlayerAnimators>();
            //GetComponent<Rigidbody2D>().velocity = movement;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            var currentPos = transform.position;

            if (isControllable)
            {
                float inputX = Input.GetAxisRaw("Horizontal");
                float inputY = Input.GetAxisRaw("Vertical");
                movement = new Vector2(inputX, inputY);
                transform.Translate(movement * speed * Time.deltaTime);
            }

            Debug.Log(currentPos + " - " + previousPosition);
            var heading = currentPos - previousPosition;
            var distance = heading.magnitude;
            var direction = heading / distance;
            if (currentPos != previousPosition)
            {

                Debug.Log("speed: " + direction);
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
                Debug.Log("lastSpeed: " + lastDirection);


            }
            previousPosition = currentPos;
        }
    }
}