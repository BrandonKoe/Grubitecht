/*****************************************************************************
// File Name : Targeter.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Handles selecting nearby targets for an attacker.
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(SphereCollider))]
    public class Targeter : MonoBehaviour
    {
        [field: SerializeField] public Team Team { get; private set; }
        [SerializeField] private float detectionRange;
        [SerializeField, Tooltip("Controls what teams thic omponent will target.  If true, then this object will" +
            " target all attackables in range, even if they are on the same team.")] 
        private bool targetAll;

        public readonly List<Attackable> inRange = new List<Attackable>();

        // Events.
        public event Action<Attackable> OnGainTarget;
        public event Action<Attackable> OnLoseTarget;

        #region Component References
        [SerializeReference, HideInInspector] private SphereCollider detectionArea;

        /// <summary>
        /// Assign component references on component reset.
        /// </summary>
        private void Reset()
        {
            detectionArea = GetComponent<SphereCollider>();
            detectionArea.isTrigger = true;
        }
        #endregion

        #region Properties
        public List<Attackable> TargetsInRange
        {
            get
            {
                return inRange;
            }
        }

        public Attackable ClosestTarget
        {
            get
            {
                return inRange.OrderBy(item => Vector3.Distance(item.transform.position, transform.position))
                    .FirstOrDefault();
            }
        }
        public bool HasTarget
        {
            get
            {
                return inRange.Count > 0;
            }
        }
        #endregion


        /// <summary>
        /// Updates the radius of the circle collider in the inspector naturally.
        /// </summary>
        public void UpdateDetectionRange()
        {
            detectionArea.radius = detectionRange;
        }

        /// <summary>
        /// When an attackable object enters this targeter's collider, it becomes in range.
        /// </summary>
        /// <param name="other">The object that entered this targeter's range.</param>
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("New Target");
            if (other.TryGetComponent(out Attackable atk))
            {
                // if targetAll is false, then we should only attack attackables not teamed with this object.
                if (targetAll || atk.Team != Team)
                {
                    inRange.Add(atk);
                    OnGainTarget?.Invoke(atk);
                }
            }
        }
        /// <summary>
        /// When an attackable object leaves this targeter's collider, it is no longer in range.
        /// </summary>
        /// <param name="other">The object that entered this targeter's range.</param>
        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Lost Target");
            if (other.TryGetComponent(out Attackable atk) && inRange.Contains(atk))
            {
                inRange.Remove(atk);
                OnLoseTarget?.Invoke(atk);
            }
        }
    }
}