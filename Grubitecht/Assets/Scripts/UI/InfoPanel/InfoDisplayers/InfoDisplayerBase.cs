/*****************************************************************************
// File Name : InfoDisplayerBase.cs
// Author : Brandon Koederitz
// Creation Date : March 31, 2025
//
// Brief Description : Base class for components that display a specific type of info value on the info panel.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.UI.InfoPanel
{
    public abstract class InfoDisplayerBase : MonoBehaviour
    {
        /// <summary>
        /// Unloads this info displayer.
        /// </summary>
        public virtual void Unload()
        {
            Destroy(gameObject);
        }
    }
}