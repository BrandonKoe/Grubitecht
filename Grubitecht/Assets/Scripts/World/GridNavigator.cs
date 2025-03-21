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
using Grubitecht.World.Objects;

namespace Grubitecht.World.Pathfinding
{
    [RequireComponent(typeof(GridObject))]
    public class GridNavigator : GridBehaviour
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
        private int jumpHeight;
        [SerializeField, Tooltip("Whether this object should ignore spaces that are blocked when navigating the " +
            "world")] 
        private bool ignoreBlockedSpaces;
        [SerializeField, Tooltip("If checked, then this object is only able to move in one cardinal direction " +
            "while moving along a path.")] 
        private bool restrictMovementAxes;


        #region Component References
        [SerializeReference, HideInInspector] private GridObject gridObject;
        #endregion
        private Coroutine movementRoutine;
        private List<Vector3Int> currentPath;
        // Store references to the last values passed into SetDestination so that we can reuse them when recalculating
        // a path to that destination if the path is obstructed.
        private Vector3Int lastGivenDestination;
        private bool lastIncludeAdjSetting;

        #region Propeties
        public bool IsMoving
        {
            get
            {
                return movementRoutine != null;
            }
        }
        #endregion

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
        /// <param name="destinationSpace">The tile to move to.</param>
        /// <param name="includeAdjacent">
        /// If true, then this object will pathfind to a tile that is adjacent to the given destination tile.
        /// </param>
        public void SetDestination(Vector3Int destinationSpace, bool includeAdjacent = false)
        {
            //Debug.Log("Set destination of object" + gameObject.name + " to " + destination);
            Vector3Int tileToStart = JUMP_TO_TARGET ? gridObject.GetApproximateSpace() : gridObject.CurrentSpace;
            currentPath = Pathfinder.FindPath(tileToStart, destinationSpace, jumpHeight, includeAdjacent);
            // Update stored values for destination and include adjacent settings.
            lastGivenDestination = destinationSpace;
            lastIncludeAdjSetting = includeAdjacent;

            if (movementRoutine == null)
            {
                if (restrictMovementAxes)
                {
                    movementRoutine = StartCoroutine(LinearMovementRoutine());
                }
                else
                {
                    movementRoutine = StartCoroutine(LinearMovementRoutine());
                }
                
            }
        }

        /// <summary>
        /// Stops this object's movement.
        /// </summary>
        public void StopMoving()
        {
            if (movementRoutine != null)
            {
                StopCoroutine(movementRoutine);
                movementRoutine = null;
            }
        }

        /// <summary>
        /// Whenever the map refreshes, we need to check if the path this object is on has changed.
        /// </summary>
        /// <param name="movedObject">The object that was moved during this map refresh.</param>
        /// <param name="oldSpace">The old tile position of the moved object.</param>
        /// <param name="newSpace">The new tile position of the moved object.</param>
        protected override void OnMapRefresh(GridObject movedObject, Vector3Int oldSpace, Vector3Int newSpace)
        {
            // Never run this function if the moved object is this object.
            if (movedObject == gridObject) { return; }
            // If a point on our path is now occupied, then we must calculate a new path.
            if (currentPath != null && currentPath.Contains(newSpace))
            {
                SetDestination(lastGivenDestination, lastIncludeAdjSetting);
            }
        }

        /// <summary>
        /// Continually moves this object along a given path.
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator LinearMovementRoutine()
        {
            // If jump to target is set to true, then this object is considered to be on the last space in the path
            // immediately as it starts moving.
            if (JUMP_TO_TARGET && currentPath.Count > 1)
            {
                gridObject.SetCurrentSpace(currentPath[^1]);
            }
            while (currentPath.Count > 0)
            {
                float step = moveSpeed * Time.deltaTime;
                Vector3 tilePos = gridObject.GetOccupyPosition(currentPath[0]);

                transform.position = Vector3.MoveTowards(transform.position, tilePos, step);

                if (Vector3.Distance(transform.position, tilePos) < PATH_CLAMP)
                {
                    // If jump to target isnt set to true, then this object updates it's current space as it moves along the path.
                    if (!JUMP_TO_TARGET)
                    {
                        gridObject.SetCurrentSpace(currentPath[0]);
                    }
                    currentPath.RemoveAt(0);
                }

                yield return null;
            }

            movementRoutine = null;
        }

        /// <summary>
        /// Continually moves an object along the path, but restricts movement to only 1 axis at a time.
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator RestrictedMovementRoutine()
        {
            // If jump to target is set to true, then this object is considered to be on the last space in the path
            // immediately as it starts moving.
            if (JUMP_TO_TARGET && currentPath.Count > 1)
            {
                gridObject.SetCurrentSpace(currentPath[^1]);
            }
            bool moveVertical = false;
            while (currentPath.Count > 0)
            {
                float step = moveSpeed * Time.deltaTime;
                Vector3 tilePos = gridObject.GetOccupyPosition(currentPath[0]);

                // Change this line to restrict movement.
                transform.position = Vector3.MoveTowards(transform.position, tilePos, step);

                if (Vector3.Distance(transform.position, tilePos) < PATH_CLAMP)
                {
                    // If jump to target isnt set to true, then this object updates it's current space as it moves along the path.
                    if (!JUMP_TO_TARGET)
                    {
                        gridObject.SetCurrentSpace(currentPath[0]);
                    }
                    currentPath.RemoveAt(0);
                }

                yield return null;
            }

            movementRoutine = null;
        }
    }
}