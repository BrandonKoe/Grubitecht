/*****************************************************************************
// File Name : SelectableChunk.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Allows a chunk of the world to be selected.
*****************************************************************************/
using Grubitecht.Tilemaps;
using UnityEngine;

namespace Grubitecht.World
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class SelectableChunk : Chunk, ISelectable
    {
        /// <summary>
        /// Nothing happens (at present) when the Voxel Tilemap is selected.  It just needs to be selectable for
        /// the selection system to be able to select specific spaces.
        /// </summary>
        /// <param name="oldObj"></param>
        public void OnSelect(ISelectable oldObj)
        {
            // Nothing happens.
        }
        public void OnDeselect(ISelectable newObj)
        {
            // nothing happens.
        }
    }
}