/*****************************************************************************
// File Name : StackableBuff.cs
// Author : Brandon Koederitz
// Creation Date : May 7, 2025
//
// Brief Description : A modifier that can numerically stack on the same effect.
*****************************************************************************/
using Grubitecht.Audio;
using UnityEngine;

namespace Grubitecht.Combat
{
    public abstract class StackableModifier<T> : Modifier<T> where T : ModifiableCombatBehaviour<T>
    {
        #region Nested Classes
        public class StackableModInstance : ModifierInstance<T>
        {
            private readonly StackableModifier<T> stackMod;
            private int count;
            public StackableModInstance(StackableModifier<T> mod) : base(mod)
            {
                stackMod = mod;
            }

            public override void OnModifierAdded(T thisBehaviour)
            {
                ChangeStackCount(thisBehaviour, 1);
                base.OnModifierAdded(thisBehaviour);
            }

            /// <summary>
            /// Increase the current count when this modifier is reapplied.
            /// </summary>
            /// <param name="thisBehaviour"></param>
            public override void HandleModifierReapplied(T thisBehaviour)
            {
                ChangeStackCount(thisBehaviour, 1);
                base.HandleModifierReapplied(thisBehaviour);
            }

            /// <summary>
            /// Increases this modifier's stack count
            /// </summary>
            /// <param name="thisBehaviour"></param>
            private void ChangeStackCount(T thisBehaviour, int change)
            {
                count += change;
                stackMod.OnModifierStack(thisBehaviour, count);
                //Debug.Log(count);
            }

            /// <summary>
            /// If this modifier is checked for removal, we should decrement the count and then return if count is 0;
            /// </summary>
            /// <returns></returns>
            public override bool CheckRemovable(T thisBehaviour)
            {
                if (!stackMod.preventRemoval)
                {
                    ChangeStackCount(thisBehaviour, -1);
                    return count <= 0;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        public override ModifierInstance<T> NewInstance()
        {
            return new StackableModInstance(this);
        }

        /// <summary>
        /// Called whenever the stack count of this modifier is changed.
        /// </summary>
        /// <param name="appliedBehaviour">The CombatBehaviour that is being modified.</param>
        /// <param name="stackCount">the current StackCount of this modifier instance.</param>
        protected virtual void OnModifierStack(T appliedBehaviour, int stackCount) { }
    }
}