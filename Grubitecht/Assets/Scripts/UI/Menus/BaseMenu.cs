/*****************************************************************************
// File Name : BaseMenu.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Controls the sub-menus of the main and pause menu.
*****************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Grubitecht.UI
{
    public class BaseMenu : MonoBehaviour
    {
        #region CONSTS
        protected const string MAIN_MENU_NAME = "MainMenu";
        #endregion
        private readonly List<SubMenu> subMenuHierarchy = new List<SubMenu>();

        /// <summary>
        /// Add/Remove functions fot the subMenuHierarchy.
        /// </summary>
        /// <param name="subMenu"></param>
        public void AddSubMenu(SubMenu subMenu)
        {
            subMenuHierarchy.Add(subMenu);
        }
        public void RemoveSubMenu(SubMenu subMenu)
        {
            subMenuHierarchy.Remove(subMenu);
        }

        /// <summary>
        /// Closes all sub-menus opened under this menu.
        /// </summary>
        protected void CloseAllSubMenus()
        {
            foreach (SubMenu subMenu in subMenuHierarchy)
            {
                subMenu.UnloadMenuRaw();
            }
            subMenuHierarchy.Clear();
        }

        /// <summary>
        /// Closes the highest sub menu in the hierarchy.
        /// </summary>
        protected void CloseSubMenu()
        {
            if (subMenuHierarchy.Count == 0) { return; }

            subMenuHierarchy[^1].UnloadMenu();
        }

        /// <summary>
        /// Returns to the main menu.
        /// </summary>
        public void ReturnToMainMenu()
        {
            // Update this with proper transitions later.
            SceneManager.LoadScene(MAIN_MENU_NAME);
        }
    }
}