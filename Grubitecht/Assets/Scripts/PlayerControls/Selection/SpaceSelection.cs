/*****************************************************************************
// File Name : SpaceSelection.cs
// Author : Brandon Koederitz
// Creation Date : March 21, 2025
//
// Brief Description : Represents a space in the world that the player has selected.  Dummy class created by the
// selector when it clicks on the tilemap.
*****************************************************************************/
using Grubitecht.Tilemaps;
using UnityEngine;

namespace Grubitecht
{
    public class SpaceSelection : ISelectable
    {
        public VoxelTile Tile { get; set; }
        public Vector3 WorldPosition { get; set; }

        public SpaceSelection(VoxelTile tile, Vector3 worldPosition)
        {
            Tile = tile;
            WorldPosition = worldPosition;
        }

        public void OnSelect(ISelectable oldObj)
        {
            // Nothing happens.
            //Debug.Log(GridPosition);
        }

        public void OnDeselect(ISelectable newObj)
        {
            // Nothing happens.
        }
    }
}
