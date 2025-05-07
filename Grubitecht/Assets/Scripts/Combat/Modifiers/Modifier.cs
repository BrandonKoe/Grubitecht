/*****************************************************************************
// File Name : Modifier.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : Root class for all modifiers that can be inflicted on combat behavours.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    public abstract class Modifier<T> : ScriptableObject where T : ModifiableCombatBehaviour<T>
    {
        [field: Header("Base Modifier Settings")]
        [field: SerializeField] public GameObject VisualEffects { get; private set; }
        [field: SerializeField] public bool AllowDuplicates { get; private set; }
        [SerializeField] protected bool preventRemoval;
        /// <summary>
        /// Called whenever this modifier is added to/removed from a CombatBehaviour
        /// </summary>
        /// <param name="appliedBehaviour">
        /// The CombatBehaviour that has this modifier.
        /// </param>
        public virtual void OnModifierAdded(T appliedBehaviour)
        {
            
        }
        public virtual void OnModifierRemoved(T appliedBehaviour)
        {

        }
        /// <summary>
        /// Called when a second instance of this modifier is attempted to be added to a CombatBehaviour
        /// </summary>
        /// <param name="appliedBehaviour">The CombatBehaviour that had a second modifier applied.</param>
        public virtual void OnModifierReapplied(T appliedBehaviour)
        {

        }

        /// <summary>
        /// Creates a new instance of this modifier.
        /// </summary>
        /// <typeparam name="t">The instance type of this modifier.</typeparam>
        /// <returns>The new instance of the modifier to create.</returns>
        public virtual ModifierInstance<T> NewInstance()
        {
            return new ModifierInstance<T>(this);
        }

        /// <summary>
        /// Checks if this object is removable.
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckRemovable(T appliedBehaviour)
        {
            return !preventRemoval;
        }
    }
}