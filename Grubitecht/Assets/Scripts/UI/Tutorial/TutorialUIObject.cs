/*****************************************************************************
// File Name : TutorialText.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Controls tutorial text objects that are shown during the tutorial.
*****************************************************************************/
using System;
using TMPro;
using UnityEngine;

namespace Grubitecht.UI.Tutorial
{
    public class TutorialUIObject : MonoBehaviour
    {
        [SerializeField] private RectTransform arrow;
        [field: SerializeField] public TMP_Text TextObject { get; private set; }

        [SerializeField] private Vector3 worldOffset = new Vector3(0, 1, 0);
        [SerializeField] private Vector2 UIOffset;
        [SerializeField] private float padding;

        private TutorializedObject target;
        private Vector3 targetPositionOverride;

        #region Properties
        private RectTransform rectTransform => (RectTransform)transform;

        private Vector3 TargetPosition
        {
            get
            {
                if (target != null)
                {
                    return target.transform.position;
                }
                return targetPositionOverride;
            }
        }
        #endregion

        /// <summary>
        /// Subscribe to camera update event so that the tutorial text moves when the camera is updated.
        /// </summary>
        private void Awake()
        {
            CameraController.OnCameraUpdate += UpdatePosition;
        }
        private void OnDestroy()
        {
            CameraController.OnCameraUpdate -= UpdatePosition;
        }

        /// <summary>
        /// initializes the tutorial with a target object.
        /// </summary>
        /// <param name="target"></param>
        public void Initialize(TutorializedObject target)
        {
            this.target = target;
            UpdatePosition();
        }

        /// <summary>
        /// initializes the tutorial with a target object.
        /// </summary>
        /// <param name="target"></param>
        public void Initialize(Vector3 targetPosition)
        {
            this.targetPositionOverride = targetPosition;
            UpdatePosition();
        }

        /// <summary>
        /// Updates the position of the tutorial so it's near the object it's talking about.
        /// </summary>
        private void UpdatePosition()
        {
            if (target.transform is RectTransform rTrans)
            {
                UpdateUIPosition(rTrans);
            }
            else
            {
                UpdatePositionWorld();
            }
        }

        /// <summary>
        /// Updates the position of this tutorial so it is near a UI object it is explaining.
        /// </summary>
        private void UpdateUIPosition(RectTransform target)
        {
            Vector2 realOriginPos = target.anchoredPosition + UIOffset;

            rectTransform.anchoredPosition = realOriginPos;

            UpdateArrow(realOriginPos);
        }

        /// <summary>
        /// Updates the position of this object to be within the bounds of the screen.
        /// </summary>
        private void UpdatePositionWorld()
        {
            // The position in screen space that represents the position of the spawn point this
            // predictor represents.
            Vector2 realOriginPos = RectTransformUtility.WorldToScreenPoint(Camera.main,
                TargetPosition + worldOffset) + UIOffset;

            Vector2 margins = rectTransform.sizeDelta + (padding * Vector2.one);
            // The actual base position that this object will be located at after it has been clamped to
            // be within the bounds of the canvas.
            Vector2 displayOriginPos = new Vector2(
                Mathf.Clamp(realOriginPos.x, margins.x, Screen.width - margins.x),
                Mathf.Clamp(realOriginPos.y, margins.y, Screen.height - margins.y));

            rectTransform.anchoredPosition = displayOriginPos;

            UpdateArrow(realOriginPos);
        }

        /// <summary>
        /// Updates the rotation of this tutorial's arrow that points toward the object it's talking about.
        /// </summary>
        /// <param name="realOriginPos">The screen position that the pointer should point toward.</param>
        private void UpdateArrow(Vector2 realOriginPos)
        {
            Vector2 GetDominantDirection(Vector2 pointingVector)
            {
                if (Mathf.Abs(pointingVector.x) > Mathf.Abs(pointingVector.y))
                {
                    return new Vector2(MathHelpers.GetSign(pointingVector.x), 0);
                }
                else
                {
                    return new Vector2(0, MathHelpers.GetSign(pointingVector.y));
                }
            }

            Vector2 dominantDirection = GetDominantDirection(realOriginPos - rectTransform.anchoredPosition);

            // Updates the rotation of the arrow to face the dominant direction.
            float angle = MathHelpers.VectorToDegAngle(dominantDirection);
            arrow.eulerAngles = new Vector3(arrow.eulerAngles.x, arrow.eulerAngles.y, angle);

            Vector2 boundingBox = new Vector2((rectTransform.rect.width + arrow.rect.width) / 2,
                (rectTransform.rect.height + arrow.rect.height) / 2);
            // Updates the arrow's position to move along the edge of the tutorial object.
            // If horizontal is our dominant direction...
            if (dominantDirection.x > dominantDirection.y)
            {
                Vector2 pos = arrow.anchoredPosition;
                pos.x = boundingBox.x * dominantDirection.x;
                pos.y = Mathf.Clamp(realOriginPos.y, -boundingBox.y, boundingBox.y);
                arrow.anchoredPosition = pos;
            }
            // If vertical is our dominant direction...
            else
            {
                Vector2 pos = arrow.anchoredPosition;
                pos.y = boundingBox.y * dominantDirection.y;
                pos.x = Mathf.Clamp(realOriginPos.x, -boundingBox.x, boundingBox.x);
                arrow.anchoredPosition = pos;
            }
        }
    }
}