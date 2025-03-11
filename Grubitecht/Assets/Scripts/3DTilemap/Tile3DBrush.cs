/*****************************************************************************
// File Name : GroundBrush.cs
// Author : Brandon Koederitz
// Creation Date : March 7, 2025
//
// Brief Description : A grid brush that can paint ground objects on a 3D tilemap.
*****************************************************************************/
using UnityEngine;


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
        [SerializeField] private Tile3D tile;
        [SerializeField] private Vector3 offset;

        [field: SerializeField] public bool HalfStepPlacement { get; set; }

        private Tilemap3DLayer currentLayer;
        private GameObject currentTilemapObject;
        #endregion

        #region EditorOnly
#if UNITY_EDITOR
        public static void CreateTestBrush()
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

            // Updates the current layer that is selected by the brush.  When a new grid is selected, then we get the
            // 3D layer of that grid.  Done this way to save a GetComponent call each paint.
            if (currentTilemapObject != brushTarget)
            {
                currentLayer = brushTarget.GetComponent<Tilemap3DLayer>();
                Debug.Log("Set current layer to " + currentLayer);
                currentTilemapObject = brushTarget;
            }

            // If there is already an object in a cell, dont paint it.
            if (GetObjectInCell(gridLayout, brushTarget.transform, position) != null)
            {
                return;
            }

            PlaceTile(gridLayout, brushTarget.transform, position);
        }

        private void PlaceTile(GridLayout gridLayout, Transform targetTransform, Vector3Int position)
        {
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
            UpdateRuleModel(gridLayout, position, createdTile, true);
        }

        /// <summary>
        /// Updates a given tile's rule model component
        /// </summary>
        /// <param name="gridLayout"></param>
        /// <param name="position"></param>
        /// <param name="tileToUpdate"></param>
        private void UpdateRuleModel(GridLayout gridLayout, Vector3Int position, Tile3D tileToUpdate, 
            bool updateAdjacent)
        {
            if (tileToUpdate.RuleModel != null)
            {
                AdjacentCellInfo cellInfo = new();
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
                            Vector3Int adj = new(x, y, z);
                            cellInfo.AdjacentTiles[adj] = GetTileInCell(gridLayout, layer.transform,
                                position + adj);
                        }
                    }
                }

                tileToUpdate.RuleModel.UpdateRuleModel(cellInfo);
            }
        }

        /// <summary>
        /// Erases the Tile3D at the location of the brush on the selected tilemap layer.
        /// </summary>
        /// <param name="gridLayout">The grid layout (tilemap in this case) to erase from.</param>
        /// <param name="brushTarget">
        /// The target of the brush.  This is the GameObject of the tilemap we are painting on.
        /// </param>
        /// <param name="position">The position to erase at.</param>
        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            Transform toErase = GetObjectInCell(gridLayout, brushTarget.transform, position);
            if (toErase != null)
            {
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
            // Create a value that lets us center our spawne game objects to line up with the grid.
            Vector3 center = new Vector3(CELL_SIZE / 2, CELL_SIZE / 2, 0f);
            //Vector3Int cellPos = new Vector3Int(position.x, yPos, position.y);
            Vector3 worldPos = grid.LocalToWorld(grid.CellToLocalInterpolated(position + center));
            // Set Y position manually because Unity tilemaps doesnt support depth by default.
            worldPos.y = targetTransform.position.y;
            return worldPos;
        }
    }
}
