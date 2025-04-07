/*****************************************************************************
// File Name : VoxelTile.cs
// Author : Brandon Koederitz
// Creation Date : April 7, 2025
//
// Brief Description : Represents a tile on the voxel tilemap.
// I'm going insane.
*****************************************************************************/
using Grubitecht.World.Objects;
using UnityEngine;

namespace Grubitecht.Tilemaps
{
    [System.Serializable]
    public class VoxelTile
    {
        [SerializeField] public Vector3Int GridPosition { get; private set; }
        public GridObject ContainedObject { get; set; }

        #region Properties
        public Vector2Int GridPosition2
        {
            get
            {
                return (Vector2Int)GridPosition;
            }
        }
        #endregion

        public VoxelTile(Vector3Int position)
        {
            GridPosition = position;
        }
    }
}