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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Grubitecht.Tilemaps
{
    [RequireComponent(typeof(GridLayout))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class VoxelTilemap3D : MonoBehaviour
    {
        #region CONSTS
        private const float CELL_SIZE = 1f;
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
        [SerializeField] private Material testMaterial;
        [SerializeField] private List<Vector3Int> groundTiles;
        [SerializeField] private List<Vector3Int> wallTiles;

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
                    // Removes redundant tiles.
                    if (wallTiles.Contains(position))
                    {
                        wallTiles.Remove(position);
                    }
                    groundTiles.Add(position);
                    break;
                case TileType.Wall:
                    if (groundTiles.Contains(position))
                    {
                        groundTiles.Remove(position);
                    }
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
            internal Material material;
        }

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
            void AddVoxelFaces(List<Vector3Int> gridPositions)
            {
                foreach(Vector3Int gridPos in gridPositions)
                {
                    foreach (Vector3Int direction in CARDINAL_DIRECTIONS)
                    {
                        // If there is a voxel adjacent to this one, then we skip drawing a face.
                        if (CheckCell(gridPos + direction))
                        {
                            continue;
                        }

                        AddFace(gridPos, direction, testMaterial);
                    }
                }
            }

            List<int> trianglesList = new List<int>();
            // Adds a face that should be created when the mesh is baked.
            void AddFace(Vector3Int gridPosition, Vector3Int direction, Material material)
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
                signature.material = material;

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
            AddVoxelFaces(groundTiles);
            AddVoxelFaces(wallTiles);

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

                // Insert correct UV math to tile the voxels correctly here.
                Vector2 uv = new Vector2(0, 0);

                uvs[index] = uv;
            }

            mesh.Clear();
            mesh.vertices = verticies;
            mesh.uv = uvs;
            mesh.triangles = triangles;
        }
        #endregion
    }
}