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
using Unity.VisualScripting;
using System;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Grubitecht.Tilemaps
{
    [RequireComponent(typeof(GridLayout))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class VoxelTilemap3D : MonoBehaviour, ISelectable
    {
        #region CONSTS
        public const float CELL_SIZE = 1f;
        private static readonly Vector3Int[] CARDINAL_DIRECTIONS = new Vector3Int[]
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right,
            Vector3Int.forward,
            Vector3Int.back
        };
        private static readonly Vector3 GRID_CORNET_OFFSET = new Vector3(-0.5f, -0.5f, -0.5f);
        private const string ASSET_FOLDER = "Assets";
        private const string MESH_FILE_EXTENSION = ".mesh";
        #endregion
        [SerializeField] private string meshFilePath;
        [SerializeField] private string meshFileName;
        [Header("Tilemap Settings.")]
        [SerializeField] private SubTilemap[] subTilemaps;

        private static VoxelTilemap3D instance;

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
            BakeMesh();
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
            BakeMesh();
        }

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

        /// <summary>
        /// Returns the world position of a cell in a grid layout at a given position.
        /// </summary>
        /// <param name="grid">The grid that contains the cell to get the world position of.</param>
        /// <param name="position">The cell position to get the world position of.</param>
        /// <param name="targetTransform">The transform of the object that contains the tilemap.</param>
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

        #region Mesh Construction
#if UNITY_EDITOR
        /// <summary>
        /// Creates and assigns a mesh asset for this tilemap.
        /// </summary>
        [Button]
        private void CreateMeshAsset()
        {
            Mesh mesh = new Mesh();
            meshFilter.sharedMesh = mesh;
            string filePath = System.IO.Path.Join(ASSET_FOLDER, meshFilePath, meshFileName + MESH_FILE_EXTENSION);
            AssetDatabase.CreateAsset(mesh, filePath);
        }
#endif

        private struct VertexSignature
        {
            internal Vector3Int position;
            internal Vector3Int normal;
            internal TileType tileType;
        }

        /// <summary>
        /// Creates the mesh that will visualize the tilemap.
        /// </summary>
        private void BakeMesh()
        {
            // Define a dictionary to store indicies of our verticies.
            Dictionary<VertexSignature, int> vertexIndicies = new Dictionary<VertexSignature, int>();
            int vertexCount = 0;

            #region Local Functions
            // Gets a vertex index with a given signature.
            int GetVertexIndex(VertexSignature signature)
            {
                // Attempts to get a vertex index that already exists.
                if (!vertexIndicies.TryGetValue(signature, out int index))
                {
                    // If no vertex exists with the given signature, then we add a new vertex with that signature.
                    vertexCount++;
                    index = vertexCount - 1;
                    vertexIndicies.Add(signature, index);
                }
                return index;
            }

            // Loops through all voxels in a list and queues faces that need to be rendered for them.
            void AddVoxelFaces(SubTilemap submap)
            {
                foreach(Vector3Int gridPos in submap.tiles)
                {
                    foreach (Vector3Int direction in CARDINAL_DIRECTIONS)
                    {
                        // If there is a voxel adjacent to this one, then we skip drawing a face.
                        if (CheckCell(gridPos + direction))
                        {
                            continue;
                        }
                        // Uses ground by default.  Fix this.
                        AddFace(gridPos, direction, submap.tileType);
                    }
                }
            }

            List<int> trianglesList = new List<int>();
            // Adds a face that should be created when the mesh is baked.
            void AddFace(Vector3Int gridPosition, Vector3Int direction, TileType type)
            {
                direction = GridToLocalDirection(direction);
                Vector3Int[] vertexOffsets = FaceLookup.GetVertexOffsets(direction);

                // Gets the local position of the bottom-left-back corner of this grid position.
                // Calling this Corner Space.  Verticies are calculated in corner space because the world is
                // entirely made of 1 x 1 x 1 voxels that should have their corners exist at integer positions.
                Vector3Int cornerPos = GridToLocalCorner(gridPosition);

                // Initializes the vertex signature that will be used for the verticies of this face.
                VertexSignature signature;
                signature.normal = direction;
                signature.tileType = type;

                // Creates or gets references to indicies based on the signature at each vertex position.
                // Dont need to create new signatures as structs are defined on a per-variable basis.
                // Passing in signature gives the function and entirely new data set, it doesnt pass a reference.
                // This part makes the assumption that 4 vertex offsets were obtained from GetVertexOffsets.
                signature.position = cornerPos + vertexOffsets[0];
                int bottomLeft = GetVertexIndex(signature);

                signature.position = cornerPos + vertexOffsets[1];
                int topLeft = GetVertexIndex(signature);

                signature.position = cornerPos + vertexOffsets[2];
                int topRight = GetVertexIndex(signature);

                signature.position = cornerPos + vertexOffsets[3];
                int bottomRight = GetVertexIndex(signature);

                // Add these newly created verticies to the triangles list
                trianglesList.Add(bottomLeft);
                trianglesList.Add(topLeft);
                trianglesList.Add(topRight);

                trianglesList.Add(bottomLeft);
                trianglesList.Add(topRight);
                trianglesList.Add(bottomRight);
            }
            #endregion

            // Add faces for ground and wall tiles.
            foreach (SubTilemap submap in subTilemaps)
            {
                AddVoxelFaces(submap);
            }

            // Mesh creation
            Vector3[] verticies = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            int[] triangles = trianglesList.ToArray();

            // Gets a reference to a pre-existing mesh file.
            Mesh mesh = meshFilter.sharedMesh;

            foreach (var pair in vertexIndicies)
            {
                int index = pair.Value;
                verticies[index] = pair.Key.position;

                // Uses a sin wave with a period of 4 to alternate the UV values so that the faces tile correctly.

                // Adjust UVs to account for texture offset here.
                Vector2 uv = ProjectPositionToUV(pair.Key.position, pair.Key.normal, pair.Key.tileType);

                uvs[index] = uv;
            }

            DebugHelpers.LogCollection(uvs);
            mesh.Clear();
            mesh.vertices = verticies;
            mesh.uv = uvs;
            mesh.triangles = triangles;
        }

        /// <summary>
        /// Uses a sin wave to calculate the correct texturing of the UVs of the mesh.
        /// </summary>
        /// <remarks>
        /// Makes a lot of assumptions about texture formatting.  Materials need to have a tiling value of 1/x and 1/y.
        /// Textures need to be formatted so that each row has 3 faces, in the order side, top, bottom and each
        /// Column is for a different type of tile.
        /// </remarks>
        /// <param name="pos">The position of the vertex we're calculating the UV for.</param>
        /// <param name="direction">The direction of that vertex's normal.</param>
        /// <returns>A Vector2 UV for that normal.s</returns>
        private Vector2 ProjectPositionToUV(Vector3Int pos, Vector3Int direction, TileType type)
        {
            int xVal;
            int yVal;
            switch(direction.x, direction.y, direction.z)
            {
                case (0, 1, 0):
                case (0, -1, 0):
                    xVal = pos.x;
                    yVal = pos.z;
                    break;
                case (1, 0, 0):
                case (-1, 0, 0):
                    xVal = pos.z;
                    yVal = pos.y;
                    break;
                case (0, 0, 1):
                case (0, 0, -1):
                    xVal = pos.x;
                    yVal = pos.y;
                    break;
                default:
                    xVal = 0;
                    yVal = 0;
                    break;
            }
            float uvx = Mathf.Abs(Mathf.Sin(xVal * (Mathf.PI / 2)));
            float uvy = Mathf.Abs(Mathf.Sin(yVal * (Mathf.PI / 2)));
            // For Up and down faces, their texture should be different than the texture for side faces.
            if (direction == Vector3Int.up)
            {
                uvx += 1;
            }
            else if (direction == Vector3Int.down)
            {
                uvx += 2;
            }
            // Offsets the Y value by the type's corresponding int value to offset the UV's on the texture.
            uvy += (int)type;
            return new Vector2(uvx, uvy);
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