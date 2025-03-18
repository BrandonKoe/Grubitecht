/*****************************************************************************
// File Name : MovableObject.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Allows an object to be selected and moved along the world grid by the player.
*****************************************************************************/
using Grubitecht.World.Pathfinding;
using UnityEngine;

namespace Grubitecht.World
{
    [RequireComponent(typeof(GridNavigator))]
    public class MovableObject : MonoBehaviour, ISelectable
    {
        #region Component References
        [SerializeReference, HideInInspector] private GridNavigator gridNavigator;
        #endregion

        /// <summary>
        /// Assign component references.
        /// </summary>
        private void Reset()
        {
            gridNavigator = GetComponent<GridNavigator>();
        }

        public void OnSelect(ISelectable oldObj)
        {
            // Do nothing on select.
            Debug.Log(this.name + " was selected.");
        }

        /// <summary>
        /// Navigates this object to a new selected space when it is deselected.
        /// </summary>
        /// <param name="newObj">The newly selected object.</param>
        public void OnDeselect(ISelectable newObj)
        {
            // If the player selects a ground tile right after selecting a moveable object, then the object should
            // move to that selected position.
            if (newObj is GroundTile tile)
            {
                gridNavigator.SetDestination(tile);
            }
            Debug.Log(this.name + " was deselected.");
        }
    }
}