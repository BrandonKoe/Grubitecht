/*****************************************************************************
// File Name : UISelectionIndicator.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Indicates what object is selected through use of a UI object.
*****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grubitecht.World;

namespace Grubitecht.UI
{
    public class UISelectionIndicator : SelectionIndicator
    {
        [SerializeField] private Vector2 offset;

        public override Type[] SelectedComponentTypes => new Type[] 
        { 
            typeof(GroundTile),
            typeof(MovableObject)
        };

        /// <summary>
        /// Moves this UI objec to the screen position of the selected object.
        /// </summary>
        /// <param name="selectedComponent">The component to move to.</param>
        public override void IndicateSelected(MonoBehaviour selectedComponent)
        {
            RectTransform rectTrans = (RectTransform)transform;
            rectTrans.position = (Vector2)Camera.main.WorldToScreenPoint(selectedComponent.transform.position)
                + offset;
        }
    }
}