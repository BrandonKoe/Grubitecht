/*****************************************************************************
// File Name : Attackable.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Allows an object to have health and be attacked by attackers.
*****************************************************************************/
using Grubitecht.UI;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(Combatant))]
    public class Attackable : CombatBehaviour
    {
        [SerializeField] private int maxHealth;
        [field: SerializeField, ReadOnly] public int Health { get; private set; }

        public event Action OnDeath;

        public static event Action<Attackable> DeathBroadcast;

        /// <summary>
        /// Set health to max.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Health = maxHealth;
        }

        /// <summary>
        /// Deals a given amount of damage to this object, taking into account damage reduction effects.
        /// </summary>
        /// <param name="incomingDamage">
        /// The amount of incoming damage the attacker is dealing, before any reductions.
        /// </param>
        public void TakeDamage(int incomingDamage)
        {
            // Apply any damage reductions here.
            //Debug.Log($"{gameObject.name} was hit for {incomingDamage} damage");
            ChangeHealth(-incomingDamage);
        }

        /// <summary>
        /// Changes this object's health value.
        /// </summary>
        /// <param name="value">The amount to add or subtract from this objecct's health.</param>
        public void ChangeHealth(int value)
        {
            // Show the change to the health value here.
            Health += value;
            DamageIndicator.DisplayHealthChange(value, this);
            if (Health <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Handles behaviour that should happen when this object dies.
        /// </summary>
        private void Die()
        {
            OnDeath?.Invoke();
            // Broadcast out to any listeners that this object has died.
            DeathBroadcast?.Invoke(this);
            // Destroy the game object when objects die for now.
            Destroy(gameObject);
        }
    }
}