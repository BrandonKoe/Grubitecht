/*****************************************************************************
// File Name : ObjectTweener.cs
// Author : Brandon Koederitz
// Creation Date : April 29, 2025
//
// Brief Description : Tween an object between its current position and a new position.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Grubitecht.Credits
{
    public class CameraTweener : MonoBehaviour
    {
        #region CONSTS
        private const float END_PROXIMITY = 0.1f;
        #endregion
        [SerializeField] private Transform targetTransform;
        [SerializeField] private float tweenTime;
        [SerializeField] private UnityEvent onFinishTweenEvent;

        #region Properties
        public float Speed
        {
            get
            {
                return Vector3.Distance(targetTransform.position, transform.position) / tweenTime;
            }
        }
        #endregion

        /// <summary>
        /// Start tweening on awake.
        /// </summary>
        private void Awake()
        {
            StartCoroutine(TweenRoutine());
        }

        /// <summary>
        /// Tweens the object between it's current position and the target position.
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator TweenRoutine()
        {
            Vector3 startingPosition = transform.position;
            float timer = tweenTime;
            while (timer > 0)
            {
                float normalizedProgress = 1 - (timer / tweenTime);

                transform.position = Vector3.Lerp(startingPosition, targetTransform.position, normalizedProgress);

                timer -= Time.deltaTime;
                yield return null;  
            }
            onFinishTweenEvent?.Invoke();
        }
    }
}