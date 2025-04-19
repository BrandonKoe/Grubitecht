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
            VoxelTile previousSpace = null;
            while (isMoving)
            {
                List<VoxelTile> possibleSpaces = new List<VoxelTile>();

                // Get the target space.
                foreach (Vector2Int dir in CardinalDirections.ORTHOGONAL_2D)
                {
                    VoxelTile adjTile = gridObject.CurrentTile.GetAdjacent(dir);
                    if (adjTile != null)
                    {
                        possibleSpaces.Add(adjTile);
                    }
                }
                // Exclude inaccessible spaces here.
                Debug.Log(possibleSpaces.RemoveAll(item => item.
                ContainsObjectOnLayer(gridObject.Layer)));
                possibleSpaces.RemoveAll(item => Mathf.Abs(gridObject.CurrentTile.GridPosition.z - 
                    item.GridPosition.z) > climbHeight);
                if (ignorePreviousSpaces)
                {
                    possibleSpaces.RemoveAll(item => item == previousSpace);
                }

                VoxelTile nextSpace = possibleSpaces.OrderBy(item => navMap.GetDistanceValue(item)).FirstOrDefault();

                //Debug.Log($"Moving to space {nextSpace} with distance value: {navMap.GetDistanceValue(nextSpace)}");

                // If there are no valid spaces, stop and return to this routine next frame.
                if (nextSpace == null)
                {
                    yield return null;
                    continue;
                }

                // Set out current space to the next space.
                previousSpace = gridObject.CurrentTile;
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