/*****************************************************************************
// File Name : Projectil.cs
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
        private const float IMPACT_DIST = 0.1f;
        #endregion
        [SerializeField] private float speed;
        public void Launch(Attackable target, ProjectileAttackAction attackActionCallback)
        {
            StartCoroutine(MovementRoutine(target, attackActionCallback));
        }

        private IEnumerator MovementRoutine(Attackable target, ProjectileAttackAction callback)
        {
            while (target != null && Vector2.Distance(transform.position, target.transform.position) > IMPACT_DIST)
            {
                // Continually moves thtis object towards the target.
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