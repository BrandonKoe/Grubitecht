/*****************************************************************************
// File Name : ApplyModifier.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Attacker Mod that applies a modifier to the target attackable when they are attacked.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class ApplyModifier : AttackerMod
    {
        [SerializeField] private Modifier<Attackable>[] inflictedModifiers;

        /// <summary>
        /// Applies all of the listed modifiers to the target when this attacker attacks.
        /// </summary>
        /// <param name="target">The target of the attack.</param>
        protected override void OnAttack(Attackable target)
        {
            foreach (var modifier in inflictedModifiers)
            {
                target.ApplyModifier(modifier);
            }
        }
    }
}