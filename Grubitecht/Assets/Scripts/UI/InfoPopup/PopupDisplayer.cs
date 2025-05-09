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
        #region Consts
        private static readonly Vector2Int[] CORNER_DIRECTION_REFERENCE = new Vector2Int[4]
        {
                new Vector2Int(1, 1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, -1),
                new Vector2Int(-1, 1)
        };
        #endregion
        [Header("References")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text contentText;
        [SerializeField] private InputActionAsset inputActions;
        [Header("Settings")]
        [SerializeField] private Vector2 offset;

        private InputAction mousePosAction;

        #region Properties
        private InputAction MousePosAction
        {
            get
            {
                if (mousePosAction == null)
                {
                    mousePosAction = inputActions.FindAction("MousePos");
                }
                return mousePosAction;
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

            UpdatePosition();
            MousePosAction.performed += MousePosAction_Performed;
        }

        /// <summary>
        /// Deinitializes this popup to unsubscribe events.
        /// </summary>
        public void Deinitialize()
        {
           MousePosAction.performed -= MousePosAction_Performed;
        }
        private void MousePosAction_Performed(InputAction.CallbackContext obj)
        {
            UpdatePosition();
        }

        /// <summary>
        /// Updates the popup to follow the mouse cursor.
        /// </summary>
        /// <param name="context">Unused.</param>
        public void UpdatePosition()
        {
            Vector2 offset = this.offset;

            Vector2 mousePos = MousePosAction.ReadValue<Vector2>();
            // Sets this object's position to the position of the mouse + the offset.
            transform.position = mousePos + offset;
            // If the popup is partially off screen, then we need to change the offset to ensure it ends up fully on
            // screen.
            Rect canvasRect = new Rect(0f, 0f, Screen.width, Screen.height);

            // Get the corners of this object's rect transform.
            Vector3[] rectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(rectCorners);

            Vector2Int resultantNormal = Vector2Int.zero;
            for (int i = 0; i < rectCorners.Length; i++)
            {
                // If this corner is out of bounds, then it's normal should be added to the resultant normal to find
                // what the offset should be multiplied by.
                if (!canvasRect.Contains(rectCorners[i]))
                {
                    resultantNormal += CORNER_DIRECTION_REFERENCE[i];
                }
            }

            //Debug.Log(resultantNormal);
            // If one of our corners was out of bounds.
            if (resultantNormal != Vector2.zero)
            {
                // Reverse the direction of the offset for any direction in the resultant normal that was not zero.
                offset.x = resultantNormal.x == 0 ? offset.x : -offset.x;
                offset.y = resultantNormal.y == 0 ? offset.y : -offset.y;
                // Sets this object's position to the position of the mouse + the new offset.
                transform.position = mousePos + offset;
            }
        }
    }
}