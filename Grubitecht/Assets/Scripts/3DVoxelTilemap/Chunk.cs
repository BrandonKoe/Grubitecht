/*****************************************************************************
// File Name : Chunk.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : A portion of the world tilemap that is all rendered as one mesh.
*****************************************************************************/
using UnityEngine;
using NaughtyAttributes;

namespace Grubitecht.Tilemaps
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Chunk : MonoBehaviour
    {
        [field: SerializeReference, ReadOnly] public VoxelTilemap3D Tilemap { get; set; }
        [field: SerializeField, ReadOnly] public Vector2Int ChunkPos { get; set; }
        [field: SerializeField, ReadOnly] public string MeshPath { get; set; }

        #region Properties
        public Mesh ChunkMesh
        {
            get
            {
                return meshFilter.sharedMesh;
            }
            set
            {
                meshFilter.sharedMesh = value;
                if (meshCollider != null)
                {
                    meshCollider.sharedMesh = value;
                }
            }
        }
        #endregion

        #region Component References
        [SerializeReference, HideInInspector] private MeshFilter meshFilter;
        [SerializeReference, HideInInspector] private MeshCollider meshCollider;

        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        private void Reset()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
        }
        #endregion

        /// <summary>
        /// Initializes values for this chunk when it is created.
        /// </summary>
        /// <param name="tilemap">The tilemap that this chunk belongs to.</param>
        /// <param name="chunkPos">The position of this chunk.</param>
        public void Initialize(VoxelTilemap3D tilemap, Vector2Int chunkPos)
        {
            Tilemap = tilemap;
            ChunkPos = chunkPos;
        }
    }
}
