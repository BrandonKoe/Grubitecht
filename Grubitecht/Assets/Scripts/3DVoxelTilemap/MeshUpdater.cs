#if UNITY_EDITOR
/*****************************************************************************
// File Name : MeshUpdater.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Updates the mesh of a 3D tilemap.
*****************************************************************************/
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Grubitecht.Tilemaps
{
    [System.Serializable]
    public class MeshUpdater
    {
        [SerializeField, ReadOnly] private List<Face> faces = new();
        [SerializeField, ReadOnly] private List<Vertex> vertexReferences = new();
        [SerializeField, ReadOnly] private List<Vector3> verticies = new();
        [SerializeField, ReadOnly] private List<Vector2> uvs = new();
        [SerializeField, ReadOnly] private List<TriangleReference> triangleReferences = new();
        [SerializeField, ReadOnly] private List<int> triangles = new();

        #region Nested Classes
        [System.Serializable]
        private struct Face
        {
            [SerializeField, ReadOnly] internal Vector3Int position;
            [SerializeField, ReadOnly] internal Vector3Int normal;
            [SerializeField, ReadOnly] internal TileType tileType;
            [SerializeField, ReadOnly] internal Vertex[] verticies;
            [SerializeField, ReadOnly] internal TriangleReference[] triangles;
        }
        [System.Serializable]
        private struct Vertex
        {
            [SerializeField, ReadOnly] internal Vector3Int position;
            [SerializeField, ReadOnly] internal Vector3Int normal;
            [SerializeField, ReadOnly] internal TileType tileType;
        }
        [System.Serializable]
        private class TriangleReference
        {

        }
        #endregion

        #region Mesh Construction
        /// <summary>
        /// Adds a new face to the tilemap mesh.
        /// </summary>
        /// <param name="cornerPosition">
        /// The position of the bottom-left-back corner of the cell that this face will represent a part of.
        /// </param>
        /// <param name="direction">The direction of the face.</param>
        /// <param name="type">The type of tile.  This determines what material is tiled onto the face.</param>
        /// <param name="mesh">The mesh to update.</param>
        public void AddFace(Vector3Int cornerPosition, Vector3Int direction, TileType type, Mesh mesh)
        {
            // Skips over faces that already exist.
            if (CheckFace(cornerPosition, direction)) { return; }
            // If the face doesnt exist, we create a new face using references to verticies that already exist.
            Face face;
            face.position = cornerPosition;
            face.normal = direction;
            face.tileType = type;
            face.verticies = new Vertex[4];

            // Create strutcs to represent the verticies of the faces.
            Vertex vertex;
            vertex.normal = direction;
            vertex.tileType = type;
            Vector3Int[] vertexOffsets = FaceLookup.GetVertexOffsets(direction);

            // Adds each vertex based on the vertex offsets for this face.  Then, try to add them to the stored vertex
            // lists.  If they already exist then no new vertex is added.
            vertex.position = face.position + vertexOffsets[0];
            face.verticies[0] = vertex;
            int bottomLeft = GetVertex(vertex);

            vertex.position = face.position + vertexOffsets[1];
            face.verticies[1] = vertex;
            int topLeft = GetVertex(vertex);

            vertex.position = face.position + vertexOffsets[2];
            face.verticies[2] = vertex;
            int topRight = GetVertex(vertex);

            vertex.position = face.position + vertexOffsets[3];
            face.verticies[3] = vertex;
            int bottomRight = GetVertex(vertex);

            // Add Triangles
            face.triangles = AddTriangles(bottomLeft, topLeft, topRight, bottomRight);

            faces.Add(face);

            // No need to update any indicies here since adding things only adds indicies at the end of the list.
            UpdateMesh(mesh);
        }

        /// <summary>
        /// Gets the index of the vertex with a given signature.
        /// </summary>
        /// <param name="vertex">The vertex signature to get the index of.</param>
        private int GetVertex(Vertex vertex)
        {
            // Return a vertex that already exists.
            if (vertexReferences.Contains(vertex))
            {
                return vertexReferences.IndexOf(vertex);
            }
            // If no vertex already exists, then we make a new one.
            int index = verticies.Count;
            AddVertex(vertex);
            return index;
        }

        /// <summary>
        /// Adds a new vertex.
        /// </summary>
        /// <param name="vertex">The vertex to add.</param>
        private void AddVertex(Vertex vertex)
        {
            vertexReferences.Add(vertex);
            verticies.Add(vertex.position);

            // Adds UVs for the added vertex.
            Vector2 uv = ProjectPositionToUV(vertex.position, vertex.normal, vertex.tileType);
            uvs.Add(uv);
        }

        /// <summary>
        /// Removes a vertex from this mesh.
        /// </summary>
        /// <param name="vertex">The vertex to remove.</param>
        /// <returns> The index of the removed vertex.</returns>
        private int RemoveVertex(Vertex vertex)
        {
            // Only remove verticies that exist.
            if (vertexReferences.Contains(vertex))
            {
                // Removes both the vertex reference and the actual stored vertex associated with that reference at
                // the same index.
                int index = vertexReferences.IndexOf(vertex);
                vertexReferences.RemoveAt(index);
                verticies.RemoveAt(index);
                uvs.RemoveAt(index);
                return index;
            }
            return -1;
        }

        /// <summary>
        /// Adds triangles to the triangles array based on the indicies of the verticies of a face.
        /// </summary>
        /// <param name="bottomLeft">The index of the bottom left vertex for that face.</param>
        /// <param name="topLeft">The index of the top left vertex for that face.</param>
        /// <param name="topRight">The index of the top right vertex for that face.</param>
        /// <param name="bottomRight">The index of the bottom right vertex for that face.</param>
        /// <returns>The array of triangle references that will be used to remove triangles.</returns>
        private TriangleReference[] AddTriangles(int bottomLeft, int topLeft, int topRight, int bottomRight)
        {
            TriangleReference ref1 = new TriangleReference();
            triangleReferences.Add(ref1);
            triangles.Add(bottomLeft);
            triangles.Add(topLeft);
            triangles.Add(topRight);

            TriangleReference ref2 = new TriangleReference();
            triangleReferences.Add(ref2);
            triangles.Add(bottomLeft);
            triangles.Add(topRight);
            triangles.Add(bottomRight);

            return new TriangleReference[] { ref1, ref2 };
        }

        /// <summary>
        /// Removes a triangle from the mesh.
        /// </summary>
        /// <param name="triangleReference">The reference to the triangle to remove.</param>
        private void RemoveTriangle(TriangleReference triangleReference)
        {
            if (triangleReferences.Contains(triangleReference))
            {
                int index = triangleReferences.IndexOf(triangleReference);
                triangleReferences.RemoveAt(index);
                // Removes the three triangles associated with the given triangle reference.
                index *= 3;
                triangles.RemoveAt(index);
                triangles.RemoveAt(index);
                triangles.RemoveAt(index);
            }
        }

        public void RemoveFace(Vector3Int cornerPosition, Vector3Int direction, Mesh mesh)
        {
            // Skips over faces that dont exist.
            if (!CheckFace(cornerPosition, direction)) { return; }
            Face face = GetFace(cornerPosition, direction);

            // Remove Triangles
            RemoveTriangle(face.triangles[0]);
            RemoveTriangle(face.triangles[1]);

            // Remove Verticies
            foreach (Vertex vert in face.verticies)
            {
                Debug.Log(vert.position);
                bool vertexUsed = false;
                foreach (Vector3Int dir in CardinalDirections.CARDINAL_DIRECTIONS)
                {
                    foreach (Vector3Int nor in CardinalDirections.CARDINAL_DIRECTIONS)
                    {
                        if (!CheckFace(dir, nor)) { continue; } 
                        Face adjFace = GetFace(cornerPosition + dir, nor);
                        if (adjFace.verticies.Contains(vert))
                        {
                            vertexUsed = true;
                        }
                    }
                }
                if (!vertexUsed)
                {
                    int removedIndex = RemoveVertex(vert);
                    // Update the indicies of all the triangles whose indexes were shifted by removing verticies.
                    for (int i = 0; i < triangles.Count; i++)
                    {
                        if (triangles[i] > removedIndex)
                        {
                            triangles[i] -= 1;
                        }
                    }
                }
            }

            faces.Remove(face);

            UpdateMesh(mesh);
        }

        /// <summary>
        /// Checks if a given face already exists.
        /// </summary>
        /// <param name="cornerPosition">The corner position to reference the face.</param>
        /// <param name="direction">The direction of the face.</param>
        /// <returns>True if the face exists.</returns>
        private bool CheckFace(Vector3Int cornerPosition, Vector3Int direction)
        {
            return faces.Any(item => item.position == cornerPosition && item.normal == direction);
        }

        /// <summary>
        /// Gets a pre-existing face from the face list based on a position and direction.
        /// </summary>
        /// <param name="cornerPosition">The position of the face to get.</param>
        /// <param name="direction">The direction of the face to get.</param>
        /// <returns>The face at that position pointing in that direction, if one exists.</returns>
        private Face GetFace(Vector3Int cornerPosition, Vector3Int direction)
        {
            return faces.Find(item => item.position == cornerPosition && item.normal == direction);
        }

        /// <summary>
        /// Updates the mesh with the currently calculated values.
        /// </summary>
        /// <param name="mesh">The mesh to update.</param>
        private void UpdateMesh(Mesh mesh)
        {
            mesh.Clear();
            mesh.vertices = verticies.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = triangles.ToArray();
        }

        ///// <summary>
        ///// Creates the mesh that will visualize the tilemap.
        ///// </summary>
        //private void BakeMesh()
        //{
        //    // Define a dictionary to store indicies of our verticies.
        //    Dictionary<Vertex, int> vertexIndicies = new Dictionary<Vertex, int>();
        //    int vertexCount = 0;

        //    #region Local Functions
        //    // Gets a vertex index with a given signature.
        //    int GetVertexIndex(Vertex signature)
        //    {
        //        // Attempts to get a vertex index that already exists.
        //        if (!vertexIndicies.TryGetValue(signature, out int index))
        //        {
        //            // If no vertex exists with the given signature, then we add a new vertex with that signature.
        //            vertexCount++;
        //            index = vertexCount - 1;
        //            vertexIndicies.Add(signature, index);
        //        }
        //        return index;
        //    }

        //    // Loops through all voxels in a list and queues faces that need to be rendered for them.
        //    void AddVoxelFaces(SubTilemap submap)
        //    {
        //        foreach (Vector3Int gridPos in submap.tiles)
        //        {
        //            foreach (Vector3Int direction in CardinalDirections.CARDINAL_DIRECTIONS)
        //            {
        //                // If there is a voxel adjacent to this one, then we skip drawing a face.
        //                if (CheckCell(gridPos + direction))
        //                {
        //                    continue;
        //                }
        //                // Uses ground by default.  Fix this.
        //                AddFace(gridPos, direction, submap.tileType);
        //            }
        //        }
        //    }

        //    List<int> trianglesList = new List<int>();
        //    // Adds a face that should be created when the mesh is baked.
        //    void AddFace(Vector3Int gridPosition, Vector3Int direction, TileType type)
        //    {
        //        direction = GridToLocalDirection(direction);
        //        Vector3Int[] vertexOffsets = FaceLookup.GetVertexOffsets(direction);

        //        // Gets the local position of the bottom-left-back corner of this grid position.
        //        // Calling this Corner Space.  Verticies are calculated in corner space because the world is
        //        // entirely made of 1 x 1 x 1 voxels that should have their corners exist at integer positions.
        //        Vector3Int cornerPos = GridToLocalCorner(gridPosition);

        //        // Initializes the vertex signature that will be used for the verticies of this face.
        //        Vertex signature;
        //        signature.normal = direction;
        //        signature.tileType = type;

        //        // Creates or gets references to indicies based on the signature at each vertex position.
        //        // Dont need to create new signatures as structs are defined on a per-variable basis.
        //        // Passing in signature gives the function and entirely new data set, it doesnt pass a reference.
        //        // This part makes the assumption that 4 vertex offsets were obtained from GetVertexOffsets.
        //        signature.position = cornerPos + vertexOffsets[0];
        //        int bottomLeft = GetVertexIndex(signature);

        //        signature.position = cornerPos + vertexOffsets[1];
        //        int topLeft = GetVertexIndex(signature);

        //        signature.position = cornerPos + vertexOffsets[2];
        //        int topRight = GetVertexIndex(signature);

        //        signature.position = cornerPos + vertexOffsets[3];
        //        int bottomRight = GetVertexIndex(signature);

        //        // Add these newly created verticies to the triangles list
        //        trianglesList.Add(bottomLeft);
        //        trianglesList.Add(topLeft);
        //        trianglesList.Add(topRight);

        //        trianglesList.Add(bottomLeft);
        //        trianglesList.Add(topRight);
        //        trianglesList.Add(bottomRight);
        //    }
        //    #endregion

        //    // Add faces for ground and wall tiles.
        //    foreach (SubTilemap submap in subTilemaps)
        //    {
        //        AddVoxelFaces(submap);
        //    }

        //    // Mesh creation
        //    Vector3[] verticies = new Vector3[vertexCount];
        //    Vector2[] uvs = new Vector2[vertexCount];
        //    int[] triangles = trianglesList.ToArray();

        //    // Gets a reference to a pre-existing mesh file.
        //    Mesh mesh = meshFilter.sharedMesh;

        //    foreach (var pair in vertexIndicies)
        //    {
        //        int index = pair.Value;
        //        verticies[index] = pair.Key.position;

        //        // Uses a sin wave with a period of 4 to alternate the UV values so that the faces tile correctly.

        //        // Adjust UVs to account for texture offset here.
        //        Vector2 uv = ProjectPositionToUV(pair.Key.position, pair.Key.normal, pair.Key.tileType);

        //        uvs[index] = uv;
        //    }

        //    //DebugHelpers.LogCollection(uvs);
        //    mesh.Clear();
        //    mesh.vertices = verticies;
        //    mesh.uv = uvs;
        //    mesh.triangles = triangles;
        //}

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
            switch (direction.x, direction.y, direction.z)
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
    }

}
#endif