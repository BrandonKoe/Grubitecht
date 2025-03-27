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
using Grubitecht.Tilemaps;

namespace Grubitecht.World.Pathfinding
{
    public delegate void MovementFinishCallback();
    [RequireComponent(typeof(GridObject))]
    public class GridNavigator : MonoBehaviour
    {
        #region CONSTS
        private const float PATH_CLAMP = 0.001f;
        #endregion

        [field: Header("Movement Settings")]
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [SerializeField, Tooltip("How large of an upward incline this object can move up.")] 
        private int jumpHeight;
        [SerializeField, Tooltip("Whether this object should ignore spaces that are blocked when navigating the " +
            "world")] 
        private bool ignoreBlockedSpaces;
        [SerializeField, Tooltip("If checked, then this object is only able to move in one cardinal direction " +
            "while moving along a path.")] 
        private bool restrictMovementAxes;


        #region Component References
        [field: SerializeReference, HideInInspector] public GridObject GridObject { get; private set; }

        /// <summary>
        /// Assign necessary component references on reset.
        /// </summary>
        private void Reset()
        {
            GridObject = GetComponent<GridObject>();
        }
        #endregion
        private Coroutine movementRoutine;
        private List<Vector3Int> currentPath;

        #region Propeties
        public Vector2Int Direction { get; private set; }
        public bool IsMoving
        {
            get
            {
                return movementRoutine != null;
            }
        }
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
            //Debug.Log("Set destination of object" + gameObject.name + " to " + destination);
            Vector3Int tileToStart = GridObject.CurrentSpace;
            currentPath = Pathfinder.FindPath(tileToStart, destinationSpace, jumpHeight, includeAdjacent);

            if (movementRoutine != null)
            {
                StopCoroutine(movementRoutine);
                movementRoutine = null;
            }
            movementRoutine = StartCoroutine(MovementRoutine(destinationSpace, includeAdjacent, finishCallback));
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
        /// Continually moves this object along a given path.
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator MovementRoutine(Vector3Int destination, bool includeAdj, 
            MovementFinishCallback finishCallback)
        {
            while (currentPath.Count > 0)
            {
                float step = MoveSpeed * Time.deltaTime;
                // If the space we're attempting to move into is occupied, then we should attempt to find a new path.
                if (GridObject.CheckOccupied(currentPath[0]))
                {
                    SetDestination(destination, includeAdj);
                }
                Vector3 tilePos = GridObject.GetOccupyPosition(currentPath[0]);

                PerformMove(step, tilePos);

                if (Vector3.Distance(transform.position, tilePos) < PATH_CLAMP)
                {
                    GridObject.SetCurrentSpace(currentPath[0]);
                    GridObject.SnapToSpace();
                    currentPath.RemoveAt(0);
                    // Update direction here to ensure directions are updated for later code execution.
                    if (currentPath.Count > 0)
                    {
                        Direction = (Vector2Int)(currentPath[0] - GridObject.CurrentSpace);
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
                Debug.Log("Moving");
                transform.position = Vector3.MoveTowards(transform.position, tilePos, step);
            }
        }
    }
}