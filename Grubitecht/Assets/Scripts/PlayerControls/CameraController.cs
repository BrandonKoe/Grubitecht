/*****************************************************************************
// File Name : CameraController.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : Base class for scripts that control the camera.
*****************************************************************************/
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grubitecht
{
    public abstract class CameraController : MonoBehaviour
    {
        private InputAction deltaAction;
        private InputAction toggleAction;
        protected bool isControlling;

        // Use this event to update graphics that change when the user moves their camera around.
        public static event Action OnCameraUpdate;

        #region Properties
        protected abstract string ToggleActionName
        {
            get;
        }
        #endregion

        /// <summary>
        /// Set up and unsubscribe input.
        /// </summary>
        private void Awake()
        {
            if (TryGetComponent(out PlayerInput playerInput))
            {
                deltaAction = playerInput.currentActionMap.FindAction("Delta");
                toggleAction = playerInput.currentActionMap.FindAction(ToggleActionName);

                deltaAction.performed += DeltaAction_Performed;
                toggleAction.started += ToggleAction_Started;
                toggleAction.canceled += ToggleAction_Canceled;
            }
        }
        private void OnDestroy()
        {
            deltaAction.performed -= DeltaAction_Performed;
            toggleAction.started -= ToggleAction_Started;
            toggleAction.canceled -= ToggleAction_Canceled;
        }


        /// <summary>
        /// Toggles camera panning while the player holds down right click.
        /// </summary>
        /// <param name="obj"></param>
        private void ToggleAction_Started(InputAction.CallbackContext obj)
        {
            isControlling = true;
        }
        private void ToggleAction_Canceled(InputAction.CallbackContext obj)
        {
            isControlling = false;
        }

        /// <summary>
        /// When the player moves their mouse, use the delta to move the camera.
        /// </summary>
        /// <param name="obj"></param>
        private void DeltaAction_Performed(InputAction.CallbackContext obj)
        {
            if (isControlling)
            {
                Vector2 delta = obj.ReadValue<Vector2>();
                OnProcessInput(delta);
                OnCameraUpdate?.Invoke();
            }
        }

        /// <summary>
        /// Delegate control to the child class for how it should process the camera change.
        /// </summary>
        /// <param name="delta">The delta of the mouse to process in this update.</param>
        protected abstract void OnProcessInput(Vector2 delta);
    }
}