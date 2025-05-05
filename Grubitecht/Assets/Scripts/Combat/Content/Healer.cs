/*****************************************************************************
// File Name : Banner.cs
// Author : Brandon Koederitz
// Creation Date : April 17, 2025
//
// Brief Description : Structure type that can heal nearby objectives over time.
*****************************************************************************/
using Grubitecht.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grubitecht.UI.InfoPanel;
using System;
using System.Linq;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(Combatant))]
    [RequireComponent(typeof(AttackableTargeter))]
    public class Healer : CombatBehaviour, IInfoProvider
    {
        [field: Header("Stats")]
        [field: SerializeField] public int HealingStrength { get; set; }
        [SerializeField] private StatFormatter healingStrengthFormatter;
        [field: SerializeField] public float HealDelay { get; set; }
        [SerializeField] private StatFormatter healDelayFormatter;
        private bool isHealing;

        public event Action<Attackable> OnHeal;
        // OnAttackAction is only called once ever.  Attack is called for every hit target.
        public event Action<Attackable> OnHealAction;

        #region Component References
        [SerializeReference, HideInInspector] private AttackableTargeter targeter;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            targeter = GetComponent<AttackableTargeter>();
        }
        #endregion

        /// <summary>
        /// Subscribe/Unsubscribe from targeter events.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            targeter.OnGainTarget += HandleOnGainTarget;
            targeter.OnLoseTarget += HandleOnLoseTarget;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            targeter.OnGainTarget -= HandleOnGainTarget;
            targeter.OnLoseTarget -= HandleOnLoseTarget;
        }

        /// <summary>
        /// When a new target is found, start the attack routine if it isnt already running.
        /// </summary>
        private void HandleOnGainTarget()
        {
            if (!isHealing)
            {
                isHealing = true;
                StartCoroutine(HealRoutine());
            }
        }

        /// <summary>
        /// When a target is lost, thne stop the attack routine if there are no valid targets anymore.
        /// </summary>
        private void HandleOnLoseTarget()
        {
            if (!targeter.HasTarget && isHealing)
            {
                //Debug.Log("Healing has stopped");
                isHealing = false;
            }
        }
        /// <summary>
        /// Continually calls the attack function with a given delay in between,
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator HealRoutine()
        {
            while (isHealing && LevelManager.IsPlaying)
            {
                yield return new WaitForSeconds(HealDelay);
                HealAction();
            }
        }

        /// <summary>
        /// Gets the closest target that has the lowest health.
        /// </summary>
        /// <returns>The closest attackable target with the lowest health.</returns>
        private Attackable GetUnhealedTarget()
        {
            List<Attackable> targets = new List<Attackable>(); 
            targets.AddRange(targeter.TargetsInRange);
            // Low health targets are always healed first.
            return targets.OrderBy(item => item.Health).FirstOrDefault();
        }

        /// <summary>
        /// Has this structure perform a heal action to heal the closest target with low health.
        /// </summary>
        private void HealAction()
        {
            Attackable target = GetUnhealedTarget();
            // Stop healing if we attempt to heal a null target.
            if (target == null)
            {
                HandleOnLoseTarget();
                return;
            }
            Debug.Log("Healing target " + target + " who has " + target.Health + " HP");
            Heal(target);
            OnHealAction?.Invoke(target);
        }

        /// <summary>
        /// Heals a given target
        /// </summary>
        /// <param name="target">The target to heal</param>
        public void Heal(Attackable target)
        {
            if (target == null) { return; }
            // Dont heal a target that has max health.
            if (target.Health >= target.MaxHealth)
            {
                return;
            }
            // Heals the target.
            target.ChangeHealth(HealingStrength);
            OnHeal?.Invoke(target);
        }

        public InfoValueBase[] InfoGetter()
        {
            return new InfoValueBase[]
            {
                new NumValue(HealingStrength, 10, healingStrengthFormatter),
                new NumValue(HealDelay, 11, healDelayFormatter)
            };
        }
    }
}