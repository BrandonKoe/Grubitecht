/*****************************************************************************
// File Name : MovableObject.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Allows an object to be selected and moved along the world grid by the player.
*****************************************************************************/
using Grubitecht.UI.InfoPanel;
using Grubitecht.World.Pathfinding;
using UnityEngine;

namespace Grubitecht.World.Objects
{
    [RequireComponent(typeof(PathNavigator))]
    [RequireComponent(typeof(SelectableObject))]
    public class MovableObject : MonoBehaviour
    {
        #region Component References
        [field: SerializeReference, HideInInspector] public PathNavigator GridNavigator { get; private set; }
        [SerializeReference, HideInInspector] private SelectableObject selectable;
        /// <summary>
        /// Assign component references.
        /// </summary>
        private void Reset()
        {
            GridNavigator = GetComponent<PathNavigator>();
            selectable = GetComponent<SelectableObject>();
        }
        #endregion

        /// <summary>
        /// Subscribe/unsubscribe from the OnDeselectEvent to control movement.
        /// </summary>
        private void Awake()
        {
            selectable.OnDeselectEvent += MoveObject;
        }
        private void OnDestroy()
        {
            selectable.OnDeselectEvent -= MoveObject;
        }

        /// <summary>
        /// Navigates this object to a new selected space when it is deselected.
        /// </summary>
        /// <param name="newObj">The newly selected object.</param>
        public void MoveObject(ISelectable newObj)
        {
            // If the player selects a ground tile right after selecting a moveable object, then the object should
            // move to that selected position.
            if (newObj is SpaceSelection space)
            {
                if (GridNavigator.IsMoving || GrubManager.CheckGrub())
                {
                    GridNavigator.SetDestination(space.Tile, false, RecallGrub);
                    GrubManager.AssignGrub(this);
                }
            }
        }

        /// <summary>
        /// Recalls a grub delegated to moving this object from the grub controller.
        /// </summary>
        private void RecallGrub(bool reachedDestination)
        {
            GrubManager.ReturnGrub(this);
        }
    }
}