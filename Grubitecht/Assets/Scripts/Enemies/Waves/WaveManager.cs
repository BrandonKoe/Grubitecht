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

namespace Grubitecht.Waves
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text waveTimerText;
        [SerializeField] private GameObject waveTimerObject;
        [SerializeField] private float delayBetweenWaves;
        [SerializeField] private float firstWaveDelay;
        private int totalWaves;
        private int completedSpawnPoints;
        private bool allEnemiesDead;
        
        private static WaveManager currentLevel;
        //public static event Action<int> StartWaveEvent;
        private SpawnPoint[] spawnPoints;
        private static readonly List<EnemyController> enemies = new List<EnemyController>();

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
        public static void AddEnemy(EnemyController enemy)
        {
            enemies.Add(enemy);
            if (currentLevel != null && currentLevel.allEnemiesDead)
            {
                currentLevel.allEnemiesDead = false;
            }
        }
        public static void RemoveEnemy(EnemyController enemy)
        {
            enemies.Remove(enemy);
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
                waveTimerObject.SetActive(true);
                while (waveDelayTimer > 0)
                {
                    // Update the UI here.
                    if (waveTimerText != null)
                    {
                        waveTimerText.text = ((int)waveDelayTimer + 1).ToString();
                    }
                    waveDelayTimer -= Time.deltaTime;
                    yield return null;
                }
                waveTimerObject.SetActive(false);

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
            }

            // Win the level once all waves are detfeated.
            LevelManager.WinLevel();
        }
    }
}