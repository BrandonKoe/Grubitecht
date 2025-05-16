/*****************************************************************************
// File Name : GridFollower.cs
// Author : Brandon Koederitz
// Creation Date : March 26, 2025
//
// Brief Description : Causes a grid object to follow another as it moves.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.World.Objects;
using Grubitecht.World.Pathfinding;
using System;
using System.Collections;
using UnityEngine;

namespace Grubitecht.World
{
    [RequireComponent(typeof(GridObject))]
    public class GrubController : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        private VoxelTile targetSpace;
        private Coroutine followRoutine;
        private bool isFollowing;
        private float dampAngleSmoother;

        #region Component References
        [SerializeReference, HideInInspector] private GridObject gridObject;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        private void Reset()
        {
            gridObject = GetComponent<GridObject>();
        }
        #endregion

        /// <summary>
        /// Initializes this component with values.
        /// </summary>
        /// <param name="follow">The GridNavigator to follow.</param>
        public void Initialize(PathNavigator follow)
        {
            if (followRoutine != null)
            {
                StopCoroutine(followRoutine);
                followRoutine = null;
            }
            gridObject.SetCurrentSpace(follow.gridObject.CurrentTile.GetAdjacent(follow.Direction));
            gridObject.SnapToSpace();
            isFollowing = true;
            followRoutine = StartCoroutine(FollowRoutine(follow));
        }

        /// <summary>
        /// Causes this grub to visually follow a moving object to give the look that it is pushing that object.
        /// </summary>
        /// <param name="followedObject">The object that this grub is following.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator FollowRoutine(PathNavigator followedObject)
        {
            // Rotates the grub towards the direction the followed object is moving in.
            void RotateToward(Vector2Int direction)
            {
                Vector3 eulers = transform.eulerAngles;
                float angle = MathHelpers.VectorToDegAngleWorld(direction);
                // Calculate the speed our angle should rotate at based on the time it takes the object we're following to
                // reach it's next space based on it's speed.
                float angleTime = VoxelTilemap3D.CELL_SIZE / followedObject.MoveSpeed;
                //Debug.Log(dampAngleSmoother);
                // If dampAngleSmoother is Nan, then set it to 0 to prevent errors.
                dampAngleSmoother = float.IsNaN(dampAngleSmoother) ? 0f : dampAngleSmoother;
                //Debug.Log($"dampAngleSmoother: {dampAngleSmoother}");
                //Debug.Log($"Angle: {angle}");
                eulers.y = Mathf.SmoothDampAngle(eulers.y, angle, ref dampAngleSmoother, angleTime);
                //eulers.y = WhatTheSparkIsWrongWithYou.SmoothDampAngle(eulers.y, angle, ref dampAngleSmoother, angleTime);
                //Debug.Log($"Eulers: {eulers}");
                //eulers.y = angle;
                transform.eulerAngles = eulers;
            }

            // Use an event called by the path navigator we're following to update the space we should bne moving
            // towards.
            followedObject.NewSpaceEvent += UpdateTargetSpace;

            // Updates the grub to start standing on the spot adjacent to the object that he will be pushing.
            VoxelTile startingTile = followedObject.gridObject.CurrentTile.GetAdjacent(-followedObject.Direction);
            transform.position = gridObject.GetOccupyPosition(startingTile);
            targetSpace = followedObject.gridObject.CurrentTile;
            Vector3 eulers = transform.eulerAngles;
            eulers.y = MathHelpers.VectorToDegAngleWorld(followedObject.Direction);
            transform.eulerAngles = eulers;

            while (isFollowing)
            {
                // If our followed object is null, then we're no longer following an object and we should return this
                // grub.
                if (followedObject == null)
                {
                    GrubManager.ReturnGrub(this);
                    yield break;
                }
                // Moves this grub towards the space the followe object was most recently at.
                float step = followedObject.MoveSpeed * Time.deltaTime;
                Vector3 tilePos = gridObject.GetOccupyPosition(targetSpace);
                RotateToward(followedObject.Direction);
                transform.position = Vector3.MoveTowards(transform.position, tilePos, step);

                yield return null;
            }
            followedObject.NewSpaceEvent -= UpdateTargetSpace;
        }

        /// <summary>
        /// Updates the target space of this grub while it's following an object.
        /// </summary>
        /// <param name="tSpace">The spcae to follow.</param>
        private void UpdateTargetSpace(VoxelTile tSpace)
        {
            targetSpace = tSpace;
        }

        /// <summary>
        /// Perform behaviour that should happen when this grub is recalled.
        /// </summary>
        public void RecallGrub()
        {
            Destroy(gameObject);
        }
    }
}