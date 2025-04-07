/*****************************************************************************
// File Name : BufferedNavigationMap.cs
// Author : Brandon Koederitz
// Creation Date : March 30, 2025
//
// Brief Description : Uses a pseudo-Dijkstra pathfinding algorith to construct a map of the level with distances
// pertaining to the distance from a certain set of positions.  This type of map will continually update, but will
// buffer it's updates across multiple frames using a coroutine.  This ensures the map is always being updated, but
// it wont cause massive lag spikes in return for some stale time.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.World.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Grubitecht.World.Pathfinding
{
    public delegate VoxelTile[] DestinationGetterFunc();
    public class BufferedNavigationMap : NavigationMap
    {
        #region CONSTS
        // The number of tiles that get evaluated per frame. More means more chance of lag, but a faster refresh time.
        private const int TILES_PER_SECOND = 120;
        #endregion
        private readonly DestinationGetterFunc destinationGetter;
        private readonly int climbHeight;
        private readonly float minRefreshDelay;

        private Coroutine updateRoutine;
        private Coroutine passThroughRoutine;
        private bool isUpdating;
        private bool isPassThrough;

        private MonoBehaviour anchoredComponent;

        protected static Dictionary<VoxelTile, int> mapBuffer;

        public BufferedNavigationMap(DestinationGetterFunc destinationGetter, int climbHeight, float minRefreshDelay)
        {
            this.destinationGetter = destinationGetter;
            this.climbHeight = climbHeight;
            this.minRefreshDelay = minRefreshDelay;
        }

        public override void ResetMap()
        {
            base.ResetMap();
            mapBuffer = null;
            StopUpdatingImmediate();
        }
        public override void CreateMap()
        {
            base.CreateMap();
            mapBuffer = new Dictionary<VoxelTile, int>();
            mapBuffer.AddRange(mapDict);
        }

        /// <summary>
        /// Starts the continual update process of this NavMap.
        /// </summary>
        /// <param name="anchorComponent">
        /// The component that will be running the update coroutine.  Should be a component that shouldnt be destroyed
        /// like a manager.
        /// </param>
        public void StartUpdating(MonoBehaviour anchorComponent)
        {
            if (isUpdating) { return; }
            isUpdating = true;
            anchoredComponent = anchorComponent;
            updateRoutine = anchorComponent.StartCoroutine(BufferedUpdateProcess(anchorComponent));
        }
        /// <summary>
        /// Stops the routine updating of the map, but allows any current updates to finish.
        /// </summary>
        public void StopUpdating()
        {
            anchoredComponent = null;
            isUpdating = false;
        }
        /// <summary>
        /// Immediately stops the updating of this map, including any current updates.
        /// </summary>
        public void StopUpdatingImmediate()
        {
            if (updateRoutine != null)
            {
                anchoredComponent.StopCoroutine(updateRoutine);
                updateRoutine = null;
            }
            if (passThroughRoutine != null)
            {
                anchoredComponent.StopCoroutine(passThroughRoutine);
                passThroughRoutine = null;
            }
            StopUpdating();
        }

        /// <summary>
        /// Controls the continual updating of this NavMap.
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator BufferedUpdateProcess(MonoBehaviour anchoredComponent)
        {
            while (isUpdating)
            {
                passThroughRoutine = anchoredComponent.StartCoroutine(UpdateNavigationMap(destinationGetter(), 
                    climbHeight));

                float timer = minRefreshDelay;
                while (isPassThrough || timer > 0)
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Updates a path map using A Dijkra style pathfinding algorithm.
        /// </summary>
        /// <remarks>
        /// If I can optimize this, I should.  Probbaly can use 1 lsit with bools for visited/not visited.
        /// </remarks>
        /// <param name="destinations">The destinations that objects following the path map should end up at.</param>
        /// <returns>Coroutine.</returns>
        public IEnumerator UpdateNavigationMap(VoxelTile[] destinations, int climbHeight)
        {
            // Update the bool that tracks if we are going through an update currently.
            isPassThrough = true;

            mapBuffer = ResetNavigationMap(mapDict);
            Debug.Log("Updating Navigation Map with " + mapBuffer.Count + " spaces and " + destinations.Length + 
                " destinations");
            // Sets up lists of visited and unvisited nodes for this pass through of a pathfinding map.
            List<VoxelTile> unvisited = new List<VoxelTile>();
            List<VoxelTile> visited = new List<VoxelTile>();
            unvisited.AddRange(mapBuffer.Keys);
            int iterationCounter = 0;

            // Sets the distance to each destination to 0.
            foreach (VoxelTile dest in destinations)
            {
                mapBuffer[dest] = 0;
            }

            VoxelTile currentNode;
            // Continually loop through all unvisited tiles.
            while (unvisited.Count > 0)
            {
                // Gets the node with the shortest distance.
                currentNode = unvisited.OrderBy(item => mapBuffer[item]).FirstOrDefault();
                // Loop through each adjacent tile.
                foreach (Vector2Int dir in CardinalDirections.ORTHOGONAL_2D)
                {
                    VoxelTile target = currentNode.GetAdjacent(dir);
                    // Skips over already visited nodes, nodes that are out of climbing height, and nodes that
                    // are occupied.  (This version can afford to update for occupied cells.
                    if (target == null || 
                        visited.Contains(target) ||
                        Mathf.Abs(currentNode.GridPosition.z - target.GridPosition.z) > climbHeight ||
                       target.ContainedObject != null)
                    { continue; }

                    // Sets the distance value of this target node to the lower of it's current value or the
                    // distance of the current node + 1.
                    mapBuffer[target] = Mathf.Min(mapBuffer[target], mapBuffer[currentNode] + 1);
                }
                // Mark this current node as visited.
                visited.Add(currentNode);
                unvisited.Remove(currentNode);
                // Only evaluate a certain number of tiles each second before yielding control to avoid lag.
                iterationCounter++;
                if (iterationCounter >= Mathf.RoundToInt(TILES_PER_SECOND * Time.deltaTime))
                {
                    iterationCounter = 0;
                    yield return null;
                }
            }
            // Swaps the buffer and main map dict to allow for seamless map transitions.
            (mapDict, mapBuffer) = (mapBuffer, mapDict);
            isPassThrough = false;

        }
    }
}