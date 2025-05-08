/*****************************************************************************
// File Name : Buffer.cs
// Author : Brandon Koederitz
// Creation Date : March 25, 2025
//
// Brief Description : Base class for components that apply buffs to allies and debuffs to enemies.
*****************************************************************************/
using Grubitecht.Audio;
using Grubitecht.UI.InfoPanel;
using Grubitecht.World.Objects;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(AttackerTargeter))]
    [RequireComponent(typeof(Combatant))]
    public abstract class Effector<T, t> : CombatBehaviour where T : ModifiableCombatBehaviour<T> where t : 
        TargeterGeneric<T>
    {
        [SerializeField] private Modifier<T> appliedModifier;
        [SerializeField] private Sound buffSound;
        #region Component References
        [SerializeReference, HideInInspector] private t targeter;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            targeter = GetComponent<t>();
        }
        #endregion

        /// <summary>
        /// Subscribe/Unsubscribe from targeter events.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            targeter.OnGainTargetGeneric += HandleOnGainTarget;
            targeter.OnLoseTargetGeneric += HandleOnLoseTarget;
        }
        protected override void OnDestroy()
        {
            base.Awake();
            targeter.OnGainTargetGeneric -= HandleOnGainTarget;
            targeter.OnLoseTargetGeneric -= HandleOnLoseTarget;
        }

        /// <summary>
        /// When a new target is found, apply this buff to it.
        /// </summary>
        private void HandleOnGainTarget(T target)
        {
            ApplyBuff(target);
        }

        /// <summary>
        /// When a target is lost, then remove the buff from it.
        /// </summary>
        private void HandleOnLoseTarget(T target)
        {
            RemoveBuff(target);
        }

        /// <summary>
        /// Applied/removes the given buff to the target when they enter/exit this object's range.
        /// </summary>
        /// <param name="buffedTarget">The attacker being buffed/debuffed.</param>
        protected virtual void ApplyBuff(T buffedTarget)
        {
            buffedTarget.ApplyModifier(appliedModifier);
            AudioManager.PlaySoundAtPosition(buffSound, buffedTarget.transform.position);
        }
        protected virtual void RemoveBuff(T buffedTarget)
        {
            buffedTarget.RemoveModifier(appliedModifier);
        }
    }
}