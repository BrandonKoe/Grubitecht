/*****************************************************************************
// File Name : VoxelTile.cs
// Author : Brandon Koederitz
// Creation Date : April 7, 2025
//
// Brief Description : Represents a tile on the voxel tilemap.
// I'm going insane.
*****************************************************************************/
using Grubitecht.World.Objects;
using Grubitecht.World.Pathfinding;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Tilemaps
{
    [System.Serializable]
    public class VoxelTile
    {
        #region CONSTS
        private static readonly Dictionary<Vector2Int, int> ADJACENT_INDEX_REFERENCE = new Dictionary<Vector2Int, int>()
        {
            {Vector2Int.right, 0},
            {Vector2Int.left, 1},
            {Vector2Int.up, 2},
            {Vector2Int.down, 3},
            {new Vector2Int(1, 1), 4},
            {new Vector2Int(1, -1), 5},
            {new Vector2Int(-1, 1), 6},
            {new Vector2Int(-1, -1), 7}
        };
        #endregion
        [field: SerializeField] public Vector3Int GridPosition { get; private set; }
        public GridObject ContainedObject { get; set; }

        private Pathfinder.PathNode node;
        private AdjTileInfo adjTiles = new AdjTileInfo();

        #region Properties
        public Vector2Int GridPosition2
        {
            get
            {
                return (Vector2Int)GridPosition;
            }
        }
        public bool ContainsObject
        {
            get
            {
                return ContainedObject != null;
            }
        }
        public Pathfinder.PathNode Node
        {
            get
            {
                if (node == null)
                {
                    node = new Pathfinder.PathNode(this);
                }
                return node;
            }
        }
        #endregion

        #region Nested Classes
        /// <summary>
        /// Need to store references to adjacent tiles inside of a separate class because if it's stored in the 
        /// base VoxelTile class, the array creates a serialization loop and breaks unity.
        /// </summary>
        private class AdjTileInfo
        {
            internal readonly VoxelTile[] tiles = new VoxelTile[8];
        }
        #endregion

        public VoxelTile(Vector3Int position)
        {
            GridPosition = position;
        }

        ///// <summary>
        ///// Gives this tile references to tiles adjacent to it.
        ///// </summary>
        ///// <param name="adjTiles">The tiles adjacent to this tile.</param>
        //public void SetAdjacnets(VoxelTile[] adjTiles)
        //{
        //    adjacentTiles = adjTiles;
        //}

        /// <summary>
        /// Has this tile get and store references to adjacent tiles so they can be accessed easier.
        /// </summary>
        public void FindAdjacents()
        {
            adjTiles = new AdjTileInfo();
            for (int i = 0; i < CardinalDirections.DIAGONAL_2D.Length; i++)
            {
                //Debug.Log(adjTiles);

                adjTiles.tiles[i] = VoxelTilemap3D.Main_GetTile(GridPosition2 + CardinalDirections.DIAGONAL_2D[i]);
            }
        }

        /// <summary>
        /// Gets a tile adjacent to this one.
        /// </summary>
        /// <param name="direction">The direction to get the voxel adjacent voxel of.</param>
        /// <returns>The tile adjacent to this one in the given direction.</returns>
        public VoxelTile GetAdjacent(Vector2Int direction)
        {
            return adjTiles.tiles[ADJACENT_INDEX_REFERENCE[direction]];
            //return VoxelTilemap3D.Main_GetTile(GridPosition2 + direction);
        }
    }
}