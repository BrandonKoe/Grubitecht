/*****************************************************************************
// File Name : AttackerTargeter.cs
// Author : Brandon Koederitz
// Creation Date : March 25, 2025
//
// Brief Description : Targeting system that targets nearby attacker objects.  Used by buffs.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(SphereCollider))]
    public class AttackerTargeter : TargeterGeneric<Attacker>
    {
        protected override void SubscribeDeadRemoval()
        {
            Attacker.DeathBroadcast += RemoveTarget;
        }

        protected override void UnsubscribeDeadRemoval()
        {
            Attacker.DeathBroadcast -= RemoveTarget;
        }
    }
}