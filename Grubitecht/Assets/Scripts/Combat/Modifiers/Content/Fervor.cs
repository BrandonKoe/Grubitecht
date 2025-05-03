/*****************************************************************************
// File Name : Fervor.cs
// Author : Brandon Koederitz
// Creation Date : May 2, 2025
//
// Brief Description : Buff to all enemies they recieve upon destroying a strucutre to incentivize protecting multiple
// objectives.
*****************************************************************************/
using Grubitecht.World;
using Grubitecht.World.Pathfinding;
using UnityEngine;

namespace Grubitecht.Combat
{
    [CreateAssetMenu(fileName = "Fervor", menuName = "Grubitecht/Modifiers/Fervor")]
    public class Fervor : Modifier<Attacker>
    {
        [Header("Fervor Settings")]
        [SerializeField] private int damageBoost;
        [SerializeField] private int attackSpeedBoost;
        [SerializeField] private float speedBoost;

        /// <summary>
        /// When this modifier is added/removed, toggle the panicked movement of the grid navigator on this object.
        /// </summary>
        /// <param name="appliedBehaviour">The component this modifier is affecting.</param>
        public override void OnModifierAdded(Attacker appliedBehaviour)
        {
            base.OnModifierAdded(appliedBehaviour);
            // Applies a buff to the attack damage, attack speed, and movement speed of affected enemies.
            appliedBehaviour.AttackStat += damageBoost;
            appliedBehaviour.AttackCooldown -= attackSpeedBoost;
            if (appliedBehaviour.TryGetComponent(out GridNavigator nav))
            {
                nav.MoveSpeed += speedBoost;
            }
        }
        public override void OnModifierRemoved(Attacker appliedBehaviour)
        {
            base.OnModifierRemoved(appliedBehaviour);
            appliedBehaviour.AttackStat -= damageBoost;
            appliedBehaviour.AttackCooldown += attackSpeedBoost;
            if (appliedBehaviour.TryGetComponent(out GridNavigator nav))
            {
                nav.MoveSpeed -= speedBoost;
            }
        }
    }
}
