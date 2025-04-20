/*****************************************************************************
// File Name : GoliathCannons.cs
// Author : Brandon Koederitz
// Creation Date : April 19, 2025
//
// Brief Description : Controls the cannon attack of the tiphia goliath boss.
*****************************************************************************/
using Grubitecht.World.Objects;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class GoliathCannons : ProjectileAttacker
    {
        /// <summary>
        /// Has the goliath cannons start attacking immediately on awake so they are constantly attacking.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            isAttacking = true;
            StartCoroutine(AttackRoutine());
        }
        /// <summary>
        /// Goliath cannons don't care about targeting being toggled since they target all objectives regardless of 
        /// range
        /// </summary>
        protected override void HandleOnGainTarget()
        {
            //base.HandleOnGainTarget();
        }
        protected override void HandleOnLoseTarget()
        {
            //base.HandleOnLoseTarget();
        }

        /// <summary>
        /// When the goliath cannons perform an attack action, they should attack all objectives.
        /// </summary>
        protected override void AttackAction()
        {
            // Stop attacking once there are no more current objectives.
            if (Objective.CurrentObjectives.Count == 0)
            {
                isAttacking = false;
                return;
            }
            foreach (Objective objective in Objective.CurrentObjectives)
            {
                if (objective == null) { continue; }
                //base.AttackAction();
                Attackable target = objective.Attackable;
                // Call the OnPerformedAttack event here to denote the attack has been used, it just hasnt triggered as
                // an action yet.
                CallOnPerformedAttackEvent(target);
                // Create a new projectile and launch it at the target.
                Projectile proj = Instantiate(projectilePrefab, transform.position + projectileOffset,
                    Quaternion.identity, transform);
                proj.Launch(target, ProjectileAttackAction);
            }
        }
    }
}
