/*****************************************************************************
// File Name : GridNavigator.cs
// Author : Brandon Koederitz
// Creation Date : March 18, 2025
//
// Brief Description : Allows a grid object to move along the grid using pathfinding.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.World.Pathfinding
{
    [RequireComponent(typeof(GridObject))]
    public class GridNavigator : MonoBehaviour
    {
        #region CONSTS
        // Determines how these objects consider what space they are on while moving.
        // If true, then while moving they are considered to already be at their new position.
        // If false, then they are considered to be on whatever space they are currently at along their path.
        // This setting primarily affects how enemies will pathfind.
        // This is not set as const purely to stop VS from yelling at me that some code us unreachable.
        private static readonly bool JUMP_TO_TARGET = true;
        private const float PATH_CLAMP = 0.001f;
        #endregion

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed;
        [SerializeField, Tooltip("How large of an upward incline this object can move up.")] 
        private float jumpHiehgt;


        #region Component References
        [SerializeReference, HideInInspector] private GridObject gridObject;
        #endregion
        private Coroutine movementRoutine;

        /// <summary>
        /// Assign necessary component references on reset.
        /// </summary>
        private void Reset()
        {
            gridObject = GetComponent<GridObject>();
        }

        /// <summary>
        /// Moves an object to a target destination on the grid.
        /// </summary>
        /// <param name="tile">The tile to move to.</param>
        public void SetDestination(GroundTile tile)
        {
            if (movementRoutine != null)
            {
                StopCoroutine(movementRoutine);
                movementRoutine = null;
            }

            movementRoutine = StartCoroutine(MovementRoutine(
                Pathfinder.FindPath(gridObject.CurrentSpace, tile, jumpHiehgt)));
        }

        private IEnumerator MovementRoutine(List<GroundTile> path)
        {
            // If jump to target is set to true, then this object is considered to be on the last space in the path
            // immediately as it starts moving.
            if (JUMP_TO_TARGET && path.Count > 1)
            {
                gridObject.SetCurrentSpace(path[^1]);
            }
            while (path.Count > 0)
            {
                float step = moveSpeed * Time.deltaTime;
                Vector3 tilePos = gridObject.GetTilePosition(path[0]);

                transform.position = Vector3.MoveTowards(transform.position, tilePos, step);

                if (Vector3.Distance(transform.position, tilePos) < PATH_CLAMP)
                {
                    // If jump to target isnt set to true, then this object updates it's current space as it moves along the path.
                    if (!JUMP_TO_TARGET)
                    {
                        gridObject.SetCurrentSpace(path[0]);
                    }
                    path.RemoveAt(0);
                }

                yield return null;
            }

            movementRoutine = null;
        }
    }
}