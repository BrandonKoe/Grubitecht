/*****************************************************************************
// File Name : DurationModifier.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : A modifier that has a given duration.
*****************************************************************************/
using System.Collections;
using UnityEngine;

namespace Grubitecht.Combat
{
    public abstract class DurationModifier<T> : Modifier<T> where T : ModifiableCombatBehaviour<T>
    {
        [SerializeField] private float duration;

        #region Nested Classes
        public class DurationModInstance : ModifierInstance<T>
        {
            private readonly DurationModifier<T> durMod;
            private readonly float duration;
            public DurationModInstance(DurationModifier<T> mod, float duration) : base(mod)
            {
                durMod = mod;
                this.duration = duration;
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
                yield return new WaitForSeconds(duration);
                durMod.OnModifierExpired(appliedBehaviour);
                // Remove this modifier once it has expired.
                appliedBehaviour.RemoveModifier(this);
            }
        }
        #endregion

        public override ModifierInstance<T> NewInstance()
        {
            return new DurationModInstance(this, duration);
        }

        protected virtual void OnModifierExpired(T appliedBehaviour) { }
    }
}