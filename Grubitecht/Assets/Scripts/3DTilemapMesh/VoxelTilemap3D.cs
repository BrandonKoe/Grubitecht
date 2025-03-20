/*****************************************************************************
// File Name : Tilemap3D.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Component that creates a composite tilemap mesh out of multiple positions painted on the world.
*****************************************************************************/
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Tilemaps
{
    [RequireComponent(typeof(GridLayout))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class VoxelTilemap3D : MonoBehaviour
    {
        #region CONSTS
        private const float CELL_SIZE = 1f;
        #endregion

        [SerializeField] private List<Vector3Int> groundTiles;
        [SerializeField] private List<Vector3Int> wallTiles;

        #region Component References
        [SerializeReference, HideInInspector] private GridLayout gridLayout;
        [SerializeReference, HideInInspector] private MeshFilter meshFilter;

        /// <summary>
        /// Assign Component references on reset.
        /// </summary>
        private void Reset()
        {
            gridLayout = GetComponent<GridLayout>();
            meshFilter = GetComponent<MeshFilter>();
        }
        #endregion

        /// <summary>
        /// Paints a voxel on this tilemap.
        /// </summary>
        /// <param name="position">The position to paint at.</param>
        /// <param name="type">The type of tile to paint.</param>
        public void Paint(Vector3Int position, TileType type)
        {
            switch (type)
            {
                case TileType.Ground:
                    groundTiles.Add(position);
                    break;
                case TileType.Wall:
                    wallTiles.Add(position);
                    break;
                default:
                    break;
            }
            BakeMesh();
        }

        /// <summary>
        /// Erases a voxel from this tilemap.
        /// </summary>
        /// <param name="position">The position to erase at.</param>
        public void Erase(Vector3Int position)
        {
            if (groundTiles.Contains(position))
            {
                groundTiles.Remove(position);
            }
            if (wallTiles.Contains(position))
            {
                wallTiles.Remove(position);
            }
            BakeMesh();
        }

        /// <summary>
        /// Checks if a tile occupies a given cell.
        /// </summary>
        /// <param name="position">The grid position of the cell to check.</param>
        /// <returns>True if there is a voxel in that cell, false if there is not.</returns>
        public bool CheckCell(Vector3Int position)
        {
            return groundTiles.Contains(position) | wallTiles.Contains(position);
        }

        /// <summary>
        /// Checks if a specific type of tile occupies a given cell.
        /// </summary>
        /// <param name="position">The grid position of the cell to check.</param>
        /// <returns>True if there is a voxel in that cell, false if there is not.</returns>
        public bool CheckCell(Vector3Int position, TileType type)
        {
            switch (type)
            {
                case TileType.Ground:
                    return groundTiles.Contains(position);
                case TileType.Wall:
                    return wallTiles.Contains(position);
                default:
                    return groundTiles.Contains(position) | wallTiles.Contains(position);
            }
        }

        /// <summary>
        /// Returns the world position of a cell in a grid layout at a given position.
        /// </summary>
        /// <param name="grid">The grid that contains the cell to get the world position of.</param>
        /// <param name="position">The cell position to get the world position of.</param>
        /// <param name="targetTransform">The transform of the object that contains the tilemap.</param>
        /// <returns>The world position of the cell.</returns>
        private Vector3 GetWorldPositionCentered(Vector3Int position)
        {
            // Create a value that lets us center our spawned game objects to line up with the grid.
            Vector3 center = new Vector3(CELL_SIZE / 2, CELL_SIZE / 2, 0f);
            //Vector3Int cellPos = new Vector3Int(position.x, yPos, position.y);
            Vector3 worldPos = gridLayout.LocalToWorld(gridLayout.CellToLocalInterpolated(position + center));
            // Set Y position manually because Unity tilemaps doesnt support depth by default.
            worldPos.y = position.z;
            return worldPos;
        }

        #region Mesh Construction
        private void BakeMesh()
        {

        }
        #endregion
    }
}