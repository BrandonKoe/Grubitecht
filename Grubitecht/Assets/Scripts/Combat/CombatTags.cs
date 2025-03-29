/*****************************************************************************
// File Name : CombatTags.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Set of special tags that can be added to combatants to differentiate them.
*****************************************************************************/
using System;

namespace Grubitecht.Combat
{
    [Flags]
    public enum CombatTags
    {
        None = 0,
        Flying = 1 << 0,
    }
}
