/*****************************************************************************
// File Name : ModifiableCombatBehaviour.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : base Class form combat behaviour that can be modified.
*****************************************************************************/
using Grubitecht.Combat;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class ModifiableCombatBehaviour<T> : CombatBehaviour where T : ModifiableCombatBehaviour<T>
{
        protected readonly List<ModifierInstance<T>> modifiers = new List<ModifierInstance<T>>();


        /// <summary>
        /// Applies an attackable modifier to this attackable.
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        public void ApplyModifier(Modifier<T> modifier)
        {
            ModifierInstance<T> inst = modifiers.Find(item => item.Modifier == modifier);
            // Prevent duplicate modifiers from being added.
            if (!modifier.AllowDuplicates && inst != null) 
            { 
                if (modifier is DurationModifier<T> durMod && durMod.ExtendDuration)
                {
                    // Refreshes a duration modifier that is set to extend duration bny removing the existing
                    // instance and adding a new one.
                    modifiers.Remove(inst);
                }
                else
                {
                    return;
                }
            }
            inst = modifier.NewInstance();
            modifiers.Add(inst);
            inst.OnModifierAdded(this as T);
        }

        /// <summary>
        /// Removes an attackable modifier from this attackable.
        /// </summary>
        /// <param name="inst">The modifier to remove.</param>
        public void RemoveModifier(ModifierInstance<T> inst)
        {
            modifiers.Remove(inst);
            inst.OnModifierRemoved(this as T);
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