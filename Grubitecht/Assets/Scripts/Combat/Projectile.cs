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
        [SerializeField] private ParticleSystem projectileParticles;
        [SerializeField] private ProjectileAnimator projectileAnimator;
        [SerializeField] private float initialSpeed;
        //[SerializeField] private float acceleration;
        public void Launch(Attackable target, ProjectileAttackAction attackActionCallback)
        {
            // Updates the particle effects of this projectile so they are in line with the time this projectile will
            // take to reach it's target.
            if (projectileParticles != null)
            {
                // Estimates the travel time based on the current distance between the projectile and the target.
                // x = vt + 1/2at^2 => (-v + Sqrt(v^2-2ax))/a by quadratic formula
                float travelTime = Vector3.Distance(target.transform.position, transform.position) / initialSpeed;
                
                var mainModule = projectileParticles.main;
                mainModule.startLifetime = travelTime;
                projectileParticles.Play();
            }
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