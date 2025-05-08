/*****************************************************************************
// File Name : LevelManager.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Manages operations that should be carried out once per level.
*****************************************************************************/
using Grubitecht.Audio;
using NaughtyAttributes;
using UnityEngine;

namespace Grubitecht.World
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Level Settings")]
        [SerializeField] private int requiredObjectiveAmount;
        [Header("Level End")]
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

        #region Properties
        private static LevelManager Current
        {
            get
            {
                if (current == null)
                {
                    current = FindObjectOfType<LevelManager>();
                }
                return current;
            }
        }
        public static int RequiredObjectiveAmount
        {
            get
            {
                return Current.requiredObjectiveAmount;
            }
        }
        #endregion

        #region Win & Lose
        /// <summary>
        /// Handles behaviour that should happen when you win a level.
        /// </summary>
        [Button]
        public static void WinLevel()
        {
            Current.winLevelDisplay.SetActive(true);
            AudioManager.PlaySoundAtPosition(Current.winSound, Current.transform.position);
            IsPlaying = false;
        }

        /// <summary>
        /// Handles behaviour that should happen when you lose a level.
        /// </summary>
        [Button]
        public static void LoseLevel()
        {
            Current.loseLevelDisplay.SetActive(true);
            AudioManager.PlaySoundAtPosition(Current.loseSound, Current.transform.position);
            IsPlaying = false;
        }
        #endregion
    }
}