/*****************************************************************************
// File Name : SPAPathfinder.cs
// Author : Brandon Koederitz
// Creation Date : April 8, 2025
//
// Brief Description : Uses an JPA* Pathfinding algorith to  find a path connecting two points on the grid.  Should
// be more efficient than the old A* Pathfinding.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.World.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grubitecht
{
    public static class JPAPathfinder
    {
        //private readonly static List<PathNode> storedNodes = new();

        #region Nested Classes
        public class PathNode
        {
            internal VoxelTile tile;
            internal Vector2Int[] directions;
            internal int g;
            internal int h;
            internal bool isClosed;

            internal PathNode previousNode;

            internal int f
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
            ///// <param name="start">The starting tile of the path.</param>
            ///// <param name="end">The ending tile of the path.</param>
            ///// <returns>A newly created path node for this tile.</returns>
            //internal static PathNode NewNode(Vector3Int tile)
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

        #region JPA* Pathfinding
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
            bool includeAdjacent = false, bool ignoreBlockedSpaces = false)
        {

            Debug.Log("Finding Path");
            // Create two lists to manage what tiles need to be evaluated and what tiles have already been evaluated.
            List<PathNode> openList = new();
            List<PathNode> closedList = new();

            void AddToOpenList(PathNode node)
            {
                node.CalculateForPath(startingTile, endingTile);
                openList.Add(node);
            }

            PathNode startNode = startingTile.JPANode;
            AddToOpenList(startNode);

            int iterationNum = 0;
            // Continually loop through the nodes to check in the open list.
            while (openList.Count > 0)
            {
                // Gets the node with the lowest f cost and mark it as evaluated.
                PathNode current = openList.OrderBy(item => item.f).First();
                openList.Remove(current);
                current.isClosed = true;
                closedList.Add(current);

                iterationNum++;

                Vector3 wPos = VoxelTilemap3D.Main_GridToWorldPos(current.tile.GridPosition);
                Debug.DrawLine(wPos, wPos + Vector3.up, Color.red, 10f);

                // If this node corresponds to the ending node, then we finalize the path as we have reached our
                // destination.
                if (current.tile == endingTile)
                {
                    Debug.Log(iterationNum);
                    return FinalizePath(startNode, current);
                }

                // Check the neighboring tiles.
                //List<Vector3Int> neighbors = GetAdjacentTiles(current.space);
                //foreach (Vector3Int neighbor in neighbors)
                //{
                //    // If this path is marked to end at a tile adjacent to the target tile, then we finalize the path
                //    // here where the neighbor to our current tile is the ending tile.
                //    if (includeAdjacent && neighbor == endingTile)
                //    {
                //        Debug.Log(iterationNum);
                //        return FinalizePath(startNode, current);
                //    }

                //    // Gets the node that represents this tile from the open list.  If none exists, then we create a 
                //    // new node to represent this tile and add it to the open list.
                //    PathNode neighborNode = openList.Find(item => item.space == neighbor);
                //    if (neighborNode == null)
                //    {
                //        neighborNode = GetNode(neighbor, startingTile, endingTile);
                //        openList.Add(neighborNode);
                //    }

                //    // Exclude any inaccessible tiles here.
                //    if ((!ignoreBlockedSpaces && GridObject.CheckOccupied(neighbor)) || 
                //        closedList.Contains(neighborNode) ||
                //        Mathf.Abs(current.space.z - neighbor.z) > climbHeight)
                //    {
                //        continue;
                //    }
                //    // Set the neighboring node's previous node to this current node.  This will be used during path
                //    // finalization as we loop through previous nodes to create a path.
                //    neighborNode.previousNode = current;
                //}    
                bool EvaluateOrthogonal(ref VoxelTile evalTile, Vector2Int direction, 
                    out List<Vector2Int> forcedNeighborDirections)
                {
                    // Get the vector that is perpendicular to our direction using a cross product.  This is what 
                    // we'll use to evaluate for obstacles that create forced neighbors.
                    Vector2Int perpVector = (Vector2Int)Vector3Int.RoundToInt(Vector3.Cross((Vector3Int)direction,
                        VoxelTilemap3D.NORMAL));

                    forcedNeighborDirections = new List<Vector2Int>();
                    // If the space ahead of us is blocked, we discard this direction.
                    if (CheckSpace(evalTile.GetAdjacent(direction)))
                    {
                        return false;
                    }
                    while (true)
                    {
                        evalTile = evalTile.GetAdjacent(direction);
                        //evalSpace = evalSpace + direction;
                        // If the space ahead of us is blocked, we discard this direction.
                        if (CheckSpace(evalTile.GetAdjacent(direction)))
                        {
                            return false;
                        }
                        // If the space we're evaluating is adjacent to a blocked space, then this space has a
                        // forced neighbor and we want to re-evaluate it.
                        if (CheckSpace(evalTile.GetAdjacent(perpVector)))
                        {
                            forcedNeighborDirections.Add(direction + perpVector);
                        }
                        if (CheckSpace(evalTile.GetAdjacent(-perpVector)))
                        {
                            forcedNeighborDirections.Add(direction - perpVector);
                        }

                        if (forcedNeighborDirections.Count > 0)
                        {
                            forcedNeighborDirections.Add(direction);
                            return true;
                        }
                    }
                }

                // Evaluates for interesting nodes in the diagonal direction.  Also evaluates orthogonals along that
                // diagonal path.
                bool EvaluateDiagonal(ref VoxelTile evalTile, Vector2Int direction, 
                    out List<Vector2Int> forcedNeighborDirections)
                {
                    forcedNeighborDirections = new List<Vector2Int>();
                    // If the space ahead of us is blocked, we discard this direction.
                    if (CheckSpace(evalTile.GetAdjacent(direction)))
                    {
                        return false;
                    }
                    while (true)
                    {
                        evalTile = evalTile.GetAdjacent(direction);
                        // If the space ahead of us is blocked, we discard this direction.
                        if (CheckSpace(evalTile.GetAdjacent(direction)))
                        {
                            return false;
                        }

                        // If any of our orthogonal directions yield a forced neighbor, then this node should be
                        // re-evaluated in the open list.
                        VoxelTile hEvalSpace = evalTile;
                        VoxelTile vEvalSpace = evalTile;
                        Vector2Int hDir = new Vector2Int(direction.x, 0);
                        Vector2Int vDir = new Vector2Int(0, direction.y);
                        if (EvaluateOrthogonal(ref hEvalSpace, hDir, out List<Vector2Int> dummyHList) ||
                            EvaluateOrthogonal(ref vEvalSpace, vDir, out List<Vector2Int> dummyVList))
                        {
                            forcedNeighborDirections.Add(hDir);
                            forcedNeighborDirections.Add(vDir);
                            forcedNeighborDirections.Add(direction);
                            return true;
                        }
                    }
                }

                // Returns true if the space is blocked.
                bool CheckSpace(VoxelTile tile)
                {
                    return ((!ignoreBlockedSpaces && tile.ContainsObject) ||
                        Mathf.Abs(current.tile.GridPosition.z - tile.GridPosition.z) > climbHeight);
                }

                VoxelTile evaluateTile = current.tile;
                foreach (Vector2Int dir in current.directions)
                {
                    List<Vector2Int> nextNodeDir = new List<Vector2Int>();
                    bool spaceOfInterestFound = false;
                    // Only diagonals will have a magnitude greater than 1.
                    if (dir.magnitude > 1)
                    {
                        spaceOfInterestFound = EvaluateDiagonal(ref evaluateTile, dir, out nextNodeDir);
                    }
                    else
                    {
                        spaceOfInterestFound = EvaluateOrthogonal(ref evaluateTile, dir, out nextNodeDir);
                    }
                    if (spaceOfInterestFound)
                    {
                        PathNode neighborNode = openList.Find(item => item.tile == evaluateTile);
                        if (neighborNode == null)
                        {
                            neighborNode = evaluateTile.JPANode;
                            AddToOpenList(neighborNode);
                        }
                        neighborNode.directions = nextNodeDir.ToArray();
                        neighborNode.previousNode = current;
                    }
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

        ///// <summary>
        ///// Gets a node for a specific tile.
        ///// </summary>
        ///// <remarks>
        ///// Attempts to first get a node that is stored if one exists for the given tile.
        ///// </remarks>
        ///// <param name="tile">The tile to get a node for.</param>
        ///// <param name="startTile">The starting tile of the path.</param>
        ///// <param name="endingTile">The ending tile of the path.</param>
        ///// <returns>The path node at a given tile.</returns>
        //private static PathNode GetNode(Vector3Int tile, Vector3Int startTile, Vector3Int endingTile)
        //{
        //    PathNode outNode = storedNodes.Find(item => item.space == tile);
        //    if (outNode == null)
        //    {
        //        outNode = PathNode.NewNode(tile);
        //    }
        //    outNode.CalculateForPath(startTile, endingTile);
        //    return outNode;
        //}
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