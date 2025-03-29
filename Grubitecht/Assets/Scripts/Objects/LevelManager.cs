/*****************************************************************************
// File Name : LevelManager.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Manages operations that should be carried out once per level.
*****************************************************************************/
using Grubitecht.World.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.World
{
    public class LevelManager : MonoBehaviour
    {
        private static LevelManager current;

        /// <summary>
        /// Assign/Deassign the singleton instance.
        /// </summary>
        private void Awake()
        {
            if (current != null && current != this)
            {
                Debug.Log("Duplicate LevelManger found.");
                return;
            }
            else
            {
                current = this;
            }
        }
        private void OnDestroy()
        {
            if (current == this)
            {
                current = null;
            }
            // Reset the navigation map when the level is unloaded & this object is destroyed.
            Objective.NavMap.ResetMap();
        }

        /// <summary>
        /// Bake the original objective navigation map in start for now.  Will likely move this to some OnLevelBegin
        /// function when proper transitions are in.
        /// </summary>
        private void Start()
        {
            Objective.NavMap.CreateMap();
            Objective.UpdateNavMap();
        }
    }
}