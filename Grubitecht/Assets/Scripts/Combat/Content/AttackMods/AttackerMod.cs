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
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            attacker.OnAttack -= OnAttack;
            attacker.OnAttackAction -= OnAttackAction;
        }

        /// <summary>
        /// Abstract function that allows mod to perform special behaviour when an attack happens.
        /// </summary>
        /// <param name="target">The target of the attack.</param>
        protected virtual void OnAttack(Attackable target) { }

        /// <summary>
        /// Abstract function that allows mod to perform special behaviour when an attack happens.
        /// </summary>
        /// <param name="target">The target of the attack.</param>
        protected virtual void OnAttackAction(Attackable target) { }
    }
}