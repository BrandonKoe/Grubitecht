/*****************************************************************************
// File Name : NavigationMap.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Uses a pseudo-Dijkra pathfinding algorith to construct a map of the level with distances
// pertaining to the distance from a certain set of positions.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.World.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grubitecht.World.Pathfinding
{
    public class NavigationMap
    {
        #region CONSTS
        // This value is used as the default distance value for the Dijkra style pathfinding.  Should be a number that
        // that is larger than the expected distance between any given space in a level and an objective.
        public const int REALLY_BIG_NUMBER = 1000;
        // Use 1 as the default climb height for creating a navigation map.
        private const int BASE_CLIMB_HEIGHT = 1;
        #endregion
        private static Dictionary<Vector3Int, int> mapDict;
        #region Navigation Map
        /// <summary>
        /// Creates a new navigation map for the current level.
        /// </summary>
        public void CreateMap()
        {
            mapDict = CreateNavigationMap();
        }

        /// <summary>
        /// Updates the objective navigation map to adapt to objectives being moved.
        /// </summary>
        /// <param name="positions">The positions of the objects to update the map with.</param>
        public void UpdateMap(Vector3Int[] positions)
        {
            mapDict = UpdateNavigationMap(mapDict, positions);
        }

        /// <summary>
        /// Resets the current navigation map.
        /// </summary>
        public void ResetMap()
        {
            mapDict = null;
        }

        /// <summary>
        /// Gets the distance value from the map of a given grid position.
        /// </summary>
        /// <param name="gridPos">The grid position to check.</param>
        /// <returns>
        /// The distance from that position to an objective.  
        /// Returns a really large number if that space doesnt exist in the map
        /// </returns>
        public int GetDistanceValue(Vector3Int gridPos)
        {
            if (mapDict.ContainsKey(gridPos))
            {
                return mapDict[gridPos];
            }
            return REALLY_BIG_NUMBER;
        }
        #endregion

        #region Dijkra's Style Pathfinding Nav-Mesh

        /// <summary>
        /// Generates a path map based on the ground positions of the voxel tilemap
        /// </summary>
        /// <returns>A dictionary that represents a map of ground tiles and their distance to an objective.</returns>
        public static Dictionary<Vector3Int, int> CreateNavigationMap()
        {
            Debug.Log("Creating Navigation Map");
            Dictionary<Vector3Int, int> map = new Dictionary<Vector3Int, int>();
            List<Vector3Int> tiles = VoxelTilemap3D.Main_GetTilemap(GridObject.VALID_GROUND_TYPE);
            foreach (Vector3Int tile in tiles)
            {
                // Really big number is defined above in CONSTS.  I just named it that because I though it would be
                // funny since that's its main purpose, to be a big number.
                map.Add(tile, REALLY_BIG_NUMBER);
            }
            return map;
        }

        /// <summary>
        /// Updates a path map using A Dijkra style pathfinding algorithm.
        /// </summary>
        /// <remarks>
        /// If I can optimize this, I should.  Probbaly can use 1 lsit with bools for visited/not visited.
        /// </remarks>
        /// <param name="referenceMap">The map of grid tile positions to update.</param>
        /// <param name="destinations">The destinations that objects following the path map should end up at.</param>
        /// <returns>An update map of values.</returns>
        public static Dictionary<Vector3Int, int> UpdateNavigationMap(Dictionary<Vector3Int, int> referenceMap,
            Vector3Int[] destinations)
        {
            //Debug.Log("Updating Navigation Map" + referenceMap.Count);
            // Sets up lists of visited and unvisited nodes for this pass through of a pathfinding map.
            List<Vector3Int> unvisited = new List<Vector3Int>();
            List<Vector3Int> visited = new List<Vector3Int>();
            unvisited.AddRange(referenceMap.Keys);

            // Sets the distance to each destination to 0.
            foreach (Vector3Int dest in destinations)
            {
                referenceMap[dest] = 0;
            }

            Vector3Int currentNode;
            // Continually loop through all unvisited tiles.
            while (unvisited.Count > 0)
            {
                // Gets the node with the shortest distance.
                currentNode = unvisited.OrderBy(item => referenceMap[item]).FirstOrDefault();
                // Loop through each adjacent tile.
                foreach (Vector2Int dir in CardinalDirections.CARDINAL_DIRECTIONS_2)
                {
                    foreach (Vector3Int target in VoxelTilemap3D.Main_GetCellsInColumn((Vector2Int)currentNode + dir,
                        GridObject.VALID_GROUND_TYPE))
                    {
                        // Skips over already visited nodes and nodes that are out of climbint height.
                        if (visited.Contains(target) || Mathf.Abs(currentNode.z - target.z) > BASE_CLIMB_HEIGHT)
                        { continue; }

                        // Sets the distance value of this target node to the lower of it's current value or the
                        // distance of the current node + 1.
                        referenceMap[target] = Mathf.Min(referenceMap[target], referenceMap[currentNode] + 1);
                    }
                }
                // Mark this current node as visited.
                visited.Add(currentNode);
                unvisited.Remove(currentNode);
            }

            // Return the map with updated distance values.
            return referenceMap;
        }
        #endregion
    }
}