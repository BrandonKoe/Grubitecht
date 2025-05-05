/*****************************************************************************
// File Name : LowMorale.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : Debuff effect that will nerf structures when an objective is destroyed.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.Combat
{
    [CreateAssetMenu(fileName = "LowMorale", menuName = "Grubitecht/Modifiers/Low Morale")]
    public class LowMorale : DurationModifier<Attacker>
    {
        [Header("Low Morale Settings")]
        [SerializeField] private int damageReduction;

        public override void OnModifierAdded(Attacker appliedBehaviour)
        {
            base.OnModifierAdded(appliedBehaviour);
            appliedBehaviour.AttackDamage -= damageReduction;
        }
        public override void OnModifierRemoved(Attacker appliedBehaviour)
        {
            base.OnModifierRemoved(appliedBehaviour);
            appliedBehaviour.AttackDamage += damageReduction;
        }
    }
}
