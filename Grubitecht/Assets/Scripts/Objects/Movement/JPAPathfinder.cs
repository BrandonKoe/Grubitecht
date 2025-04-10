/*****************************************************************************
// File Name : SPAPathfinder.cs
// Author : Brandon Koederitz
// Creation Date : April 8, 2025
//
// Brief Description : Uses an JPA* Pathfinding algorith to  find a path connecting two points on the grid.  Should
// be more efficient than the old A* Pathfinding.
*****************************************************************************/
using Grubitecht.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grubitecht.World.Pathfinding
{
    public static class JPAPathfinder
    {
        #region Nested Classes
        public class PathNode
        {
            internal VoxelTile tile;
            internal Vector2Int[] directions;
            internal Vector2Int interpolateDirection;
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

                // If this node corresponds to the ending node, then we finalize the path as we have reached our
                // destination.
                if (current.tile == endingTile)
                {
                    //Debug.Log(iterationNum);
                    return FinalizePath(startNode, current, closedList);
                }
                
                bool EvaluateOrthogonal(ref VoxelTile evalTile, Vector2Int direction, 
                    out List<Vector2Int> continueDirections)
                {
                    // Get the vector that is perpendicular to our direction using a cross product.  This is what 
                    // we'll use to evaluate for obstacles that create forced neighbors.
                    Vector2Int perpVector = (Vector2Int)Vector3Int.RoundToInt(Vector3.Cross((Vector3Int)direction,
                        VoxelTilemap3D.NORMAL));

                    //Debug.Log(direction + " is perpendicular to " + perpVector);

                    continueDirections = new List<Vector2Int>();
                    while (true)
                    {
                        evalTile = evalTile.GetAdjacent(direction);

                        // If the next tile is null, then we should always treat it as blocked.
                        if (evalTile == null)
                        {
                            return false;
                        }

                        // Check for the goal.
                        if (evalTile == endingTile)
                        {
                            return true;
                        }
                        //evalSpace = evalSpace + direction;
                        // If our next tile is the ending tile and we're set to include adjacent tiles, then we should
                        // treat this tile as the ending tile.
                        else if (includeAdjacent && evalTile.GetAdjacent(direction) == endingTile)
                        {
                            endingTile = evalTile;
                            return true;
                        }

                        // If this space is blocked, then we ignore this direction.
                        if (CheckSpace(evalTile))
                        {
                            return false;
                        }

                        // Debug
                        Vector3 wPos = VoxelTilemap3D.Main_GridToWorldPos(evalTile.GridPosition);
                        Vector3Int d = new Vector3Int(direction.x, 0, direction.y);
                        Debug.DrawLine(wPos + (Vector3.up / 2), wPos + (Vector3.up / 2) + d, Color.gray, 10f);
                        Debug.DrawLine(wPos, wPos + Vector3.up / 2, Color.white, 10f);


                        // If the space we're evaluating is adjacent to a blocked space, then this space has a
                        // forced neighbor and we want to re-evaluate it.
                        if (CheckSpace(evalTile.GetAdjacent(perpVector)))
                        {
                            if (!CheckSpace(evalTile.GetAdjacent(direction + perpVector)))
                            {
                                // If there is a valid space to move that would only be accessible from this node, then
                                // we say that our currently evaluated space is of interest and we return true.
                                continueDirections.Add(direction + perpVector);
                            }
                        }
                        if (CheckSpace(evalTile.GetAdjacent(-perpVector)))
                        {
                            // If there is a valid space to move that would only be accessible from this node, then
                            // we say that our currently evaluated space is of interest and we return true.
                            if (!CheckSpace(evalTile.GetAdjacent(direction - perpVector)))
                            {
                                continueDirections.Add(direction - perpVector);
                            }
                        }

                        if (continueDirections.Count > 0)
                        {
                            continueDirections.Add(direction);
                            return true;
                        }
                    }
                }

                // Evaluates for interesting nodes in the diagonal direction.  Also evaluates orthogonals along that
                // diagonal path.
                bool EvaluateDiagonal(ref VoxelTile evalTile, Vector2Int direction, 
                    out List<Vector2Int> continueDirections)
                {
                    continueDirections = new List<Vector2Int>();

                    while (true)
                    {
                        evalTile = evalTile.GetAdjacent(direction);

                        if (evalTile == null)
                        {
                            return false;
                        }

                        // Always return true if we find the ending tile.
                        if (evalTile == endingTile)
                        {
                            return true;
                        }
                        // If our next tile is the ending tile and we're set to include adjacent tiles, then we should
                        // treat this tile as the ending tile.
                        else if (includeAdjacent && evalTile.GetAdjacent(direction) == endingTile)
                        {
                            endingTile = evalTile;
                            return true;
                        }

                        // If the space ahead of us is blocked and we didnt find the goal, we discard this direction.
                        if (CheckSpace(evalTile))
                        {
                            return false;
                        }

                        // Debug
                        Vector3 wPos = VoxelTilemap3D.Main_GridToWorldPos(evalTile.GridPosition);
                        Vector3Int d = new Vector3Int(direction.x, 0, direction.y);
                        Debug.DrawLine(wPos + (Vector3.up / 2), wPos + (Vector3.up / 2) + d, Color.gray, 10f);
                        Debug.DrawLine(wPos, wPos + Vector3.up / 2, Color.white, 10f);

                        // Break the diagonal direction up into it's horizontal and vertical components.
                        Vector2Int hDir = new Vector2Int(direction.x, 0);
                        Vector2Int vDir = new Vector2Int(0, direction.y);

                        // Evaluate for forced neighbors caused by out diagonal.
                        if (CheckSpace(evalTile.GetAdjacent(-hDir)))
                        {
                            // Always check orthogonals first
                            continueDirections.Add(hDir);
                            continueDirections.Add(vDir);
                            continueDirections.Add(vDir - hDir);
                            continueDirections.Add(direction);
                            return true;
                        }
                        if (CheckSpace(evalTile.GetAdjacent(-vDir)))
                        {
                            // Always check orthogonals first.
                            continueDirections.Add(hDir);
                            continueDirections.Add(vDir);
                            continueDirections.Add(hDir - vDir);
                            continueDirections.Add(direction);
                            return true;
                        }

                        // If any of our orthogonal directions yield a forced neighbor, then this node should be
                        // re-evaluated in the open list.
                        VoxelTile hEvalSpace = evalTile;
                        VoxelTile vEvalSpace = evalTile;
                        if (EvaluateOrthogonal(ref hEvalSpace, hDir, out List<Vector2Int> dummyHList) ||
                            EvaluateOrthogonal(ref vEvalSpace, vDir, out List<Vector2Int> dummyVList))
                        {
                            continueDirections.Add(hDir);
                            continueDirections.Add(vDir);
                            continueDirections.Add(direction);
                            return true;
                        }
                    }
                }

                // Returns true if the space is blocked.
                bool CheckSpace(VoxelTile tile)
                {
                    // Spaces that dont exist always cound as blocked.
                    if (tile == null) { return true; }

                    //Debug.Log("Checking space " + tile.GridPosition2);

                    return ((!ignoreBlockedSpaces && tile.ContainsObject) ||
                        Mathf.Abs(current.tile.GridPosition.z - tile.GridPosition.z) > climbHeight);
                }

                VoxelTile evaluateTile = current.tile;
                // If there are no pre-set directions, then we should evaluate every direction.
                if (current.directions == null)
                {
                    current.directions = CardinalDirections.DIAGONAL_2D;
                }
                foreach (Vector2Int dir in current.directions)
                {
                    // Reset our evaluate tile to our current tile each loop iteration.
                    evaluateTile = current.tile;
                    List<Vector2Int> nextNodeDir = new List<Vector2Int>();
                    bool spaceOfInterestFound = false;
                    // Only diagonals will have a magnitude greater than 1.
                    if (dir.magnitude > 1)
                    {
                        //Debug.Log("Evaluating Diagonal");
                        spaceOfInterestFound = EvaluateDiagonal(ref evaluateTile, dir, out nextNodeDir);
                    }
                    else
                    {
                        //Debug.Log("Evaluating Orthogonal");
                        spaceOfInterestFound = EvaluateOrthogonal(ref evaluateTile, dir, out nextNodeDir);
                    }
                    //
                    if (spaceOfInterestFound)
                    {
                        //Debug.Log(evaluateTile.GridPosition);
                        if (!openList.Contains(evaluateTile.JPANode))
                        {
                            AddToOpenList(evaluateTile.JPANode);
                        }
                        //PathNode neighborNode = openList.Find(item => item.tile == evaluateTile);
                        //if (neighborNode == null)
                        //{
                        //    neighborNode = evaluateTile.JPANode;
                        //    AddToOpenList(neighborNode);
                        //}
                        evaluateTile.JPANode.directions = nextNodeDir.ToArray();
                        // Give the node a vector that is reversed the direction it was found in so that we can
                        // interpolate the tiles inbetween nodes.
                        evaluateTile.JPANode.interpolateDirection = -dir;
                        evaluateTile.JPANode.previousNode = current;

                        // Debug Code
                        Vector3 wPos = VoxelTilemap3D.Main_GridToWorldPos(evaluateTile.GridPosition);
                        Debug.DrawLine(wPos, wPos + Vector3.up, Color.red, 10f);

                        if (evaluateTile.JPANode.directions == null)
                        {
                            evaluateTile.JPANode.directions = CardinalDirections.DIAGONAL_2D;
                        }
                        foreach (Vector2Int direction in evaluateTile.JPANode.directions)
                        {
                            Vector3Int d = new Vector3Int(direction.x, 0, direction.y);
                            Debug.DrawLine(wPos + (Vector3.up / 2), wPos + (Vector3.up / 2) + d, Color.green, 10f);
                        }
                    }
                }
            }
            // If all else fails, then we return null and let the caller handle the null ref.
            return new();
        }

        /// <summary>
        /// Finalizes a path between two nodes and interpolated for tiles that lie in between them.
        /// </summary>
        /// <param name="startNode">The starting node of the path.</param>
        /// <param name="endingNode">The ending node of the path.</param>
        /// <returns>A list of tiles that represents the path.</returns>
        private static List<VoxelTile> FinalizePath(PathNode startNode, PathNode endingNode, List<PathNode> closedList)
        {
            List<VoxelTile> result = new();
            PathNode current = endingNode;
            while (current != startNode)
            {
                result.Add(current.tile);
                int iterationNum = 0;
                PathNode interpolateNode = current;
                while  (interpolateNode != current.previousNode)
                {
                    Debug.Log(current.interpolateDirection);

                    iterationNum++;
                    if (iterationNum >= 100)
                    {
                        Debug.LogError("Interpolation iteration limit reached");
                        return null;
                    }

                    interpolateNode = interpolateNode.tile.GetAdjacent(current.interpolateDirection).JPANode;
                    Debug.Log(interpolateNode.tile.GridPosition);
                    // Skip adding the tile if we've reached the next node so that tiles are not added twice.
                    if (interpolateNode != current.previousNode)
                    {
                        result.Add(interpolateNode.tile);
                    }
                }
                current = current.previousNode;
            }
            // Reverse the results list so that the path is in the correct order.
            result.Reverse();

            // Unclose all nodes in the closed list.
            foreach (PathNode node in closedList)
            {
                node.isClosed = false;
            }
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
        #endregion
    }
}