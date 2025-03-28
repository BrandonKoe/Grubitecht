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
    public delegate void MovementFinishCallback();
    [RequireComponent(typeof(GridObject))]
    public class GridNavigator : GridBehaviour
    {
        #region CONSTS
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
        /// <param name="finishCallback">
        /// A function to call when this object has finished moving.  Note that if a new destination is set while an
        /// object is already moving, then the finish callback will be overwritten with the finish callback
        /// for the new destination.
        /// </param>
        public void SetDestination(Vector3Int destinationSpace, bool includeAdjacent = false, 
            MovementFinishCallback finishCallback = null)
        {
            //Debug.Log("Set destination of object" + gameObject.name + " to " + destination);
            Vector3Int tileToStart = gridObject.CurrentSpace;
            currentPath = Pathfinder.FindPath(tileToStart, destinationSpace, jumpHeight, includeAdjacent);
            // Update stored values for destination and include adjacent settings.
            lastGivenDestination = destinationSpace;
            lastIncludeAdjSetting = includeAdjacent;

            if (movementRoutine != null)
            {
                StopCoroutine(movementRoutine);
                movementRoutine = null;
            }
            movementRoutine = StartCoroutine(MovementRoutine(finishCallback));
        }

        /// <summary>
        /// Stops this object's movement.
        /// </summary>
        public void StopMoving()
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
        /// Whenever the map refreshes, we need to check if the path this object is on has changed.
        /// </summary>
        /// <param name="movedObject">The object that was moved during this map refresh.</param>
        /// <param name="oldSpace">The old tile position of the moved object.</param>
        /// <param name="newSpace">The new tile position of the moved object.</param>"
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
        private IEnumerator MovementRoutine(MovementFinishCallback finishCallback)
        {
            while (currentPath.Count > 0)
            {
                float step = moveSpeed * Time.deltaTime;
                Vector3 tilePos = gridObject.GetOccupyPosition(currentPath[0]);

                PerformMove(step, tilePos);

                if (Vector3.Distance(transform.position, tilePos) < PATH_CLAMP)
                {
                    gridObject.SetCurrentSpace(currentPath[0]);
                    gridObject.SnapToSpace();
                    currentPath.RemoveAt(0);
                }

                yield return null;
            }
            // Yield an extra time here to prevent an infinite loop where the finish callback re-calls set destination.
            // This yield will turn that loop into a buffered loop each frame so it will still continue to loop, but
            // wont cause the computer to crash and instead will refresh each frame.
            yield return null;
            // Invoke the given finish callback.
            finishCallback?.Invoke();
            movementRoutine = null;
        }

        /// <summary>
        /// Moves this object towards it's next tile position by a given step.
        /// </summary>
        /// <remarks>
        /// Takes into account restricted movement type.
        /// </remarks>
        /// <param name="step">The amount to move towards the next tile position.</param>
        /// <param name="tilePos">The position of the next tile in the path.</param>
        private void PerformMove(float step, Vector3 tilePos)
        {
            if (restrictMovementAxes)
            {
                Vector3 pos = transform.position;
                if (tilePos.y > pos.y ||
                    (Mathf.Approximately(tilePos.x, pos.x) && Mathf.Approximately(tilePos.z, pos.z)))
                {
                    // If the next tile is higher than our current position or we are currently above our next tile,
                    // then move vertically.
                    pos.y = Mathf.MoveTowards(pos.y, tilePos.y, step);
                }
                else
                {
                    // Move horizontally otherwise.
                    pos.x = Mathf.MoveTowards(pos.x, tilePos.x, step);
                    pos.z = Mathf.MoveTowards(pos.z, tilePos.z, step);
                }
                transform.position = pos;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, tilePos, step);
            }
        }

        ///// <summary>
        ///// Continually moves an object along the path, but restricts movement to only 1 axis at a time.
        ///// </summary>
        ///// <returns>Coroutine.</returns>
        //private IEnumerator RestrictedMovementRoutine()
        //{
        //    while (currentPath.Count > 0)
        //    {
        //        float step = moveSpeed * Time.deltaTime;
        //        Vector3 tilePos = gridObject.GetOccupyPosition(currentPath[0]);

        //        // Change this line to restrict movement.
        //        //transform.position = Vector3.MoveTowards(transform.position, tilePos, step);
        //        Vector3 pos = transform.position;
        //        if (tilePos.y > pos.y ||
        //            (Mathf.Approximately(tilePos.x, pos.x) && Mathf.Approximately(tilePos.z, pos.z)))
        //        {
        //            // If the next tile is higher than our current position or we are currently above our next tile,
        //            // then move vertically.
        //            pos.y = Mathf.MoveTowards(pos.y, tilePos.y, step);
        //        }
        //        else
        //        {
        //            // Move horizontally otherwise.
        //            pos.x = Mathf.MoveTowards(pos.x, tilePos.x, step);
        //            pos.z = Mathf.MoveTowards(pos.z, tilePos.z, step);
        //        }
        //        transform.position = pos;

        //        if (Vector3.Distance(transform.position, tilePos) < PATH_CLAMP)
        //        {
        //            gridObject.SetCurrentSpace(currentPath[0]);
        //            gridObject.SnapToSpace();
        //            currentPath.RemoveAt(0);
        //        }

        //        yield return null;
        //    }

        //    movementRoutine = null;
        //}

        //private 
    }
}