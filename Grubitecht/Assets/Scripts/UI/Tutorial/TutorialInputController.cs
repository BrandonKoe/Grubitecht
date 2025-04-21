/*****************************************************************************
// File Name : TutorialInputController.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Controls players inputting the default input to confirm continuing the tutorial.
*****************************************************************************/
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grubitecht.UI.Tutorial
{
    [RequireComponent(typeof(PlayerInput))]
    public class TutorialInputController : MonoBehaviour
    {
        private InputAction confirmAction;

        public static event Action OnConfirmTutorialEvent;

        /// <summary>
        /// Setup/unsubscribe input.
        /// </summary>
        private void Awake()
        {
            if (TryGetComponent(out PlayerInput playerInput))
            {
                confirmAction = playerInput.currentActionMap.FindAction("Confirm");

                confirmAction.performed += ConfirmAction_Performed;
            }
        }
        private void OnDestroy()
        {
            if (confirmAction != null)
            {
                confirmAction.performed -= ConfirmAction_Performed;
            }
        }

        /// <summary>
        /// Broadcasts out an event to let other tutorials know that the player has pressed the confirm button.
        /// </summary>
        /// <param name="obj">Unused.</param>
        private void ConfirmAction_Performed(InputAction.CallbackContext obj)
        {
            OnConfirmTutorialEvent?.Invoke();
        }
    }
}