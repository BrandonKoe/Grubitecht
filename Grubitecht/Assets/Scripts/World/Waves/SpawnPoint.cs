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
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

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
            // End of the wave.
        }

        /// <summary>
        /// Spawns a given subwave
        /// </summary>
        /// <param name="subwave">The subwavea to spawn.</param>
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
                    int yRange = range - x;
                    // Loops through both positive and negative values for y that yields a manhatten distance of
                    // range from the spawn point.
                    for (int y = -range; y <= range; y *= -1)
                    {
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
                            return checkCell;
                        }
                    }
                }
                // If we were not able to find a cell through looping, then increment range and recursively call this
                // function agian.
                range++;
                return FindPosition();
            }

            foreach (EnemyController enemy in subwave.Enemies)
            {
                EnemyController spawnedEnemy = Instantiate(enemy, EnemyParent);

                spawnedEnemy.GridObject.SetCurrentSpace(FindPosition());
                spawnedEnemy.GridObject.SnapToSpace();
            }
        }
    }
}
