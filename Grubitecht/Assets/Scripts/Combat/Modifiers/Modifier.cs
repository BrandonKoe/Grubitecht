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
        [field: SerializeField] public bool AllowDuplicates { get; private set; }
        /// <summary>
        /// Called whenever this modifier is added to/removed from a CombatBehaviour
        /// </summary>
        /// <param name="instance">
        /// The instance of this modifier that is occuring
        /// </param>
        public virtual void OnModifierAdded(T appliedBehaviour)
        {
            
        }
        public virtual void OnModifierRemoved(T appliedBehaviour)
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
    }
}