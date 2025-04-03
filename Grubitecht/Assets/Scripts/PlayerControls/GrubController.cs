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
using System.Collections;
using UnityEngine;

namespace Grubitecht.World
{
    public class GrubController : MonoBehaviour
    {
        [SerializeField] private float risingSpeed;
        [SerializeField] private float followDistance = 1f;
        private Coroutine followRoutine;
        private bool isFollowing;

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
            while (isFollowing)
            {

                //if (followedObject.gridObject.CurrentSpace != referenceSpace)
                //{
                //    // updates this object's position whenever the followed object moves to a new space.
                //    referenceSpace = followedObject.gridObject.CurrentSpace;
                //    gridObject.SetCurrentSpace(referenceSpace - (Vector3Int)followedObject.Direction);
                //    gridObject.SnapToSpace();
                //}

                

                //// Moves this grub towards the followed grid navigator.
                //float step = followedObject.MoveSpeed * Time.deltaTime;
                //Vector3 tilePos = gridObject.GetOccupyPosition(followedObject.gridObject.CurrentSpace);
                //SetRotation(followedObject.Direction);
                //transform.position = Vector3.MoveTowards(transform.position, tilePos, step);
                ////Vector3 pos = followedObject.transform.position + -(Vector3Int)followedObject.Direction;

                yield return null;
            }
        }


        private void SnapToTransform(Transform followedTransform, Vector3Int gridDir)
        {
            Vector3Int direction = new Vector3Int(gridDir.x, gridDir.z, gridDir.y);
            float savedY = transform.position.y;
            transform.position = followedTransform.position;
        }

        /// <summary>
        /// Perform behaviour that should happen when this grub is recalled.
        /// </summary>
        public void RecallGrub()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Rotates the grub to face in the direction it is mkving.
        /// </summary>
        /// <param name="direction">The direction it is moving.</param>
        private void SetRotation(Vector2Int direction)
        {
            float angle = MathHelpers.VectorToDegAngle(direction);
            Vector3 eulers = transform.eulerAngles;
            eulers.y = angle;
            transform.eulerAngles = eulers;
        }
    }
}