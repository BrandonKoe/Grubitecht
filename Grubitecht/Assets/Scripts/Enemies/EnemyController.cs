/*****************************************************************************
// File Name : EnemyMovement.cs
// Author : Brandon Koederitz
// Creation Date : March 19, 2025
//
// Brief Description : Controls enemy movement along the grid using pathfinding to the closest objective.
*****************************************************************************/
using Grubitecht.Combat;
using Grubitecht.Waves;
using Grubitecht.World.Objects;
using Grubitecht.World.Pathfinding;
using NaughtyAttributes;
using UnityEngine;

namespace Grubitecht.World
{
    [RequireComponent(typeof(MapNavigator))]
    [RequireComponent(typeof(Targeter))]
    public class EnemyController : MonoBehaviour
    {
        #region Component References
        [field: SerializeReference, HideInInspector] public GridObject gridObject { get; private set; }
        [SerializeReference, HideInInspector] private Targeter targeter;
        [SerializeReference, HideInInspector] private MapNavigator mapNavigator;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        private void Reset()
        {
            gridObject = GetComponent<GridObject>();
            targeter = GetComponent<Targeter>();
            mapNavigator = GetComponent<MapNavigator>();
        }
        #endregion

        /// <summary>
        /// Subscribe and unsubscribe to targeter events that handle when this enemy updates its movement.
        /// </summary>
        private void Awake()
        {
            // Adds this enemy to the wave manager's enemy list.
            WaveManager.AddEnemy(this);
            targeter.OnGainTarget += HandleOnGainTarget;
            targeter.OnLoseTarget += HandleOnLoseTarget;
        }
        private void OnDestroy()
        {
            // Removes this enemy from the wave manager's enemy list when it is destroyed.
            WaveManager.RemoveEnemy(this);
            targeter.OnGainTarget -= HandleOnGainTarget;
            targeter.OnLoseTarget -= HandleOnLoseTarget;
        }

        /// <summary>
        /// When this enemy loses a new target, if there are no valid targets, it pathfinds to the nearest objective.
        /// </summary>
        private void HandleOnGainTarget()
        {
            mapNavigator.StopMoving();
        }

        /// <summary>
        /// When this enemy gains a new target, it stops moving.  Dont need to move if the target is in range alreaedy.
        /// </summary>
        private void HandleOnLoseTarget()
        {
            StartMoving();
        }

        /// <summary>
        /// Starts this enemy's movement.
        /// </summary>
        /// <remarks>
        /// Called by the spawn point when this enemy spawns.
        /// </remarks>
        [Button]
        public void StartMoving()
        {
            mapNavigator.StartMoving(Objective.NavMap);
        }

        ///// <summary>
        ///// Has this enemy pathfind to the nearest objective if it doesnt have any targets.
        ///// </summary>
        //private void PathToNearestObjective()
        //{
        //    if (!targeter.HasTarget)
        //    {
        //        currentTarget = Objective.GetNearestObjective(transform.position);
        //        // Nearest objective should never be null in actual gameplay as if it is then the level is lost.
        //        // Double check for null here to avoid errors.
        //        if (currentTarget != null)
        //        {
        //            //Debug.Log("Set destination to " + currentTarget.gridObject.CurrentSpace);
        //            // When the enemy arrives at it's destination, if it doesnt have a target still, then we
        //            // attempt to pathfind again.
        //            GridNavigator.SetDestination(currentTarget.gridObject.CurrentSpace, true, PathToNearestObjective);
        //        }
        //    }
        //}
    }
}