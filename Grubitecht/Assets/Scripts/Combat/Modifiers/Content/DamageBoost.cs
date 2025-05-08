/*****************************************************************************
// File Name : OnFire.cs
// Author : Brandon Koederitz
// Creation Date : April 19, 2025
//
// Brief Description : Debuff effect that deals DoT damage to an attackable periodically and causes moving attackables
// to run around in a frenzy.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.Combat
{
    [CreateAssetMenu(fileName = "StrengthBoost", menuName = "Grubitecht/Modifiers/Strength Boost")]
    public class DamageBoost : StackableModifier<Attacker>
    {
        [Header("Strength Boost Settings")]
        [SerializeField] private int damageBoost;

        /// <summary>
        /// When this modifier is added/removed, toggle the panicked movement of the grid navigator on this object.
        /// </summary>
        /// <param name="appliedBehaviour">The component this modifier is affecting.</param>
        public override void OnModifierAdded(Attacker appliedBehaviour)
        {
            base.OnModifierAdded(appliedBehaviour);
            appliedBehaviour.AttackDamage += damageBoost;
        }
        public override void OnModifierRemoved(Attacker appliedBehaviour)
        {
            base.OnModifierRemoved(appliedBehaviour);
            appliedBehaviour.AttackDamage -= damageBoost;
        }
    }
}
