/*****************************************************************************
// File Name : AttackerMod.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Base class for components that add additional behaviour to the attacks of this object's 
attacker script.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(Attacker))]
    public abstract class AttackerMod : CombatBehaviour
    {
        #region Component References
        [SerializeReference, HideInInspector] protected Attacker attacker;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            attacker = GetComponent<Attacker>();
        }
        #endregion

        /// <summary>
        /// Subscribe/unsubscribe the OnAttack function from the attackers OnAttack event to call that function when 
        /// an attack happens
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            attacker.OnAttack += OnAttack;
            attacker.OnAttackAction += OnAttackAction;
            attacker.OnPerformAttack += OnAttackPerformed;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            attacker.OnAttack -= OnAttack;
            attacker.OnAttackAction -= OnAttackAction;
            attacker.OnPerformAttack -= OnAttackPerformed;
        }

        /// <summary>
        /// Abstract functions that allows mod to perform special behaviour when an attack happens.
        /// </summary>
        /// <param name="target">The target of the attack.</param>
        // Called when an attack happens and damage is dealt to a target.
        protected virtual void OnAttack(Attackable target) { }
        // Called when the attack is going to happen as an entire action
        protected virtual void OnAttackAction(Attackable target) { }
        // Called when the attacker uses the attack.
        protected virtual void OnAttackPerformed(Attackable target) { }
    }
}