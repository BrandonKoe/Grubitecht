/*****************************************************************************
// File Name : Attacker.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Base class for components that let an object attack and deal damage to attackable objects.
*****************************************************************************/
using Grubitecht.World;
using Grubitecht.World.Objects;
using System;
using System.Collections;
using Grubitecht.UI.InfoPanel;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(AttackableTargeter))]
    [RequireComponent(typeof(Combatant))]
    public class Attacker : ModifiableCombatBehaviour<Attacker>, IInfoProvider
    {
        [field: Header("Stats")]
        [field: SerializeField] public float AttackDelay { get; set; }
        [field: SerializeField] public int AttackStat { get; set; }
        private bool isAttacking;

        public event Action<Attackable> OnAttack;
        // OnAttackAction is only called once ever.  Attack is called for every hit target.
        public event Action<Attackable> OnAttackAction;
        public static event Action<Attacker> DeathBroadcast;
        #region Component References
        [field: SerializeReference, HideInInspector] public AttackableTargeter targeter { get; private set; }

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
                yield return new WaitForSeconds(AttackDelay);
                AttackAction();
            }
        }

        /// <summary>
        /// Causes this object to perform an attack.
        /// </summary>
        protected virtual void AttackAction()
        {
            Attackable target = targeter.ClosestTarget;
            // Stop attacking if we attempt to attack a null target.
            if (target == null)
            {
                isAttacking = false;
                return;
            }
            Attack(target);
            OnAttackAction?.Invoke(target);
        }

        /// <summary>
        /// Causes this object to attack a perticular target.
        /// </summary>
        /// <param name="target">The target to attack.</param>
        public virtual void Attack(Attackable target)
        {
            // Prevent attacks on null targets.
            if (target == null) { return; }
            // Attack the closest target.
            target.TakeDamage(AttackStat);
            OnAttack?.Invoke(target);
        }

        /// <summary>
        /// Broadcasts the object this component is attached to has died.
        /// </summary>
        protected override void BroadcastDeath()
        {
            DeathBroadcast?.Invoke(this);
        }

        /// <summary>
        /// Provides this component's values to display on the info panel when selected.
        /// </summary>
        /// <returns>The info about this component to display when this object is selected.</returns>
        public InfoValueBase[] InfoGetter()
        {
            return new InfoValueBase[]
            {
                new NumValue(AttackStat, 2, "Attack"),
                new NumValue(AttackDelay, 3, "Attack Delay")
            };
        }
    }
}