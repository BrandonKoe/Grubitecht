/*****************************************************************************
// File Name : ObjectContainer.cs
// Author : Brandon Koederitz
// Creation Date : March 12, 2025
//
// Brief Description : Represnets a ground tile that an object can lay on.
*****************************************************************************/
using Grubitecht.Tilemaps;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht
{
    public class GroundTile : Tile3D
    {
        [Header("Ground Settings")]
        [SerializeField, ReadOnly] private Vector3Int gridPos;

        private static readonly Dictionary<Vector2Int, GroundTile> groundDict = new();

        #region Properties
        public Vector2Int GridPos2
        {
            get
            {
                return (Vector2Int)gridPos;
            }
        }
        #endregion

        public override void OnTileCreation(Vector3Int position)
        {
            gridPos = position;
        }
    }
}