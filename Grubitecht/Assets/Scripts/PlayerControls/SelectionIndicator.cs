/*****************************************************************************
// File Name : SelectionIndicator.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Abstract script that defines an object that communicates the currently selected object to the
// player.
*****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht
{
    public abstract class SelectionIndicator : MonoBehaviour
    {
        public abstract Type[] SelectedComponentTypes { get; }

        public abstract void IndicateSelected(MonoBehaviour selectedComponent);

        /// <summary>
        /// Enables and disables this indicator by the Selector.
        /// </summary>
        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }
        public virtual void Enable()
        {
            gameObject.SetActive(true);
        }
    }
}