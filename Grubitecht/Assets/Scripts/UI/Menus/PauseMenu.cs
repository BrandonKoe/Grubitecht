/*****************************************************************************
// File Name : PauseMenu.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Controls opening the pause menu and pausing the game.
*****************************************************************************/
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Grubitecht.UI
{
    [RequireComponent(typeof(PlayerInput))]
    public class PauseMenu : BaseMenu
    {
        [SerializeField,Tooltip("The GameObjec that contains all of the pause menu content")] 
        private GameObject pauseMenuObject;
        public static bool IsPaused { get; private set; }

        private InputAction pauseAction;

        #region Setup
        /// <summary>
        /// Assign the singleton instance.
        /// </summary>
        private void Awake()
        {
            if (TryGetComponent(out PlayerInput input))
            {
                pauseAction = input.currentActionMap.FindAction("Pause");

                pauseAction.performed += PauseAction_Performed;
            }
        }

        /// <summary>
        /// Remove the singleton reference & unsubscribe input functions.
        /// </summary>
        private void OnDestroy()
        {
            IsPaused = false;
            pauseAction.performed -= PauseAction_Performed;
        }
        #endregion

        #region Pause Toggling
        /// <summary>
        /// Handles the payler pressing the pause button.
        /// </summary>
        /// <param name="obj"></param>
        private void PauseAction_Performed(InputAction.CallbackContext obj)
        {
            if (IsPaused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }

        /// <summary>
        /// Pauses the game and opens the pause menu.
        /// </summary>
        public void Pause()
        {
            // Note: Pausing the game only stops time if pause mode is on. (Not implemented yet).
            TogglePause(true);
        }

        /// <summary>
        /// Unpauses the game.
        /// </summary>
        public void Unpause()
        {
            CloseAllSubMenus();
            TogglePause(false);
        }

        /// <summary>
        /// Toggles the state of the pause menu.
        /// </summary>
        /// <param name="val">Whether to have the pause menu enabled or disabled.</param>
        private void TogglePause(bool val)
        {
            IsPaused = val;
            pauseMenuObject.SetActive(val);
        }
        #endregion
    }
}
