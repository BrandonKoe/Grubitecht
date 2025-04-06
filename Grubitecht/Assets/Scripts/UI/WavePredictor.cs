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
        [SerializeField] private RectTransform backImageTransform;
        [SerializeField] private TMP_Text enemyNumberText;
        [SerializeField, Tooltip("The distance from the central position this object should offset itself.")] 
        private float offset;
        [SerializeField, Tooltip("The angle in degrees that each predictor will be offset from each other rotating " +
            "around the origin point")] 
        private float angularOffset;
        [SerializeField] private float padding;

        private Vector3 basePosition;
        private int typeNumber;


        #region Properties
        private RectTransform rectTransform => (RectTransform)transform;
        #endregion
        /// <summary>
        /// Initializes this object with values when it spawns.
        /// </summary>
        /// <param name="predictionSprite">The sprite to display that represents the enemy type.</param>
        /// <param name="basePosition">The position that this UI element reperesents in world space.</param>
        /// <param name="enemyNumber">The number of enemies of this type that will be in the next wave.</param>
        /// <param name="duration">The amount of time until this wave spawns.</param>
        /// <param name="typeNumber">
        /// The number of other predictors.  Used to offset this predictor by an angle.
        /// </param>
        public void Initialize(Sprite predictionSprite, Vector3 basePosition, int enemyNumber, float duration, 
            int typeNumber)
        {
            enemyPredictionImage.sprite = predictionSprite;
            enemyNumberText.text = enemyNumber.ToString();
            this.typeNumber = typeNumber;
            this.basePosition = basePosition;
            // Subscribe to the camera pan event because this object's position should only ever be updated if
            // the camera moves and it needs to be.
            CameraPanner.OnCameraPan += UpdatePosition;
            UpdatePosition();
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
            Unload();
        }

        /// <summary>
        /// Updates the position of this object to be within the bounds of the screen.
        /// </summary>
        private void UpdatePosition()
        {
            // The position in screen space that represents the position of the spawn point this
            // predictor represents.
            Vector2 realOriginPos = RectTransformUtility.WorldToScreenPoint(Camera.main,
                basePosition + baseOffset);
            Vector2 margins = rectTransform.sizeDelta + (2 * offset * Vector2.one);
            // The actual base position that this predictor will be centered around after it has been clamped to
            // be within the bounds of the canvas.
            Vector2 displayOriginPos = new Vector2(
                Mathf.Clamp(realOriginPos.x, margins.x, Screen.width - margins.x),
                Mathf.Clamp(realOriginPos.y, offset + margins.y, Screen.height - margins.y));
            //Debug.Log(rectTransform.sizeDelta);
            float angle = angularOffset * typeNumber;
            Vector2 offsetAngle = MathHelpers.DegAngleToUnitVector(90 - angle);
            //Debug.Log(offsetAngle);
            rectTransform.anchoredPosition = displayOriginPos + offsetAngle * offset;
            //transform.localPosition += (Vector3)offsetAngle * offset;

            UpdatePointerRotation(realOriginPos);
        }

        /// <summary>
        /// Updates the rotation of this predictor's background image so that it points towards the real position 
        /// of the spawn point.
        /// </summary>
        /// <param name="realOriginPos">The screen position that the pointer should point toward.</param>
        private void UpdatePointerRotation(Vector2 realOriginPos)
        {
            Vector2 pointingVector = realOriginPos - rectTransform.anchoredPosition;
            float angle = MathHelpers.VectorToDegAngle(pointingVector);
            Vector3 eulers = backImageTransform.eulerAngles;
            eulers.z = angle;
            backImageTransform.eulerAngles = eulers;
        }

        /// <summary>
        /// Handles behaviour that should happen when the object reaches the end of it's life cycle.
        /// </summary>
        private void Unload()
        {
            CameraPanner.OnCameraPan -= UpdatePosition;
            Destroy(gameObject);
        }
    }
}