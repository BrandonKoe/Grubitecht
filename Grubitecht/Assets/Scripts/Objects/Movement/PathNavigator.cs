/*****************************************************************************
// File Name : PathNavigator.cs
// Author : Brandon Koederitz
// Creation Date : March 18, 2025
//
// Brief Description : Allows a grid object to move along the grid using pathfinding.
*****************************************************************************/
using Grubitecht.Audio;
using Grubitecht.Tilemaps;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.World.Pathfinding
{
    public delegate void MovementFinishCallback(PathCallbackInfo endStatus);
    public enum PathStatus
    {
        Started,
        Completed,
        Invalid
    }
    public class PathCallbackInfo
    {
        private readonly PathStatus status;
        private readonly VoxelTile startTile;
        private readonly VoxelTile endTile;

        #region Properties
        public PathStatus Status => status;
        public VoxelTile StartTile => startTile;
        public VoxelTile EndTile => endTile;
        #endregion

        internal PathCallbackInfo(PathStatus status, VoxelTile startTile, VoxelTile endTile)
        {
            this.status = status;
            this.startTile = startTile;
            this.endTile = endTile;
        }
    }
    public class PathNavigator : GridNavigator
    {
        //    [SerializeField, Tooltip("Whether this object should update it's grid space while it is moving along a " +
        //"path, or immediately as soon as it starts moving.")]
        //    protected bool updateSpaceDuringPath;
        [Header("Sounds")]
        [SerializeField] private Sound startMovingSound;
        [SerializeField] private Sound endMovingSound;

        private List<VoxelTile> currentPath;
        private VoxelTile currentPathSpace;

        public event Action<VoxelTile> NewSpaceEvent;

        /// <summary>
        /// Moves an object to a target destination on the grid.
        /// </summary>
        /// <param name="destinationSpace">The tile to move to.</param>
        /// <param name="finishCallback">
        /// A function to call when this object has finished moving.  Note that if a new destination is set while an
        /// object is already moving, then the finish callback will be overwritten with the finish callback
        /// for the new destination.
        /// </param>
        public void SetDestination(VoxelTile destinationSpace, MovementFinishCallback finishCallback = null)
        {
            // By default, dont include adjacent spaces.
            bool includeAdjacent = false;
            // If our destination is already occupied, then we should include adjacent spaces.
            if (destinationSpace.ContainsObjectOnLayer(gridObject.Layer))
            {
                includeAdjacent = true;
            }
            //Debug.Log("Set destination of object" + gameObject.name + " to " + destination);
            VoxelTile tileToStart = gridObject.CurrentTile;
            // Use a buffer here so we can check that our path is valid before we use it as our current path.
            List<VoxelTile> pathBuffer = Pathfinder.FindPath(tileToStart, destinationSpace, climbHeight, 
                gridObject.Layer, includeAdjacent);

            // If the current path is empty, then there isnt a valid path to the given destination we should let the
            // callback know that there is no valid path.
            if (pathBuffer == null)
            {
                //Debug.Log("Invalid path");
                finishCallback?.Invoke(new PathCallbackInfo(PathStatus.Invalid, tileToStart, destinationSpace));
                return;
            }

            // If our current path contains no elements, then we are already at our destination.
            if (pathBuffer.Count == 0)
            {
                pathBuffer.Add(gridObject.CurrentTile);
            }

            if (movementRoutine != null)
            {
                StopCoroutine(movementRoutine);
                movementRoutine = null;
            }
            currentPath = pathBuffer;
            movementRoutine = StartCoroutine(MovementRoutine(finishCallback));
        }

        /// <summary>
        /// Stops this object's movement.
        /// </summary>
        public override void StopMoving()
        {
            if (movementRoutine != null && currentPath.Count > 0)
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
        private IEnumerator MovementRoutine(MovementFinishCallback finishCallback)
        {
            // Plays a sound for this object to start moving.
            AudioManager.PlaySoundAtPosition(startMovingSound, transform.position);
            VoxelTile destination = null;
            void UpdateDirection()
            {
                // Update direction here to ensure directions are updated for later code execution.
                if (currentPath.Count > 0)
                {
                    Direction = (currentPath[0].GridPosition2 - gridObject.CurrentTile.GridPosition2);
                }
                else
                {
                    Direction = Vector2Int.zero;
                }
            }

            // If we're set to only update our current space once, then set it before we begin moving and double check
            // that placement later.
            //if (!updateSpaceDuringPath)
            //{
            //    gridObject.SetCurrentSpace(destination);
            //}

            // We can make the assumption that our path has at least 1 space in it because SetDestination filters out
            // empty paths.
            destination = currentPath[^1];
            VoxelTile startSpace = currentPath[0];
            currentPathSpace = gridObject.CurrentTile;
            UpdateDirection();
            finishCallback?.Invoke(new PathCallbackInfo(PathStatus.Started, currentPath[0], destination));

            //Debug.Log(currentPathSpace.GridPosition);
            //Debug.Log(currentPath[0].GridPosition);

            while (currentPath.Count > 0)
            {
                //Debug.Log(currentPath.Count);
                // If the space we're attempting to move into is occupied, then we should attempt to find a new path.
                if (currentPath[0].ContainsObjectOnLayer(gridObject.Layer, gridObject))
                {
                    SetDestination(destination, finishCallback);
                    yield break;
                }
                Vector3 tilePos = gridObject.GetOccupyPosition(currentPath[0]);

                PerformMove(tilePos);

                if (Vector3.Distance(transform.position, tilePos) < SPACE_CLAMP)
                {
                    //Debug.Log("Updated space");
                    // Updates a var that keeps track of our current space in the path.
                    currentPathSpace = currentPath[0];
                    //Debug.Log(currentPathSpace.GridPosition);
                    gridObject.SetCurrentSpace(currentPathSpace);
                    gridObject.SnapToSpace();
                    //if (updateSpaceDuringPath)
                    //{
                        
                    //}
                    currentPath.RemoveAt(0);
                    UpdateDirection();
                    // Broadcast out that this object has reached a new space.
                    NewSpaceEvent?.Invoke(currentPathSpace);
                }

                yield return null;
                // After the yield, check to make sure our path is still valid.  It should be, but an extra check
                // to prevent errors is good
                if (currentPath == null)
                {
                    //Debug.Log("Invalid path");
                    finishCallback?.Invoke(new PathCallbackInfo(PathStatus.Invalid, startSpace, destination));
                    yield break;
                }
            }
            //// Yield an extra time here to prevent an infinite loop where the finish callback re-calls set destination.
            //// This yield will turn that loop into a buffered loop each frame so it will still continue to loop, but
            //// wont cause the computer to crash and instead will refresh each frame.
            //yield return null;
            //bool reachedDestination = destination == currentPathSpace;
            //// Double check our current space is correct.  If we ended at a space other than our destination, then we
            //// should set our current space to that space instead.
            //if (!updateSpaceDuringPath && !reachedDestination)
            //{
            //    gridObject.SetCurrentSpace(currentPathSpace);
            //    gridObject.SnapToSpace();
            //}
            currentPathSpace = null;
            // Plays a sound for this object to start moving.
            AudioManager.PlaySoundAtPosition(endMovingSound, transform.position);
            // Invoke the given finish callback.
            finishCallback?.Invoke(new PathCallbackInfo(PathStatus.Completed, startSpace, destination));
            movementRoutine = null;
        }
    }
}