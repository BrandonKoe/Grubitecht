/*****************************************************************************
// File Name : AttackableTargeter.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Targeting system that targets nearby attackable objects.  Used by attackers.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(SphereCollider))]
    public class AttackableTargeter : TargeterGeneric<Attackable>
    {
        protected override void SubscribeDeadRemoval()
        {
            Attackable.DeathBroadcast += RemoveTarget;
        }

        protected override void UnsubscribeDeadRemoval()
        {
            Attackable.DeathBroadcast -= RemoveTarget;
        }
    }
}