/*****************************************************************************
// File Name : Banner.cs
// Author : Brandon Koederitz
// Creation Date : March 25, 2025
//
// Brief Description : Banner of the grubs that empowers all nearby grub structures.
*****************************************************************************/
using Grubitecht.UI.InfoPanel;
using Grubitecht.World.Objects;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class Banner : Effector, IInfoProvider
    {
        [SerializeField] private GameObject buffEffect;
        [SerializeField] private int attackBoost;
        [SerializeField] private StatFormatter attackBoostFormatter;

        private readonly Dictionary<Attacker, GameObject> buffEffects = new Dictionary<Attacker, GameObject>();

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
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        /// <summary>
        /// Applies an attack boost to attackers within the banner's range.
        /// </summary>
        /// <param name="buffedAttacker">The attacker that is now within range of this banner.</param>
        protected override void ApplyBuff(Attacker buffedAttacker)
        {
            buffedAttacker.AttackDamage += attackBoost;
            if (buffEffect != null)
            {
                // Displays an effect at the position of buffed attackers to show their attack is increased.
                GameObject bEff = Instantiate(buffEffect, buffedAttacker.transform.position, Quaternion.identity,
                    buffedAttacker.transform);
                buffEffects.Add(buffedAttacker, bEff);
            }
        }

        /// <summary>
        /// Removes the applied attack boost to buffed attackers when they leave the banners range.
        /// </summary>
        /// <param name="buffedAttacker">The attacker that is no longer within range of this banner.</param>
        protected override void RemoveBuff(Attacker buffedAttacker)
        {
            buffedAttacker.AttackDamage -= attackBoost;
            // Destroys effects when a structure is no longer being buffed.
            if (buffEffects.ContainsKey(buffedAttacker))
            {
                Destroy(buffEffects[buffedAttacker]);
                buffEffects.Remove(buffedAttacker);
            }    
        }

        /// <summary>
        /// Provides this component's values to display on the info panel when selected.
        /// </summary>
        /// <returns>The info about this component to display when this object is selected.</returns>
        public InfoValueBase[] InfoGetter()
        {
            return new InfoValueBase[]
            {
                new NumValue(attackBoost, 10, attackBoostFormatter),
            };
        }
    }
}