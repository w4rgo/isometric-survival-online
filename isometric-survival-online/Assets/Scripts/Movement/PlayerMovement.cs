using System;
using System.Collections.Generic;
using UnityEngine;
using IsoRigidbody = UltimateIsometricToolkit.physics.IsoRigidbody;

namespace Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        public bool isControllable;

        private PlayerAnimators anim;
        public float speed = 5f;
        private Vector2 movement;
        private Vector3 previousPosition = Vector3.zero;
        private Vector3 currentPos = Vector3.zero;
        private Vector3 lastDirection = Vector3.zero;
        public float maxVelocityChange = 10.0f;
        public event Action InitiatedMelee = delegate { };
        public event Action<Vector2> OnMeleeAttack = delegate { };
        private Dictionary<Vector2,Vector2> DirectionList = new Dictionary<Vector2,Vector2>();
        private HashSet<Vector2> Cartesians = new HashSet<Vector2>();
        [SerializeField]
        private float speedAdjusmentForCartesians;

        [SerializeField] private float runningModifier;

        void Start()
        {
            DirectionList.Add(new Vector2(0, 1),new Vector2(1, 1) );
            DirectionList.Add(new Vector2(1, 1),new Vector2(1, 0) );
            DirectionList.Add(new Vector2(1, 0),new Vector2(1, -1) );
            DirectionList.Add(new Vector2(1, -1),new Vector2(0, -1) );
            DirectionList.Add(new Vector2(0, -1),new Vector2(-1, -1) );
            DirectionList.Add(new Vector2(-1, -1),new Vector2(-1, 0) );
            DirectionList.Add(new Vector2(-1, 0),new Vector2(-1, 1) );
            DirectionList.Add(new Vector2(-1, 1),new Vector2(0, 1) );
            DirectionList.Add(new Vector2(0, 0),new Vector2(0, 0) );

            Cartesians.Add(new Vector2(1, 1));
            Cartesians.Add(new Vector2(1, -1));
            Cartesians.Add(new Vector2(-1, 1));
            Cartesians.Add(new Vector2(-1, -1));

            anim = GetComponent<PlayerAnimators>();
            previousPosition = transform.position;


        }

        private void Update()
        {
            if (!isControllable && gameObject.GetComponent<IsoRigidbody>() != null)
            {
                GetComponent<IsoRigidbody>().IsKinematic = true;
            }

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
//                MoveNormal();
                MoveWithForces();
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

        private void MoveNormal()
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");
            movement = new Vector2(inputX, inputY);
            transform.Translate(movement * speed * Time.deltaTime);
        }

        private void MoveWithForces()
        {
            var rigidbody = GetComponent<IsoRigidbody>();
            // Calculate how fast we should be moving
            var axisX = -Input.GetAxisRaw("Horizontal");
            var axisY = Input.GetAxisRaw("Vertical");

            Vector2 rotatedDir = MegaHackOfDirection((int)axisX, (int)axisY);
            axisX = rotatedDir.x;
            axisY = rotatedDir.y;
            rigidbody.Velocity = new Vector3(axisY * CalculateSpeed(rotatedDir), rigidbody.Velocity.y, axisX * CalculateSpeed(rotatedDir));

//            var targetVelocity = new Vector3(axisX, axisY, 0);
//            targetVelocity = transform.TransformDirection(targetVelocity);
//            targetVelocity *= CalculateSpeed(rotatedDir);
//
//            // Apply a force that attempts to reach our target velocity
//            var velocity = rigidbody.Velocity;
//            var velocityChange = (targetVelocity - velocity);
//            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
//            velocityChange.z = 0;
//            velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
//            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        private float CalculateSpeed(Vector2 inputDirection)
        {
            var shift = Input.GetKey(KeyCode.LeftShift);
            var newSpeed = shift ? speed * runningModifier : speed;
            if (Cartesians.Contains(inputDirection))
            {
                return newSpeed * speedAdjusmentForCartesians;
            }

            return newSpeed;
        }

        private Vector2 MegaHackOfDirection(int x, int y)
        {
            return DirectionList[new Vector2(x,y)];
        }

        private void RunningAnimation()
        {
            var heading = currentPos - previousPosition;
            var distance = heading.magnitude;
            var direction = heading / distance;

//            direction = new Vector3(Mathf.Round(direction.x), Mathf.Round(direction.y), Mathf.Round(direction.z));
            anim.SetFloatToAnimators("speedX", direction.x);
            anim.SetFloatToAnimators("speedY", direction.y);
            Debug.Log(direction);
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