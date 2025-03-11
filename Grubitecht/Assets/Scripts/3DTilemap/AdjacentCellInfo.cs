/*****************************************************************************
// File Name : AdjacentCellInfo.cs
// Author : Brandon Koederitz
// Creation Date : March 6, 2025
//
// Brief Description : Set of model information for creating ground tilemaps in 3D space.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Tilemaps
{
    public class AdjacentCellInfo
    {
        public Dictionary<Vector3, Tile3D> AdjacentTiles = new();
    }
}