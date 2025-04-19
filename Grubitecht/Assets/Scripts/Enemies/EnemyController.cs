/*****************************************************************************
// File Name : EnemyMovement.cs
// Author : Brandon Koederitz
// Creation Date : March 19, 2025
//
// Brief Description : Controls enemy movement along the grid using pathfinding to the closest objective.
*****************************************************************************/
using Grubitecht.Combat;
using Grubitecht.Tilemaps;
using Grubitecht.Waves;
using Grubitecht.World.Objects;
using Grubitecht.World.Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.World
{
    [RequireComponent(typeof(PathNavigator))]
    [RequireComponent(typeof(Targeter))]
    public class EnemyController : MonoBehaviour
    {
        #region CONSTS
        private const string ENEMY_PARENT_TAG = "EnemyParent";
        #endregion
        [field: SerializeField] public Sprite EnemySpriteIcon { get; private set; }
        [SerializeField] private PathingType pathingType;
        [SerializeField] private float rePathFrequency;

        private static Transform enemyParent;
        #region Component References
        [field: SerializeReference, HideInInspector] public GridObject gridObject { get; private set; }
        [SerializeReference, HideInInspector] private Targeter targeter;
        [SerializeReference, HideInInspector] private PathNavigator pathNavigator;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        private void Reset()
        {
            gridObject = GetComponent<GridObject>();
            targeter = GetComponent<Targeter>();
            pathNavigator = GetComponent<PathNavigator>();
        }
        #endregion

        #region Properties
        private static Transform EnemyParent
        {
            get
            {
                if (enemyParent == null)
                {
                    enemyParent = GameObject.FindGameObjectWithTag(ENEMY_PARENT_TAG).transform;
                }
                return enemyParent;
            }
        }
        #endregion

        // State Machine
        internal EnemyState state = new MovingState();

        private Objective currentTarget;

        #region Nested Classes
        private enum PathingType
        {
            InOrder,
            Closest,
            Furthest
        }

        // State Machine
        internal abstract class EnemyState
        {
            internal virtual void OnGainTarget(EnemyController thisEnemy) 
            {
                // The enemy should always move to the attacking state if it has a target.
                thisEnemy.state = new AttackingState(thisEnemy);
            }

            internal virtual void OnLoseTarget(EnemyController thisEnemy) { }

            internal virtual void OnStopMoving(EnemyController thisEnemy) { }
            internal virtual void OnInvalidPath(EnemyController thisEnemy) { }
            internal virtual void OnStartedMoving(EnemyController thisEnemy) { }
        }

        /// <summary>
        /// State for when the enemy is attacking an objective.
        /// </summary>
        internal class AttackingState : EnemyState
        {
            public AttackingState(EnemyController thisEnemy)
            {
                //Debug.Log(thisEnemy + " is attacking");
                thisEnemy.pathNavigator.StopMoving();
            }
            internal override void OnGainTarget(EnemyController thisEnemy)
            {
                // Specifically DO NOT create a new attacking state when already in the attacking state.
                //base.OnGainTarget(thisEnemy);
            }
            internal override void OnLoseTarget(EnemyController thisEnemy)
            {
                thisEnemy.state = new MovingState();
                thisEnemy.PathToTarget();
            }
        }

        /// <summary>
        /// State for when the enemy is moving towards an objective.
        /// </summary>
        internal class MovingState : EnemyState
        {
            /// <summary>
            /// When the enemy stops moving, attempt to pathfind towards the target again.
            /// </summary>
            /// <param name="thisEnemy"></param>
            internal override void OnStopMoving(EnemyController thisEnemy)
            {
                thisEnemy.state = new WaitingState(thisEnemy);
            }

            internal override void OnInvalidPath(EnemyController thisEnemy)
            {
                thisEnemy.state = new WaitingState(thisEnemy);
            }
        }

        /// <summary>
        /// State for when the enemy is waiting and attempting to find a valid path.
        /// </summary>
        internal class WaitingState : EnemyState
        {
            private bool isRePathing;
            private Coroutine rePathRoutine;
            private readonly EnemyController thisEnemy;
            internal WaitingState(EnemyController thisEnemy)
            {
                //Debug.Log(thisEnemy + " is waiting");
                this.thisEnemy = thisEnemy;
                rePathRoutine = thisEnemy.StartCoroutine(RePathRoutine(thisEnemy));
            }

            /// <summary>
            /// Continually re-attempts to pathfinding towards a target objective
            /// </summary>
            /// <returns>Coroutine.</returns>
            private IEnumerator RePathRoutine(EnemyController thisEnemy)
            {
                //Debug.Log("Is now re-pathing");
                isRePathing = true;
                // Wait a frame before re-pathing starts as we need to ensure the new state has been set before we
                // start running any new pathfinding operations.
                yield return null;
                while (isRePathing)
                {
                    thisEnemy.PathToTarget();
                    yield return new WaitForSeconds(1 / thisEnemy.rePathFrequency);
                }
                isRePathing = false;
            }

            // Once we've started moving, we should switch to the moving state.
            internal override void OnStartedMoving(EnemyController thisEnemy)
            {
                isRePathing = false;
                thisEnemy.StopCoroutine(rePathRoutine);
                rePathRoutine = null;
                // Switch to the moving state.
                thisEnemy.state = new MovingState();
            }

            ~WaitingState()
            {
                if (rePathRoutine != null)
                {
                    thisEnemy.StopCoroutine(rePathRoutine);
                    rePathRoutine = null;
                }
            }
        }

        internal class PanicState : EnemyState
        {
            private readonly EnemyController thisEnemy;
            private readonly float speedBoost;
            private readonly int panicRadius;
            internal PanicState(EnemyController thisEnemy, float speedBoost, int panicRadius)
            {
                this.thisEnemy = thisEnemy;
                this.speedBoost = speedBoost;
                this.panicRadius = panicRadius;
                thisEnemy.pathNavigator.MoveSpeed += speedBoost;
                MoveRandomly(thisEnemy);
            }

            internal override void OnStopMoving(EnemyController thisEnemy)
            {
                Debug.Log("Stopped Moving");
                MoveRandomly(thisEnemy);
            }

            internal override void OnInvalidPath(EnemyController thisEnemy)
            {
                Debug.Log("Invalid Path");
                MoveRandomly(thisEnemy);
            }

            /// <summary>
            /// Causes this enemy to pathfind towards a random point nearby.
            /// </summary>
            /// <param name="thisEnemy">The enemy that should pathfind towards a random point.</param>
            private void MoveRandomly(EnemyController thisEnemy)
            {
                VoxelTile destTile = null;
                int iterationNum = 0;
                // Continue getting a tile until a valid one is found.
                while (destTile == null || destTile.ContainsObjectOnLayer(thisEnemy.gridObject.Layer))
                {
                    // Cap the random check to 100 iterations so it doesnt cause crashes if it tries to iterate too
                    // many times.
                    iterationNum++;
                    if (iterationNum == 100)
                    {
                        Debug.LogError("Random movement iteration limit was hit.");
                        return;
                    }
                    int xRand = Random.Range(-panicRadius, panicRadius);
                    // The sum of the x and y randomness should never exceed panic radius.
                    int yRand = Random.Range(Mathf.Abs(xRand) - panicRadius, panicRadius - Mathf.Abs(xRand));
                    // Gets the tile offset from this enemy by (xRand, yRand) and set that as our destination.
                    Vector2Int destPos = thisEnemy.gridObject.CurrentTile.GridPosition2 - new Vector2Int(xRand, yRand);
                    destTile = VoxelTilemap3D.Main_GetTile(destPos);
                }
                thisEnemy.pathNavigator.SetDestination(destTile, thisEnemy.OnMovingCallback);
            }

            ~PanicState()
            {
                thisEnemy.pathNavigator.MoveSpeed -= speedBoost;
            }
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
            // Defer control to the current state.
            state.OnGainTarget(this);
            //pathNavigator.StopMoving();
        }

        /// <summary>
        /// When this enemy gains a new target, it stops moving.  Dont need to move if the target is in range alreaedy.
        /// </summary>
        private void HandleOnLoseTarget()
        {
            // Defer control to the current state.
            state.OnLoseTarget(this);
            //PathToTarget();
        }

        ///// <summary>
        ///// Starts this enemy's movement.
        ///// </summary>
        ///// <remarks>
        ///// Called by the spawn point when this enemy spawns.
        ///// </remarks>
        //[Button]
        //public void StartMoving()
        //{
        //    pathNavigator.StartMoving(Objective.NavMap);
        //}

        /// <summary>
        /// Has this enemy pathfind to the nearest objective if it doesnt have any targets.
        /// </summary>
        public void PathToTarget()
        {
            if (!targeter.HasTarget)
            {
                //currentTarget = Objective.GetNearestObjective(transform.position);
                //// Nearest objective should never be null in actual gameplay as if it is then the level is lost.
                //// Double check for null here to avoid errors.
                //if (currentTarget != null)
                //{
                //    //Debug.Log("Set destination to " + currentTarget.gridObject.CurrentSpace);
                //    // When the enemy arrives at it's destination, if it doesnt have a target still, then we
                //    // attempt to pathfind again.
                //    pathNavigator.SetDestination(currentTarget.gridObject.CurrentSpace, true, PathToNearestObjective);
                //}
                // Enemies always target the same objective in priority order.
                Objective target = null;
                switch (pathingType)
                {
                    case PathingType.InOrder:
                        target = Objective.TargetObjective;
                        break;
                    case PathingType.Closest:
                        target = Objective.GetNearestObjective(transform.position);
                        break;
                    case PathingType.Furthest:
                        target = Objective.GetFurthestObjective(transform.position);
                        break;
                    default:
                        break;
                }
                if (target != null)
                {
                    //Debug.Log(target);
                    // Attempt to set our target objective as our pathfinding destination.  If pathfinding
                    // fails, then there isnt a valid path to that objective so we need to start updating our
                    // pathfinding.
                    pathNavigator.SetDestination(target.gridObject.CurrentTile, OnMovingCallback);
                    //Debug.Log(target.gridObject.CurrentTile.ToString());
                    //Debug.Log(hasPath);
                    //if (hasPath)
                    //{
                    //    // If we have a valid path, this enemy should enter the moving state.
                    //    state = new MovingState();
                    //}
                    //else
                    //{
                    //    // Defer control to the current state.
                    //    state.OnInvalidPath(this);
                    //}
                }
            }
        }

        /// <summary>
        /// If this enemy stops moving, if it doesnt have a target, then we need to attempt to find another
        /// path.
        /// </summary>
        /// <param name="reachedDestination">
        /// True if the object reached it's destination after moving along a given path.
        /// </param>
        private void OnMovingCallback(PathStatus endStatus)
        {
            switch (endStatus)
            {
                case PathStatus.Started:
                    state.OnStartedMoving(this);
                    break;
                case PathStatus.Completed:
                    state.OnStopMoving(this);
                    break;
                case PathStatus.Invalid:
                    //Debug.Log("Invalid Path");
                    state.OnInvalidPath(this);
                    break;
                default:
                    break;
            }
            //Debug.Log("OnFinishedMoving" + reachedDestination);
            //if (!targeter.HasTarget)
            //{
            //    PathToTarget();
            //}    
            // Defer control to the current state.
        }

        /// <summary>
        /// Toggles this enemy's panic state.
        /// </summary>
        /// <param name="speedBoost">The boost in speed this enemy gets while panicing</param>
        /// <param name="radius">The radius that the next space the enemy will pathfind towards will be.</param>
        public void StartPanicking(float speedBoost, int radius)
        {
            state = new PanicState(this, speedBoost, radius);
        }
        public void StopPanicking()
        {
            if (state is PanicState)
            {
                state = new MovingState();
            }
        }

        /// <summary>
        /// Spawns an enemy at the nearest open space to a given tile.
        /// </summary>
        /// <param name="enemyPrefab">The enemy prefab to spawn.</param>
        /// <param name="spawnOrigin">The tile that the enemy will spawn from.</param>
        public static void SpawnEnemy(EnemyController enemyPrefab, VoxelTile spawnOrigin, bool isSpawned = true)
        {
            EnemyController spawnedEnemy = Instantiate(enemyPrefab, EnemyParent);

            //spawnedEnemy.name = spawnedEnemy.name + i;
            VoxelTile tile = VoxelTilemap3D.Main_FindEmptyTile(spawnOrigin, spawnedEnemy.gridObject.Layer);
            spawnedEnemy.gridObject.SetCurrentSpace(tile);
            spawnedEnemy.gridObject.SnapToSpace();
            spawnedEnemy.PathToTarget();
            // Adds the spawned enemy to the wave manager's enemy list.
            WaveManager.AddEnemy(spawnedEnemy, isSpawned);
            //Debug.Log("Spawned enemy " + spawnedEnemy.name+ " at position " + pos);
        }
    }
}