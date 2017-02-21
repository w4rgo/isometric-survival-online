using System;
using System.Collections;
using UnityEngine;

namespace Movement
{
    public class WeaponCollider : MonoBehaviour
    {

        [SerializeField] private PlayerMovement Movement;

        private bool readyForCollisions;
        private Vector2 direction;
        private void Start()
        {
            Physics2D.IgnoreCollision(Movement.GetComponent<Collider2D>() , GetComponent<Collider2D>());
            Movement.OnMeleeAttack += RotateToAttackDirection;
        }

        private void RotateToAttackDirection(Vector2 direction)
        {
            var heading = Math.Atan2(direction.x, direction.y) * Mathf.Rad2Deg +90;
            transform.localRotation = Quaternion.Euler(0,0,(float) -heading);
            readyForCollisions = true;
            StartCoroutine(WaitAndRemoveCollider());
        }

        private IEnumerator WaitAndRemoveCollider()
        {
            yield return new WaitForSeconds(0.5f);
            readyForCollisions = false;
        }

        private void CollideWeapon(Collider2D other)
        {
            if (readyForCollisions)
            {
                Debug.Log("Collided with " + other.gameObject);
                other.GetComponent<PlayerMovement>().MeleeHit();
                readyForCollisions = false;
            }
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            CollideWeapon(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            CollideWeapon(other);
        }
    }
}