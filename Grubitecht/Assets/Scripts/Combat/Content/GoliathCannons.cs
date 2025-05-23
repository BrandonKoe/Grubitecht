/*****************************************************************************
// File Name : GoliathCannons.cs
// Author : Brandon Koederitz
// Creation Date : April 19, 2025
//
// Brief Description : Controls the cannon attack of the tiphia goliath boss.
*****************************************************************************/
using Grubitecht.Audio;
using Grubitecht.World.Objects;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class GoliathCannons : ProjectileAttacker
    {
        [SerializeField] private float cannonPeriod;
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
            // Starts shooting all the cannons.
            StartCoroutine(ShootRoutine());
        }

        private IEnumerator ShootRoutine()
        {
            // Create a new array for targets.
            List<Objective> targets = new List<Objective>();
            targets.AddRange(Objective.CurrentObjectives);
            foreach (Objective objective in targets)
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
                // Plays sound effects for launching the projectiles.
                AudioManager.PlaySoundAtPosition(projectileLaunchSfx, transform.position);
                yield return new WaitForSeconds(cannonPeriod);
            }
            // Forces this attacker to cool down.
            StartCoroutine(Cooldown());
        }
    }
}
