/*****************************************************************************
// File Name : Targeter.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Root class for generic targeting systems that that can target specific components.
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(SphereCollider))]
    public abstract class TargeterGeneric<T> : Targeter where T : CombatBehaviour
    {
        public readonly List<T> inRange = new List<T>();

        // Events.
        public event Action<T> OnGainTargetGeneric;
        public event Action<T> OnLoseTargetGeneric;

        #region Properties
        public List<T> TargetsInRange
        {
            get
            {
                // Whenever we access our targets, we should remove any nullrefs.
                inRange.RemoveAll(item => item == null);
                return inRange;
            }
        }

        public T ClosestTarget
        {
            get
            {
                return TargetsInRange.OrderBy(item => Vector3.Distance(item.transform.position, transform.position))
                    .FirstOrDefault();
            }
        }
        public override bool HasTarget => TargetsInRange.Count > 0;
        #endregion

        /// <summary>
        /// When an attackable object enters this targeter's collider, it becomes in range.
        /// </summary>
        /// <param name="other">The object that entered this targeter's range.</param>
        private void OnTriggerEnter(Collider other)
        {
            // This targeter should never interact with itself or other triggers.
            if (other.gameObject == gameObject || other.isTrigger) { return; }
            //Debug.Log("New Target");
            if (other.TryGetComponent(out T atk) && CheckTarget(atk.Team) && CheckTags(atk.Tags))
            {
                inRange.Add(atk);
                OnGainTargetGeneric?.Invoke(atk);
                CallOnGain();
            }
        }
        /// <summary>
        /// When an attackable object leaves this targeter's collider, it is no longer in range.
        /// </summary>
        /// <param name="other">The object that entered this targeter's range.</param>
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == gameObject) { return; }
            //Debug.Log("Lost Target");
            if (other.TryGetComponent(out T atk) && inRange.Contains(atk))
            {
                inRange.Remove(atk);
                OnLoseTargetGeneric?.Invoke(atk);
                CallOnLose();
            }
        }
    }
}