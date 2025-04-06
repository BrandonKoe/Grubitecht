/*****************************************************************************
// File Name : WavePredictor.cs
// Author : Brandon Koederitz
// Creation Date : April 3, 2025
//
// Brief Description : Controller for UI objects that displays the next upcoming enemies in a wave.
*****************************************************************************/
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grubitecht.UI
{
    public class WavePredictor : MonoBehaviour
    {
        [SerializeField] private Vector3 baseOffset = new Vector3(0, 1, 0);
        [SerializeField] private Image enemyPredictionImage;
        [SerializeField] private Image timerImage;
        [SerializeField] private TMP_Text enemyNumberText;
        [SerializeField] private float offset;
        [SerializeField] private float angularOffset;

        /// <summary>
        /// Initializes this object with values when it spawns.
        /// </summary>
        /// <param name="predictionSprite">The sprite to display that represents the enemy type.</param>
        /// <param name="basePosition">The position that this UI element reperesents in world space.</param>
        /// <param name="enemyNumber">The number of enemies of this type that will be in the next wave.</param>
        /// <param name="duration">The amount of time until this wave spawns.</param>
        /// <param name="typeNumber">The number of other predictors.  Used to offset this predictor by an angle.</param>
        public void Initialize(Sprite predictionSprite, Vector3 basePosition, int enemyNumber, float duration, int typeNumber)
        {
            enemyPredictionImage.sprite = predictionSprite;
            enemyNumberText.text = enemyNumber.ToString();
            // Update the object's angular offset here.
            transform.position = basePosition + baseOffset;
            float angle = angularOffset * typeNumber;
            Vector2 offsetAngle = MathHelpers.DegAngleToUnitVector(90 - angle);
            //Debug.Log(offsetAngle);
            transform.localPosition += (Vector3)offsetAngle * offset;

            StartCoroutine(LifeCycle(duration));
        }

        /// <summary>
        /// Controls the timer that ticks down as the enemies get closer to spawning.
        /// </summary>
        /// <param name="duration">The duration this object will stick around and act as a timer.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator LifeCycle(float duration)
        {
            float timer = duration;
            while (timer > 0)
            {
                // Updates the fill amount of the timer image so that it runs out as the timer ticks down.
                float normalizedTime = timer / duration;
                timerImage.fillAmount = normalizedTime;
                timer -= Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}