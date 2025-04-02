/*****************************************************************************
// File Name : PathNavigator.cs
// Author : Brandon Koederitz
// Creation Date : March 18, 2025
//
// Brief Description : Allows a grid object to move along the grid using pathfinding.
*****************************************************************************/
using Grubitecht.UI.InfoPanel;
using Grubitecht.World.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.World.Pathfinding
{
    public delegate void MovementFinishCallback();
    public class PathNavigator : GridNavigator, IInfoProvider
    {
        [SerializeField, Tooltip("Whether this object should update it's grid space while it is moving along a " +
    "path, or immediately as soon as it starts moving.")]
        protected bool updateSpaceDuringPath;

        private List<Vector3Int> currentPath;
        private Vector3Int currentPathSpace;

        #region Propeties
        public Vector2Int Direction { get; private set; }
        #endregion

        /// <summary>
        /// Moves an object to a target destination on the grid.
        /// </summary>
        /// <param name="destinationSpace">The tile to move to.</param>
        /// <param name="includeAdjacent">
        /// If true, then this object will pathfind to a tile that is adjacent to the given destination tile.
        /// </param>
        /// <param name="finishCallback">
        /// A function to call when this object has finished moving.  Note that if a new destination is set while an
        /// object is already moving, then the finish callback will be overwritten with the finish callback
        /// for the new destination.
        /// </param>
        public void SetDestination(Vector3Int destinationSpace, bool includeAdjacent = false, 
            MovementFinishCallback finishCallback = null)
        {
            // If our destination is already occupied, then we sould always include adjacent spaces.
            if (GridObject.CheckOccupied(destinationSpace))
            {
                includeAdjacent = true;
            }
            //Debug.Log("Set destination of object" + gameObject.name + " to " + destination);
            Vector3Int tileToStart = gridObject.CurrentSpace;
            currentPath = Pathfinder.FindPath(tileToStart, destinationSpace, climbHeight, includeAdjacent);
            // Dont move if our current path is empty.
            if (currentPath.Count == 0)
            {
                return;
            }
            if (movementRoutine != null)
            {
                StopCoroutine(movementRoutine);
                movementRoutine = null;
            }
            movementRoutine = StartCoroutine(MovementRoutine(currentPath[^1], includeAdjacent, finishCallback));
        }

        /// <summary>
        /// Stops this object's movement.
        /// </summary>
        public override void StopMoving()
        {
            if (movementRoutine != null)
            {
                //StopCoroutine(movementRoutine);
                //movementRoutine = null;
                // Instead of instantly stopping movement, we should finish snapping to whatever space we are
                // currently at.
                Vector3Int endingSpace = currentPath[0];
                currentPath.Clear();
                currentPath.Add(endingSpace);
            }
        }

        /// <summary>
        /// Continually moves this object along a given path.
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator MovementRoutine(Vector3Int destination, bool includeAdj, 
            MovementFinishCallback finishCallback)
        {
            // If we're set to only update our current space once, then set it before we begin moving and double check
            // that placement later.
            if (!updateSpaceDuringPath)
            {
                gridObject.SetCurrentSpace(destination);
            }
            while (currentPath.Count > 0)
            {
                // If the space we're attempting to move into is occupied, then we should attempt to find a new path.
                if (GridObject.CheckOccupied(currentPath[0]))
                {
                    SetDestination(destination, includeAdj, finishCallback);
                    yield break;
                }
                Vector3 tilePos = gridObject.GetOccupyPosition(currentPath[0]);

                PerformMove(tilePos);

                if (Vector3.Distance(transform.position, tilePos) < SPACE_CLAMP)
                {
                    if (updateSpaceDuringPath)
                    {
                        gridObject.SetCurrentSpace(currentPath[0]);
                        gridObject.SnapToSpace();
                    }
                    else
                    {
                        // Updates a var that keeps track of our current space in the path.
                        currentPathSpace = currentPath[0];
                    }
                    currentPath.RemoveAt(0);
                    // Update direction here to ensure directions are updated for later code execution.
                    if (currentPath.Count > 0)
                    {
                        Direction = (Vector2Int)(currentPath[0] - gridObject.CurrentSpace);
                    }
                    else
                    {
                        Direction = Vector2Int.zero;
                    }
                }

                yield return null;
            }
            // Yield an extra time here to prevent an infinite loop where the finish callback re-calls set destination.
            // This yield will turn that loop into a buffered loop each frame so it will still continue to loop, but
            // wont cause the computer to crash and instead will refresh each frame.
            yield return null;
            // Double check our current space is correct.  If we ended at a space other than our destination, then we
            // should set our current space to that space instead.
            if (!updateSpaceDuringPath && destination != currentPathSpace)
            {
                gridObject.SetCurrentSpace(currentPathSpace);
                gridObject.SnapToSpace();
            }
            // Invoke the given finish callback.
            finishCallback?.Invoke();
            movementRoutine = null;
        }

        public InfoValueBase[] InfoGetter()
        {
            return new InfoValueBase[]
            {
                new NumValue(MoveSpeed, 90, "Movement Speed"),
                new NumValue(climbHeight, 91, "Climb Height")
            };
        }
    }
}