/*****************************************************************************
// File Name : PopupDisplayer.cs
// Author : Brandon Koederitz
// Creation Date : May 4, 2025
//
// Brief Description : Displays an information popup for an object on the UI.
*****************************************************************************/
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grubitecht.UI
{
    public class PopupDisplayer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text contentText;
        [SerializeField] private InputActionAsset inputActions;
        [Header("Settings")]
        [SerializeField] private Vector2 offset;

        private InputAction mousePosAction;

        #region Properties
        private Vector2 MousePos
        {
            get
            {
                if (mousePosAction == null)
                {
                    mousePosAction = inputActions.FindAction("MousePos");
                }
                return mousePosAction.ReadValue<Vector2>();
            }
        }
        private RectTransform rectTransform => transform as RectTransform;
        #endregion

        /// <summary>
        /// Initializes this popup on spawn.
        /// </summary>
        /// <param name="title">The title to display on this popup.</param>
        /// <param name="content">The content of this popup.</param>
        public void Initialize(string title, string content)
        {
            titleText.text = title;
            contentText.text = content;

            CameraController.OnCameraUpdate += UpdatePosition;
        }

        /// <summary>
        /// Deinitializes this popup to unsubscribe events.
        /// </summary>
        public void Deinitialize()
        {
            CameraController.OnCameraUpdate -= UpdatePosition;
        }

        /// <summary>
        /// Updates the popup to follow the mouse cursor.
        /// </summary>
        public void UpdatePosition()
        {
            Vector2 offset = this.offset;

            transform.position = MousePos + offset;
            // This needs to be reworked so it works for y direction as well.

            // If the popups is not fully on screen, then we show it with a reversed x direction offset.
            if (!rectTransform.IsFullyOnScreen())
            {
                offset.x *= -1;
                transform.position = MousePos + offset;
            }
        }
    }
}