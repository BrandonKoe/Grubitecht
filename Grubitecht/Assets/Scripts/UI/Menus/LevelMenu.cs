/*****************************************************************************
// File Name : LevelMenu.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Controls opening the menu that allows for navigation within a level.  Basically a pause menu.
*****************************************************************************/
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grubitecht.UI
{
    [RequireComponent(typeof(PlayerInput))]
    public class LevelMenu : BaseMenu
    {
        [SerializeField,Tooltip("The GameObject that contains all of the level menu content")] 
        private GameObject levelMenuObject;
        public static bool IsOpen { get; private set; }

        private InputAction levelMenuAction;

        #region Setup
        /// <summary>
        /// Assign the singleton instance.
        /// </summary>
        private void Awake()
        {
            if (TryGetComponent(out PlayerInput input))
            {
                levelMenuAction = input.currentActionMap.FindAction("LevelMenu");

                levelMenuAction.performed += OpenLevelMenuAction_Performed;
            }
        }

        /// <summary>
        /// Remove the singleton reference & unsubscribe input functions.
        /// </summary>
        private void OnDestroy()
        {
            IsOpen = false;
            levelMenuAction.performed -= OpenLevelMenuAction_Performed;
        }
        #endregion

        #region Menu Toggling
        /// <summary>
        /// Handles the player pressing the level menu button.
        /// </summary>
        /// <param name="obj"></param>
        private void OpenLevelMenuAction_Performed(InputAction.CallbackContext obj)
        {
            if (IsOpen)
            {
                CloseLevelMenu();
            }
            else
            {
                OpenLevelMenu();
            }
        }

        /// <summary>
        /// Pauses the game and opens the pause menu.
        /// </summary>
        public void OpenLevelMenu()
        {
            // Note: Pausing the game only stops time if pause mode is on. (Not implemented yet).
            ToggleMenu(true);
        }

        /// <summary>
        /// Unpauses the game.
        /// </summary>
        public void CloseLevelMenu()
        {
            CloseAllSubMenus();
            ToggleMenu(false);
        }

        /// <summary>
        /// Toggles the state of the pause menu.
        /// </summary>
        /// <param name="val">Whether to have the pause menu enabled or disabled.</param>
        private void ToggleMenu(bool val)
        {
            IsOpen = val;
            levelMenuObject.SetActive(val);
            // Always check for a change in pause state when the level menu is changed.
            PauseModeController.CheckPauseState();
        }
        #endregion
    }
}
