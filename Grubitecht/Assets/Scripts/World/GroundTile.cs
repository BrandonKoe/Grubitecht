/*****************************************************************************
// File Name : GroundTile.cs
// Author : Brandon Koederitz
// Creation Date : March 12, 2025
//
// Brief Description : Represnets a ground tile that an object can exist on.
*****************************************************************************/
using Grubitecht.OldTilemaps;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using Grubitecht.World.Objects;
using System;

namespace Grubitecht.World
{
    [Obsolete("GroundTile and the Tile3D system is obsolete.  Use VoxelTilemap instead.")]
    public class GroundTile : Tile3D, ISelectable
    {
        [Header("Ground Settings")]
        [SerializeField, ReadOnly] private Vector3Int gridPos;

        public GridObject ContainedObject { get; set; }

        private static readonly Dictionary<Vector2Int, GroundTile> groundDict = new();


        #region Properties
        public Vector2Int GridPos2
        {
            get
            {
                return (Vector2Int)gridPos;
            }
        }
        public Vector3Int GridPos3
        {
            get
            {
                return gridPos;
            }
        }

        public float Height
        {
            get
            {
                return transform.position.y;
            }
        }
        #endregion

        /// <summary>
        /// Sets the position of this grid tile when it is created.
        /// </summary>
        /// <param name="position">The position of the created tile.</param>
        public override void OnTileCreation(Vector3Int position)
        {
            gridPos = position;
        }

        /// <summary>
        /// Add this ground tile to the grid so it can be searched.
        /// </summary>
        private void Awake()
        {
            groundDict.Add(GridPos2, this);
        }

        /// <summary>
        /// Gets a tile adjacent to this one, regardless of elevation.
        /// </summary>
        /// <param name="adjacent">The direction to get the adjacent tile from.</param>
        /// <returns>The tile adjacent to this one.</returns>
        public GroundTile GetAdjacentTile(Vector2Int adjacent)
        {
            if (groundDict.TryGetValue(GridPos2 + adjacent, out GroundTile tile))
            {
                return tile;
            }
            return null;
        }

        ///// <summary>
        ///// Gets a list of all tiles adjacent to this tile.
        ///// </summary>
        ///// <returns>A list of all tiles adjacent to this tile.</returns>
        //public List<GroundTile> GetAdjacentTiles()
        //{
        //    List<GroundTile> neighbors = new List<GroundTile>()
        //    {
        //        GetAdjacentTile(Vector2Int.right),
        //        GetAdjacentTile(Vector2Int.left),
        //        GetAdjacentTile(Vector2Int.up),
        //        GetAdjacentTile(Vector2Int.down)
        //    };
        //    neighbors.RemoveAll(item => item == null);
        //    return neighbors;
        //}

        public void OnSelect(ISelectable oldObj)
        {
            Debug.Log(this.name + " was selected.");
        }

        public void OnDeselect(ISelectable newObj)
        {
            Debug.Log(this.name + " was deselected.");
        }

        #region Static Helpers
        /// <summary>
        /// Gets a tile at a given position.
        /// </summary>
        /// <param name="pos">The position to get the tile of.</param>
        /// <returns>The tile at pos position.</returns>
        public static GroundTile GetTileAt(Vector2Int pos)
        {
            if (groundDict.TryGetValue(pos, out GroundTile tile))
            {
                return tile;
            }
            return null;
        }
        #endregion
    }
}