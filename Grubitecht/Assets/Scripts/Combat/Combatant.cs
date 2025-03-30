/*****************************************************************************
// File Name : Combatant.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Core class that contains universal information about the object when it takes part in combat.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class Combatant : CombatBehaviour
    {
        [field: SerializeField] public Team CombatTeam { get; private set; }
        [field: SerializeField] public CombatTags CombatTags { get; private set; }

        /// <summary>
        /// Update rigidbody values on reset, as the rigidbody should always be kinematic and never use gravity.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}