/*****************************************************************************
// File Name : Buffer.cs
// Author : Brandon Koederitz
// Creation Date : March 25, 2025
//
// Brief Description : Base class for components that apply buffs to allies and debuffs to enemies.
*****************************************************************************/
using Grubitecht.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(AttackerTargeter))]
    public abstract class Effector : CombatBehaviour
    {
        #region Component References
        [SerializeReference, HideInInspector] private AttackerTargeter targeter;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            targeter = GetComponent<AttackerTargeter>();
        }
        #endregion

        /// <summary>
        /// Subscribe/Unsubscribe from targeter events.
        /// </summary>
        private void Awake()
        {
            targeter.OnGainTargetGeneric += HandleOnGainTarget;
            targeter.OnLoseTargetGeneric += HandleOnLoseTarget;
        }
        private void OnDestroy()
        {
            targeter.OnGainTargetGeneric -= HandleOnGainTarget;
            targeter.OnLoseTargetGeneric -= HandleOnLoseTarget;
        }

        /// <summary>
        /// When a new target is found, apply this buff to it.
        /// </summary>
        private void HandleOnGainTarget(Attacker atk)
        {
            ApplyBuff(atk);
        }

        /// <summary>
        /// When a target is lost, then remove the buff from it.
        /// </summary>
        private void HandleOnLoseTarget(Attacker atk)
        {
            RemoveBuff(atk);
        }

        /// <summary>
        /// Each child class will apply their own unique type of buff or debuff.
        /// </summary>
        /// <param name="buffedAttacker">The attacker being buffed/debuffed.</param>
        protected abstract void ApplyBuff(Attacker buffedAttacker);
        protected abstract void RemoveBuff(Attacker buffedAttacker);
    }
}