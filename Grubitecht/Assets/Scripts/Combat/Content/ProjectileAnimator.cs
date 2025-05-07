/*****************************************************************************
// File Name : ProjectileAnimator.cs
// Author : Brandon Koederitz
// Creation Date : April 28, 2025
//
// Brief Description : Animates a moving projectile object's local position based on the distance to the target.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class ProjectileAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject animatedGameObject;
        [Header("X")]
        [SerializeField] private float xMin;
        [SerializeField] private float xMax;
        [SerializeField] private AnimationCurve xCurve;
        [Header("Y")]
        [SerializeField] private float yMin;
        [SerializeField] private float yMax;
        [SerializeField] private AnimationCurve yCurve;
        [Header("Z")]
        [SerializeField] private float zMin;
        [SerializeField] private float zMax;
        [SerializeField] private AnimationCurve zCurve;

        /// <summary>
        /// Starts animating this projectile based on a given target.
        /// </summary>
        /// <param name="target"></param>
        public void StartAnimating(Attackable target)
        {
            StartCoroutine(AnimRoutine(target)); 
        }

        /// <summary>
        /// Animates the target animated game object's local position based on the current distance between the
        /// projectile and the target.
        /// </summary>
        /// <param name="target">The target of the projectile.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator AnimRoutine(Attackable target)
        {
            // Need to ensure that at least 0.01 is set as base distance so we dont end up dividing by 0.
            float baseDistance = Mathf.Max(Vector2.Distance(transform.position, target.transform.position), 0.01f);
            float xVal = Random.Range(xMin, xMax);
            float yVal = Random.Range(yMin, yMax);
            float zVal = Random.Range(zMin, zMax);

            while (target != null)
            {
                float currentDistance = Vector2.Distance(transform.position, target.transform.position);
                float distanceProgress = Mathf.Clamp01(1 - (currentDistance / baseDistance));

                float x = xCurve.Evaluate(distanceProgress) * xVal;
                float y = yCurve.Evaluate(distanceProgress) * yVal;
                float z = zCurve.Evaluate(distanceProgress) * zVal;

                animatedGameObject.transform.localPosition = new Vector3(x, y, z);

                yield return null;
            }
        }
    }
}