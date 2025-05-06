/*****************************************************************************
// File Name : FlyingToggle.cs
// Author : Brandon Koederitz
// Creation Date : April 19, 2025
//
// Brief Description : Causes an enemy to periodically switch between walking grounded and flying.
// Makes the assumption that enemies start out grounded.
*****************************************************************************/
using Grubitecht.Audio;
using Grubitecht.Combat;
using Grubitecht.Tilemaps;
using Grubitecht.World.Objects;
using Grubitecht.World.Pathfinding;
using System.Collections;
using UnityEngine;

namespace Grubitecht.World
{
    [RequireComponent(typeof(PathNavigator))]
    [RequireComponent(typeof(GridObject))]
    [RequireComponent(typeof(Combatant))]
    public class FlyingToggle : MonoBehaviour
    {
        [Header("Sounds")]
        [SerializeField] private Sound ascendSound;
        [SerializeField] private Sound descendSound;
        [Header("Settings")]
        [SerializeField, Tooltip("The amount of time between when this enemy switches between flying and " +
            "grounded state.")] 
        private float switchTime;
        [SerializeField] private int flyingClimbHeight;
        [SerializeField] private Vector3 flyingOffset;

        private int groundedClimbHeight;
        private Vector3 groundedOffset;

        int iterationLimit;

        #region Component References
        [SerializeReference, HideInInspector] private PathNavigator pathNavigator;
        [SerializeReference, HideInInspector] private GridObject gridObject;
        [SerializeReference, HideInInspector] private Combatant combatant;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        private void Reset()
        {
            pathNavigator = GetComponent<PathNavigator>();
            gridObject = GetComponent<GridObject>();
            combatant = GetComponent<Combatant>();
        }
        #endregion

        private void Awake()
        {
            groundedClimbHeight = pathNavigator.ClimbHeight;
            groundedOffset = gridObject.Offset;
            StartCoroutine(SwapRoutine());
        }

