/*****************************************************************************
// File Name : Chunk.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : A portion of the world tilemap that is all rendered as one mesh.
*****************************************************************************/
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Grubitecht.Tilemaps
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Chunk : MonoBehaviour
    {
        [field: SerializeReference, ReadOnly] public VoxelTilemap3D Tilemap { get; set; }
        [field: SerializeField, ReadOnly] public Vector2Int ChunkPos { get; set; }
        [field: SerializeField, ReadOnly] public string MeshPath { get; set; }

        [SerializeField] private List<VoxelTile> tiles;

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
        public List<VoxelTile> Tiles
        {
            get
            {
                return tiles;
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

        /// <summary>
        /// Adds a tile to this chunk
        /// </summary>
        /// <param name="position">
        /// The grid position to add this tile at.  Note, this is NOT the relative position within the chunk, rather,
        /// it is the absolute position on the grid.
        /// </param>
        /// <param name="smoothAbove">Whether placing this tile should delete tiles above it.</param>
        public void AddTile(Vector3Int position, bool smoothAbove)
        {
            VoxelTile existingTile = Tiles.Find(item => item.GridPosition2 == (Vector2Int)position);
            if (existingTile != null)
            {
                if (existingTile.GridPosition == position ||
                    (!smoothAbove && existingTile.GridPosition.z > position.z))
                {
                    return;
                }
                Tiles.Remove(existingTile);
            }
            //UpdateAdjacents(newTile);
            Tiles.Add(new VoxelTile(position));
        }

        /// <summary>
        /// Erases a voxel from this chunk.
        /// </summary>
        /// <param name="position">The position to erase at in grid space.</param>
        public void RemoveTile(Vector3Int position)
        {
            VoxelTile toErase = Tiles.Find(item => item.GridPosition == position);
            if (toErase != null)
            {
                Tiles.Remove(toErase);
            }
        }

        /// <summary>
        /// Gets a tile that is within this chunk.
        /// </summary>
        /// <param name="position">The grid position to get the tile at.</param>
        /// <returns>The tile within this chunk at that grid position.</returns>
        public VoxelTile GetTile(Vector3Int position)
        {
            return Tiles.Find(item => item.GridPosition == position);
        }

        public VoxelTile GetTile(Vector2Int position)
        {
            return Tiles.Find(item => item.GridPosition2 == position);
        }
    }
}
