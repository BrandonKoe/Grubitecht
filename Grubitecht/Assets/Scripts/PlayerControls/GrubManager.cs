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

namespace Grubitecht
{
    public class GrubManager : MonoBehaviour
    {
        [SerializeField] private int maxGrubCount;
        [SerializeField] private GridFollower grubPrefab;

        public static int MaxGrubCount { get; private set; }
        public static int GrubCount { get; private set; }

        private static GrubManager instance;

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
            }
        }

        /// <summary>
        /// Requests a grub to move this object.
        /// </summary>
        /// <param name="obj">The obj requesting to move.</param>
        /// <returns>True if there is a free grub, false if all grubs are in use.</returns>
        public static bool RequestGrub(MovableObject obj)
        {
            if (GrubCount > 0)
            {

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}