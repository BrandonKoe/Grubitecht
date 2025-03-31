/*****************************************************************************
// File Name : Attacker.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Base class for components that let an object attack and deal damage to attackable objects.
*****************************************************************************/
using Grubitecht.World;
using System;
using System.Collections;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(AttackableTargeter))]
    [RequireComponent(typeof(Combatant))]
    public class Attacker : CombatBehaviour
    {
        [SerializeField] private float attackDelay;
        [field: SerializeField] public int AttackStat { get; set; }
        private bool isAttacking;
        #region Component References
        [SerializeReference, HideInInspector] private AttackableTargeter targeter;

        public static event Action<Attacker> DeathBroadcast;

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
        protected override void Awake()
        {
            base.Awake();
            targeter.OnGainTarget += HandleOnGainTarget;
            targeter.OnLoseTarget += HandleOnLoseTarget;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
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
            while (isAttacking && LevelManager.IsPlaying)
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

        /// <summary>
        /// Broadcasts the object this component is attached to has died.
        /// </summary>
        protected override void BroadcastDeath()
        {
            DeathBroadcast?.Invoke(this);
        }
    }
}