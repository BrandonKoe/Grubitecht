/*****************************************************************************
// File Name :ButtonFunctions.cs
// Author : Brandon Koederitz
// Creation Date : May 1, 2025
//
// Brief Description : Exposes a set of menu controls to buttons on the UI.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Grubitecht.UI
{
    public class ButtonFunctions : MonoBehaviour
    {
        #region CONSTS
        protected const string MAIN_MENU_NAME = "MainMenu";
        #endregion

        /// <summary>
        /// Restarts the current level.
        /// </summary>
        public void RestartLevel()
        {
            // Update this with proper transitions later.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Returns to the main menu.
        /// </summary>
        public void ReturnToMainMenu()
        {
            // Update this with proper transitions later.
            SceneManager.LoadScene(MAIN_MENU_NAME);
        }

        /// <summary>
        /// Loads the next scene in the build.
        /// </summary>
        public void NextScene()
        {
            int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextIndex < SceneManager.sceneCount)
            {
                SceneManager.LoadScene(nextIndex);
            }
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
