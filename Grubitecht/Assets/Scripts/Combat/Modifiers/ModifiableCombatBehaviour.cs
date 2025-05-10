/*****************************************************************************
// File Name : ModifiableCombatBehaviour.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : base Class form combat behaviour that can be modified.
*****************************************************************************/
using UnityEngine;
using System.Collections.Generic;

namespace Grubitecht.Combat
{
    public class ModifiableCombatBehaviour<T> : CombatBehaviour where T : ModifiableCombatBehaviour<T>
{
        [SerializeField] protected bool immuneToModifiers;
        protected readonly List<ModifierInstance<T>> modifiers = new List<ModifierInstance<T>>();


        /// <summary>
        /// Applies an attackable modifier to this attackable.
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        public ModifierInstance<T> ApplyModifier(Modifier<T> modifier)
        {
            // Prevent adding a modifier if we're immune to modifiers.
            if (immuneToModifiers) { return null;  }
            ModifierInstance<T> inst = modifiers.Find(item => item.Modifier == modifier);
            // Prevent duplicate modifiers from being added.
            if (!modifier.AllowDuplicates && inst != null) 
            { 
                //if (modifier is DurationModifier<T> durMod && durMod.ExtendDuration)
                //{
                //    // Refreshes a duration modifier that is set to extend duration bny removing the existing
                //    // instance and adding a new one.
                //    modifiers.Remove(inst);
                //}
                //else
                //{
                //    return null;
                //}
                inst.HandleModifierReapplied(this as T);
                return inst;
            }
            inst = modifier.NewInstance();
            modifiers.Add(inst);
            inst.OnModifierAdded(this as T);
            return inst;
        }

        /// <summary>
        /// Removes an attackable modifier from this attackable.
        /// </summary>
        /// <param name="modifier">The modifier to remove.</param>
        public void RemoveModifier(Modifier<T> modifier)
        {
            ModifierInstance<T> inst = modifiers.Find(item => item.Modifier == modifier);
            if (inst != null && inst.CheckRemovable(this as T))
            {
                modifiers.Remove(inst);
                inst.OnModifierRemoved(this as T);
            }
        }

        /// <summary>
        /// Removes an attackable modifier from this attackable.
        /// </summary>
        /// <param name="inst">The modifier instance to remove.</param>
        public void RemoveModifier(ModifierInstance<T> inst, bool bypassRemovableCheck = true)
        {
            if (bypassRemovableCheck || inst.CheckRemovable(this as T))
            {
                modifiers.Remove(inst);
                inst.OnModifierRemoved(this as T);
            }
        }

        /// <summary>
        /// Triggers the effects of modifiers on this combatant when a specified trigger is passed in.
        /// </summary>
        /// <param name="trigger">The trigger condition that has occured.</param>
        public void QueryTriggers<Q>(Q trigger) where Q : Trigger
        {
            foreach (ITriggeredModifier<Q> triggeredMod in modifiers)
            {
                triggeredMod.OnTriggered(trigger);
            }
        }
    }
}