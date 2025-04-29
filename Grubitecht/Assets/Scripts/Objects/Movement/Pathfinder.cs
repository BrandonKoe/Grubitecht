/*****************************************************************************
// File Name : Pathfinder.cs
// Author : Brandon Koederitz
// Creation Date : March 18, 2025
//
// Brief Description : Uses an A* Pathfinding algorith to  find a path connecting two points on the grid.
*****************************************************************************/
using Grubitecht.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grubitecht.World.Pathfinding
{
    public static class Pathfinder
    {
        //private readonly static List<PathNode> storedNodes = new();

        #region Nested Classes
        public class PathNode
        {
            internal VoxelTile tile;
            internal int g;
            internal int h;

            internal bool isClosed;

            internal PathNode previousNode;

            internal int F
            {
                get
                {
                    return g + h;
                }
            }

            public PathNode(VoxelTile tile)
            {
                this.tile = tile;
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
                g = MathHelpers.FindManhattenDistance(start.GridPosition2, tile.GridPosition2);
                h = MathHelpers.FindManhattenDistance(tile.GridPosition2, end.GridPosition2);
            }
        }
        #endregion

        #region A* Pathfinding
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
        public static List<VoxelTile> FindPath(VoxelTile startingTile, VoxelTile endingTile, int climbHeight, 
            OccupyLayer layer, bool includeAdjacent = false)
        {
            //Debug.Log("Finding Path");
            // Create two lists to manage what tiles need to be evaluated and what tiles have already been evaluated.
            List<PathNode> openList = new();
            List<PathNode> closedList = new();

            void AddToOpenList(PathNode node)
            {
                node.CalculateForPath(startingTile, endingTile);
                openList.Add(node);
            }

            PathNode startNode = startingTile.Node;
            AddToOpenList(startNode);


            //int iterationNum = 0;
            // Continually loop through the nodes to check in the open list.
            while (openList.Count > 0)
            {
                // Gets the node with the lowest f cost and mark it as evaluated.
                PathNode current = openList.OrderBy(item => item.F).First();
                openList.Remove(current);
                current.isClosed = true;
                closedList.Add(current);

                //iterationNum++;

                Vector3 wPos = VoxelTilemap3D.Main_GridToWorldPos(current.tile.GridPosition);
                Debug.DrawLine(wPos, wPos + Vector3.up, Color.red, 10f);

                // If this node corresponds to the ending node, then we finalize the path as we have reached our
                // destination.
                if (current.tile == endingTile)
                {
                    UncloseNodes(closedList);
                    //Debug.Log(iterationNum);
                    return FinalizePath(startNode, current);
                }

                // Check the neighboring tiles.
                List<VoxelTile> neighbors = GetAdjacentTiles(current.tile);
                foreach (VoxelTile neighbor in neighbors)
                {
                    // If this path is marked to end at a tile adjacent to the target tile, then we finalize the path
                    // here where the neighbor to our current tile is the ending tile.
                    if (includeAdjacent && neighbor == endingTile)
                    {
                        UncloseNodes(closedList);
                        //Debug.Log(iterationNum);
                        return FinalizePath(startNode, current);
                    }

                    // Exclude any inaccessible tiles here.
                    if (neighbor.ContainsObjectOnLayer(layer) || 
                        neighbor.Node.isClosed ||
                        Mathf.Abs(current.tile.GridPosition.z - neighbor.GridPosition.z) > climbHeight)
                    {
                        continue;
                    }

                    //// Gets the node that represents this tile from the open list.  If none exists, then we create a 
                    //// new node to represent this tile and add it to the open list.
                    //PathNode neighborNode = openList.Find(item => item.Tile == neighbor);
                    //if (neighborNode == null)
                    //{
                    //    neighborNode = GetNode(neighbor, startingTile, endingTile);
                    //    openList.Add(neighborNode);
                    //}

                    if (!openList.Contains(neighbor.Node))
                    {
                        AddToOpenList(neighbor.Node);
                    }
                    // Set the neighboring node's previous node to this current node.  This will be used during path
                    // finalization as we loop through previous nodes to create a path.
                    neighbor.Node.previousNode = current;
                }    
            }
            Debug.Log("Empty Path from " + startingTile + " to " + endingTile);
            UncloseNodes(closedList);
            // If all else fails, then we return null and let the caller handle the null ref.
            return null;
        }

        /// <summary>
        /// Finalizes a path between two nodes.
        /// </summary>
        /// <param name="startNode">The starting node of the path.</param>
        /// <param name="endingNode">The ending node of the path.</param>
        /// <returns>A list of tiles that represents the path.</returns>
        private static List<VoxelTile> FinalizePath(PathNode startNode, PathNode endingNode)
        {
            List<VoxelTile> result = new();
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

        /// <summary>
        /// Uncloses all the nodes in a given closed list
        /// </summary>
        /// <param name="closedList">The closed list to unclose.</param>
        private static void UncloseNodes(List<PathNode> closedList)
        {
            // Unclose all of our nodes once we've finished with the path.
            foreach (PathNode nod in closedList)
            {
                nod.isClosed = false;
            }
        }

        /// <summary>
        /// Gets a list of all tiles adjacent to this tile, regardless of elevation difference.
        /// </summary>
        /// <returns>A list of all tiles adjacent to this tile.</returns>
        public static List<VoxelTile> GetAdjacentTiles(VoxelTile tile)
        {
            List<VoxelTile> adjSpaces = new();
            foreach (Vector2Int direction in CardinalDirections.ORTHOGONAL_2D)
            {
                VoxelTile adjSpace = tile.GetAdjacent(direction);
                if (adjSpace != null)
                {
                    adjSpaces.Add(adjSpace);
                }
            }
            return adjSpaces;
        }

        /// <summary>
        /// Gets a node for a specific tile.
        /// </summary>
        /// <remarks>
        /// Attempts to first get a node that is stored if one exists for the given tile.
        /// </remarks>
        /// <param name="tile">The tile to get a node for.</param>
        /// <param name="startTile">The starting tile of the path.</param>
        /// <param name="endingTile">The ending tile of the path.</param>
        /// <returns>The path node at a given tile.</returns>
        private static PathNode GetNode(VoxelTile tile, VoxelTile startTile, VoxelTile endingTile)
        {
            PathNode outNode = tile.Node;
            outNode.CalculateForPath(startTile, endingTile);
            return outNode;
        }

        /// <summary>
        /// Checks if a path exists between two tiles.
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
        /// <returns>True if a valid path exists between the starting and ending tile.</returns>
        public static bool CheckPath(VoxelTile startingTile, VoxelTile endingTile, int climbHeight,
            OccupyLayer layer, bool includeAdjacent = false)
        {
            List<VoxelTile> path = FindPath(startingTile, endingTile, climbHeight, layer, includeAdjacent);
            if (path == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        ///// <summary>
        ///// Clears all stored nodes.
        ///// </summary>
        //public static void ClearNodes()
        //{
        //    storedNodes.Clear();
        //}
    }
}