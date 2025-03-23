/*****************************************************************************
// File Name : TargeterBase.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Root class for all targeting systems.
*****************************************************************************/
using System;
using UnityEngine;

namespace Grubitecht.Combat
{
    public abstract class Targeter : CombatBehaviour
    {
        [SerializeField, Min(0.1f)] private float detectionRange;
        [SerializeField, Tooltip("Controls what teams this component will target.")]
        protected TargetType targetingType;
        // Events.
        public event Action OnGainTarget;
        public event Action OnLoseTarget;

        #region Component References
        [SerializeReference, HideInInspector] private SphereCollider detectionArea;

        /// <summary>
        /// Assign component references on component reset.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            detectionArea = GetComponent<SphereCollider>();
            detectionArea.isTrigger = true;
        }
        #endregion

        #region Properties
        public abstract bool HasTarget { get; }
        #endregion

        #region Nested
        protected enum TargetType
        {
            Enemies,
            Allies,
            All
        }
        #endregion

        /// <summary>
        /// Checks if this object should target another object based on it's team.
        /// </summary>
        /// <param name="otherTeam">The team of the object that is being checked for targeting.</param>
        /// <returns>Whether this object can target it or not.</returns>
        protected bool CheckTarget(Team otherTeam)
        {
            switch (targetingType)
            {
                case TargetType.Enemies:
                    return otherTeam != Team;
                case TargetType.Allies:
                    return otherTeam == Team;
                case TargetType.All:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Updates the radius of the circle collider in the inspector naturally.
        /// </summary>
        public void UpdateDetectionRange()
        {
            detectionArea.radius = detectionRange;
        }

        /// <summary>
        /// Allows for child classes to call the OnGainTarget and OnLoseTarget events.
        /// </summary>
        protected void CallOnGain()
        {
            OnGainTarget?.Invoke();
        }
        protected void CallOnLose()
        {
            OnLoseTarget?.Invoke();
        }
    }
}