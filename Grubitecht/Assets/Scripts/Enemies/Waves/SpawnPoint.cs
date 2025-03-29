/*****************************************************************************
// File Name : SpawnPoint.cs
// Author : Brandon Koederitz
// Creation Date : March 27, 2025
//
// Brief Description : Spawns waves of enemies.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.World;
using Grubitecht.World.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Waves
{
    [RequireComponent(typeof(GridObject))]
    public class SpawnPoint : MonoBehaviour
    {
        #region CONST
        private const string ENEMY_PARENT_TAG = "EnemyParent";
        #endregion
        [SerializeField] private Wave[] waves;

        private static Transform enemyParent;

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
        #endregion

        #region Properties
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
        #endregion

        /// <summary>
        /// Subscribes/Unsubscrive starting a wave to the WaveManager's NewWaveEvent.
        /// </summary>
        private void Awake()
        {
            WaveManager.StartWaveEvent += StartWave;
        }
        private void OnDestroy()
        {
            WaveManager.StartWaveEvent -= StartWave;
        }

        /// <summary>
        /// Starts a wave at a given index.
        /// </summary>
        /// <param name="waveIndex">The index of the wave to start.</param>
        private void StartWave(int waveIndex)
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
                yield return new WaitForSeconds(subwave.Delay);

                SpawnSubwave(subwave);
            }
            // Wave is finished.
            WaveManager.MarkFinishedWave();
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
                            gridObject.CurrentSpace, GridObject.VALID_GROUND_TYPE);
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
                    spawnedEnemy.StartMoving();
                    //Debug.Log("Spawned enemy " + spawnedEnemy.name+ " at position " + pos);
                }
            }
        }
    }
}
