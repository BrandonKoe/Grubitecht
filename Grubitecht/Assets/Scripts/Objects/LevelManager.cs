/*****************************************************************************
// File Name : LevelManager.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Manages operations that should be carried out once per level.
*****************************************************************************/
using Grubitecht.World.Objects;
using UnityEngine;
using Grubitecht.World.Pathfinding;
using NaughtyAttributes;
using Grubitecht.Audio;

namespace Grubitecht.World
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameObject winLevelDisplay;
        [SerializeField] private Sound winSound;
        [SerializeField] private GameObject loseLevelDisplay;
        [SerializeField] private Sound loseSound;
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
            //// Reset the navigation map when the level is unloaded & this object is destroyed.
            //Objective.NavMap.ResetMap();
            // Need to clear all the nodes the pathfinder has stored when the level ends.
            //Pathfinder.ClearNodes();
        }

        /// <summary>
        /// Bake the original objective navigation map in start for now.  Will likely move this to some OnLevelBegin
        /// function when proper transitions are in.
        /// </summary>
        private void Start()
        {
            //Objective.NavMap.CreateMap();
            //// Need to bake an initial nav map then start buffered updates later.
            //Objective.UpdateNavMap();
            //Objective.NavMap.StartUpdating(this);
        }

        #region Win & Lose
        /// <summary>
        /// Handles behaviour that should happen when you win a level.
        /// </summary>
        [Button]
        public static void WinLevel()
        {
            current.winLevelDisplay.SetActive(true);
            AudioManager.PlaySoundAtPosition(current.winSound, current.transform.position);
            IsPlaying = false;
        }

        /// <summary>
        /// Handles behaviour that should happen when you lose a level.
        /// </summary>
        [Button]
        public static void LoseLevel()
        {
            current.loseLevelDisplay.SetActive(true);
            AudioManager.PlaySoundAtPosition(current.loseSound, current.transform.position);
            IsPlaying = false;
        }
        #endregion
    }
}