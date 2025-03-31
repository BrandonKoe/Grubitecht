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
        [SerializeField] private float delayBetweenWaves;
        private int totalWaves;
        private int waveNum;
        private int totalSpawnPoints;
        private int completedSpawnPoints;
        private bool allEnemiesDead;
        
        private static WaveManager currentLevel;
        public static event Action<int> StartWaveEvent;
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
            SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
            foreach (SpawnPoint spawnPoint in spawnPoints)
            {
                totalSpawnPoints++;
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
            while (waveNum < totalWaves)
            {
                allEnemiesDead = false;
                completedSpawnPoints = 0;

                float waveDelayTimer = delayBetweenWaves;
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

                StartWaveEvent?.Invoke(waveNum);

                // Wait until the wave is set as cleared when the last enemy is killed.
                while (!allEnemiesDead || completedSpawnPoints < totalSpawnPoints)
                {
                    yield return null;
                }

                waveNum++;
            }

            // Win the level once all waves are detfeated.
            LevelManager.WinLevel();
        }
    }
}