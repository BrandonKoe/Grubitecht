/*****************************************************************************
// File Name : TargeterBase.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Root class for all targeting systems.
*****************************************************************************/
using Grubitecht.UI.InfoPanel;
using Grubitecht.World.Objects;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(Combatant))]
    public abstract class Targeter : ModifiableCombatBehaviour<Targeter>, IInfoProvider
    {
        [Header("Targeting Stats")]
        [SerializeField] private Transform detectionVisual;
        [SerializeField, Min(0.1f)] private float detectionRange;
        [SerializeField] private StatFormatter rangeFormatter;
        [Header("Targeting Filters")]
        [SerializeField, Tooltip("Controls what teams this component will target.")]
        protected TargetType targetingType;
        [SerializeField, Tooltip("Controls what tags this targeter can target.")]
        protected CombatTags canTargetTags;
        // Events.
        public event Action OnGainTarget;
        public event Action OnLoseTarget;

        #region Component References
        [SerializeReference, HideInInspector] private SphereCollider detectionArea;
        [SerializeReference, HideInInspector] private SelectableObject selectableObject;

        /// <summary>
        /// Assign component references on component reset.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            detectionArea = GetComponent<SphereCollider>();
            selectableObject = GetComponent<SelectableObject>();
            detectionArea.isTrigger = true;
        }
        #endregion

        #region Properties
        public abstract bool HasTarget { get; }
        public float DetectionRange
        {
            get
            {
                return detectionRange;
            }
            set
            {
                detectionRange = value;
                UpdateDetectionRange();
            }
        }
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
        /// Subscribe/Unsubscribe enabling/disabling the visualizer to the selectable object component on this 
        /// object's OnSelect and OnDeselect events so the visualizer will appear when this object is selected.
        /// </summary>
        protected override void Awake()
        {
            if (selectableObject != null)
            {
                // Disable the visualizer by default
                DisableVisualizer(null);
                selectableObject.OnSelectEvent += EnableVisualizer;
                selectableObject.OnDeselectEvent += DisableVisualizer;
            }
        }
        protected override void OnDestroy()
        {
            if (selectableObject != null)
            {
                selectableObject.OnSelectEvent -= EnableVisualizer;
                selectableObject.OnDeselectEvent -= DisableVisualizer;
            }
        }

        /// <summary>
        /// Enables/Disables the visualizer for this targeter so that the player can see how far this object can
        /// target from.
        /// </summary>
        /// <param name="oldObj">Unused.</param>
        private void EnableVisualizer(ISelectable oldObj)
        {
            detectionVisual.gameObject.SetActive(true);
        }
        private void DisableVisualizer(ISelectable newObj)
        {
            detectionVisual.gameObject.SetActive(false);
        }

        /// <summary>
        /// Checks if this object should target another object based on it's team.
        /// </summary>
        /// <param name="otherTeam">The team of the object that is being checked for targeting.</param>
        /// <returns>True if the targeter should target it, false if not.</returns>
        public bool CheckTeam(Team otherTeam)
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
        /// Checks if this targeter should target another object based on it's tags.
        /// </summary>
        /// <param name="otherTags">The tags of the other object to target.</param>
        /// <returns>Whether this targeter can target the object or not.</returns>
        protected bool CheckTags(CombatTags otherTags)
        {
            return (canTargetTags & otherTags) == otherTags;
        }

        /// <summary>
        /// Updates the radius of the circle collider in the inspector naturally.
        /// </summary>
        public void UpdateDetectionRange()
        {
            detectionArea.radius = detectionRange;
            if (detectionVisual != null)
            {
                detectionVisual.localScale = Vector3.one * detectionRange * 2;
            }
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

        /// <summary>
        /// Provides information to this object's SelectableObject component about the targeter's values.
        /// </summary>
        public InfoValueBase[] InfoGetter()
        {
            List<InfoValueBase> returnVal = new List<InfoValueBase>()
            {
                new NumValue(detectionRange, 20, rangeFormatter),
                new StringValue($"Targets {targetingType.ToString()}", 21)
            };
            if (canTargetTags.HasFlag(CombatTags.Flying))
            {
                returnVal.Add(new StringValue($"Can Hit Flying Enemies", 22));
            }
            return returnVal.ToArray();
        }
    }
}