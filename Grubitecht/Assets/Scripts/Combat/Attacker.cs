/*****************************************************************************
// File Name : Attacker.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Base class for components that let an object attack and deal damage to attackable objects.
*****************************************************************************/
using Grubitecht.UI.InfoPanel;
using System;
using System.Collections;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(AttackableTargeter))]
    [RequireComponent(typeof(Combatant))]
    public class Attacker : ModifiableCombatBehaviour<Attacker>, IInfoProvider
    {
        [SerializeField] protected GameObject attackEffects;
        [field: Header("Stats")]
        [field: SerializeField] public float AttackCooldown { get; set; }
        [field: SerializeField] public int AttackStat { get; set; }
        protected bool onCooldown;

        public event Action<Attackable> OnAttack;
        // OnAttackAction is only called once ever.  Attack is called for every hit target.
        public event Action<Attackable> OnAttackAction;
        // OnPerformAttack is called right before the attacker performs an attack.
        public event Action<Attackable> OnPerformAttack;
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
            //targeter.OnLoseTarget += HandleOnLoseTarget;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            targeter.OnGainTarget -= HandleOnGainTarget;
            //targeter.OnLoseTarget -= HandleOnLoseTarget;
        }

        /// <summary>
        /// When a new target is found, perform an attack immediately if the attack is not already on cooldown.
        /// </summary>
        protected virtual void HandleOnGainTarget()
        {
            if (!onCooldown)
            {
                AttackAction();
            }
            //if (!onCooldown)
            //{
            //    onCooldown = true;
            //    StartCoroutine(AttackRoutine());
            //}
        }

        ///// <summary>
        ///// When a target is lost, thne stop the attack routine if there are no valid targets anymore.
        ///// </summary>
        //protected virtual void HandleOnLoseTarget()
        //{
        //    //if (!targeter.HasTarget && onCooldown)
        //    //{
        //    //    onCooldown = false;
        //    //}
        //}

        /// <summary>
        /// Continually calls the attack function with a given delay in between,
        /// </summary>
        /// <returns>Coroutine.</returns>
        protected virtual IEnumerator Cooldown()
        {
            //while (onCooldown && LevelManager.IsPlaying)
            //{
            //    AttackAction();
            //    yield return new WaitForSeconds(AttackCooldown);
            //}
            onCooldown = true;
            yield return new WaitForSeconds(Mathf.Max(AttackCooldown, 0.25f));
            onCooldown = false;
            // Immediately attack once our attack comes off cooldown if we have a valid target.
            if (targeter.HasTarget)
            {
                AttackAction();
            }
        }

        /// <summary>
        /// Causes this object to perform an attack.
        /// </summary>
        protected virtual void AttackAction()
        {
            Attackable target = targeter.ClosestTarget;
            //Debug.Log(name + " Attacked " + target);
            // Stop the attack if we attempt to attack a null target.
            if (target == null)
            {
                //HandleOnLoseTarget();
                return;
            }
            CallOnPerformedAttackEvent(target);
            Attack(target);
            CallAttackActionEvent(target);
            // Play effects at the target of the attack.
            if (attackEffects != null)
            { 
                Instantiate(attackEffects, target.transform.position, Quaternion.identity);
            }
            // Forces this attacker to cool down.
            StartCoroutine(Cooldown());
        }

        /// <summary>
        /// Allows children to call the attack action event.
        /// </summary>
        /// <param name="target">The target of the attack action.</param>
        protected void CallAttackActionEvent(Attackable target)
        {
            OnAttackAction?.Invoke(target);
        }

        protected void CallOnPerformedAttackEvent(Attackable target)
        {
            OnPerformAttack?.Invoke(target);
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
                new NumValue(AttackStat, 10, "Attack"),
                new NumValue(AttackCooldown, 11, "Attack Cooldown")
            };
        }
    }
}