        /// <summary>
        /// Continually swaps this enemy's state between flying and grounded.
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator SwapRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(switchTime);
                SetFlying();
                yield return new WaitForSeconds(switchTime);
                SetGrounded();
            }
        }

        /// <summary>
        /// Switches this enemy to be flying.
        /// </summary>
        public void SetFlying()
        {
            Debug.Log(name + " is switching to flying.");
            MoveToFlying(null);
        }

        /// <summary>
        /// Actually sets this enemy to flying internally.  Needs to take in a pathStatus so it can be used as a 
        /// path  navigator callback.
        /// </summary>
        /// <param name="callbackInfo">
        /// The status of the pathing.  We dont care about this at all, it's only here so that this function can be
        /// passed as a movement callback delegate.
        /// </param>
        private void MoveToFlying(PathCallbackInfo callbackInfo)
        {
            // Prevent potential infinite loops temporarily until I can further diagonse the problem.
            iterationLimit++;
            if (iterationLimit > 100)
            {
                iterationLimit = 0;
                return;
            }
            // If our current space has something in it on the ground layer already, we cant switch states here so we
            // need to move to a valid position first.
            if (gridObject.CurrentTile.ContainsObjectOnLayer(OccupyLayer.Air))
            {
                VoxelTile targetTile = FindEmptyTile(gridObject.CurrentTile, 1);
                pathNavigator.SetDestination(targetTile, MoveToFlying);
            }
            else
            {
                // If our current space has nothing in the air in it, then we can automatically switch to the
                // flying state.
                SetFlyingState();
            }
        }

        /// <summary>
        /// Finds the nearest empty tile from a given origin tile.  Special case compared to the normal that checks
        /// both air and ground layers.
        /// </summary>
        /// <param name="originTile">The tile to start finding a tile from.</param>
        /// <param name="startingRange">The initial starting range to use when finding a space.</param>
        /// <param name="maxRange">The max search range that to find a tile within.</param>
        /// <returns>The closest unoccupied tile to the origin tile.</returns>
        public VoxelTile FindEmptyTile(VoxelTile originTile, int startingRange = 0, int maxRange = 100)
        {
            int range = startingRange;
            while (range < maxRange)
            {
                for (int x = -range; x <= range; x++)
                {
                    // Find the possible variance in y positions for a given range.
                    int yRange = range - Mathf.Abs(x);
                    // Loops through both positive and negative values for y that yields a manhatten distance of
                    // range from the spawn point.
                    for (int i = -1; i < 2; i += 2)
                    {
                        int y = i * yRange;
                        // Check the positive and negative cells that have a manhatten distance of range.
                        Vector2Int checkPos = new Vector2Int(x, y) + originTile.GridPosition2;
                        VoxelTile checkCell = VoxelTilemap3D.Main_GetTile(checkPos);
                        // If checkCell is returned as null, then the cell we're trying to get does not exist on the
                        // tilemap and we should ignore it.
                        if (checkCell == null)
                        {
                            continue;
                        }
                        // If this position isnt occupied, then return it.
                        if (!checkCell.ContainsObjectOnLayer(OccupyLayer.Air) && 
                            !checkCell.ContainsObjectOnLayer(OccupyLayer.Ground))
                        {
                            return checkCell;
                        }
                    }
                }
                // If we were not able to find a cell through looping, then increment range and recursively call this
                // function agian.
                range++;
            }
            // If max range is exceeded, then we didnt find a valid adjacent tile.
            return null;
        }

        /// <summary>
        /// Actually updates the values that make this enemy a flying enemy.
        /// </summary>
        private void SetFlyingState()
        {
            pathNavigator.ClimbHeight = flyingClimbHeight;
            gridObject.Layer = OccupyLayer.Air;
            gridObject.Offset = flyingOffset;
            combatant.CombatTags |= CombatTags.Flying;

            // Plays a sound when this enemy switches to the flying state.
            AudioManager.PlaySoundAtPosition(ascendSound, transform.position);

            iterationLimit = 0;
        }

        /// <summary>
        /// Switches this enemy to be grounded.
        /// </summary>
        public void SetGrounded()
        {
            Debug.Log(name + " is switching to grounded.");
            MoveToGrounded(null);
        }

        /// <summary>
        /// Actually sets this enemy to flying internally.  Needs to take in a pathStatus so it can be used as a 
        /// path  navigator callback.
        /// </summary>
        /// <param name="callbackInfo">
        /// The status of the pathing.  We dont care about this at all, it's only here so that this function can be
        /// passed as a movement callback delegate.
        /// </param>
        private void MoveToGrounded(PathCallbackInfo callbackInfo)
        {
            // Prevent potential infinite loops temporarily until I can further diagonse the problem.
            iterationLimit++;
            if (iterationLimit > 100)
            {
                Debug.Log("MoveToGrounded iteration limit was hit");
                iterationLimit = 0;
                return;
            }
            // If our current space has something in it on the ground layer already, we cant switch states here so we
            // need to move to a valid position first.
            if (gridObject.CurrentTile.ContainsObjectOnLayer(OccupyLayer.Ground))
            {
                VoxelTile targetTile = VoxelTilemap3D.Main_FindEmptyTile(gridObject.CurrentTile,
                            OccupyLayer.Ground, 1);
                pathNavigator.SetDestination(targetTile, MoveToGrounded);
            }
            else
            {
                // If our current space has nothing in the air in it, then we can automatically switch to the
                // flying state.
                SetGroundedState();
            }
        }

        /// <summary>
        /// Switches this enemy to be grounded.
        /// </summary>
        public void SetGroundedState()
        {
            pathNavigator.ClimbHeight = groundedClimbHeight;
            gridObject.Layer = OccupyLayer.Ground;
            gridObject.Offset = groundedOffset;
            combatant.CombatTags &= ~CombatTags.Flying;

            // Plays a sound when this enemy switches to the flying state.
            AudioManager.PlaySoundAtPosition(descendSound, transform.position);

            iterationLimit = 0;
        }
    }
}
