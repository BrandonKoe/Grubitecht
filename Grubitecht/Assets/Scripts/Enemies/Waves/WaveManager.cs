/*****************************************************************************
// File Name : WaveManager.cs
// Author : Brandon Koederitz
// Creation Date : March 27, 2025
//
// Brief Description : Controls tracking enemies and coordinating when waves spawn and the delay between them.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.UI;
using Grubitecht.World;
using Grubitecht.World.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Grubitecht.Waves
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text waveTimerText;
        [SerializeField] private GameObject waveTimerObject;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private float delayBetweenWaves;
        [SerializeField] private float firstWaveDelay;
        private int totalWaves;
        private int completedSpawnPoints;
        private bool allEnemiesDead;
        
        private static WaveManager currentLevel;
        //public static bool IsPaused { get; set; }
        private static bool isPausedExternal;
        private static bool isPausedInternal;
        //public static event Action<int> StartWaveEvent;
        private SpawnPoint[] spawnPoints;
        private static readonly List<EnemyController> enemies = new List<EnemyController>();

        public static event Action OnFinishWave;

        #region Properties
        public static int EnemyNumber
        {
            get
            {
                return enemies.Count;
            }
        }
        public static SpawnPoint[] SpawnPoints
        {
            get
            {
                if (currentLevel == null) { return null; }
                return currentLevel.spawnPoints;
            }
        }
        public static bool IsPaused
        {
            get
            {
                return isPausedExternal || isPausedInternal;
            }
            set
            {
                isPausedExternal = value;
            }
        }    
        #endregion

        /// <summary>
        /// Assign and de-assign the wave manager for the current level when the object awakes and is destroyed.
        /// </summary>
        private void Awake()
        {
            if (currentLevel != null && currentLevel != this)
            {
                Debug.LogError("Duplicate WaveManager found.");
                return;
            }
            else
            {
                FindSpawnPoints();
                currentLevel = this;
                //// The wave should initially start out paused and only start once the player has started moving things.
                //SetPausedInternal(true);
                // Starts the wave routine for this level.
                StartCoroutine(WaveRoutine());
            }
        }
        private void OnDestroy()
        {
            if (currentLevel == this)
            {
                currentLevel = null;
                // Reset isPausedExternal on scene unload.
                isPausedExternal = false;
                SetPausedInternal(false);
            }
        }

        /// <summary>
        /// Sets if the wave manager is internally paused and should wait for player input before continuing.
        /// </summary>
        /// <param name="isPaused">WHether thw wave should be paused.</param>
        private void SetPausedInternal(bool isPaused)
        {
            isPausedInternal = isPaused;
            if (isPaused)
            {
                MovableObject.OnObjectMoveStatic += MovableObject_OnObjectMove;
            }
            else
            {
                MovableObject.OnObjectMoveStatic -= MovableObject_OnObjectMove;
            }
        }

        /// <summary>
        /// When the first object is moved, we should start the wave.
        /// </summary>
        /// <param name="obj"></param>
        private void MovableObject_OnObjectMove(MovableObject obj, VoxelTile endTile)
        {
            SetPausedInternal(false);
        }

        /// <summary>
        /// Calcualtes the total number of waves for this level based on the highest number of waves a spawn point has.
        /// </summary>
        private void FindSpawnPoints()
        {
            spawnPoints = FindObjectsOfType<SpawnPoint>();
            foreach (SpawnPoint spawnPoint in spawnPoints)
            {
                if (spawnPoint.WaveCount > totalWaves)
                {
                    totalWaves = spawnPoint.WaveCount;
                }
            }    
        }

        /// <summary>
        /// Adds/removes enemies from the enemy list.
        /// </summary>
        /// <param name="enemy">The enemy to add/remove</param>
        /// <param name="isSpawned">if this enemy was spawned outside of a normal wave.</param>
        public static void AddEnemy(EnemyController enemy, bool isSpawned = true)
        {
            enemies.Add(enemy);
            // Prevent null refs.
            if (currentLevel == null) { return; }
            if (isSpawned && currentLevel.progressBar != null)
            {
                currentLevel.progressBar.LogEnemyChange(1);
            }
            if (currentLevel != null && currentLevel.allEnemiesDead)
            {
                currentLevel.allEnemiesDead = false;
            }
        }
        public static void RemoveEnemy(EnemyController enemy)
        {

            enemies.Remove(enemy);
            // Prevents null refs.
            if (currentLevel == null) { return; }
            if (currentLevel.progressBar != null)
            {
                currentLevel.progressBar.LogEnemyChange(-1);
            }
            if (enemies.Count == 0 && currentLevel != null)
            {
                // Advances the wave routine.
                currentLevel.allEnemiesDead = true;
            }
        }

        /// <summary>
        /// Increments the number of spawn points that have finished their waves.  Once all spawn points have
        /// finished, then the next wave will start.
        /// </summary>
        public static void MarkFinishedWave()
        {
            if (currentLevel != null)
            {
                currentLevel.completedSpawnPoints++;
            }
        }

        /// <summary>
        /// Controls the progression of waves for this level.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaveRoutine()
        {
            bool isFirst = true;
            for (int waveNum = 0; waveNum < totalWaves; waveNum++)
            {
                allEnemiesDead = false;
                completedSpawnPoints = 0;

                float waveDelayTimer = isFirst ? delayBetweenWaves + firstWaveDelay : delayBetweenWaves;
                isFirst = false;
                // Has each wave display the upcoming enemies for the first subwave.
                foreach (SpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.DisplayPredictions(waveNum, waveDelayTimer);
                }

                // Enable the timer object when waiting for a wave, disable it after the wave begins.
                if (waveTimerObject != null)
                {
                    waveTimerObject.SetActive(true);
                
                }

                void UpdateUI()
                {
                    // Initially update the UI here.
                    if (waveTimerText != null)
                    {
                        waveTimerText.text = ((int)waveDelayTimer + 1).ToString();
                    }
                }

                UpdateUI();
                // Internally pause the wave manager at the start of each wave to give players time to think and input.
                SetPausedInternal(true);
                while (waveDelayTimer > 0)
                {
                    // Continually loop here while the wave manager is paused.
                    if (IsPaused) 
                    { 
                        yield return null;
                        continue; 
                    }
                    UpdateUI();
                    waveDelayTimer -= Time.deltaTime;
                    yield return null;
                }
                if (waveTimerObject != null)
                {
                    waveTimerObject.SetActive(false);
                }

                // Gets the total number of enemies in this wave and updates the progress bar to reflect that amount.
                int enemyAmount = 0;
                foreach (SpawnPoint spawnPoint in spawnPoints)
                {
                    enemyAmount += spawnPoint.GetEnemyCount(waveNum);
                }
                if (progressBar != null)
                {
                    progressBar.StartWave(enemyAmount);
                }

                // Has each spawn point start the next wave.
                foreach (SpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.StartWave(waveNum);
                }

                // Wait until the wave is set as cleared when the last enemy is killed.
                while (!allEnemiesDead || completedSpawnPoints < spawnPoints.Length)
                {
                    yield return null;
                }
                if (progressBar != null)
                {
                    progressBar.EndWave();
                }
                OnFinishWave?.Invoke();
            }

            // Win the level once all waves are detfeated.
            LevelManager.WinLevel();
        }
    }
}