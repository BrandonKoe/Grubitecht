/*****************************************************************************
// File Name : Projectile.cs
// Author : Brandon Koederitz
// Creation Date : April 29, 2025
//
// Brief Description : Scrolls credit objects upward when they're spawned by the credits manager.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

namespace Grubitecht.Credits
{
    public class CreditsObject : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private TMP_Text bodyText;
        [field: Header("Settings")]
        [field: SerializeField] public float Height { get; private set; }

        /// <summary>
        /// Initializes this credits object with text and begins scrolling.
        /// </summary>
        public void Initialize(string header, string body, Vector3 velocity, float fontSize)
        {
            headerText.text = header;
            bodyText.text = body;
            bodyText.fontSize = fontSize;
            StartCoroutine(ScrollRoutine(velocity));
        }

        /// <summary>
        /// Scrolls the credits upwards.
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator ScrollRoutine(Vector3 velocity)
        {
            void PerformMove()
            {
                transform.position += velocity * Time.deltaTime;
            }

            float totalTime = 0f;
            //The object needs to move until the back is in focus...
            while (!Camera.main.CheckObjectInBounds(transform.position))
            {
                totalTime += Time.deltaTime;
                PerformMove();
                yield return null;
            }

            // Then it needs to move until the back is out of focus.
            while (Camera.main.CheckObjectInBounds(transform.position))
            {
                totalTime += Time.deltaTime;
                PerformMove();
                yield return null;
            }

            Debug.Log("Total elapsed time: " + totalTime);
            // Once the credits objecct has left the camera, it should be destroyed.
            Destroy(gameObject);
        }
    }
}