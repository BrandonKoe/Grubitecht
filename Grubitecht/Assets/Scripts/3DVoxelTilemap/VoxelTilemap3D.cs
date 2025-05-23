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
using System.Linq;
using Grubitecht.World;





#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Grubitecht.Tilemaps
{
    [RequireComponent(typeof(GridLayout))]
    //[RequireComponent(typeof(MeshFilter))]
    //[RequireComponent(typeof(MeshRenderer))]
    public class VoxelTilemap3D : MonoBehaviour
    {
        #region CONSTS
        public const float CELL_SIZE = 1f;
        private static readonly Vector3 GRID_CORNET_OFFSET = new Vector3(-0.5f, -0.5f, -0.5f);
        private const string ASSET_FOLDER = "Assets";
        private const string MESH_FILE_EXTENSION = ".mesh";
        #endregion
        [SerializeField,Tooltip("The file path where the chunk meshes for this tilemap are stored.")]
        private string meshFilePath;
        [Header("Chunks")]
        [SerializeField] private Chunk chunkPrefab;
        [SerializeField, ReadOnly] private List<Chunk> chunks;
        [SerializeField] private int chunkSize = 16;
        [Header("Tilemap Settings.")]
        //[SerializeField] private List<VoxelTile> tiles;

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

        /// <summary>
        /// Assign Component references on reset.
        /// </summary>
        private void Reset()
        {
            gridLayout = GetComponent<GridLayout>();
        }
        #endregion

        #region Nested Classes
        ///// <summary>
        ///// Class to represent a specific sub-tilemap of this tilemap.
        ///// </summary>
        //[System.Serializable]
        //private class SubTilemap
        //{
        //    [SerializeField] internal TileType tileType;
        //    [SerializeField] internal List<Vector3Int> tiles;
        //}

        private enum TileType
        {
            Ground,
            Wall
        }
        #endregion

        #region Setup
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
        #endregion

        #region Adding/Removing Tiles
        /// <summary>
        /// Paints a voxel on this tilemap.
        /// </summary>
        /// <param name="position">The position to paint at.</param>
        /// <param name="smoothAbove">Whether placing this tile should delete tiles above it.</param>
        public void Paint(Vector3Int position, bool smoothAbove)
        {
            //foreach (SubTilemap submap in subTilemaps)
            //{
            //    if (submap.tiles.Contains(position) && submap.tileType != type)
            //    {
            //        submap.tiles.Remove(position);
            //    }
            //    else if (!submap.tiles.Contains(position) && submap.tileType == type)
            //    {
            //        submap.tiles.Add(position);
            //    }
            //}

            //VoxelTile existingTile = tiles.Find(item => item.GridPosition2 == (Vector2Int)position);
            //if (existingTile != null)
            //{
            //    if (existingTile.GridPosition == position || 
            //        (!smoothAbove && existingTile.GridPosition.z > position.z)) 
            //    {
            //        return; 
            //    }
            //    tiles.Remove(existingTile);
            //}
            ////UpdateAdjacents(newTile);
            //tiles.Add(new VoxelTile(position));

            Vector2Int chunkPos = GetChunkPos(position);
            // Ensures no null references exist when finding chunks.
            chunks.RemoveAll(item => item == null);
            Chunk chunk = GetChunk(chunkPos);
            if (chunk == null)
            {
#if UNITY_EDITOR
                // Create a new chunk if the chunk this position is at is null.
                chunk = CreateNewChunk(chunkPos);
#endif
            }
            chunk.AddTile(position, smoothAbove);

            //if (refreshMesh)
            //{
            //    BakeMesh(position);
            //}
        }

        ///// <summary>
        ///// Updates all tiles adjacent to a given tile, and gives the passed in tile references to those
        ///// adjacent tiles.
        ///// </summary>
        ///// <param name="tile">The tile to update the adjacence for.</param>
        //private void UpdateAdjacents(VoxelTile tile)
        //{
        //    VoxelTile[] adjTiles = new VoxelTile[8];
        //    for (int i = 0; i < CardinalDirections.DIAGONAL_2D.Length; i++)
        //    {
        //        adjTiles[i] = GetTile(tile.GridPosition2 + CardinalDirections.DIAGONAL_2D[i]);
        //    }
        //    tile.SetAdjacnets(adjTiles);
        //}

        /// <summary>
        /// Erases a voxel from this tilemap.
        /// </summary>
        /// <param name="position">The position to erase at.</param>
        public void Erase(Vector3Int position)
        {
            //VoxelTile toErase = tiles.Find(item => item.GridPosition == position);
            //if (toErase != null)
            //{
            //    tiles.Remove(toErase);
            //}

            Chunk chunk = GetChunk(GetChunkPos(position));
            // Ensures no null references exist when finding chunks.
            chunks.RemoveAll(item => item == null);
            if (chunk != null)
            {
                chunk.RemoveTile(position);
                // If we removed the last tile from a chunk, then we should destroy that chunk.
                if (chunk.Tiles.Count == 0)
                {
#if UNITY_EDITOR
                    DestroyChunk(chunk);
#endif
                }
            }
            //foreach (SubTilemap submap in subTilemaps)
            //{
            //    if (submap.tiles.Contains(position))
            //    {
            //        submap.tiles.Remove(position);
            //    }
            //}
            //if (refreshMesh)
            //{
            //    BakeMesh(position);
            //}
        }
        #endregion

        #region Static Functions
        ///// <summary>
        ///// Gets all cells of a certain type that hace the same 2D coordinates.
        ///// </summary>
        ///// <param name="position">The 2D position to get cells at.</param>
        ///// <returns>All the cells with the given 2D position in a column.</returns>
        //public static List<Vector3Int> Main_GetCellsInColumn(Vector2Int position)
        //{
        //    return Instance.GetCellsInColumn(position);
        //}

        ///// <summary>
        ///// Finds the cell in a column that is the closest to a reference position
        ///// </summary>
        ///// <param name="position">The poistion of the clumn to get a cell from</param>
        ///// <param name="referencePosition">
        ///// The position to use as a reference when evaluating the closest cell.
        ///// </param>
        ///// <returns>The cell in the column closest to the reference position.</returns>
        //public static Vector3Int Main_GetClosestCellInColumn(Vector2Int position, Vector3Int referencePosition)
        //{
        //    return Instance.GetClosestCellInColumn(position, referencePosition);
        //}

        /// <summary>
        /// Checks the if a specific type of tile occupies a given cell in the main tilemap.
        /// </summary>
        /// <param name="position">The grid position of the cell to check.</param>
        /// <returns>True if there is a voxel in that cell, false if there is not.</returns>
        public static bool Main_CheckCell(Vector3Int position)
        {
            return Instance.CheckCell(position);
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

        /// <summary>
        /// Gets the list of tiles in this tilemap of a certain tile type.
        /// </summary>
        /// <param name="tileType">The type of tilemap list to get.</param>
        /// <returns>The list of all tiles of a given type in the main tilemap.</returns>
        public static List<VoxelTile> Main_GetTilemap()
        {
            throw new System.NotImplementedException();
            //return Instance.tiles;
        }

        /// <summary>
        /// Allows external scripts to get references to tiles.
        /// </summary>
        /// <param name="position">The position to get the tile at.</param>
        /// <returns>The tile at that position.</returns>
        public static VoxelTile Main_GetTile(Vector3Int position)
        {
            return Instance.GetTile(position);
        }

        public static VoxelTile Main_GetTile(Vector2Int position)
        {
            return Instance.GetTile(position);
        }

        /// <summary>
        /// Finds the empty tile nearest to this one.
        /// </summary>
        /// <param name="originTile">The tile to originate the check from.</param>
        /// <param name="layer">The layer we're checking for vacancy at.</param>
        /// <param name="startingRange">The initial starting range of the check.</param>
        /// <param name="maxRange">The maximum range that can be checked.</param>
        /// <returns>The tile closest to the origin tile that doesnt contain an object on a given layer.</returns>
        public static VoxelTile Main_FindEmptyTile(VoxelTile originTile, OccupyLayer layer, int startingRange = 0, int maxRange = 100)
        {
            return Instance.FindEmptyTile(originTile, layer, startingRange, maxRange);
        }


        /// <summary>
        /// Gets an approximation of the space that this object's transform is currently at.
        /// </summary>
        /// <returns>The space that this object's transform is physically at in world space.</returns>
        public static VoxelTile Main_GetApproximateSpace(Vector3 pos)
        {
            return Instance.GetApproximateSpace(pos);
        }
        #endregion
        #region Cell Checking

        /// <summary>
        /// Checks if a tile occupies a given cell.
        /// </summary>
        /// <param name="position">The grid position of the cell to check.</param>
        /// <returns>True if there is a voxel in that cell, false if there is not.</returns>
        public bool CheckCell(Vector3Int position)
        {
            //bool returnVal = false;
            //foreach (SubTilemap submap in subTilemaps)
            //{
            //    returnVal |= submap.tiles.Contains(position);
            //}
            Chunk chunk = GetChunk(GetChunkPos(position));
            // If the chunk doesnt exist, then there must not be a cell at that location.
            if (chunk == null)
            {
                return false;
            }
            return chunk.Tiles.Any(item => item.GridPosition == position);
        }
        
        /// <summary>
        /// Checks if a tile exists at a particular position that should stop a face from rendering.
        /// </summary>
        /// <param name="position">The position to check for a face that prevents rendering.</param>
        /// <returns>True if this face should be skipped.</returns>
        private bool CheckFace(Vector3Int position)
        {
            Chunk chunk = GetChunk(GetChunkPos(position));
            // If the chunk is null, then there must be no tiles in there that may occlude a face so it should be
            // rendered.
            if (chunk == null)
            {
                return false;
            }
            return chunk.Tiles.Any(item => item.GridPosition2 == (Vector2Int)position && 
            item.GridPosition.z >= position.z);
        }

        ///// <summary>
        ///// Checks if a specific type of tile occupies a given cell.
        ///// </summary>
        ///// <param name="position">The grid position of the cell to check.</param>
        ///// <returns>True if there is a voxel in that cell, false if there is not.</returns>
        //public bool CheckCell(Vector3Int position)
        //{
        //    List<Vector3Int> tileList = Array.Find(subTilemaps, item => item.tileType == type).tiles;
        //    return tileList.Contains(position);
        //    //switch (type)
        //    //{
        //    //    case TileType.Ground:
        //    //        return groundTiles.Contains(position);
        //    //    case TileType.Wall:
        //    //        return wallTiles.Contains(position);
        //    //    default:
        //    //        return groundTiles.Contains(position) | wallTiles.Contains(position);
        //    //}
        //}

        ///// <summary>
        ///// Gets all tiles that share a 2D position.
        ///// </summary>
        ///// <param name="position">The position to get the cells at.</param>
        ///// <param name="type">The type of cells to get.</param>
        ///// <returns>A list of all cells with the given 2D position, regardless of height.</returns>
        //public List<Vector3Int> GetCellsInColumn(Vector2Int position)
        //{
        //    List<Vector3Int> cells = Array.Find(instance.subTilemaps, item => item.tileType == type).tiles;
        //    return cells.FindAll(item => item.x == position.x && item.y == position.y);
        //}

        ///// <summary>
        ///// Finds the cell in a column that is the closest to a reference position
        ///// </summary>
        ///// <param name="position">The poistion of the clumn to get a cell from</param>
        ///// <param name="referencePosition">
        ///// The position to use as a reference when evaluating the closest cell.
        ///// </param>
        ///// <param name="type">The type of cells to get.</param>
        ///// <returns>The cell in the column closest to the reference position.</returns>
        //public Vector3Int GetClosestCellInColumn(Vector2Int position, Vector3Int referencePosition, TileType type)
        //{
        //    List<Vector3Int> cells = Array.Find(instance.subTilemaps, item => item.tileType == type).tiles;
        //    return cells.FindAll(item => item.x == position.x && item.y == position.y).
        //        OrderBy(item => Vector3.Distance(item, referencePosition)).FirstOrDefault();
        //}

        /// <summary>
        /// Finds the tile at a given position.
        /// </summary>
        /// <param name="position">The position to get the tile at.</param>
        /// <returns>The tile at that position.</returns>
        public VoxelTile GetTile(Vector3Int position)
        {
            Chunk chunk = GetChunk(GetChunkPos(position));
            if (chunk == null)
            {
                return null;
            }
            return chunk.GetTile(position);
        }

        public VoxelTile GetTile(Vector2Int position)
        {
            Chunk chunk = GetChunk(GetChunkPos((Vector3Int)position));
            if (chunk == null)
            {
                return null;
            }
            return chunk.GetTile(position);
            //return tiles.Find(item => item.GridPosition2 == position);
        }

        /// <summary>
        /// Finds the nearest empty tile from a given origin tile.
        /// </summary>
        /// <param name="originTile">The tile to start finding a tile from.</param>
        /// <param name="layer">The layer to get an empty space of.</param>
        /// <param name="startingRange">The initial starting range to use when finding a space.</param>
        /// <param name="maxRange">The max search range that to find a tile within.</param>
        /// <returns>The closest unoccupied tile to the origin tile.</returns>
        public VoxelTile FindEmptyTile(VoxelTile originTile, OccupyLayer layer, int startingRange = 0, 
            int maxRange = 100)
        {
            int range = startingRange;
            while (range < maxRange)
            {
                for (int x = -range; x <= range; x++)
                {
                    // Find the possible variance in y positions for a given range.
                    int yRange = range - Mathf.Abs(x);
                    // Loops through both positive and negative values for y that yields a manhatten distance of
                    // range from the spawn point.
                    for (int i = -1; i < 2; i += 2)
                    {
                        int y = i * yRange;
                        // Check the positive and negative cells that have a manhatten distance of range.
                        Vector2Int checkPos = new Vector2Int(x, y) + originTile.GridPosition2;
                        VoxelTile checkCell = GetTile(checkPos);
                        // If checkCell is returned as null, then the cell we're trying to get does not exist on the
                        // tilemap and we should ignore it.
                        if (checkCell == null)
                        {
                            continue;
                        }
                        // If this position isnt occupied, then return it.
                        if (!checkCell.ContainsObjectOnLayer(layer))
                        {
                            return checkCell;
                        }
                    }
                }
                // If we were not able to find a cell through looping, then increment range and recursively call this
                // function agian.
                range++;
            }
            // If max range is exceeded, then we didnt find a valid adjacent tile.
            return null;
        }

        /// <summary>
        /// Gets all tiles within a certain radius of a given tile.
        /// </summary>
        /// <param name="centerTile">The tile that acts as the center of the check.</param>
        /// <param name="radius">The radius to get all tiles within.</param>
        /// <returns>All the tiles within that radius of the center tile.</returns>
        public List<VoxelTile> GetTilesInRadius(VoxelTile centerTile, int radius)
        {
            List<VoxelTile> outputList = new List<VoxelTile>();
            List<VoxelTile> currentList = new List<VoxelTile>() { centerTile };
            // Loop through each radius of tiles.
            for (int i = 0; i < radius; i++)
            {
                List<VoxelTile> bufferList = new List<VoxelTile>();
                // loop through all items in our current list of tiles to evaluate.
                foreach (VoxelTile tile in currentList)
                {
                    // Loop through all tiles adjacent to the current tile.
                    foreach (Vector2Int direction in CardinalDirections.ORTHOGONAL_2D)
                    {
                        // If we haven't already seen this tile, then we added it to our output list and our buffer
                        // list.
                        VoxelTile adjTile = tile.GetAdjacent(direction);
                        if (!outputList.Contains(adjTile))
                        {
                            outputList.Add(adjTile);
                            bufferList.Add(adjTile);
                        }
                    }
                }
                // Once we've looped through all the items in our current list, our buffer list becomes our new
                // current list and we continue on to the next radius.
                currentList = bufferList;
            }
            return outputList;
        }

        /// <summary>
        /// Gets an approximation of the space that this object's transform is currently at.
        /// </summary>
        /// <returns>The space that this object's transform is physically at in world space.</returns>
        public VoxelTile GetApproximateSpace(Vector3 pos)
        {
            Vector2Int approxSpace = new Vector2Int();
            approxSpace.x = Mathf.RoundToInt(pos.x - VoxelTilemap3D.CELL_SIZE / 2);
            approxSpace.y = Mathf.RoundToInt(pos.z - VoxelTilemap3D.CELL_SIZE / 2);
            //Debug.Log(approxSpace);
            // Gets a list of possible spaces this object could exist at based on it's 2D position and then finds
            // the one with the closest elevation.  This ensures that the object snaps from gravity.
            return GetTile(approxSpace);
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

        #region Chunking
        /// <summary>
        /// Gets the chunk at a given chunk position.
        /// </summary>
        /// <param name="chunkPos">The chunk position to check.</param>
        /// <returns>The chunk at the given chunk position.</returns>
        private Chunk GetChunk(Vector2Int chunkPos)
        {
            return chunks.Find(item => item.ChunkPos == chunkPos);
        }

        /// <summary>
        /// Gets the chunk position of a certain space on the grid.
        /// </summary>
        /// <param name="gridPos">The grid position to get the chunk of.</param>
        /// <returns>The position of the chunk this grid lies on.</returns>
        private Vector2Int GetChunkPos(Vector3Int gridPos)
        {
            gridPos.x = GetChunkPos(gridPos.x);
            gridPos.y = GetChunkPos(gridPos.y);
            return (Vector2Int)gridPos;
        }

        /// <summary>
        /// Converts a component of a grid position into a chunk position.
        /// </summary>
        /// <param name="gridPos">The position on the grid.</param>
        /// <returns>The chunk position the grid is within.</returns>
        private int GetChunkPos(int gridPos)
        {
            int skew = MathHelpers.GetSign(gridPos) < 0 ? -(chunkSize - 1) : 0;
            return (gridPos + skew) / chunkSize;
        }

        /// <summary>
        /// Converts a grid position into a relative position within it's chunk.
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        private Vector3Int GridToRelativePos(Vector3Int gridPos)
        {
            gridPos.x = MathHelpers.Mod(gridPos.x, chunkSize);
            gridPos.y = MathHelpers.Mod(gridPos.y, chunkSize);
            return gridPos;
        }
        #endregion

        #region EditorOnly
#if UNITY_EDITOR
        #region Mesh Construction
        /// <summary>
        /// Creates and assigns a mesh asset for a given chunk of the tilemap.
        /// </summary>
        private void CreateMeshAsset(Chunk chunk)
        {
            Mesh mesh = new Mesh();
            chunk.ChunkMesh = mesh;
            mesh.MarkDynamic();
            string filePath = System.IO.Path.Join(ASSET_FOLDER, meshFilePath, chunk.gameObject.name + 
                chunk.ChunkPos.ToString() + MESH_FILE_EXTENSION);
            chunk.MeshPath = filePath;
            AssetDatabase.CreateAsset(mesh, filePath);
        }

        private struct VertexSignature
        {
            internal Vector3Int position;
            internal Vector3Int normal;
            internal TileType tileType;
        }

        /// <summary>
        /// Updates the mesh that renders the tilemap.
        /// </summary>
        /// <param name="gridPos">The position on the tilemap that was edited.</param>
        public void BakeMesh(Vector3Int gridPos)
        {
            Vector2Int chunkPos = GetChunkPos(gridPos);
            // Get the chunk we are working with.
            Chunk chunk = GetChunk(chunkPos);
            //// Make a new chunk if we are working with a chunk that doesn't exist.
            //if (chunk == null)
            //{
            //    chunk = CreateNewChunk(chunkPos);
            //}
            BakeChunkMesh(chunk);

            // If we are painting on the edge where two chunks intersect, then we need to update both chunks to avoid
            // a chunk not updating properly and showing a hole.
            Vector3Int relativePos = GridToRelativePos(gridPos);
            if (relativePos.x == 0 || relativePos.x == chunkSize - 1)
            {
                Vector2Int offsetChunkPos = chunkPos + (MathHelpers.GetSign(relativePos.x - (chunkSize / 2)) * 
                    Vector2Int.right);
                Chunk offsetChunk = GetChunk(offsetChunkPos);
                if (offsetChunk != null)
                {
                    //Debug.Log(relativePos + "X");
                    BakeChunkMesh(offsetChunk);
                }
            }
            if (relativePos.y == 0 || relativePos.y == chunkSize - 1)
            {
                Vector2Int offsetChunkPos = chunkPos + (MathHelpers.GetSign(relativePos.y - (chunkSize / 2)) * 
                    Vector2Int.up);
                Chunk offsetChunk = GetChunk(offsetChunkPos);
                if (offsetChunk != null)
                {
                    //Debug.Log(relativePos + "Y");
                    BakeChunkMesh(offsetChunk);
                }
            }
        }

        /// <summary>
        /// Creates the mesh that will visualize a specific chunk of the tilemap.
        /// </summary>
        private void BakeChunkMesh(Chunk chunk)
        {
            // If the chunk we're updating doesnt exist, return.  This can happen if the chunk is deleted by having
            // no tiles in it earlier in an erase call.
            if (chunk == null) { return; }
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

            // Loops through all voxels in a submap that are within the chunk add adds their faces to the mesh.
            bool AddVoxelFaces(List<VoxelTile> tilemap, Vector2Int chunkPos)
            {
                bool returnValue = false;
                List<Vector3Int> inChunk = tilemap.Select(item => item.GridPosition).ToList()
                    .FindAll(item => GetChunkPos(item) == chunkPos);
                foreach (Vector3Int gridPos in inChunk)
                {
                    // If at least one position was evaluated, then were will return true.
                    // Only return false if this chunk is empty.
                    returnValue |= true;
                    foreach (Vector3Int direction in CardinalDirections.ORTHOGONAL_3D)
                    {
                        // Top and bottom faces should always render.  Bottom faces need to go to the bottom of the
                        // tilemap.
                        // Need to use back here instead of down because of how the Grid uses Z as the up/down axis.
                        if (direction == Vector3Int.back)
                        {
                            Vector3Int bottomedPos = new Vector3Int(gridPos.x, gridPos.y, 0);
                            AddFace(GridToRelativePos(bottomedPos), direction, TileType.Ground);
                            continue;
                        }
                        else if (direction == Vector3Int.forward)
                        {
                            AddFace(GridToRelativePos(gridPos), direction, TileType.Ground);
                            continue;
                        }

                        // If there is a voxel adjacent to this one that occludes this face, then skip it.
                        if (CheckFace(gridPos + direction))
                        {
                            continue;
                        }
                        // Turns the grid position into a position relative to the chunk we are in.
                        AddFace(GridToRelativePos(gridPos), direction, TileType.Ground);

                        // If this face is a horizontal face, then we should also add faces downward until we reach 
                        // a space where that face is occluded.

                        // Check if this face is is not up/down facing.
                        if (direction.z == 0)
                        {
                            Vector3Int cascadePos = gridPos + Vector3Int.back;
                            while (cascadePos.z >= 0)
                            {
                                // If there is a voxel that would now block this face, then we should stop cascading
                                // and adding wall faces.
                                if (CheckCell(cascadePos + direction))
                                {
                                    break;
                                }
                                // Turns the grid position into a position relative to the chunk we are in.
                                AddFace(GridToRelativePos(cascadePos), direction, TileType.Wall);

                                cascadePos = cascadePos + Vector3Int.back;
                            }
                        }
                    }
                }
                return returnValue;
            }

            List<int> trianglesList = new List<int>();
            // Adds a face that should be created when the mesh is baked.
            void AddFace(Vector3Int relativePos, Vector3Int direction, TileType type)
            {
                direction = GridToLocalDirection(direction);
                Vector3Int[] vertexOffsets = FaceLookup.GetVertexOffsets(direction);

                // Gets the local position of the bottom-left-back corner of this grid position.
                // Calling this Corner Space.  Verticies are calculated in corner space because the world is
                // entirely made of 1 x 1 x 1 voxels that should have their corners exist at integer positions.
                Vector3Int cornerPos = GridToLocalCorner(relativePos);

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

            AddVoxelFaces(chunk.Tiles, chunk.ChunkPos);
            bool notEmpty = AddVoxelFaces(chunk.Tiles, chunk.ChunkPos);
            // No longer need to create/destroy chunks when baking a mesh because that should now be handled by the
            // paint and erase functions.
            //// If, after looping through all the submaps, our chunk contains no values then we should destroy this
            //// chunk.
            //if (!notEmpty)
            //{
            //    DestroyChunk(chunk);
            //    return;
            //}

            // Mesh creation
            Vector3[] verticies = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            int[] triangles = trianglesList.ToArray();

            // Gets a reference to a pre-existing mesh file.
            Mesh mesh = chunk.ChunkMesh;

            foreach (var pair in vertexIndicies)
            {
                int index = pair.Value;
                verticies[index] = pair.Key.position;

                // Uses a sin wave with a period of 4 to alternate the UV values so that the faces tile correctly.

                // Adjust UVs to account for texture offset here.
                Vector2 uv = ProjectPositionToUV(pair.Key.position, pair.Key.normal, pair.Key.tileType);

                uvs[index] = uv;
            }

            //DebugHelpers.LogCollection(uvs);
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

        #region Chunk Creation

        /// <summary>
        /// Creates a new chunk at a given chunk position.
        /// </summary>
        /// <param name="chunkPos">The chunk position to create the chunk at.</param>
        /// <returns>The created chunk.</returns>
        private Chunk CreateNewChunk(Vector2Int chunkPos)
        {
            //Chunk chunk = PrefabUtility.InstantiatePrefab(chunkPrefab, transform) as Chunk;
            Chunk chunk = Instantiate(chunkPrefab, transform);
            chunk.transform.position = new Vector3(chunkPos.x, 0, chunkPos.y) * chunkSize;
            chunk.Initialize(this, chunkPos);
            chunks.Add(chunk);
            CreateMeshAsset(chunk);
            return chunk;
        }

        /// <summary>
        /// Destroys a given chunk.
        /// </summary>
        /// <param name="chunk">The chunk to destroy.</param>
        private void DestroyChunk(Chunk chunk)
        {
            chunks.Remove(chunk);
            // Deletes the mesh associated with this chunk.
            AssetDatabase.DeleteAsset(chunk.MeshPath);
            DestroyImmediate(chunk.gameObject);
        }
        #endregion
        #endif
        #endregion
    }
}