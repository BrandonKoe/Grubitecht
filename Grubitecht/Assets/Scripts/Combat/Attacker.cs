/*****************************************************************************
// File Name : Attacker.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Base class for components that let an object attack and deal damage to attackable objects.
*****************************************************************************/
using System.Collections;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(AttackableTargeter))]
    public class Attacker : CombatBehaviour
    {
        [SerializeField] private float attackDelay;
        [field: SerializeField] public int AttackStat { get; set; }
        private bool isAttacking;
        #region Component References
        [SerializeReference, HideInInspector] private AttackableTargeter targeter;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            targeter = GetComponent<AttackableTargeter>();
        }
        #endregion

        /// <summary>
        /// Subscribe/Unsubscribe from targeter events.
        /// </summary>
        private void Awake()
        {
            targeter.OnGainTarget += HandleOnGainTarget;
            targeter.OnLoseTarget += HandleOnLoseTarget;
        }
        private void OnDestroy()
        {
            targeter.OnGainTarget -= HandleOnGainTarget;
            targeter.OnLoseTarget -= HandleOnLoseTarget;
        }

        /// <summary>
        /// When a new target is found, start the attack routine if it isnt already running.
        /// </summary>
        private void HandleOnGainTarget()
        {
            if (!isAttacking)
            {
                isAttacking = true;
                StartCoroutine(AttackRoutine());
            }
        }

        /// <summary>
        /// When a target is lost, thne stop the attack routine if there are no valid targets anymore.
        /// </summary>
        private void HandleOnLoseTarget()
        {
            if (!targeter.HasTarget && isAttacking)
            {
                isAttacking = false;
            }
        }

        /// <summary>
        /// Continually calls the attack function with a given delay in between,
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator AttackRoutine()
        {
            while (isAttacking)
            {
                yield return new WaitForSeconds(attackDelay);
                Attack();
            }
        }

        /// <summary>
        /// Causes this object to attack it's target.
        /// </summary>
        protected virtual void Attack()
        {
            Attackable target = targeter.ClosestTarget;
            // Stop attacking if we attempt to attack a null target.
            if (target == null)
            {
                isAttacking = false;
                return;
            }
            // Attack the closest target.
            target.TakeDamage(AttackStat);
        }
    }
}