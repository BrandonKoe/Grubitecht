/*****************************************************************************
// File Name : PathNavigator.cs
// Author : Brandon Koederitz
// Creation Date : March 18, 2025
//
// Brief Description : Allows a grid object to move along the grid using pathfinding.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.UI.InfoPanel;
using System;
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

        private List<VoxelTile> currentPath;
        private VoxelTile currentPathSpace;

        public event Action<VoxelTile> NewSpaceEvent;

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
        public void SetDestination(VoxelTile destinationSpace, bool includeAdjacent = false, 
            MovementFinishCallback finishCallback = null)
        {
            // If our destination is already occupied, then we should always include adjacent spaces.
            if (destinationSpace.ContainedObject == null)
            {
                includeAdjacent = true;
            }
            //Debug.Log("Set destination of object" + gameObject.name + " to " + destination);
            VoxelTile tileToStart = gridObject.CurrentSpace;
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
                VoxelTile endingSpace = currentPath[0];
                currentPath.Clear();
                currentPath.Add(endingSpace);
            }
        }

        /// <summary>
        /// Continually moves this object along a given path.
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator MovementRoutine(VoxelTile destination, bool includeAdj, 
            MovementFinishCallback finishCallback)
        {
            void UpdateDirection()
            {
                // Update direction here to ensure directions are updated for later code execution.
                if (currentPath.Count > 0)
                {
                    Direction = (currentPath[0].GridPosition2 - gridObject.CurrentSpace.GridPosition2);
                }
                else
                {
                    Direction = Vector2Int.zero;
                }
            }

            // If we're set to only update our current space once, then set it before we begin moving and double check
            // that placement later.
            if (!updateSpaceDuringPath)
            {
                gridObject.SetCurrentSpace(destination);
            }
            // Update our current space and direction at the beginning.
            if (currentPath.Count > 0)
            {
                currentPathSpace = gridObject.CurrentSpace;
                UpdateDirection();
            }
            while (currentPath.Count > 0)
            {
                // If the space we're attempting to move into is occupied, then we should attempt to find a new path.
                if (currentPath[0].ContainsObject)
                {
                    SetDestination(destination, includeAdj, finishCallback);
                    yield break;
                }
                Vector3 tilePos = gridObject.GetOccupyPosition(currentPath[0]);

                PerformMove(tilePos);

                if (Vector3.Distance(transform.position, tilePos) < SPACE_CLAMP)
                {
                    // Updates a var that keeps track of our current space in the path.
                    currentPathSpace = currentPath[0];
                    if (updateSpaceDuringPath)
                    {
                        gridObject.SetCurrentSpace(currentPathSpace);
                        gridObject.SnapToSpace();
                    }
                    currentPath.RemoveAt(0);
                    UpdateDirection();
                    // Broadcast out that this object has reached a new space.
                    NewSpaceEvent?.Invoke(currentPathSpace);

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
            currentPathSpace = null;
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