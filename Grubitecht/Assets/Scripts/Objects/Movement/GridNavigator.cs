/*****************************************************************************
// File Name : GridNavigator.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Base class for all components that control movement along the grid.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.World.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.World.Pathfinding
{
    [RequireComponent(typeof(GridObject))]
    public abstract class GridNavigator : MonoBehaviour
    {
        #region CONSTS
        protected const float SPACE_CLAMP = 0.001f;
        #endregion

        [Header("Model Rotation")]
        [SerializeField, Tooltip("The model to rotate to correspond to the direction this object is moving.")]
        protected Transform rotateModel;
        [SerializeField] private float rotationTime;
        [field: Header("Movement Settings")]
        [field: SerializeField] public float MoveSpeed { get; protected set; }
        [SerializeField, Tooltip("How large of an upward incline this object can move up.")]
        protected int climbHeight;
        //[SerializeField, Tooltip("Whether this object should ignore spaces that are blocked when navigating the " +
        //    "world")]
        //protected bool ignoreBlockedSpaces;
        [SerializeField, Tooltip("If checked, then this object is only able to move in one cardinal direction " +
            "while moving along a path.")]
        protected bool restrictMovementAxes;

        #region Component References
        [field: SerializeReference, HideInInspector] public GridObject gridObject { get; private set; }

        /// <summary>
        /// Assign necessary component references on reset.
        /// </summary>
        private void Reset()
        {
            gridObject = GetComponent<GridObject>();
        }
        #endregion
        protected Coroutine movementRoutine;
        private float dampAngleSmoother;

        public Vector2Int Direction { get; protected set; }

        #region Properties
        public virtual bool IsMoving
        {
            get
            {
                return movementRoutine != null;
            }
        }
        #endregion

        public abstract void StopMoving();

        /// <summary>
        /// Moves this object towards it's next tile position by a given step.
        /// </summary>
        /// <remarks>
        /// Takes into account restricted movement type.
        /// </remarks>
        /// <param name="tilePos">The position of the next tile in the path.</param>
        protected void PerformMove(Vector3 tilePos)
        {
            float step = MoveSpeed * Time.deltaTime;
            if (restrictMovementAxes)
            {
                Vector3 pos = transform.position;
                if (tilePos.y > pos.y ||
                    (Mathf.Approximately(tilePos.x, pos.x) && Mathf.Approximately(tilePos.z, pos.z)))
                {
                    // If the next tile is higher than our current position or we are currently above our next tile,
                    // then move vertically.
                    pos.y = Mathf.MoveTowards(pos.y, tilePos.y, step);
                }
                else
                {
                    // Move horizontally otherwise.
                    pos.x = Mathf.MoveTowards(pos.x, tilePos.x, step);
                    pos.z = Mathf.MoveTowards(pos.z, tilePos.z, step);
                }
                transform.position = pos;
            }
            else
            {
                //Debug.Log("Moving");
                transform.position = Vector3.MoveTowards(transform.position, tilePos, step);
            }

            // Rotates the model based on the direction the object is moving.
            if (rotateModel != null)
            {
                RotateToward(rotateModel, Direction);
            }
        }

        /// <summary>
        /// Rotates the model of this object to point in the direction the object is facing.
        /// </summary>
        /// <param name="rotateTransform">The transform of the object to rotate.</param>
        /// <param name="direction">The direction the object is moving in.</param>
        protected void RotateToward(Transform rotateTransform, Vector2Int direction)
        {
            Vector3 eulers = rotateTransform.eulerAngles;
            float angle = MathHelpers.VectorToDegAngleWorld(direction);
            // Calculate the speed our angle should rotate at based on the time it takes the object we're following to
            // reach it's next space based on it's speed.
            eulers.y = Mathf.SmoothDampAngle(eulers.y, angle, ref dampAngleSmoother, rotationTime);
            //eulers.y = angle;
            rotateTransform.eulerAngles = eulers;
        }
    }
}
