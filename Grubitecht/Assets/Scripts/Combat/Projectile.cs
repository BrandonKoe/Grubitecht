/*****************************************************************************
// File Name : Projectile.cs
// Author : Brandon Koederitz
// Creation Date : April 19, 2025
//
// Brief Description : A projectile launched by a projectile attacker.
*****************************************************************************/
using System.Collections;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class Projectile : MonoBehaviour
    {
        #region CONSTS
        private const float IMPACT_DIST = 0.3f;
        #endregion
        [SerializeField] private ProjectileAnimator projectileAnimator;
        [SerializeField] private float initialSpeed;
        //[SerializeField] private float acceleration;
        public void Launch(Attackable target, ProjectileAttackAction attackActionCallback)
        {
            if (projectileAnimator != null)
            {
                projectileAnimator.StartAnimating(target);
            }
            StartCoroutine(MovementRoutine(target, attackActionCallback));
        }

        private IEnumerator MovementRoutine(Attackable target, ProjectileAttackAction callback)
        {
            float speed = initialSpeed;
            while (target != null && Vector2.Distance(transform.position, target.transform.position) > IMPACT_DIST)
            {
                // Continually moves thtis object towards the target.
                //speed += acceleration * Time.deltaTime;
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);

                // Rotate the projectile so it faces the right direction.
                transform.LookAt(target.transform.position);
                yield return null;
            }

            // When the projectile hits...
            callback?.Invoke(target);
            Destroy(gameObject);
        }
    }
}