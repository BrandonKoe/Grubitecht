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
            Debug.Log("Modifier Instance Added.");
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
    }
}