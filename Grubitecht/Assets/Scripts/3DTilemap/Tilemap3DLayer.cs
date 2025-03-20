/*****************************************************************************
// File Name : Tilemap3DLayer.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Additional component that allows a Tilemap to act as a 3D tilemap
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Grubitecht.OldTilemaps
{
    [RequireComponent(typeof(Tilemap))]
    public class Tilemap3DLayer : MonoBehaviour
    {
        [field: SerializeField, HideInInspector] public Tilemap RootTilemap { get; private set; }
        [field: SerializeField] public Tilemap3DLayer AboveLayer { get; private set; }
        [field: SerializeField] public Tilemap3DLayer BelowLayer { get; private set; }

        /// <summary>
        /// Stores a reference to the tilemap component on this object when the component is reset.
        /// </summary>
        private void Reset()
        {
            RootTilemap = GetComponent<Tilemap>();
        }
    }
}