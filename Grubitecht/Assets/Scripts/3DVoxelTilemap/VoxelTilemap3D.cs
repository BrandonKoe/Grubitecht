/*****************************************************************************
// File Name : Tilemap3D.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Component that creates a composite tilemap mesh out of multiple positions painted on the world.
*****************************************************************************/
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Grubitecht.Tilemaps
{
    [RequireComponent(typeof(GridLayout))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class VoxelTilemap3D : MonoBehaviour, ISelectable
    {
        #region CONSTS
        public const float CELL_SIZE = 1f;
        private static readonly Vector3 GRID_CORNET_OFFSET = new Vector3(-0.5f, -0.5f, -0.5f);
        private const string ASSET_FOLDER = "Assets";
        private const string MESH_FILE_EXTENSION = ".mesh";
        #endregion

        [SerializeField] private string meshFilePath = "Meshes";
        [SerializeField] private string meshFileName = "Level1Grid";
        [Header("Tilemap Settings.")]
        [SerializeField] private SubTilemap[] subTilemaps;
#if UNITY_EDITOR
        [SerializeField] private MeshUpdater meshUpdater;
#endif

        private static VoxelTilemap3D instance;

        private static VoxelTilemap3D Instance
        {
            get
            {
                if (instance == null)
                {
                    // If instance is null, then we attempt to find one.  Prevents race conditions.
                    instance = FindObjectOfType<VoxelTilemap3D>();
                    if (instance == null)
                    {
                        Debug.LogError("No VoxelTilemap exists in the scene");
                    }
                }
                return instance;
            }
        }

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

        #region Nested Classes
        /// <summary>
        /// Class to represent a specific sub-tilemap of this tilemap.
        /// </summary>
        [System.Serializable]
        private class SubTilemap
        {
            [SerializeField] internal TileType tileType;
            [SerializeField] internal List<Vector3Int> tiles;
        }
        #endregion

        /// <summary>
        /// Assigns and de-assigns the instance of the tilemap that will be used as the default.
        /// </summary>
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError("Multiple Voxel Tilemaps detected.  Please ensure each scene has only one tilemap.");
                return;
            }
            else
            {
                instance = this;
            }
        }
        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Paints a voxel on this tilemap.
        /// </summary>
        /// <param name="position">The position to paint at.</param>
        /// <param name="type">The type of tile to paint.</param>
        public void Paint(Vector3Int position, TileType type)
        {
            //switch (type)
            //{
            //    case TileType.Ground:
            //        // Removes redundant tiles.
            //        if (wallTiles.Contains(position))
            //        {
            //            wallTiles.Remove(position);
            //        }
            //        groundTiles.Add(position);
            //        break;
            //    case TileType.Wall:
            //        if (groundTiles.Contains(position))
            //        {
            //            groundTiles.Remove(position);
            //        }
            //        wallTiles.Add(position);
            //        break;
            //    default:
            //        break;
            //}
            foreach (SubTilemap submap in subTilemaps)
            {
                if (submap.tiles.Contains(position) && submap.tileType != type)
                {
                    submap.tiles.Remove(position);
                }
                else if (!submap.tiles.Contains(position) && submap.tileType == type)
                {
                    submap.tiles.Add(position);
                }
            }
            AddMeshPosition(position, type);
        }

        /// <summary>
        /// Erases a voxel from this tilemap.
        /// </summary>
        /// <param name="position">The position to erase at.</param>
        public void Erase(Vector3Int position)
        {
            foreach (SubTilemap submap in subTilemaps)
            {
                if (submap.tiles.Contains(position))
                {
                    submap.tiles.Remove(position);
                }
            }
            RemoveMeshPosition(position);
        }

        #region Mesh Updating
#if UNITY_EDITOR
        /// <summary>
        /// Creates and assigns a mesh asset for this tilemap.
        /// </summary>
        [Button]
        private void CreateMeshAsset()
        {
            Mesh mesh = new Mesh();
            string filePath = System.IO.Path.Join(ASSET_FOLDER, meshFilePath, meshFileName + MESH_FILE_EXTENSION);
            AssetDatabase.CreateAsset(mesh, filePath);
        }
#endif

        /// <summary>
        /// Updates the mesh for this tilemap by adding and removing faces based on an added or removed position.
        /// </summary>
        /// <remarks>
        /// Only works in the Unity Editor.
        /// </remarks>
        /// <param name="gridPos">The position that was added/removed from the tilemap.</param>
        private void AddMeshPosition(Vector3Int gridPos, TileType type)
        {
#if UNITY_EDITOR
            foreach (Vector3Int direction in CardinalDirections.CARDINAL_DIRECTIONS)
            {
                // If there is a voxel adjacent to this one, then we remove the face from that position in the opposite
                // direction of direction.
                if (CheckCell(gridPos + direction))
                {
                    // Because the mesh needs to work in world space, we always pass in the corner position that
                    // corresponds to the bottl-left-back corner of the cell we are evaluating.
                    meshUpdater.RemoveFace(GridToLocalCorner(gridPos + direction), -GridToLocalDirection(direction), 
                        meshFilter.sharedMesh);
                    // Dont run AddFace here as internal faces should not exist.
                }
                else
                {
                    // Add a face if there is not a filled tile in this relative direction.
                    meshUpdater.AddFace(GridToLocalCorner(gridPos), GridToLocalDirection(direction), 
                        type, meshFilter.sharedMesh);
                }
            }
#endif
        }
        private void RemoveMeshPosition(Vector3Int gridPos)
        {
#if UNITY_EDITOR
            foreach (Vector3Int direction in CardinalDirections.CARDINAL_DIRECTIONS)
            {
                // If there is a voxel adjacent to this one, then we add a face for it so no holes exist.
                if (CheckCell(gridPos + direction))
                {

                    // Because the mesh needs to work in world space, we always pass in the corner position that
                    // corresponds to the bottl-left-back corner of the cell we are evaluating.
                    meshUpdater.AddFace(GridToLocalCorner(gridPos + direction), -GridToLocalDirection(direction),
                        GetTileType(gridPos + direction), meshFilter.sharedMesh);
                    // Dont run AddFace here as internal faces should not exist.
                }
                // Always remove faces that point outwards from the erased tile.
                meshUpdater.RemoveFace(GridToLocalCorner(gridPos), GridToLocalDirection(direction),
                        meshFilter.sharedMesh);
            }
#endif
        }
        #endregion

        #region Cell Checking
        #region Static Functions
        /// <summary>
        /// Gets all cells of a certain type that hace the same 2D coordinates.
        /// </summary>
        /// <param name="position">The 2D position to get cells at.</param>
        /// <returns>All the cells with the given 2D position in a column.</returns>
        public static List<Vector3Int> Main_GetCellsInColumn(Vector2Int position, TileType type)
        {
            return Instance.GetCellsInColumn(position, type);
        }

        /// <summary>
        /// Checks the if a specific type of tile occupies a given cell in the main tilemap.
        /// </summary>
        /// <param name="position">The grid position of the cell to check.</param>
        /// <returns>True if there is a voxel in that cell, false if there is not.</returns>
        public static bool Main_CheckCell(Vector3Int position, TileType type)
        {
            return Instance.CheckCell(position, type);
        }

        /// <summary>
        /// Get the world position of a space.
        /// </summary>
        /// <param name="position">The grid position of the cell to check.</param>
        /// <returns>The world position of the cell.</returns>
        public static Vector3 Main_GridToWorldPos(Vector3Int position)
        {
            return Instance.GridToWorldPos(position);
        }
        #endregion

        /// <summary>
        /// Checks if a tile occupies a given cell.
        /// </summary>
        /// <param name="position">The grid position of the cell to check.</param>
        /// <returns>True if there is a voxel in that cell, false if there is not.</returns>
        public bool CheckCell(Vector3Int position)
        {
            bool returnVal = false;
            foreach (SubTilemap submap in subTilemaps)
            {
                returnVal |= submap.tiles.Contains(position);
            }
            return returnVal;
        }

        /// <summary>
        /// Checks if a specific type of tile occupies a given cell.
        /// </summary>
        /// <param name="position">The grid position of the cell to check.</param>
        /// <returns>True if there is a voxel in that cell, false if there is not.</returns>
        public bool CheckCell(Vector3Int position, TileType type)
        {
            List<Vector3Int> tileList = Array.Find(subTilemaps, item => item.tileType == type).tiles;
            return tileList.Contains(position);
            //switch (type)
            //{
            //    case TileType.Ground:
            //        return groundTiles.Contains(position);
            //    case TileType.Wall:
            //        return wallTiles.Contains(position);
            //    default:
            //        return groundTiles.Contains(position) | wallTiles.Contains(position);
            //}
        }

        public TileType GetTileType(Vector3Int position)
        {
            foreach (SubTilemap submap in subTilemaps)
            {
                if (submap.tiles.Contains(position))
                {
                    return submap.tileType;
                }
            }
            // Return wall as the default.
            return TileType.Wall;
        }

        /// <summary>
        /// Gets all tiles that share a 2D position.
        /// </summary>
        /// <param name="position">The position to get the cells at.</param>
        /// <param name="type">The type of cells to get.</param>
        /// <returns>A list of all cells with the given 2D position, regardless of height.</returns>
        public List<Vector3Int> GetCellsInColumn(Vector2Int position, TileType type)
        {
            List<Vector3Int> cells = Array.Find(instance.subTilemaps, item => item.tileType == type).tiles;
            return cells.FindAll(item => item.x == position.x && item.y == position.y);
        }
        #endregion

        #region Conversions
        /// <summary>
        /// Returns the world position of a cell in a grid layout at a given position.
        /// </summary>
        /// <param name="position">The cell position to get the world position of.</param>
        /// <returns>The world position of the cell.</returns>
        public Vector3 GridToWorldPos(Vector3Int position)
        {
            // Create a value that lets us center our spawned game objects to line up with the grid.
            Vector3 center = new Vector3(CELL_SIZE / 2, CELL_SIZE / 2, 0f);
            //Vector3Int cellPos = new Vector3Int(position.x, yPos, position.y);
            Vector3 worldPos = gridLayout.LocalToWorld(gridLayout.CellToLocalInterpolated(position + center));
            // Set Y position manually because Unity tilemaps doesnt support depth by default.
            worldPos.y = position.z;
            return worldPos;
        }

        /// <summary>
        /// Gets the bottom-left-back corner of a cell in local corner space.
        /// </summary>
        /// <param name="position">The cell position to get the bottom left back corner of.</param>
        /// <returns>The bottom-left-back cornter position of the cell.</returns>
        public Vector3Int GridToLocalCorner(Vector3Int position)
        {
            Vector3 localPos = gridLayout.CellToLocalInterpolated(position);
            // Set the Y position manually, and subtract 1 cell to make it so that the top of the cell lines up with
            // the layer.
            localPos.y = position.z - CELL_SIZE;
            return Vector3Int.RoundToInt(localPos);
        }

        /// <summary>
        /// Converts a direction in grid space to a direction in local space.
        /// </summary>
        /// <remarks>
        /// Simply swaps the Y and Z components of the direction because in grid space, Z represents the height and Y
        /// moves along the grid.
        /// </remarks>
        /// <param name="direction">The direction to convert to local space.</param>
        /// <returns>The direction in local space.</returns>
        public Vector3Int GridToLocalDirection(Vector3Int direction)
        {
            (direction.y, direction.z) = (direction.z, direction.y);
            return direction;
        }

        /// <summary>
        /// Gets the grid position of a world position.
        /// </summary>
        /// <param name="worldPos">The world position to get a grid position of.</param>
        /// <returns>The grid position of that world position.</returns>
        public Vector3Int WorldToGridPos(Vector3 worldPos)
        {
            Vector3Int gridPos = gridLayout.WorldToCell(worldPos);
            // Need to assign the Z (or elevation) value manually.
            gridPos.z = Mathf.RoundToInt(worldPos.y);
            return gridPos;
        }
        #endregion

        #region Selection Dummies
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
        #endregion
    }
}