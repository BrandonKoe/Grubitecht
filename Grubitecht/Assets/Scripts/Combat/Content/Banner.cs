/*****************************************************************************
// File Name : Banner.cs
// Author : Brandon Koederitz
// Creation Date : March 25, 2025
//
// Brief Description : Banner of the grubs that empowers all nearby grub structures.
*****************************************************************************/
using Grubitecht.UI.InfoPanel;
using Grubitecht.World.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class Banner : Effector
    {
        [SerializeField] private int attackBoost;

        #region Component References
        [SerializeReference, HideInInspector] private SelectableObject selectableObject;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            selectableObject = GetComponent<SelectableObject>();
        }
        #endregion

        /// <summary>
        /// Adds this component's info getter so that the attack boost can be displayed.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (selectableObject != null)
            {
                selectableObject.AddInfoGetter(InfoGetter);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (selectableObject != null)
            {
                selectableObject.RemoveInfoGetter(InfoGetter);
            }
        }

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

        /// <summary>
        /// Provides this component's values to display on the info panel when selected.
        /// </summary>
        /// <returns>The info about this component to display when this object is selected.</returns>
        private InfoValueBase[] InfoGetter()
        {
            return new InfoValueBase[]
            {
                new NumValue(attackBoost, 2, "Attack Boost"),
            };
        }
    }
}