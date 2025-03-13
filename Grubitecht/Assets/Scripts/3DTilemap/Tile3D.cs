/*****************************************************************************
// File Name : Tile3D.cs
// Author : Brandon Koederitz
// Creation Date : March 6, 2025
//
// Brief Description : Foundational component for game objects that act as 3D tiles.
*****************************************************************************/
using UnityEngine;
using UnityEngine.Events;

namespace Grubitecht.Tilemaps
{
    public class Tile3D : MonoBehaviour
    {
        [field: SerializeField] public RuleModel RuleModel { get; private set; }

        /// <summary>
        /// Callback functions to manage setting values on tile creation and destruction in edit mode.
        /// </summary>
        /// <param name="position">The position of this tile.</param>
        public virtual void OnTileCreation(Vector3Int position) { } 
        public virtual void OnTileDestruction(Vector3Int position) { }
    }
}