/*****************************************************************************
// File Name : UISelectionIndicator.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Indicates what object is selected through use of a UI object.
*****************************************************************************/
using Grubitecht.World;
using Grubitecht.World.Objects;
using System;
using UnityEngine;

namespace Grubitecht.UI
{
    public class UISelectionIndicator : SelectionIndicator
    {
        [SerializeField] private Vector2 offset;
        [SerializeField] private Transform trackedTransform;

        #region Properties
        public override Type[] SelectedComponentTypes => new Type[] 
        { 
            typeof(MovableObject)
        };
        private RectTransform rectTransform => (RectTransform)transform;
        #endregion

        /// <summary>
        /// Moves this UI objec to the screen position of the selected object.
        /// </summary>
        /// <param name="selection">The selected object</param>
        public override void IndicateSelected(ISelectable selection)
        {
            if (selection is MonoBehaviour behaviour)
            {
                trackedTransform = behaviour.transform;
            }
        }

        /// <summary>
        /// Continually updates the indicator's position to match the selected object's position.
        /// This is a bit lazy but I can add better implementation later if needed.
        /// </summary>
        private void Update()
        {
            rectTransform.position = (Vector2)Camera.main.WorldToScreenPoint(trackedTransform.position) + offset;
        }
    }
}