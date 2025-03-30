/*****************************************************************************
// File Name : GrubController.cs
// Author : Brandon Koederitz
// Creation Date : March 26, 2025
//
// Brief Description : Controls the assignment of grubitechts.  Will spawn grubs to move objects visually and manages
// the number of grubs in the field.
*****************************************************************************/
using Grubitecht.World.Objects;
using Grubitecht.World;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace Grubitecht
{
    public class GrubManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text grubText;
        [SerializeField] private int maxGrubCount;
        [SerializeField] private GrubController grubPrefab;

        public static int MaxGrubCount { get; private set; }

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
                UpdateText();
            }
        }

        /// <summary>
        /// Requests a grub to move this object.
        /// </summary>
        /// <param name="obj">The obj requesting to move.</param>
        /// <returns>True if there is a free grub, false if all grubs are in use.</returns>
        public static bool RequestGrub(MovableObject obj)
        {
            if (AvailableGrubs > 0)
            {
                GrubController spawnedGrub = Instantiate(instance.grubPrefab, instance.transform);
                // Spawn a grub.
                dispatchedGrubs.Add(obj, spawnedGrub);
                spawnedGrub.Initialize(obj.GridNavigator);
                UpdateText();
                return true;
            }
            else
            {
                // If no grubs are available, then we return false and the mover cannot move.
                return false;
            }
        }

        /// <summary>
        /// Returns a delegated grub to the pool of ready grubs
        /// </summary>
        /// <param name="obj">The moovable object that the grub was delegated to.</param>
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
        /// Updates the text that displays the number of available grubs.
        /// </summary>
        private static void UpdateText()
        {
            if (instance == null || instance.grubText == null) { return; }
            instance.grubText.text = $"{AvailableGrubs}/{MaxGrubCount}";
        }
    }
}