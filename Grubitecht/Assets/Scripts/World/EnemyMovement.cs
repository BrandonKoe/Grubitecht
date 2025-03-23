/*****************************************************************************
// File Name : EnemyMovement.cs
// Author : Brandon Koederitz
// Creation Date : March 19, 2025
//
// Brief Description : Controls enemy movement along the grid using pathfinding to the closest objective.
*****************************************************************************/
using Grubitecht.Combat;
using Grubitecht.World.Objects;
using Grubitecht.World.Pathfinding;
using UnityEngine;

namespace Grubitecht.World
{
    [RequireComponent(typeof(GridNavigator))]
    [RequireComponent(typeof(Targeter))]
    public class EnemyMovement : MonoBehaviour
    {
        private Objective currentTarget;

        #region Component References
        [SerializeReference, HideInInspector] private GridNavigator gridNavigator;
        [SerializeReference, HideInInspector] private GridObject gridObject;
        [SerializeReference, HideInInspector] private Targeter targeter;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        private void Reset()
        {
            gridNavigator = GetComponent<GridNavigator>();
            gridObject = GetComponent<GridObject>();
            targeter = GetComponent<Targeter>();

        }
        #endregion

        /// <summary>
        /// Subscribe and unsubscribe to targeter events that handle when this enemy updates its movement.
        /// </summary>
        private void Awake()
        {
            targeter.OnGainTarget += HandleOnGainTarget;
            targeter.OnLoseTarget += HandleOnLoseTarget;
        }
        private void OnDestroy()
        {
            targeter.OnGainTarget -= HandleOnGainTarget;
            targeter.OnLoseTarget -= HandleOnLoseTarget;
        }

        /// <summary>
        /// Start pathfinding to the objective on game start for now.
        /// </summary>
        private void Start()
        {
            PathToNearestObjective();
        }

        /// <summary>
        /// When this enemy loses a new target, if there are no valid targets, it pathfinds to the nearest objective.
        /// </summary>
        /// <param name="target">That target that just left range.</param>
        private void HandleOnGainTarget(Attackable target)
        {
            gridNavigator.StopMoving();
        }

        /// <summary>
        /// When this enemy gains a new target, it stops moving.  Dont need to move if the target is in range alreaedy.
        /// </summary>
        /// <param name="target">The target that is now in range.</param>
        private void HandleOnLoseTarget(Attackable target)
        {
            PathToNearestObjective();
        }

        /// <summary>
        /// Has this enemy pathfind to the nearest objective if it doesnt have any targets.
        /// </summary>
        private void PathToNearestObjective()
        {
            if (!targeter.HasTarget)
            {
                currentTarget = Objective.GetNearestObjective(transform.position);
                // Nearest objective should never be null in actual gameplay as if it is then the level is lost.
                // Double check for null here to avoid errors.
                if (currentTarget != null)
                {
                    //Debug.Log("Set destination to " + currentTarget.gridObject.CurrentSpace);
                    // When the enemy arrives at it's destination, if it doesnt have a target still, then we
                    // attempt to pathfind again.
                    gridNavigator.SetDestination(currentTarget.gridObject.CurrentSpace, true, PathToNearestObjective);
                }
            }
        }
    }
}