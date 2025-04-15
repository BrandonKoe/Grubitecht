/*****************************************************************************
// File Name : Attackable.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Allows an object to have health and be attacked by attackers.
*****************************************************************************/
using Grubitecht.UI;
using Grubitecht.UI.InfoPanel;
using Grubitecht.World.Objects;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(Combatant))]
    public class Attackable : ModifiableCombatBehaviour<Attackable>, IInfoProvider
    {
        [SerializeField] private int maxHealth;
        [field: SerializeField, ReadOnly] public int Health { get; private set; }
        [SerializeField] private Color damageIndicatorColor;
        [SerializeField] private bool destroyOnDeath;
        [SerializeField] private bool hasHealthBar;

        private HealthBar hpBar;

        public event Action OnDeath;

        public static event Action<Attackable> DeathBroadcast;

        #region Component References
        [SerializeReference, HideInInspector] private SelectableObject selectableObject;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            selectableObject = GetComponent<SelectableObject>();
        }
        #endregion

        #region Nested Classes
        private HealthBar HPBar
        {
            get
            {
                if (hpBar == null)
                {
                    hpBar = HealthBarManager.CreateHealthBar(this, maxHealth);
                }
                return hpBar;
            }
        }
        #endregion

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
            DamageIndicator.DisplayHealthChange(value, this, damageIndicatorColor);
            if (hasHealthBar)
            {
                HPBar.UpdateHealth(Health);
            }
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
            if (hasHealthBar)
            {
                HPBar.DestroyHealthBar();
            }
            if (destroyOnDeath)
            {
                // Destroy the game object when objects die for now.
                Destroy(gameObject);
            }
        }


        /// <summary>
        /// Provides this component's values to display on the info panel when selected.
        /// </summary>
        /// <returns>The info about this component to display when this object is selected.</returns>
        public InfoValueBase[] InfoGetter()
        {
            return new InfoValueBase[]
            {
                new NumValue(maxHealth, 1, "Max Health")
            };
        }
    }
}