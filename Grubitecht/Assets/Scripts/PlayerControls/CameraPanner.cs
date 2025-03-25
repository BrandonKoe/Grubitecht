/*****************************************************************************
// File Name : CameraPanner.cs
// Author : Brandon Koederitz
// Creation Date : March 25, 2025
//
// Brief Description : Allows the player to pan the camera within certain bounds.
*****************************************************************************/
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grubitecht
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(PlayerInput))]
    public class CameraPanner : MonoBehaviour
    {
        [SerializeField, Tooltip("The min (x) and max (y) value that this camera's local x position cannot exceed.")] 
        private Vector2 panBoundsX;
        [SerializeField, Tooltip("The min (x) and max (y) value that this camera's local y position cannot exceed.")] 
        private Vector2 panBoundsY;
        [SerializeField] private float panSpeed;
        private InputAction deltaAction;
        private InputAction panAction;

        private bool isPanning;

        /// <summary>
        /// Set up and unsubscribe input.
        /// </summary>
        private void Awake()
        {
            if (TryGetComponent(out PlayerInput playerInput))
            {
                deltaAction = playerInput.currentActionMap.FindAction("Delta");
                panAction = playerInput.currentActionMap.FindAction("Pan");

                deltaAction.performed += DeltaAction_Performed;
                panAction.started += PanAction_Started;
                panAction.canceled += PanAction_Canceled;
            }
        }
        private void OnDestroy()
        {
            deltaAction.performed -= DeltaAction_Performed;
            panAction.started -= PanAction_Started;
            panAction.canceled -= PanAction_Canceled;
        }

        /// <summary>
        /// Toggles camera panning while the player holds down right click.
        /// </summary>
        /// <param name="obj"></param>
        private void PanAction_Started(InputAction.CallbackContext obj)
        {
            isPanning = true;
        }
        private void PanAction_Canceled(InputAction.CallbackContext obj)
        {
            isPanning = false;   
        }

        /// <summary>
        /// When the player moves their mouse, use the delta to move the camera.
        /// </summary>
        /// <param name="obj"></param>
        private void DeltaAction_Performed(InputAction.CallbackContext obj)
        {
            if (isPanning)
            {
                Vector2 delta = obj.ReadValue<Vector2>();
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
}
