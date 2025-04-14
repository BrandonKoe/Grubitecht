/*****************************************************************************
// File Name : CameraPanner.cs
// Author : Brandon Koederitz
// Creation Date : March 25, 2025
//
// Brief Description : Allows the player to pan the camera within certain bounds.
*****************************************************************************/
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grubitecht
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(PlayerInput))]
    public class CameraPanner : CameraController
    {
        [SerializeField, Tooltip("The min (x) and max (y) value that this camera's local x position cannot exceed.")] 
        private Vector2 panBoundsX;
        [SerializeField, Tooltip("The min (x) and max (y) value that this camera's local y position cannot exceed.")] 
        private Vector2 panBoundsY;
        [SerializeField] private float panSpeed;

        protected override string ToggleActionName => "Pan";

        /// <summary>
        /// Handles panning the camera when the player holds down middle mouse and moves the mouse.
        /// </summary>
        /// <param name="delta"></param>
        protected override void OnProcessInput(Vector2 delta)
        {
            // Reverse the delta vector as the camera should move in the opposite direction of the mouse delta.
            delta *= -1;
            Vector3 newPos = transform.localPosition + ((Vector3)delta * panSpeed);
            newPos.x = Mathf.Clamp(newPos.x, panBoundsX.x, panBoundsX.y);
            newPos.y = Mathf.Clamp(newPos.y, panBoundsY.x, panBoundsY.y);
            // Update the new camera position.
            transform.localPosition = newPos;
        }
    }
}
