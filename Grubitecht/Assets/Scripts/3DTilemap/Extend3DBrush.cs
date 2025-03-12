/*****************************************************************************
// File Name : Extend3DBrush.cs
// Author : Brandon Koederitz
// Creation Date : March 12, 2025
//
// Brief Description : A grid brush that paints 3D tiles and extends them downward.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Tilemaps
{
    [CreateAssetMenu]
    [CustomGridBrush(false, true, false, "Extend 3D Brush")]
    public class Extend3DBrush : Tile3DBrush
    {
        [Header("Extend Brush Settings")]
        [SerializeField] private Tile3D wallTile;
        /// <summary>
        /// Places a tile and then places wall tiles on all layers below that tile.
        /// </summary>
        /// <param name="tile">The tile to place.</param>
        /// <param name="gridLayout">The grid layout to place on.</param>
        /// <param name="targetTransform">The transform of the tilemap layer we are placing on.</param>
        /// <param name="position">The position to place at.</param>
        /// <param name="layer">The layer we are placing on.</param>
        protected override void PlaceTile(Tile3D tile, GridLayout gridLayout, Transform targetTransform, 
            Vector3Int position, Tilemap3DLayer layer)
        {
            base.PlaceTile(tile, gridLayout, targetTransform, position, layer);
            Tilemap3DLayer belowLayer = layer.BelowLayer;
            // Recursively calls PlaceTile for each layer below this one.
            if (belowLayer != null)
            {
                PlaceTile(wallTile, gridLayout, belowLayer.transform, position, belowLayer);
            }
        }

        /// <summary>
        /// Erases a given tile and all tiles on lower layers at the same position.
        /// </summary>
        /// <param name="gridLayout">The grid layout to erase on.</param>
        /// <param name="targetTransform">The transform of the tilemap layer we are erasing on.</param>
        /// <param name="position">The position to erase at.</param>
        /// <param name="layer">The layer we are erasing on.</param>
        protected override void EraseTile(GridLayout gridLayout, Transform targetTransform, Vector3Int position, 
            Tilemap3DLayer layer)
        {
            base.EraseTile(gridLayout, targetTransform, position, layer);
            Tilemap3DLayer belowLayer = layer.BelowLayer;
            // Recursively calls PlaceTile for each layer below this one.
            if (belowLayer != null)
            {
                EraseTile(gridLayout, belowLayer.transform, position, belowLayer);
            }
        }
    }

}