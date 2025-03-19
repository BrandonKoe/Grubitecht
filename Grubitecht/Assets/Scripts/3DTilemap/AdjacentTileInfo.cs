/*****************************************************************************
// File Name : AdjacentTileInfo.cs
// Author : Brandon Koederitz
// Creation Date : March 12, 2025
//
// Brief Description : Custom dictionary style object (because dictionaries arent serializable) that stores data
// about tiles adjacent to a given tile.
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grubitecht.Tilemaps
{
    [Serializable]
    public class AdjacentTileInfo
    {
        [SerializeField] private List<AdjTile> adjacentTiles = new List<AdjTile>();

        #region Nested Classes
        [Serializable]
        private class AdjTile
        {
            internal Vector3Int key;
            internal Tile3D value;

            internal AdjTile(Vector3Int key, Tile3D value)
            {
                this.key = key;
                this.value = value;
            }
        }
        #endregion


        /// <summary>
        /// Gets a tile value based on a given key.
        /// </summary>
        /// <param name="key">The relative position to check.</param>
        /// <returns>The tile at that relative position.</returns>
        public Tile3D Get(Vector3Int key)
        {
            AdjTile adj = adjacentTiles.Find(x => x.key == key);
            if (adj != null)
            {
                return adj.value;
            }
            else
            {
                return null;
            }
        }

        public void Set(Vector3Int key, Tile3D value)
        {
            AdjTile adj = adjacentTiles.Find(x => x.key == key);
            if (adj != null)
            {
                adj.value = value;
            }
        }

        /// <summary>
        /// Checks if this set of adjacent tiles contains info about a tile at a certain relative position.
        /// </summary>
        /// <param name="key">The relative position to check.</param>
        /// <returns>If this class contains and item with the passed in key.</returns>
        public bool ContainsKey(Vector3Int key)
        {
            return adjacentTiles.Any(item => item.key == key);
        }

        /// <summary>
        /// Adds new information about an adjacent tile to this adjacent tile info.
        /// </summary>
        /// <param name="key">The relative position from this tile that the value is located at.</param>
        /// <param name="value">The tile adjacent to this tile.</param>
        public void Add(Vector3Int key, Tile3D value)
        {
            adjacentTiles.Add(new AdjTile(key, value));
        }
    }
}