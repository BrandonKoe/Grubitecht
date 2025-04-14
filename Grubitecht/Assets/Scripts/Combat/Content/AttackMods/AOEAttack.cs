/*****************************************************************************
// File Name : AOEAttack.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : Causes all attacks to deal damage to nearby attackables within a certain radius of the target
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class AOEAttack : AttackerMod
    {
        
        [SerializeField] private float aoeRange;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        protected override void OnAttack(Attackable target)
        {
            RaycastHit[] hits = Physics.SphereCastAll(target.transform.position, aoeRange, Vector3.zero, 0.1f, 
                LayerMask.GetMask("Enemies"));
            Debug.Log(hits.Length);

            List<Attackable> hitTargets = new List<Attackable>();
            foreach (RaycastHit hit in hits)
            {
                if (hit.rigidbody.TryGetComponent(out Attackable aoeTarget) && !hitTargets.Contains(aoeTarget))
                {
                    Debug.Log("Hit target for AOE");
                    // Deals damage to all enemies within the range of the AOE.
                    aoeTarget.TakeDamage(attacker.AttackStat);
                    hitTargets.Add(aoeTarget);
                }
            }
        }
    }
}