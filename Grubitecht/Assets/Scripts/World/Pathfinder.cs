/*****************************************************************************
// File Name : Pathfinder.cs
// Author : Brandon Koederitz
// Creation Date : March 18, 2025
//
// Brief Description : Uses an A* Pathfinding algorith to  find a path connecting two points on the grid.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.World.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grubitecht
{
    public static class Pathfinder
    {
        #region Nested Classes
        private class PathNode
        {
            internal Vector3Int space;
            internal PathNode previousNode;
            internal int g;
            internal int h;

            internal int f
            {
                get
                {
                    return g + h;
                }
            }

            private PathNode(Vector3Int tile, Vector3Int start, Vector3Int end)
            {
                this.space = tile;
                g = FindManhattenDistance(start, tile);
                h = FindManhattenDistance(tile, end);
            }

            /// <summary>
            /// Creates a new path node.
            /// </summary>
            /// <param name="tile">The tile that this node represents.</param>
            /// <param name="start">The starting tile of the path.</param>
            /// <param name="end">The ending tile of the path.</param>
            /// <returns>A newly created path node for this tile.</returns>
            internal static PathNode NewNode(Vector3Int tile, Vector3Int start, Vector3Int end)
            {
                if (tile == null)
                {
                    return null;
                }
                return new PathNode(tile, start, end);
            }

            /// <summary>
            /// Finds the Manhatten distance (or the total number of spaces between the two tiles when restricted to
            /// orthogonal movement) between two tiles.
            /// </summary>
            /// <param name="tile1">The first tile.</param>
            /// <param name="tile2">The second tile.</param>
            /// <returns>The total number of spaces between the two tiles.</returns>
            public static int FindManhattenDistance(Vector3Int tile1, Vector3Int tile2)
            {
                return Mathf.Abs(tile1.x - tile2.x) + Mathf.Abs(tile1.y - tile2.y);
            }
        }
        #endregion

        /// <summary>
        /// Finds a path between two tiles.
        /// </summary>
        /// <param name="startingTile">
        /// The tile to begin the path from.  This is usually the tile that the object is already on.
        /// </param>
        /// <param name="endingTile">
        /// The ending tile of the path.  This is the tile that the object should end up at.
        /// </param>
        /// <param name="climbHeight">
        /// The height that this object can move vertically at once.  Determines how well the object can handle slopes.
        /// </param>
        /// <param name="includeAdjacent">
        /// Whether to stop the path at a tile adjacent to the ending tile or if the path must include the ending tile.
        /// </param>
        /// <param name="ignoreBlockedSpaces">
        /// Whether this path should go around spaces that have objects in them.
        /// </param>
        /// <returns>A list of tiles representing the path between the start and ending tiles.</returns>
        public static List<Vector3Int> FindPath(Vector3Int startingTile, Vector3Int endingTile, int climbHeight, 
            bool includeAdjacent = false, bool ignoreBlockedSpaces = false)
        {
            // Create two lists to manage what tiles need to be evaluated and what tiles have already been evaluated.
            List<PathNode> openList = new List<PathNode>();
            List<Vector3Int> closedList = new List<Vector3Int>();

            PathNode startNode = PathNode.NewNode(startingTile, startingTile, endingTile);
            openList.Add(startNode);

            // Continually loop through the nodes to check in the open list.
            while (openList.Count > 0)
            {
                // Gets the node with the lowest f cost and mark it as evaluated.
                PathNode current = openList.OrderBy(item => item.f).First();
                openList.Remove(current);
                closedList.Add(current.space);

                // If this node corresponds to the ending node, then we finalize the path as we have reached our
                // destination.
                if (current.space == endingTile)
                {
                    return FinalizePath(startNode, current);
                }

                // Check the neighboring tiles.
                List<Vector3Int> neighbors = GetAdjacentTiles(current.space);
                foreach (Vector3Int neighbor in neighbors)
                {
                    // If this path is marked to end at a tile adjacent to the target tile, then we finalize the path
                    // here where the neighbor to our current tile is the ending tile.
                    if (includeAdjacent && neighbor == endingTile)
                    {
                        return FinalizePath(startNode, current);
                    }

                    // Exclude any inaccessible tiles here.
                    if ((GridObject.GetObjectAtSpace(neighbor) != null && !ignoreBlockedSpaces) || 
                        closedList.Contains(neighbor) || 
                        Mathf.Abs(current.space.z - neighbor.z) > climbHeight)
                    {
                        continue;
                    }

                    // Gets the node that represents this tile from the open list.  If none exists, then we create a 
                    // new node to represent this tile and add it to the open list.
                    PathNode neighborNode = openList.Find(item => item.space == neighbor);
                    if (neighborNode == null)
                    {
                        neighborNode = PathNode.NewNode(neighbor, startingTile, endingTile);
                        openList.Add(neighborNode);
                    }
                    // Set the neighboring node's previous node to this current node.  This will be used during path
                    // finalization as we loop through previous nodes to create a path.
                    neighborNode.previousNode = current;
                }    
            }
            // If all else fails, then we return null and let the caller handle the null ref.
            return new();
        }

        /// <summary>
        /// Finalizes a path between two nodes.
        /// </summary>
        /// <param name="startNode">The starting node of the path.</param>
        /// <param name="endingNode">The ending node of the path.</param>
        /// <returns>A list of tiles that represents the path.</returns>
        private static List<Vector3Int> FinalizePath(PathNode startNode, PathNode endingNode)
        {
            List<Vector3Int> result = new List<Vector3Int>();
            PathNode current = endingNode;
            while (current != startNode)
            {
                result.Add(current.space);
                current = current.previousNode;
            }
            // Reverse the results list so that the path is in the correct order.
            result.Reverse();
            return result;
        }

        /// <summary>
        /// Gets a list of all tiles adjacent to this tile, regardless of elevation difference.
        /// </summary>
        /// <returns>A list of all tiles adjacent to this tile.</returns>
        public static List<Vector3Int> GetAdjacentTiles(Vector3Int position)
        {
            List<Vector3Int> adjSpaces = new List<Vector3Int>();
            foreach (Vector2Int direction in CardinalDirections.CARDINAL_DIRECTIONS_2)
            {
                adjSpaces.AddRange(VoxelTilemap3D.Main_GetCellsInColumn((Vector2Int)position + direction, 
                    GridObject.VALID_GROUND_TYPE));
            }
            return adjSpaces;
        }
    }
}