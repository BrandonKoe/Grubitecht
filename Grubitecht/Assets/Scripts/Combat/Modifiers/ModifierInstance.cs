/*****************************************************************************
// File Name : ModifierInstance.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : An instantiable set of class data for a specific modifier.
*****************************************************************************/
using UnityEditor;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class ModifierInstance<T> where T : ModifiableCombatBehaviour<T>
    {
        internal T appliedBehaviour;
        protected readonly Modifier<T> mod;
        protected GameObject modifierVFX;

        #region Properties
        public Modifier<T> Modifier
        {
            get
            {
                return mod;
            }
        }
        #endregion

        public ModifierInstance(Modifier<T> mod)
        {
            this.mod = mod;
        }

        /// <summary>
        /// Controls behaviour for this instance when this modifier is added or removed.
        /// </summary>
        /// <param name="thisBehaviour">The CombatBehaviour this modifier is affecting.</param>
        public virtual void OnModifierAdded(T thisBehaviour)
        {
            appliedBehaviour = thisBehaviour;
            //Debug.Log("Modifier Instance Added.");
            // Show VFX when this modifier is added.
            modifierVFX = GameObject.Instantiate(mod.VisualEffects, thisBehaviour.transform);
            mod.OnModifierAdded(thisBehaviour);
        }

        public virtual void OnModifierRemoved(T thisBehaviour)
        {
            // Destroy the vfx for this modifier once the modifier is removed.
            GameObject.Destroy(modifierVFX);
            mod.OnModifierRemoved(thisBehaviour);
        }

        /// <summary>
        /// Controls what happens to this modifier when a second instance is applied to a target.
        /// </summary>
        /// <param name="thisBehaviour">The CombatBehaviour that had a second modifier applied.</param>
        public virtual void HandleModifierReapplied(T thisBehaviour)
        {
            mod.OnModifierReapplied(thisBehaviour);
        }

        /// <summary>
        /// Checks if this object is removable;
        /// </summary>
        /// <returns>True if this modifier can be removed.</returns>
        public virtual bool CheckRemovable(T thisBehaviour)
        {
            return mod.CheckRemovable(thisBehaviour);
        }
    }
}