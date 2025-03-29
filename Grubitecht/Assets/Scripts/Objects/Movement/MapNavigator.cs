/*****************************************************************************
// File Name : MapNavigator.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Allows a grid object to move along the grid using a navigation map.
*****************************************************************************/
using Grubitecht.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grubitecht.World.Objects;

namespace Grubitecht.World.Pathfinding
{
    public class MapNavigator : GridNavigator
    {
        private bool isMoving;

        public override bool IsMoving => isMoving;
        /// <summary>
        /// Starts/stops this enemie's movement.
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
                possibleSpaces.RemoveAll(item => !ignoreBlockedSpaces && GridObject.GetObjectAtSpace(item) != null);
                possibleSpaces.RemoveAll(item => Mathf.Abs(gridObject.CurrentSpace.z - item.z) > jumpHeight);
                possibleSpaces.RemoveAll(item => item == previousSpace);

                Vector3Int nextSpace = possibleSpaces.OrderBy(item => navMap.GetDistanceValue(item))
                    .FirstOrDefault();

                Vector3 tilePos = gridObject.GetOccupyPosition(nextSpace);
                // Move to that space.
                while (Vector3.Distance(transform.position, tilePos) > SPACE_CLAMP)
                {
                    PerformMove(tilePos);
                    Debug.Log("Moving towards " + tilePos);
                    yield return null;
                }

                // Once the object has reached the space...
                previousSpace = gridObject.CurrentSpace;
                gridObject.SetCurrentSpace(nextSpace);
                gridObject.SnapToSpace();
                // In here for now to stop potential infinite loops.
                yield return null;
            }
            movementRoutine = null;
        }
    }

}