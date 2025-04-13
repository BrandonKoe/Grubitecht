/*****************************************************************************
// File Name : SpawnPoint.cs
// Author : Brandon Koederitz
// Creation Date : March 27, 2025
//
// Brief Description : Spawns waves of enemies.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.UI;
using Grubitecht.World;
using Grubitecht.World.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grubitecht.Waves
{
    [RequireComponent(typeof(GridObject))]
    public class SpawnPoint : MonoBehaviour
    {
        #region CONST
        private const string PREDICTOR_PARENT_TAG = "WavePredictors";
        #endregion
        [SerializeField] private WavePredictor predictorPrefab;
        [SerializeField] private float subwavePredictionTime = 15f;
        [SerializeField] private Wave[] waves;

        private static Transform predictorParent;

        #region Component Reference
        [SerializeReference, HideInInspector] private GridObject gridObject;
        /// <summary>
        /// Assign component references on restart.
        /// </summary>
        private void Reset()
        {
            gridObject = GetComponent<GridObject>();
        }
        #endregion

        #region Properties
        public int WaveCount
        {
            get
            {
                return waves.Length;
            }
        }
        private static Transform PredictorParent
        {
            get
            {
                if (predictorParent == null)
                {
                    predictorParent = GameObject.FindGameObjectWithTag(PREDICTOR_PARENT_TAG).transform;
                }
                return predictorParent;
            }
        }
        #endregion

        ///// <summary>
        ///// Subscribes/Unsubscrive starting a wave to the WaveManager's NewWaveEvent.
        ///// </summary>
        //private void Awake()
        //{
        //    WaveManager.StartWaveEvent += StartWave;
        //}
        //private void OnDestroy()
        //{
        //    WaveManager.StartWaveEvent -= StartWave;
        //}

        /// <summary>
        /// Starts a wave at a given index.
        /// </summary>
        /// <param name="waveIndex">The index of the wave to start.</param>
        public void StartWave(int waveIndex)
        {
            if (waves.Length > waveIndex && waves[waveIndex] != null)
            {
                StartCoroutine(WaveCoroutine(waves[waveIndex]));
            }
        }

        /// <summary>
        /// Gets the enemy count of a given wave.
        /// </summary>
        /// <param name="waveIndex">The index of the wave to get.</param>
        /// <returns>The number of enemies in that wave.</returns>
        public int GetEnemyCount(int waveIndex)
        {
            if (waves.Length > waveIndex && waves[waveIndex] != null)
            {
                return waves[waveIndex].EnemyCount;
            }
            return 0;
        }

        /// <summary>
        /// Loops through and spawns the enemies in a given wave over time.
        /// </summary>
        /// <param name="wave">The wave that is currently happening.</param>
        /// <returns>Coroutine</returns>
        private IEnumerator WaveCoroutine(Wave wave)
        {
            foreach (Wave.Subwave subwave in wave.Subwaves)
            {
                bool hasGivenPrediction = false;
                float timer = subwave.Delay;
                while (timer > 0)
                {
                    // Once the subwave is close to spawning, a prediction of the enemies that will be found in that
                    // subwave should be displayed.
                    timer -= Time.deltaTime;
                    if (timer < subwavePredictionTime && !hasGivenPrediction)
                    {
                        DisplayPredictions(subwave, subwavePredictionTime);
                        hasGivenPrediction = true;
                    }
                    yield return null;
                }
                //yield return new WaitForSeconds(subwave.Delay);

                // Stop spawning waves if the level is finished.
                if (!LevelManager.IsPlaying)
                {
                    yield break;
                }
                SpawnSubwave(subwave);
            }
            // Wave is finished.
            WaveManager.MarkFinishedWave();
        }

        /// <summary>
        /// Displays the enemies in the upcoming wave on the UI.
        /// </summary>
        /// <param name="waveIndex">The wave to display predictions for.</param>
        /// <param name="time">The amount of time before the wave spawns.</param>
        public void DisplayPredictions(int waveIndex, float time)
        {
            // Return if we dont have a wave at WaveIndex.
            if (waveIndex >= waves.Length) { return; }
            Wave.Subwave subwave = waves[waveIndex].Subwaves.FirstOrDefault();
            DisplayPredictions(subwave, time + subwave.Delay);
        }

        /// <summary>
        /// Displays the enemies in the upcoming wave on the UI.
        /// </summary>
        /// <param name="subwave">The subwave to display a prediction for.</param>
        /// <param name="time">The amount of time before the wave spawns.</param>
        private void DisplayPredictions(Wave.Subwave subwave, float time)
        {
            for(int i = 0; i < subwave.Enemies.Length; i++) 
            {
                Wave.EnemyType enemy = subwave.Enemies[i];
                WavePredictor predictor = Instantiate(predictorPrefab, PredictorParent);
                predictor.Initialize(enemy.EnemyPrefab.EnemySpriteIcon, transform.position, enemy.Count, time, i);
            }
        }

        /// <summary>
        /// Spawns a given subwave over time.
        /// </summary>
        /// <param name="subwave">The subwavea to spawn.</param>
        /// <returns>Coroutine.</returns>
        private void SpawnSubwave(Wave.Subwave subwave)
        {
            // Spawns each enemy with the designated quantity.
            foreach (Wave.EnemyType enemy in subwave.Enemies)
            {
                for (int i  = 0; i < enemy.Count; i++)
                {
                    EnemyController.SpawnEnemy(enemy.EnemyPrefab, gridObject.CurrentTile);
                }
            }
        }
    }
}
