/*****************************************************************************
// File Name : CreditsManager.cs
// Author : Brandon Koederitz
// Creation Date : April 29, 2025
//
// Brief Description : Controls the progression of the credit sequence.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Grubitecht.Credits
{
    public class CreditsManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera cam;
        [SerializeField] private CameraTweener tweener;
        [SerializeField] private Transform spawnPoint;
        [Header("Settings")]
        [SerializeField] private float initialDelay;
        [SerializeField, Tooltip("The amount of time each credits object should be on screen.")] 
        private float creditsTime;
        [SerializeField] private float creditsHeight = 15;
        [SerializeField, Tooltip("The amount of space between each credits object.  Base should be the height of" +
            " the credit object.")] 
        private float creditsSpacing = 10;
        [SerializeField] private CreditsData[] credits;
        #region Nested
        [System.Serializable]
        private class CreditsData
        {
            [SerializeField] internal string header;
            [SerializeField] internal float fontSize;
            [SerializeField, TextArea] internal string body;
            [SerializeField] internal CreditsObject creditsPrefab;
            [SerializeField] internal UnityEvent OnCreditSpawnEvent;
        }
        #endregion

        /// <summary>
        /// Start playing the credits on awake.
        /// </summary>
        private void Awake()
        {
            StartCoroutine(CreditsRoutine());
        }

        /// <summary>
        /// Spawns credits sequentially based on the credits data provided.
        /// </summary>
        /// <remarks>
        /// Assumes the credits will be scrolling upwards in the positive Z direction.
        /// </remarks>
        /// <returns>Coroutine.</returns>
        private IEnumerator CreditsRoutine()
        {
            //The amount of total distance that the credit objects will have to cover in the given credits time.
            float totalCredDist = (tweener.Speed * creditsTime) + (cam.orthographicSize * 2f) + creditsHeight;
            // The speed the individual credits object have to move at to clear the screen in the specified credits
            // time.
            float creditVelocity = totalCredDist / creditsTime;
            // The amount of time that should be delayed between spawning each credit object.
            float newCreditsTime = (creditsSpacing + creditsHeight) / creditVelocity;
            
            yield return new WaitForSeconds(initialDelay);

            // Continually spawns new credits until all have been shown.
            List<CreditsData> currentCredits = credits.ToList();
            while (currentCredits.Count > 0)
            {
                // Spawns a credit object based on the current credit data.
                CreditsData current = currentCredits[0];
                CreditsObject credObj = Instantiate(current.creditsPrefab, spawnPoint.position, Quaternion.identity, 
                    transform);
                credObj.Initialize(current.header, current.body, creditVelocity * Vector3.forward, current.fontSize);

                current.OnCreditSpawnEvent?.Invoke();

                currentCredits.Remove(current);
                yield return new WaitForSeconds(newCreditsTime);
            }
        }
    }
}