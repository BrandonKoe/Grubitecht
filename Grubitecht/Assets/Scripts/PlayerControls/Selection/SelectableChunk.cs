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
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(Chunk))]
    public class SelectableChunk : MonoBehaviour, ISelectable
    {

        public Vector3 Position => transform.position;

        #region Component References
        [field: SerializeReference, HideInInspector] public Chunk Chunk { get; private set; }

        /// <summary>
        /// Assign components on reset.
        /// </summary>
        private void Reset()
        {
            Chunk = GetComponent<Chunk>();
        }
        #endregion

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