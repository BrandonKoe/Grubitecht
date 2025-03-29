/*****************************************************************************
// File Name : MainMenu.cs
// Author : Brandon Koederitz
// Creation Date : March 28, 2025
//
// Brief Description : Controls the main menu buttons and their functionality.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Grubitecht.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField,Tooltip("The name of the scene to load when the player hits play.")] 
        private string playScene;
        /// <summary>
        /// Move the player into gameplay.
        /// </summary>
        /// <remarks>
        /// In future versions, this will take them to a level select screen.
        /// </remarks>
        public void Play()
        {
            SceneManager.LoadScene(playScene);
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
