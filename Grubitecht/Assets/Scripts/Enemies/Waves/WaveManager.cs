/*****************************************************************************
// File Name : WaveManager.cs
// Author : Brandon Koederitz
// Creation Date : March 27, 2025
//
// Brief Description : Controls tracking enemies and coordinating when waves spawn and the delay between them.
*****************************************************************************/
using Grubitecht.World;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Grubitecht.UI;

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
        public static bool IsPaused { get; set; }
        //public static event Action<int> StartWaveEvent;
        private SpawnPoint[] spawnPoints;
        private static readonly List<EnemyController> enemies = new List<EnemyController>();

        #region Properties
        public static int EnemyNumber
        {
            get
            {
                return enemies.Count;
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
                // Starts the wave routine for this level.
                StartCoroutine(WaveRoutine());
            }
        }
        private void OnDestroy()
        {
            if (currentLevel == this)
            {
                currentLevel = null;
            }
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
                while (waveDelayTimer > 0)
                {
                    // Continually loop here while the wave manager is paused.
                    if (IsPaused) 
                    { 
                        yield return null;
                        continue; 
                    }
                    // Update the UI here.
                    if (waveTimerText != null)
                    {
                        waveTimerText.text = ((int)waveDelayTimer + 1).ToString();
                    }
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
            }

            // Win the level once all waves are detfeated.
            LevelManager.WinLevel();
        }
    }
}