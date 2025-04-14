/*****************************************************************************
// File Name : AOEAttack.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : Causes all attacks to deal damage to nearby attackables within a certain radius of the target
*****************************************************************************/
using Grubitecht.UI.InfoPanel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class AOEAttack : AttackerMod, IInfoProvider
    {
        [SerializeField] private float aoeRange;
        [SerializeField] private bool friendlyFire;

        public InfoValueBase[] InfoGetter()
        {
            return new InfoValueBase[]
            { 
                new NumValue(aoeRange, 50, "AOE Range")
            };
        }

        /// <summary>
        /// Deals extra damage to any attackable objects within a certain spherical radius of the target of an attack
        /// </summary>
        /// <param name="target">The target of the attack.</param>
        protected override void OnAttack(Attackable target)
        {
            RaycastHit[] hits = Physics.SphereCastAll(target.transform.position, aoeRange, Vector3.right, 0.1f, 
                LayerMask.GetMask("Attackable"));

            List<Attackable> hitTargets = new List<Attackable>();
            foreach (RaycastHit hit in hits)
            {
                if (hit.rigidbody.TryGetComponent(out Attackable aoeTarget) && !hitTargets.Contains(aoeTarget))
                {
                    // Skips over targets that are on the same team as us if friendly fire is disabled.
                    if (!friendlyFire && !attacker.targeter.CheckTeam(aoeTarget.Team)) { continue; }
                    // Deals damage to all enemies within the range of the AOE.
                    aoeTarget.TakeDamage(attacker.AttackStat);
                    hitTargets.Add(aoeTarget);
                }
            }
        }
    }
}