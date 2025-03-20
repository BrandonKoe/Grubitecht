/*****************************************************************************
// File Name : GridObject.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Allows an object to take up space in the world grid.
*****************************************************************************/
using Grubitecht.OldTilemaps;
using System;
using UnityEngine;

namespace Grubitecht.World.Objects
{
    public class GridObject : GridBehaviour
    {
        [SerializeField, Tooltip("The offset from the tile's position that this object should be at while on that " +
            "tile.")] 
        private Vector3 offset;
        [SerializeField, Tooltip("Whether this object should occupy space in the world.  If true then other objects" +
            " that occupy space cannot be inside the same space as this object.")]
        private bool occupySpace;
        public GroundTile CurrentSpace { get; set; }

        /// <summary>
        /// Assigns this object a space when it is created.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            SetCurrentSpace(GetApproximateSpace());
            //Debug.Log(CurrentSpace.ToString());
        }

        /// <summary>
        /// Gets an approximation of the space that this object's transform is currently at.
        /// </summary>
        /// <returns>The space that this object's transform is physically at in world space.</returns>
        public GroundTile GetApproximateSpace()
        {
            Vector2Int approxGridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x -
                Tile3DBrush.CELL_SIZE / 2), Mathf.RoundToInt(transform.position.z - Tile3DBrush.CELL_SIZE / 2));
            return GroundTile.GetTileAt(approxGridPos);
        }

        /// <summary>
        /// Moves this object to a new space.
        /// </summary>
        /// <param name="space">The space to move this object to.</param>
        public void SetCurrentSpace(GroundTile space)
        {
            GroundTile oldSpace = CurrentSpace;
            CurrentSpace = space;
            // Only assign the space's contained object value if this object is set to occupy space.
            if (occupySpace)
            {
                if (oldSpace != null)
                {
                    oldSpace.ContainedObject = null;
                }
                if (CurrentSpace != null)
                {
                    CurrentSpace.ContainedObject = this;
                }
                // Invokes the OnMapRefresh event so that paths can be updated based on changes to the map.
                // Only need to refresh the map if it has changed due to the movement of an object that occupies space.
                RefreshMap(this, oldSpace, CurrentSpace);
            }
        }

        /// <summary>
        /// Gets the world space position this object should occupy when it is on a given tile.
        /// </summary>
        /// <param name="tile">The tile to get the position for this object of.</param>
        /// <returns>The position of the tile plus the set offset of this object.</returns>
        public Vector3 GetTilePosition(GroundTile tile)
        {
            return tile.transform.position + offset;
        }
    }
}