/*****************************************************************************
// File Name : GridObject.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Allows an object to take up space in the world grid.
*****************************************************************************/
using Grubitecht.Tilemaps;
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
        public Vector3Int GridPosition { get; set; }

        public Vector2Int GridPos2
        {
            get
            {
                return (Vector2Int)GridPosition;
            }
        }

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
        public Vector3Int GetApproximateSpace()
        {
            Vector3Int approxGridSpace = new Vector3Int();
            approxGridSpace.x = Mathf.RoundToInt(transform.position.x - VoxelTilemap3D.CELL_SIZE / 2);
            approxGridSpace.y = Mathf.RoundToInt(transform.position.z - VoxelTilemap3D.CELL_SIZE / 2);
            approxGridSpace.z = Mathf.RoundToInt(transform.position.y - VoxelTilemap3D.CELL_SIZE / 2);
            return approxGridSpace;
        }

        /// <summary>
        /// Moves this object to a new space.
        /// </summary>
        /// <param name="space">The space to move this object to.</param>
        public void SetCurrentSpace(Vector3Int space)
        {
            Vector3Int oldSpace = GridPosition;
            GridPosition = space;
            // Only assign the space's contained object value if this object is set to occupy space.
            if (occupySpace)
            {
                // Invokes the OnMapRefresh event so that paths can be updated based on changes to the map.
                // Only need to refresh the map if it has changed due to the movement of an object that occupies space.
                RefreshMap(this, oldSpace, GridPosition);
            }
        }

        /// <summary>
        /// Gets the world space position this object should occupy when it is on a given tile.
        /// </summary>
        /// <param name="tile">The tile to get the position for this object of.</param>
        /// <returns>The position of the tile plus the set offset of this object.</returns>
        public Vector3 GetTilePosition(Vector3Int space)
        {
            return space + offset;
        }
    }
}