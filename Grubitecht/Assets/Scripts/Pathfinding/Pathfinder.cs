/*****************************************************************************
// File Name : Pathfinder.cs
// Author : Brandon Koederitz
// Creation Date : March 18, 2025
//
// Brief Description : Uses an A* Pathfinding algorith to  find a path connecting two points on the grid.
*****************************************************************************/
using Grubitecht.World;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

namespace Grubitecht
{
    public static class Pathfinder
    {
        #region Nested Classes
        private class PathNode
        {
            internal GroundTile tile;
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

            private PathNode(GroundTile tile, GroundTile start, GroundTile end)
            {
                this.tile = tile;
                g = GroundTile.FindManhattenDistance(start, tile);
                h = GroundTile.FindManhattenDistance(tile, end);
            }

            /// <summary>
            /// Creates a new path node.
            /// </summary>
            /// <param name="tile">The tile that this node represents.</param>
            /// <param name="start">The starting tile of the path.</param>
            /// <param name="end">The ending tile of the path.</param>
            /// <returns>A newly created path node for this tile.</returns>
            internal static PathNode NewNode(GroundTile tile, GroundTile start, GroundTile end)
            {
                if (tile == null)
                {
                    return null;
                }
                return new PathNode(tile, start, end);
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
        /// <returns>A list of tiles representing the path between the start and ending tiles.</returns>
        public static List<GroundTile> FindPath(GroundTile startingTile, GroundTile endingTile, float climbHeight, 
            bool includeAdjacent = false)
        {
            // Create two lists to manage what tiles need to be evaluated and what tiles have already been evaluated.
            List<PathNode> openList = new List<PathNode>();
            List<GroundTile> closedList = new List<GroundTile>();

            PathNode startNode = PathNode.NewNode(startingTile, startingTile, endingTile);
            openList.Add(startNode);

            // Continually loop through the nodes to check in the open list.
            while (openList.Count > 0)
            {
                // Gets the node with the lowest f cost and mark it as evaluated.
                PathNode current = openList.OrderBy(item => item.f).First();
                openList.Remove(current);
                closedList.Add(current.tile);

                // If this node corresponds to the ending node, then we finalize the path as we have reached our
                // destination.
                if (current.tile == endingTile)
                {
                    return FinalizePath(startNode, current);
                }

                // Check the neighboring tiles.
                List<GroundTile> neighbors = current.tile.GetAdjacentTiles();
                foreach (GroundTile neighbor in neighbors)
                {
                    // If this path is marked to end at a tile adjacent to the target tile, then we finalize the path
                    // here where the neighbor to our current tile is the ending tile.
                    if (includeAdjacent && neighbor == endingTile)
                    {
                        return FinalizePath(startNode, current);
                    }

                    // Exclude any inaccessible tiles here.
                    if (neighbor.ContainedObject != null || 
                        closedList.Contains(neighbor) || 
                        Mathf.Abs(current.tile.Height - neighbor.Height) > climbHeight)
                    {
                        continue;
                    }

                    // Gets the node that represents this tile from the open list.  If none exists, then we create a 
                    // new node to represent this tile and add it to the open list.
                    PathNode neighborNode = openList.Find(item => item.tile == neighbor);
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
        private static List<GroundTile> FinalizePath(PathNode startNode, PathNode endingNode)
        {
            List<GroundTile> result = new List<GroundTile>();
            PathNode current = endingNode;
            while (current != startNode)
            {
                result.Add(current.tile);
                current = current.previousNode;
            }
            // Reverse the results list so that the path is in the correct order.
            result.Reverse();
            return result;
        }
    }
}