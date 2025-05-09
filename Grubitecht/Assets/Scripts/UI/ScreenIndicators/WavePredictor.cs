/*****************************************************************************
// File Name : WavePredictor.cs
// Author : Brandon Koederitz
// Creation Date : April 3, 2025
//
// Brief Description : Controller for UI objects that displays the next upcoming enemies in a wave.
*****************************************************************************/
using Grubitecht.DebugFeatures;
using Grubitecht.UI.InfoPanel;
using Grubitecht.Waves;
using Grubitecht.World;
using Grubitecht.World.Objects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grubitecht.UI
{
    public class WavePredictor : SelectableObject, ISelectable
    {
        [Header("References")]
        [SerializeField] private Image enemyPredictionImage;
        [SerializeField] private Image timerImage;
        [SerializeField] private RectTransform backImageTransform;
        [SerializeField] private TMP_Text enemyNumberText;
        [Header("Settings")]
        [SerializeField] private Vector3 baseOffset = new Vector3(0, 1, 0);
        [SerializeField, Tooltip("The distance from the central position this object should offset itself.")] 
        private float offset;
        [SerializeField, Tooltip("The angle in degrees that each predictor will be offset from each other rotating " +
            "around the origin point")] 
        private float angularOffset;
        [SerializeField] private float padding;

        private Vector3 basePosition;
        private int typeNumber;

        #region Component References
        [SerializeReference, HideInInspector] private InfoPopup infoPopup;
        /// <summary>
        /// Assign component references on component reset.
        /// </summary>
        private void Reset()
        {
            infoPopup = GetComponent<InfoPopup>();
        }
        #endregion

        #region Properties
        //private RectTransform rectTransform => (RectTransform)transform;
        #endregion

        #region Nested
        private delegate List<InfoValueBase> InfoGetterReplacer();
        #endregion
        /// <summary>
        /// Don't load info getters on awake, as they are loaded on initialize.
        /// </summary>
        protected override void Awake()
        {
            //base.Awake();
        }

        /// <summary>
        /// Initializes this object with values when it spawns.
        /// </summary>
        /// <param name="enemyPrefab">The sprite to display that represents the enemy type.</param>
        /// <param name="basePosition">The position that this UI element reperesents in world space.</param>
        /// <param name="enemyNumber">The number of enemies of this type that will be in the next wave.</param>
        /// <param name="duration">The amount of time until this wave spawns.</param>
        /// <param name="typeNumber">
        /// The number of other predictors.  Used to offset this predictor by an angle.
        /// </param>
        public void Initialize(EnemyController enemyPrefab, Vector3 basePosition, int enemyNumber, float duration, 
            int typeNumber)
        {
            // If this object is not active in the hierarchy, we can't start the life cycle coroutine so we should
            // just destroy this game object to avoid clutter.
            if (!gameObject.activeInHierarchy)
            {
                Destroy(gameObject);
            }
            enemyPredictionImage.sprite = enemyPrefab.EnemySpriteIcon;
            infoPopup.TitleText = enemyPrefab.EnemyName;
            infoPopup.BodyText = enemyPrefab.EnemyDescription;
            // Sets the info getter this object will use to present info values to the panel
            // This code is here to make it so that when the predictor is selected, it displays info about the enemy.
            objectName = enemyPrefab.EnemyName;
            objectDesription = enemyPrefab.EnemyDescription;
            LoadGettersForObject(infoGetters, enemyPrefab.gameObject);

            enemyNumberText.text = enemyNumber.ToString();
            this.typeNumber = typeNumber;
            this.basePosition = basePosition;
            // Subscribe to the camera pan event because this object's position should only ever be updated if
            // the camera moves and it needs to be.
            CameraController.OnCameraUpdate += UpdatePosition;
            UpdatePosition();
            // If this object is already active in the hierarchy, then we start the life cycle.
            StartCoroutine(LifeCycle(duration));
        }

        /// <summary>
        /// If this object is disabled by disabling the UI with H, then we should destroy this object immediately
        /// so it doesnt cause clutter.
        /// </summary>
        private void OnDisable()
        {
            if (HideUI.IsUIHidden)
            {
                Destroy(gameObject);
            }
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
                // Continually delay here while the wave is paused.
                if (WaveManager.IsPaused)
                {
                    yield return null;
                    continue;
                }
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
            //Vector2 realOriginPos = RectTransformUtility.WorldToScreenPoint(Camera.main,
            //    basePosition + baseOffset);

            RectTransform rectTransform = transform as RectTransform;
            Vector2 realOriginPos = Camera.main.WorldToScreenPoint(basePosition + baseOffset);
            Vector2 margins = rectTransform.sizeDelta + (2 * offset * Vector2.one) + (Vector2.one * padding);
            // The actual base position that this predictor will be centered around after it has been clamped to
            // be within the bounds of the canvas.
            Vector2 displayOriginPos = new Vector2(
                Mathf.Clamp(realOriginPos.x, margins.x, Screen.width - margins.x),
                Mathf.Clamp(realOriginPos.y, offset + margins.y, Screen.height - margins.y));
            //Debug.Log(rectTransform.sizeDelta);
            float angle = angularOffset * typeNumber;
            Vector2 offsetAngle = MathHelpers.DegAngleToUnitVector(90 - angle);
            //Debug.Log(offsetAngle);
            rectTransform.position = displayOriginPos + offsetAngle * offset;
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
            RectTransform rectTransform = transform as RectTransform;
            Vector2 pointingVector = realOriginPos - (Vector2)rectTransform.position;
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
            CameraController.OnCameraUpdate -= UpdatePosition;
            Destroy(gameObject);
        }

        /// <summary>
        /// Unsubscribe events on destroy.
        /// </summary>
        private void OnDestroy()
        {
            CameraController.OnCameraUpdate -= UpdatePosition;
        }
    }
}