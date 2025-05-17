/*****************************************************************************
// File Name : GrubController.cs
// Author : Brandon Koederitz
// Creation Date : March 26, 2025
//
// Brief Description : Controls the assignment of grubitechts.  Will spawn grubs to move objects visually and manages
// the number of grubs in the field.
*****************************************************************************/
using Grubitecht.World;
using Grubitecht.World.Objects;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Grubitecht
{
    public class GrubManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text grubText;
        [SerializeField] private GrubController grubPrefab;
        [SerializeField] private int maxGrubCount;

        //public static int MaxGrubCount { get; private set; }
        private static int mgCount;

        private static GrubManager instance;

        private static readonly Dictionary<MovableObject, GrubController> dispatchedGrubs = new();

        #region Properties
        public static int AvailableGrubs
        {
            get
            {
                return MaxGrubCount - dispatchedGrubs.Count;
            }
        }
        public static int MaxGrubCount
        {
            get
            {
                return Objective.CurrentObjectives.Count;
                //return mgCount;
            }
            private set
            {
                mgCount = value;
            }
        }
        #endregion

        /// <summary>
        /// Assign the singleton instance on awake.
        /// </summary>
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.Log("Duplicate GrubManager found.");
                return;
            }
            else
            {
                instance = this;
                MaxGrubCount = maxGrubCount;
                //Debug.LogError("Hit that part");
            }
        }

        /// <summary>
        /// Update the text in start to ensure that it happens after objectives have been assigned.
        /// </summary>
        private void Start()
        {
            if (instance == this)
            {
                UpdateText();
            }
        }

        /// <summary>
        /// Clears the dispatchedGrubs list when this scene is unloaded.
        /// </summary>
        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
            dispatchedGrubs.Clear();
        }

        /// <summary>
        /// Checks if there is a grub available to move an object.
        /// </summary>
        /// <returns>True if there is a free grub, false if all grubs are in use.</returns>
        public static bool CheckGrub()
        {
            return AvailableGrubs > 0;
        }

        /// <summary>
        /// Assigns a grub to visually move a given object.
        /// </summary>
        /// <param name="obj">The object to move.</param>
        public static void AssignGrub(MovableObject obj)
        {
            if (!CheckGrub())
            {
                Debug.Log("There are no more grubs left to assign.");
                return;
            }
            if (dispatchedGrubs.ContainsKey(obj)) { return; }
            GrubController spawnedGrub = Instantiate(instance.grubPrefab, instance.transform);
            // Spawn a grub.
            dispatchedGrubs.Add(obj, spawnedGrub);
            spawnedGrub.Initialize(obj.GridNavigator);
            UpdateText();
        }

        /// <summary>
        /// Returns a delegated grub to the pool of ready grubs
        /// </summary>
        /// <param name="obj">The movable object that the grub was delegated to.</param>
        public static void ReturnGrub(MovableObject obj)
        {
            if (dispatchedGrubs.ContainsKey(obj))
            {
                dispatchedGrubs[obj].RecallGrub();
                dispatchedGrubs.Remove(obj);
                UpdateText();
            }
        }

        /// <summary>
        /// Returns a delegated grub to the pool of ready grubs
        /// </summary>
        /// <param name="grub">The grub that was managing a moving object.</param>
        public static void ReturnGrub(GrubController grub)
        {
            if (dispatchedGrubs.ContainsValue(grub))
            {
                grub.RecallGrub();
                dispatchedGrubs.Remove(dispatchedGrubs.First(item => item.Value == grub).Key);
                UpdateText();
            }
        }

        /// <summary>
        /// Updates the text that displays the number of available grubs.
        /// </summary>
        public static void UpdateText()
        {
            if (instance == null || instance.grubText == null)
            {
                //Debug.LogError("Returned");
                return; 
            }
            instance.grubText.text = $"{Mathf.Max(AvailableGrubs, 0)}/{MaxGrubCount}";
            //Debug.LogError(instance.grubText.text);
        }
    }
}