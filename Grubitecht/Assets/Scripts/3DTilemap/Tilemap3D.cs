/*****************************************************************************
// File Name : Tilemap3D.cs
// Author : Brandon Koederitz
// Creation Date : March 6, 2025
//
// Brief Description : Additional component that allows a Tilemap to act as a 3D tilemap
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Grubitecht.Tilemaps
{
    [RequireComponent(typeof(Tilemap))]
    public class Tilemap3D : MonoBehaviour
    {
        [field: SerializeField] public Tilemap RootTilemap { get; private set; }
    }
}