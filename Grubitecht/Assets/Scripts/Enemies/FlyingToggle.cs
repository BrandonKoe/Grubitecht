/*****************************************************************************
// File Name : FlyingToggle.cs
// Author : Brandon Koederitz
// Creation Date : April 19, 2025
//
// Brief Description : Causes an enemy to periodically switch between walking grounded and flying.
// Makes the assumption that enemies start out grounded.
*****************************************************************************/
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
        [SerializeField, Tooltip("The amount of time between when this enemy switches between flying and " +
            "grounded state.")] 
        private float switchTime;
        [SerializeField] private int flyingClimbHeight;
        [SerializeField] private Vector3 flyingOffset;

        private int groundedClimbHeight;
        private Vector3 groundedOffset;

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
            MoveToFlying(PathStatus.Invalid);
        }

        /// <summary>
        /// Actually sets this enemy to flying internally.  Needs to take in a pathStatus so it can be used as a 
        /// path  navigator callback.
        /// </summary>
        /// <param name="pathStatus">
        /// The status of the pathing.  We dont care about this at all, it's only here so that this function can be
        /// passed as a movement callback delegate.
        /// </param>
        private void MoveToFlying(PathStatus pathStatus)
        {
            // If our current space has something in it on the ground layer already, we cant switch states here so we
            // need to move to a valid position first.
            if (gridObject.CurrentTile.ContainsObjectOnLayer(OccupyLayer.Air))
            {
                VoxelTile targetTile = VoxelTilemap3D.Main_FindEmptyTile(gridObject.CurrentTile,
                            OccupyLayer.Air, 1);
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
        /// Actually updates the values that make this enemy a flying enemy.
        /// </summary>
        private void SetFlyingState()
        {
            pathNavigator.ClimbHeight = flyingClimbHeight;
            gridObject.Layer = OccupyLayer.Air;
            gridObject.Offset = flyingOffset;
            combatant.CombatTags |= CombatTags.Flying;
        }

        /// <summary>
        /// Switches this enemy to be grounded.
        /// </summary>
        public void SetGrounded()
        {
            Debug.Log(name + " is switching to grounded.");
            MoveToGrounded(PathStatus.Invalid);
        }

        /// <summary>
        /// Actually sets this enemy to flying internally.  Needs to take in a pathStatus so it can be used as a 
        /// path  navigator callback.
        /// </summary>
        /// <param name="pathStatus">
        /// The status of the pathing.  We dont care about this at all, it's only here so that this function can be
        /// passed as a movement callback delegate.
        /// </param>
        private void MoveToGrounded(PathStatus pathStatus)
        {
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
        }
    }
}
