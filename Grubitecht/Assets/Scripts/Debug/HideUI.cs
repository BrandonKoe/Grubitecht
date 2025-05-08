/*****************************************************************************
// File Name : HideUI.cs
// Author : Brandon Koederitz
// Creation Date : May 8, 2025
//
// Brief Description : Allows me to hide the UI with a keyboard input for capturing footage.
*****************************************************************************/
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grubitecht.DebugFeatures
{
    public class HideUI : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private GameObject canvas;
        private InputAction hideUIAction;

        /// <summary>
        /// Subscribes to the input event.
        /// </summary>
        private void Awake()
        {
            hideUIAction = inputActions.FindAction("HideUI");
            hideUIAction.performed += HideUIAction_Performed;
        }
        private void OnDestroy()
        {
            Debug.Log("OnDestroy called");
            hideUIAction.performed -= HideUIAction_Performed;
        }

        /// <summary>
        /// Toggles this object's active state when the hide UI button is pressed.
        /// </summary>
        /// <param name="obj"></param>
        private void HideUIAction_Performed(InputAction.CallbackContext obj)
        {
            Debug.Log("Hiding UI");
            canvas.SetActive(!canvas.activeSelf);
        }
    }
}