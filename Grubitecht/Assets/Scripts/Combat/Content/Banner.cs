/*****************************************************************************
// File Name : Banner.cs
// Author : Brandon Koederitz
// Creation Date : March 25, 2025
//
// Brief Description : Banner of the grubs that empowers all nearby grub structures.
*****************************************************************************/
using Grubitecht.Audio;
using Grubitecht.UI.InfoPanel;
using Grubitecht.World.Objects;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class Banner : Effector<Attacker, AttackerTargeter>, IInfoProvider
    {
        [SerializeField] private int displayedBoost;
        [SerializeField] private StatFormatter buffFormatter;

        /// <summary>
        /// Provides this component's values to display on the info panel when selected.
        /// </summary>
        /// <returns>The info about this component to display when this object is selected.</returns>
        public InfoValueBase[] InfoGetter()
        {
            return new InfoValueBase[]
            {
                new NumValue(displayedBoost, 10, buffFormatter),
            };
        }
    }
}