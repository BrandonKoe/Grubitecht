/*****************************************************************************
// File Name : PathNode.cs
// Author : Brandon Koederitz
// Creation Date : April 7, 2025
//
// Brief Description : Set of information a tile stores that is used by the pathfinder when finding paths.
*****************************************************************************/
using Grubitecht.Tilemaps;
using UnityEngine;

namespace Grubitecht.World.Pathfinding
{
    public class PathNode
    {
        public VoxelTile Tile { get; private set; }
        private int g;
        private int h;
        public bool IsClosed { get; set; }

        public PathNode PreviousNode { get; set; }

        public int F
        {
            get
            {
                return g + h;
            }
        }

        public PathNode(VoxelTile tile)
        {
            this.Tile = tile;
        }

        ///// <summary>
        ///// Creates a new path node.
        ///// </summary>
        ///// <param name="tile">The tile that this node represents.</param>
        ///// <returns>A newly created path node for this tile.</returns>
        //internal static PathNode NewNode(VoxelTile tile)
        //{
        //    if (tile == null)
        //    {
        //        return null;
        //    }
        //    return new PathNode(tile);
        //}

        /// <summary>
        /// Calculates the distance values for this node based on the given start and end points.
        /// </summary>
        /// <param name="start">The start point of the path.</param>
        /// <param name="end">The end point of the path.</param>
        internal void CalculateForPath(VoxelTile start, VoxelTile end)
        {
            g = MathHelpers.FindManhattenDistance(start.GridPosition2, Tile.GridPosition2);
            h = MathHelpers.FindManhattenDistance(Tile.GridPosition2, end.GridPosition2);
        }
    }
}