/*****************************************************************************
// File Name : Tile3DBrush.cs
// Author : Brandon Koederitz
// Creation Date : March 7, 2025
//
// Brief Description : A grid brush that can paint ground objects on a 3D tilemap.
*****************************************************************************/
using UnityEngine;
using Unity.VisualScripting;
using System.Dynamic;
using System.Collections.Generic;
using UnityEngine.UIElements;






#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Grubitecht.Tilemaps
{
    [CreateAssetMenu]
    [CustomGridBrush(false, true, false, "Tile 3D Brush")]
    public class Tile3DBrush : GridBrushBase
    {
        #region vars
        #region CONSTS
        private const float CELL_SIZE = 1f;
        #endregion
        [Header("Tile 3D Brush Settings")]
        [SerializeField] private Tile3D tile;
        [SerializeField] private Vector3 offset;

        [field: SerializeField] public bool HalfStepPlacement { get; set; }

        //[SerializeReference] private Tilemap3DLayer currentLayer;
        //[SerializeReference] private GameObject currentTilemapObject;
        #endregion

        #region EditorOnly
        #if UNITY_EDITOR
        public static void CreateGridBrush3D()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Tile 3D Brush", "New Tile 3D Brush", "asset", 
                "Saved Tile 3D Brush");
            if (path == null) { return; }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<Tile3DBrush>(), path);
        }
        #endif
        #endregion

        /// <summary>
        /// Paints 3D tilemaps to the currently selected tilemap layer.
        /// </summary>
        /// <param name="gridLayout">The grid layout (tilemap in this case) to paint to.</param>
        /// <param name="brushTarget">
        /// The target of the brush.  This is the GameObject of the tilemap we are painting on.
        /// </param>
        /// <param name="position">The position to paint at.</param>
        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.Paint(gridLayout, brushTarget, position);

            // We cant store a layer reference as assets cannot reference objects in a scene.
            //if (currentTilemapObject != brushTarget)
            //{
            //    currentLayer = brushTarget.GetComponent<Tilemap3DLayer>();
            //    //Debug.Log("Set current layer to " + currentLayer);
            //    currentTilemapObject = brushTarget;
            //}

            PlaceTile(tile, gridLayout, brushTarget.transform, position, brushTarget.GetComponent<Tilemap3DLayer>());
        }

        /// <summary>
        /// Places a 3D tile on a given tilemap.
        /// </summary>
        /// <param name="tile">The tile to place.</param>
        /// <param name="gridLayout">The grid layout this tilemap belongs to.</param>
        /// <param name="targetTransform">The transform of the tilemap layer we are painting on.</param>
        /// <param name="position">The position of the tile.</param>
        /// <param name="currentLayer">The current layer we are painting on.</param>
        protected virtual void PlaceTile(Tile3D tile, GridLayout gridLayout, Transform targetTransform, Vector3Int position, 
            Tilemap3DLayer currentLayer)
        {
            // If there is already an object in a cell, dont paint it.
            if (GetObjectInCell(gridLayout, targetTransform, position) != null)
            {
                return;
            }

            Vector3 worldPos = GetWorldPositionCentered(gridLayout, position, targetTransform);
            worldPos = worldPos + offset;
            // If the user has pressed the space key, then we increment the position of the tile by half the cell
            // size to allow for the easy creation of stepping stones in our map.
            if (HalfStepPlacement) { worldPos.y += CELL_SIZE / 2; }
            // Use InstantiatePrefab to keep a prefab link on created tile prefabs.
            Tile3D createdTile = PrefabUtility.InstantiatePrefab(tile, targetTransform) as Tile3D;
            // Need to manually set Y because unity tilemaps dont normally support depth.
            createdTile.transform.position = worldPos;
            // Run logic with the RuleModel script here.

            // Update rule tiles
            // Get the tile in that cell if it is not provided.
            Dictionary<Vector3, Tile3D> adjInfo = UpdateAdjacentRules(gridLayout, position, currentLayer, createdTile);
            // Sends the info for the created tile to update the rule model.
            if (createdTile.RuleModel != null)
            {
                createdTile.RuleModel.SetRuleModel(adjInfo);
            }
        }

        /// <summary>
        /// Updates all adjacent rule tiles now that this tile has changed
        /// </summary>
        /// <param name="gridLayout">The grid layout that the tile belongs to.</param>
        /// <param name="position">The position of the tile.</param>
        /// <param name="currentLayer">The current layer</param>
        /// <param name="tileToUpdate">The tile to update.</param>
        /// <returns>Information about those adjacent tiles.</returns>
        private static Dictionary<Vector3, Tile3D> UpdateAdjacentRules(GridLayout gridLayout, Vector3Int position, 
            Tilemap3DLayer currentLayer, Tile3D tileToUpdate)
        {
            Dictionary<Vector3, Tile3D> cellInfo = new();
            for (int y = -1; y <= 1; y++)
            {
                Tilemap3DLayer layer;
                if (y == -1)
                {
                    layer = currentLayer.BelowLayer;
                }
                else if (y == 1)
                {
                    layer = currentLayer.AboveLayer;
                }
                else
                {
                    layer = currentLayer;
                }
                // Skips over layers that dont exist.
                if (layer == null) { continue; }

                for (int x = -1; x <= 1; x++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        // Adj represents the direction that we are evaluating.  Need to swap z and y because of how
                        // unity evaluates tilemap depth internally.
                        Vector3Int adj = new(x, y, z);
                        // Skip over this tile.  Rule models do not need to know about themselves.
                        if (adj == Vector3Int.zero) { continue; }
                        Tile3D adjacentTile = GetTileInCell(gridLayout, layer.transform,
                            ShiftGridPosition(position, adj));

                        if (adjacentTile != null && adjacentTile.RuleModel != null)
                        {
                            adjacentTile.RuleModel.UpdateFace(tileToUpdate, -adj);
                        }

                        cellInfo.Add(adj, adjacentTile);
                    }
                }
            }

            return cellInfo;
        }

        /// <summary>
        /// Shifts a grid position by a given offset, ignoring changes to elevation.
        /// </summary>
        /// <param name="gridPos">The position to shift.</param>
        /// <param name="offset">The offset to shift it  by.</param>
        private static Vector3Int ShiftGridPosition(Vector3Int gridPos, Vector3Int offset)
        {
            return new Vector3Int(gridPos.x + offset.x, gridPos.y + offset.z, gridPos.z);
        }

        /// <summary>
        /// Erases the Tile3D at the location of the brush on the selected tilemap layer.
        /// </summary>
        /// <param name="gridLayout">The grid layout to erase from.</param>
        /// <param name="brushTarget">
        /// The target of the brush.  This is the GameObject of the tilemap we are painting on.
        /// </param>
        /// <param name="position">The position to erase at.</param>
        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            EraseTile(gridLayout, brushTarget.transform, position, brushTarget.GetComponent<Tilemap3DLayer>());
        }

        /// <summary>
        /// Erases the Tile3D at the location of the brush on the selected tilemap layer.
        /// </summary>
        /// <param name="gridLayout">The grid layout to eraase from.</param>
        /// <param name="targetTransform">The transform of the tilemap layer to erase from.</param>
        /// <param name="position">the position to erase from.</param>
        /// <param name="layer">The tilemap layer to erase from.</param>
        protected virtual void EraseTile(GridLayout gridLayout, Transform targetTransform, Vector3Int position, 
            Tilemap3DLayer layer)
        {
            Transform toErase = GetObjectInCell(gridLayout, targetTransform, position);
            if (toErase != null)
            {
                // Need to update adjacent rule tiles when a tile is erased.
                UpdateAdjacentRules(gridLayout, position, layer, null);
                DestroyImmediate(toErase.gameObject);
            }
        }

        // Should probably add logic to ensure that GetObjectsInCell only returns Tile3D objects.
        /// <summary>
        /// Gets the object in a specific tilemap at a given grid position.
        /// </summary>
        /// <param name="grid">The grid layout that contains the cell to evaluate.</param>
        /// <param name="targetTransform">The transform of the object that contains the tilemap.</param>
        /// <param name="position">The position of the cell to evaluate.</param>
        /// <returns>The transform of the object within that cell.</returns>
        private static Transform GetObjectInCell(GridLayout grid, Transform targetTransform, Vector3Int position)
        {
            Vector3 worldPos = GetWorldPositionCentered(grid, position, targetTransform);
            Bounds bounds = new Bounds(worldPos, Vector3.one * CELL_SIZE);

            for (int i = 0; i < targetTransform.childCount; i++)
            {
                var child = targetTransform.GetChild(i);
                if (bounds.Contains(child.position))
                {
                    return child;
                }
            }
            return null;
        }

        // Should probably add logic to ensure that GetObjectsInCell only returns Tile3D objects.
        /// <summary>
        /// Gets the object in a specific tilemap at a given grid position.
        /// </summary>
        /// <param name="grid">The grid layout that contains the cell to evaluate.</param>
        /// <param name="targetTransform">The transform of the object that contains the tilemap.</param>
        /// <param name="position">The position of the cell to evaluate.</param>
        /// <returns>The transform of the object within that cell.</returns>
        private static Tile3D GetTileInCell(GridLayout grid, Transform targetTransform, Vector3Int position)
        {
            Vector3 worldPos = GetWorldPositionCentered(grid, position, targetTransform);
            Bounds bounds = new Bounds(worldPos, Vector3.one * CELL_SIZE);

            for (int i = 0; i < targetTransform.childCount; i++)
            {
                var child = targetTransform.GetChild(i);
                if (bounds.Contains(child.position) && child.TryGetComponent(out Tile3D tile))
                {
                    return tile;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the world position of a cell in a grid layout at a given position.
        /// </summary>
        /// <param name="grid">The grid that contains the cell to get the world position of.</param>
        /// <param name="position">The cell position to get the world position of.</param>
        /// <param name="targetTransform">The transform of the object that contains the tilemap.</param>
        /// <returns>The world position of the cell.</returns>
        private static Vector3 GetWorldPositionCentered(GridLayout grid, Vector3Int position, 
            Transform targetTransform)
        {
            // Create a value that lets us center our spawned game objects to line up with the grid.
            Vector3 center = new Vector3(CELL_SIZE / 2, CELL_SIZE / 2, 0f);
            //Vector3Int cellPos = new Vector3Int(position.x, yPos, position.y);
            Vector3 worldPos = grid.LocalToWorld(grid.CellToLocalInterpolated(position + center));
            // Set Y position manually because Unity tilemaps doesnt support depth by default.
            worldPos.y = targetTransform.position.y;
            return worldPos;
        }
    }
}
