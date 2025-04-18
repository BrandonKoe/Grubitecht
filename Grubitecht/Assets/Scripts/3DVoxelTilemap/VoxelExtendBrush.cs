/*****************************************************************************
// File Name : VoxelExtendBrush.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Brush for painting positions on the 3D mesh tilemap that will extend wall tiles down to lower
// layers.
*****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Tilemaps
{
    [Obsolete("The extend brush if obsolte.  No more need to add extended tiles when meshes always extend downward" +
        " naturally now.")]
    [CreateAssetMenu(menuName = "Custom Brushes/Voxel Extend Brush")]
    [CustomGridBrush(false, true, false, "Voxel Extend Brush")]
    public class VoxelExtendBrush : VoxelBrush
    {
        [Header("Extend Brush Settings")]
        //[SerializeField] private TileType extendType;
        [SerializeField, Tooltip("The minimum depth that this brush will extend tiles to.")]
        private int minDepth = -1;

        /// <summary>
        /// Adds a tile and extends it downwards with wall tiles to the minimum depth.
        /// </summary>
        /// <param name="tilemap">The tilemap to add tiles to.</param>
        /// <param name="position">The position to add a tile at.</param>
        /// <param name="type">The type of tile to add.</param>
        /// <param name="refreshMesh">Whether this tile change should re-bake the tilemap mesh.</param>
        protected override void AddTile(VoxelTilemap3D tilemap, Vector3Int position)
        {
            base.AddTile(tilemap, position);
            if (position.z > minDepth)
            {
                position.z--;
                AddTile(tilemap, position);
            }
        }

        /// <summary>
        /// Erases all tiles in a voxel tilemap in a column extending down.
        /// </summary>
        /// <param name="tilemap">The tilemap to erase tiles from.</param>
        /// <param name="position">The position to erase a tile at.</param>
        /// <param name="refreshMesh">Whether this tile change should re-bake the tilemap mesh.</param>
        protected override void EraseTile(VoxelTilemap3D tilemap, Vector3Int position)
        {
            base.EraseTile(tilemap, position);
            if (position.z > minDepth)
            {
                position.z--;
                EraseTile(tilemap, position);
            }
        }
    }
}
