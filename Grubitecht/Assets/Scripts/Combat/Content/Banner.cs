/*****************************************************************************
// File Name : Banner.cs
// Author : Brandon Koederitz
// Creation Date : March 25, 2025
//
// Brief Description : Banner of the grubs that empowers all nearby grub structures.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class Banner : Effector
    {
        [SerializeField] private int attackBoost;

        /// <summary>
        /// Applies an attack boost to attackers within the banner's range.
        /// </summary>
        /// <param name="buffedAttacker">The attacker that is now within range of this banner.</param>
        protected override void ApplyBuff(Attacker buffedAttacker)
        {
            buffedAttacker.AttackStat += attackBoost;
        }

        /// <summary>
        /// Removes the applied attack boost to buffed attackers when they leave the banners range.
        /// </summary>
        /// <param name="buffedAttacker">The attacker that is no longer within range of this banner.</param>
        protected override void RemoveBuff(Attacker buffedAttacker)
        {
            buffedAttacker.AttackStat -= attackBoost;
        }
    }
}