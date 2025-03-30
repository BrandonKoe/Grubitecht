/*****************************************************************************
// File Name : SubMenu.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Controls sub menus that pop up within the main and pause menus.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.UI
{
    public class SubMenu : MonoBehaviour
    {
        [SerializeField] private BaseMenu baseMenu;
        /// <summary>
        /// Controls loading/unloading this menu.
        /// </summary>
        public void LoadMenu()
        {
            // Do other stuff when the menu loads here.
            gameObject.SetActive(true);
            if (baseMenu != null )
            {
                baseMenu.AddSubMenu(this);
            }
        }
        public void UnloadMenu()
        {
            // Do ther stuff when the menu unloads here.
            UnloadMenuRaw();
            if (baseMenu != null)
            {
                baseMenu.RemoveSubMenu(this);
            }
        }
        /// <summary>
        /// Unloads the menu without removing the base menu's list reference to this item.  Done this way to allow
        /// for looping through the hierarchy to unload.
        /// </summary>
        public void UnloadMenuRaw()
        {
            gameObject.SetActive(false);
        }
    }
}