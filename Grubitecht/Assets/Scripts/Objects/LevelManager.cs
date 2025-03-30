/*****************************************************************************
// File Name : LevelManager.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Manages operations that should be carried out once per level.
*****************************************************************************/
using Grubitecht.World.Objects;
using UnityEngine;

namespace Grubitecht.World
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameObject winLevelDisplay;
        [SerializeField] private GameObject loseLevelDisplay;
        private static LevelManager current;
        public static bool IsPlaying { get; private set; }

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
                IsPlaying = true;
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

        #region Win & Lose
        /// <summary>
        /// Handles behaviour that should happen when you win a level.
        /// </summary>
        public static void WinLevel()
        {
            current.winLevelDisplay.SetActive(true);
            IsPlaying = false;
        }

        /// <summary>
        /// Handles behaviour that should happen when you lose a level.
        /// </summary>
        public static void LoseLevel()
        {
            current.loseLevelDisplay.SetActive(true);
            IsPlaying = false;
        }
        #endregion
    }
}