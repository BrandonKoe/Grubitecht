/*****************************************************************************
// File Name : LevelSelecctMenu.cs
// Author : Brandon Koederitz
// Creation Date : March 18, 2025
//
// Brief Description : Controls navigating from the level select menu.
*****************************************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Grubitecht.UI
{
    public class LevelSelectMenu : BaseMenu
    {
        /// <summary>
        /// loads a level scene with a given name.
        /// </summary>
        /// <param name="levelName">The name of the level to load.</param>
        public void LoadLevel(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }
    }
}