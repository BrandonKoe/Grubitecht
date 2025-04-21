/*****************************************************************************
// File Name : CameraRotater.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : Allows the player to rotate the camera around the center of the level.
*****************************************************************************/
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grubitecht
{
    public class CameraRotater : CameraController
    {
        [SerializeField] private Transform rotateObject;
        [SerializeField, Tooltip("The min (x) and max (y) value that this camera's rotation cannot exceed.")]
        private Vector2 rotateBounds = new Vector2(0, 180);
        [SerializeField] private float rotateSpeed;

        protected override string ToggleActionName => "Rotate";

        /// <summary>
        /// Rotates the camera when the player holds down right click and moves the mouse.
        /// </summary>
        /// <param name="delta"></param>
        protected override void OnProcessInput(Vector2 delta)
        {
            Vector3 eulers = rotateObject.localEulerAngles;
            float newAngle = eulers.y + (rotateSpeed * delta.x);
            // Ensures the new angle is always within 360 degrees.
            newAngle = MathHelpers.RestrictAngle(newAngle);
            newAngle = Mathf.Clamp(newAngle, rotateBounds.x, rotateBounds.y);
            eulers.y = newAngle;
            rotateObject.localEulerAngles = eulers;
        }
    }
}