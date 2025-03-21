/*****************************************************************************
// File Name : EnemyMovement.cs
// Author : Brandon Koederitz
// Creation Date : March 19, 2025
//
// Brief Description : Controls enemy movement along the grid using pathfinding to the closest objective.
*****************************************************************************/
using Grubitecht.World.Objects;
using Grubitecht.World.Pathfinding;
using UnityEngine;

namespace Grubitecht.World
{
    [RequireComponent(typeof(GridNavigator))]
    public class EnemyMovement : MonoBehaviour
    {
        #region Component References
        [SerializeReference, HideInInspector] private GridNavigator gridNavigator;
        [SerializeReference, HideInInspector] private GridObject gridObject;
        #endregion
        private Objective currentTarget;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        private void Reset()
        {
            gridNavigator = GetComponent<GridNavigator>();
            gridObject = GetComponent<GridObject>();
        }

        /// <summary>
        /// Start pathfinding to the objective on game start for now.
        /// </summary>
        private void Start()
        {
            PathToNearestObjective();
        }

        // Need to re-evaluate enemy path when the object is out of range, not on each map refresh.
        //protected override void OnMapRefresh(GridObject movedObject, GroundTile oldTile, GroundTile newTile)
        //{
        //    // Never run this function if this enemy is the one triggering the map refresh.
        //    if (movedObject == gridObject) { return; }
        //    // If the moved object was this enemy's target objecctive, then we should re-path towards the cloests objective.
        //    if (movedObject.TryGetComponent(out Objective obj))
        //    {
        //        if (obj == currentTarget)
        //        {
        //            PathToNearestObjective();
        //        }
        //    }
        //}

        /// <summary>
        /// Has this enemy pathfind to the neartest objective.
        /// </summary>
        private void PathToNearestObjective()
        {
            currentTarget = Objective.GetNearestObjective(transform.position);
            // Nearest objective should never be null in actual gameplay as if it is then the level is lost.
            // Double check for null here to avoid errors.
            if (currentTarget != null)
            {
                Debug.Log("Set destination to " + currentTarget.gridObject.CurrentSpace);
                gridNavigator.SetDestination(currentTarget.gridObject.CurrentSpace, true);
            }
        }
    }
}