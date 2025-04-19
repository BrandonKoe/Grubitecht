/*****************************************************************************
// File Name : OnFire.cs
// Author : Brandon Koederitz
// Creation Date : April 19, 2025
//
// Brief Description : Debuff effect that deals DoT damage to an attackable periodically and causes moving attackables
// to run around in a frenzy.
*****************************************************************************/
using Grubitecht.World;
using Grubitecht.World.Pathfinding;
using UnityEngine;

namespace Grubitecht.Combat
{
    [CreateAssetMenu(fileName = "OnFire", menuName = "Grubitecht/Modifiers/On Fire")]
    public class Onfire : DurationModifier<Attackable>
    {
        [Header("On Fire Settings")]
        [SerializeField] private int damageAmount;
        [SerializeField] private float panicSpeedBoost;
        [SerializeField] private int panicRadius;

        /// <summary>
        /// When this modifier is added/removed, toggle the panicked movement of the grid navigator on this object.
        /// </summary>
        /// <param name="appliedBehaviour">The component this modifier is affecting.</param>
        public override void OnModifierAdded(Attackable appliedBehaviour)
        {
            if (appliedBehaviour.TryGetComponent(out EnemyController enemy))
            {
                enemy.StartPanicking(panicSpeedBoost, panicRadius);
            }
        }
        public override void OnModifierRemoved(Attackable appliedBehaviour)
        {
            if (appliedBehaviour.TryGetComponent(out EnemyController enemy))
            {
                enemy.StopPanicking();
            }
        }

        /// <summary>
        /// Deals damage to the affected attackable each tick.
        /// </summary>
        /// <param name="appliedBehaviour">The attackable component this modifier is affecting.</param>
        protected override void OnModifierTick(Attackable appliedBehaviour)
        {
            appliedBehaviour.ChangeHealth(-damageAmount);
        }
    }
}
