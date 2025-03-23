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
    public class Combatant : CombatBehaviour
    {
        [field: SerializeField] public Team CombatTeam { get; private set; }
    }
}