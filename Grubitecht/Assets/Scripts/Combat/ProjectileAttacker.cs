/*****************************************************************************
// File Name : ProjectileAttacker.cs
// Author : Brandon Koederitz
// Creation Date : April 19, 2025
//
// Brief Description : Special attacker type that launches projectiles at their target periodically.
// Main difference with projectile attackers is that their attacks are delayed until the projectile actually hits.
*****************************************************************************/
using Grubitecht.Audio;
using UnityEngine;

namespace Grubitecht.Combat
{
    public delegate void ProjectileAttackAction(Attackable target);
    public class ProjectileAttacker : Attacker
    {
        [Header("Projectile")]
        [SerializeField] protected Sound projectileLaunchSfx;
        [SerializeField] protected Projectile projectilePrefab;
        [SerializeField] protected Vector3 projectileOffset;
        /// <summary>
        /// Instead of the default attack action, projectile attackers spawn a projectile that will call the special 
        /// ProjectileAttackAction when they hit.
        /// </summary>
        protected override void AttackAction()
        {
            //base.AttackAction();
            Attackable target = targeter.ClosestTarget;
            // Prevent the attack if we attempt to attack a null target.
            if (target == null)
            {
                return;
            }
            // Call the OnPerformedAttack event here to denote the attack has been used, it just hasnt triggered as
            // an action yet.
            CallOnPerformedAttackEvent(target);
            // Create a new projectile and launch it at the target.
            Projectile proj = Instantiate(projectilePrefab, transform.position + projectileOffset, 
                Quaternion.identity);
            proj.Launch(target, ProjectileAttackAction);
            // Plays sound effects for launching a projectile.
            AudioManager.PlaySoundAtPosition(projectileLaunchSfx, transform.position);
            // Forces this attacker to cool down.
            StartCoroutine(Cooldown());
        }

        /// <summary>
        /// Performs an attack action on the attackable hit with the projectile.
        /// </summary>
        /// <param name="target">The target of the projectile.</param>
        protected void ProjectileAttackAction(Attackable target)
        {
            // Prevent null target.
            if (target == null) { return; }
            Attack(target);
            CallAttackActionEvent(target);

            // Play effects at the target of the attack.
            if (attackEffects != null)
            {
                Instantiate(attackEffects, target.transform.position, Quaternion.identity);
            }
            // Plays sound effect for hitting the enemy.
            AudioManager.PlaySoundAtPosition(attackSfxName, target.transform.position);
        }
    }

}