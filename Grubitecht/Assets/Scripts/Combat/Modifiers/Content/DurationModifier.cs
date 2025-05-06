/*****************************************************************************
// File Name : DurationModifier.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : A modifier that has a given duration.
*****************************************************************************/
using Grubitecht.Audio;
using System.Collections;
using UnityEngine;

namespace Grubitecht.Combat
{
    public abstract class DurationModifier<T> : Modifier<T> where T : ModifiableCombatBehaviour<T>
    {
        [Header("Duration Modifier Settings")]
        [SerializeField] private Sound tickSound;
        [SerializeField] private Sound expireSound;
        [SerializeField] private float duration;
        [field: SerializeField] public bool ExtendDuration { get; private set; }
        [SerializeField, Tooltip("The amount of time between ticks of this modifier.")] 
        private float tickInterval;

        #region Nested Classes
        public class DurationModInstance : ModifierInstance<T>
        {
            private readonly DurationModifier<T> durMod;
            private readonly float duration;
            private readonly float tickInterval;
            public DurationModInstance(DurationModifier<T> mod, float duration, float tickInterval) : base(mod)
            {
                durMod = mod;
                this.duration = duration;
                this.tickInterval = tickInterval;
            }

            public override void OnModifierAdded(T thisBehaviour)
            {
                base.OnModifierAdded(thisBehaviour);
                thisBehaviour.StartCoroutine(EffectTimer());
            }

            /// <summary>
            /// Controls the lifetime of this affect and what happens when it expires.
            /// </summary>
            /// <returns>Corotuine.</returns>
            private IEnumerator EffectTimer()
            {
                float timer = duration;
                float tickTimer = tickInterval;
                while (timer > 0)
                {
                    tickTimer -= Time.deltaTime;
                    if (tickTimer < 0)
                    {
                        durMod.OnModifierTick(appliedBehaviour);
                        // Play a sound when this modifier ticks.
                        AudioManager.PlaySoundAtPosition(durMod.tickSound, appliedBehaviour.transform.position);
                        tickTimer = tickInterval;
                    }

                    timer -= Time.deltaTime;
                    yield return null;
                }
                //yield return new WaitForSeconds(duration);
                durMod.OnModifierExpired(appliedBehaviour);
                AudioManager.PlaySoundAtPosition(durMod.expireSound, appliedBehaviour.transform.position);
                // Remove this modifier once it has expired.
                appliedBehaviour.RemoveModifier(this);
            }
        }
        #endregion

        public override ModifierInstance<T> NewInstance()
        {
            return new DurationModInstance(this, duration, tickInterval);
        }

        protected virtual void OnModifierExpired(T appliedBehaviour) { }
        protected virtual void OnModifierTick(T appliedBehaviour) { }
    }
}