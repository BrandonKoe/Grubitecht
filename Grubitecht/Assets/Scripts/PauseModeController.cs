/*****************************************************************************
// File Name : PauseModeController.cs
// Author : Brandon Koederitz
// Creation Date : May 4, 2025
//
// Brief Description : Controlls engaging and disengaging pause mode to pause gameplay.
*****************************************************************************/
using Grubitecht.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grubitecht
{
    [RequireComponent(typeof(PlayerInput))]
    public class PauseModeController : MonoBehaviour
    {
        [SerializeField] private GameObject pauseUIOverlay;

        private InputAction pauseAction;

        private static bool isPausedByInput;
        private static GameObject uiOverlay;


        #region Properties
        public static bool IsPaused
        {
            get
            {
                return GameSettings.PauseMode && (isPausedByInput || LevelMenu.IsOpen);
            }
        }
        #endregion

        /// <summary>
        /// Subscribe/Unsubscribe input functions.
        /// </summary>
        private void Awake()
        {
            if (TryGetComponent(out PlayerInput playerInput))
            {
                pauseAction = playerInput.currentActionMap.FindAction("Pause");

                pauseAction.performed += PauseAction_Performed;
            }

            if (uiOverlay != null && uiOverlay != pauseUIOverlay)
            {
                Debug.Log("Duplicate PauseModeController found.");
                return;
            }
            else
            {
                uiOverlay = pauseUIOverlay;
            }
        }
        private void OnDestroy()
        {
            pauseAction.performed -= PauseAction_Performed;
            // Reset the UI Overlay because if this object is destroyed then the level has been unloaded.
            uiOverlay = null;
        }

        /// <summary>
        /// Toggles isPausedByInput whenever the pause key is pressed.
        /// </summary>
        /// <param name="obj">Unused.</param>
        private void PauseAction_Performed(InputAction.CallbackContext obj)
        {
            isPausedByInput = !isPausedByInput;
            CheckPauseState();
        }

        /// <summary>
        /// Checks if the game should be paused and updated Time.timeScale accordingly.
        /// </summary>
        /// <remarks>
        /// Should be called whenever we modify the 3 factors that influence IsPaused.
        /// </remarks>
        public static void CheckPauseState()
        {
            if (IsPaused)
            {
                Time.timeScale = 0.0f;
                uiOverlay.SetActive(true);
            }
            else
            {
                Time.timeScale = 1.0f;
                uiOverlay.SetActive(false);
            }
        }
    }
}