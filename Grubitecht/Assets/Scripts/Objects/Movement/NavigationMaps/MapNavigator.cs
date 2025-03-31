/*****************************************************************************
// File Name : MapNavigator.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Allows a grid object to move along the grid using a navigation map.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.World.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grubitecht.World.Pathfinding
{
    public class MapNavigator : GridNavigator
    {
        #region CONSTS
        // How much map navigators prefer to move away from spaces that resulted in a roadblock.  Should be less
        // than 1 so that they value moving towards the objective more.
        private const float AVOIDANCE_BIAS = 0.1f;
        #endregion
        [SerializeField, Tooltip("If this is set to true, then this object will not move to the space it was " +
            "previously on.")] 
        private bool ignorePreviousSpaces;
        private bool isMoving;

        public override bool IsMoving => isMoving;
        /// <summary>
        /// Starts/stops this object's movement.
        /// </summary>
        public void StartMoving(NavigationMap navMap)
        {
            if (movementRoutine == null)
            {
                isMoving = true;
                movementRoutine = StartCoroutine(MovementRoutine(navMap));
            }
        }
        public override void StopMoving()
        {
            isMoving = false;
        }

        /// <summary>
        /// Continually moves this object towards an objective based on a given objective navigation map.
        /// </summary>
        /// <param name="navMap">The navigation map this object should follow.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator MovementRoutine(NavigationMap navMap)
        {
            yield return null;
            Vector3Int previousSpace = Vector3Int.zero;
            while (isMoving)
            {
                List<Vector3Int> possibleSpaces = new List<Vector3Int>();

                // Get the target space.
                foreach (Vector2Int dir in CardinalDirections.CARDINAL_DIRECTIONS_2)
                {
                    possibleSpaces.AddRange(VoxelTilemap3D.Main_GetCellsInColumn((Vector2Int)gridObject.CurrentSpace + 
                        dir, GridObject.VALID_GROUND_TYPE));
                }
                // Exclude inaccessible spaces here.
                possibleSpaces.RemoveAll(item => !ignoreBlockedSpaces && (GridObject.GetObjectAtSpace(item) != null));
                possibleSpaces.RemoveAll(item => Mathf.Abs(gridObject.CurrentSpace.z - item.z) > jumpHeight);
                if (ignorePreviousSpaces)
                {
                    possibleSpaces.RemoveAll(item => item == previousSpace);
                }

                // If the object runs into another object while pathfinding, then it should attempt to get away from
                // that object as much as possible in addition to moving towards the objective.  This is to make it so
                // that objects attempt to find a new path if their current one is blocked.
                Vector3Int nextSpace = possibleSpaces.OrderBy(item => navMap.GetDistanceValue(item)).FirstOrDefault();

                //Debug.Log($"Moving to space {nextSpace} with distance value: {navMap.GetDistanceValue(nextSpace)}");

                // If zero is returned, then that is the default and there must be no valid spaces to move to at the
                // moment.
                if (nextSpace == Vector3Int.zero)
                {
                    yield return null;
                    continue;
                }

                // Set out current space to the next space.
                previousSpace = gridObject.CurrentSpace;
                gridObject.SetCurrentSpace(nextSpace);

                Vector3 tilePos = gridObject.GetOccupyPosition(nextSpace);
                // Move to that space.
                while (Vector3.Distance(transform.position, tilePos) > SPACE_CLAMP)
                {
                    PerformMove(tilePos);
                    //Debug.Log("Moving towards " + tilePos);
                    yield return null;
                }
                // Snap to the space after movement has finished.
                gridObject.SnapToSpace();
                // In here for now to stop potential infinite loops that can cause crashes.
                yield return null;
            }
            Debug.Log("Movement finished");
            movementRoutine = null;
        }
    }

}