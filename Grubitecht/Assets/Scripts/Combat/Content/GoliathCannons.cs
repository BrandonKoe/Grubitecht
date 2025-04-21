/*****************************************************************************
// File Name : GoliathCannons.cs
// Author : Brandon Koederitz
// Creation Date : April 19, 2025
//
// Brief Description : Controls the cannon attack of the tiphia goliath boss.
*****************************************************************************/
using Grubitecht.World.Objects;
using System.Collections;
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
            // The goliath cannons immediately start on cooldown and will attack once the cooldown is finished.
            StartCoroutine(Cooldown());
        }
        ///// <summary>
        ///// Goliath cannons don't care about targeting being toggled since they target all objectives regardless of 
        ///// range
        ///// </summary>
        //protected override void HandleOnGainTarget()
        //{
        //    //base.HandleOnGainTarget();
        //}
        //protected override void HandleOnLoseTarget()
        //{
        //    //base.HandleOnLoseTarget();
        //}

        protected override IEnumerator Cooldown()
        {
            onCooldown = true;
            yield return new WaitForSeconds(AttackCooldown);
            onCooldown = false;
            // Dont perform a HasTarget check here because we should always have a target.
            AttackAction();
        }

        /// <summary>
        /// When the goliath cannons perform an attack action, they should attack all objectives.
        /// </summary>
        protected override void AttackAction()
        {
            // Stop attacking once there are no more current objectives.
            if (Objective.CurrentObjectives.Count == 0)
            {
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
                    Quaternion.identity);
                proj.Launch(target, ProjectileAttackAction);
            }
            // Forces this attacker to cool down.
            StartCoroutine(Cooldown());
        }
    }
}
