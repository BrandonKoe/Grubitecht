/*****************************************************************************
// File Name : CameraZoomer.cs
// Author : Brandon Koederitz
// Creation Date : April 15, 2025
//
// Brief Description : Controls the zooming of the camera using the scroll wheel.
*****************************************************************************/
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grubitecht
{
    public class CameraZoomer : CameraController
    {
        [SerializeField] private float zoomSpeed;
        [SerializeField] private Vector2 zoomBounds;

        private InputAction scrollAction;

        #region Component References
        [SerializeReference, HideInInspector] private Camera cam;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        private void Reset()
        {
            cam = GetComponent<Camera>();
        }
        #endregion

        protected override string ToggleActionName => "Scroll";

        /// <summary>
        /// Subscribes/Unsubscribes the input function that handles scrolling.
        /// </summary>
        /// <param name="playerInput">The PlayerInput component on this object.</param>
        protected override void SubscribeInputs(PlayerInput playerInput)
        {
            scrollAction = playerInput.currentActionMap.FindAction(ToggleActionName);
            scrollAction.performed += ScrollAction_Performed;
        }
        protected override void UnsubscribeInputs()
        {
            scrollAction.performed -= ScrollAction_Performed;
        }

        /// <summary>
        /// Scrolling should not care about the mouse delta at all.
        /// </summary>
        /// <param name="delta"></param>
        protected override void OnProcessInput(Vector2 delta) { }

        /// <summary>
        /// Changes the camera's size based on the player's scroll wheel.
        /// </summary>
        /// <param name="obj">The input callback context that triggered this input event.</param>
        private void ScrollAction_Performed(InputAction.CallbackContext obj)
        {
            // Need to reverse the scroll delta value asscrolling down should increase our camera size.
            float scrollDelta = -obj.ReadValue<Vector2>().y;
            float size = cam.orthographicSize;
            size += scrollDelta * zoomSpeed;
            size = Mathf.Clamp(size, zoomBounds.x, zoomBounds.y);
            cam.orthographicSize = size;
            CallCameraUpdateEvent();
        }
    }
}