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
        private const string ENEMY_PARENT_TAG = "EnemyParent";
        private const string PREDICTOR_PARENT_TAG = "WavePredictors";
        #endregion
        [SerializeField] private WavePredictor predictorPrefab;
        [SerializeField] private float subwavePredictionTime = 15f;
        [SerializeField] private Wave[] waves;

        private static Transform enemyParent;
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
        private static Transform EnemyParent
        {
            get
            {
                if (enemyParent == null)
                {
                    enemyParent = GameObject.FindGameObjectWithTag(ENEMY_PARENT_TAG).transform;
                }
                return enemyParent;
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
            if (waves.Length > waveIndex)
            {
                StartCoroutine(WaveCoroutine(waves[waveIndex]));
            }
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
            int range = 0;
            List<Vector3Int> usedPositions = new List<Vector3Int>();
            // Finds a new position to spawn an enemy at that projects outward from the spawn point's position.
            Vector3Int FindPosition()
            {
                for (int x = -range; x <= range; x++)
                {
                    // Find the possible variance in y positions for a given range.
                    int yRange = range - Mathf.Abs(x);
                    // Loops through both positive and negative values for y that yields a manhatten distance of
                    // range from the spawn point.
                    for (int i = -1; i < 2; i += 2)
                    {
                        int y = i * yRange;
                        // Check the positive and negative cells that have a manhatten distance of range.
                        Vector2Int checkPos = new Vector2Int(x, y) + (Vector2Int)gridObject.CurrentSpace;
                        Vector3Int checkCell = VoxelTilemap3D.Main_GetClosestCellInColumn(checkPos,
                            (Vector3Int)gridObject.CurrentSpace, GridObject.VALID_GROUND_TYPE);
                        // If checkCell is returned as zero, then the cell we're trying to get does not exist on the
                        // tilemap and we should ignore it.
                        if (checkCell == Vector3Int.zero && checkPos != Vector2Int.zero)
                        {
                            continue;
                        }
                        // If this position hasnt already been used for this subwave, then return it.
                        if (!usedPositions.Contains(checkCell))
                        {
                            usedPositions.Add(checkCell);
                            return checkCell;
                        }
                    }
                }
                // If we were not able to find a cell through looping, then increment range and recursively call this
                // function agian.
                range++;
                return FindPosition();
            }

            // Spawns each enemy with the designated quantity.
            foreach (Wave.EnemyType enemy in subwave.Enemies)
            {
                for (int i  = 0; i < enemy.Count; i++)
                {
                    EnemyController spawnedEnemy = Instantiate(enemy.EnemyPrefab, EnemyParent);

                    //spawnedEnemy.name = spawnedEnemy.name + i;
                    Vector3Int pos = FindPosition();
                    spawnedEnemy.gridObject.SetCurrentSpace(pos);
                    spawnedEnemy.gridObject.SnapToSpace();
                    spawnedEnemy.PathToNearestObjective();
                    //Debug.Log("Spawned enemy " + spawnedEnemy.name+ " at position " + pos);
                }
            }
        }
    }
}
