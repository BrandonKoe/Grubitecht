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
using System.Linq;
using UnityEditor.UIElements;

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
                Debug.LogError("There are no more grubs left to assign.");
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
        private static void UpdateText()
        {
            if (instance == null || instance.grubText == null) { return; }
            instance.grubText.text = $"{AvailableGrubs}/{MaxGrubCount}";
        }
    }
}