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
    [CustomGridBrush(false, true, false, "Ground Brush")]
    public class GroundBrush : GridBrushBase
    {
        #region vars
        #region CONSTS
        private const float CELL_SIZE = 1f;
        #endregion
        [SerializeField] private Tile3D tile;
        [SerializeField] private Vector3 offset;

        [field: SerializeField] public bool HalfStepPlacement { get; set; }
        #endregion

        #region EditorOnly
#if UNITY_EDITOR
        public static void CreateTestBrush()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Ground Brush", "New Ground Brush", "asset", 
                "Saved Ground Brush");
            if (path == null) { return; }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TestGridBrush>(), path);
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
            Debug.Log(position);

            // If there is already an object in a cell, dont paint it.
            if (GetObjectInCell(gridLayout, brushTarget.transform, position) != null)
            {
                return;
            }

            Vector3 worldPos = GetWorldPositionCentered(gridLayout, position);
            worldPos = worldPos + offset;
            // If the user has pressed the space key, then we increment the position of the tile by half the cell
            // size to allow for the easy creation of stepping stones in our map.
            if (HalfStepPlacement) { worldPos.y += CELL_SIZE / 2; }
            Tile3D createdTile = Instantiate(tile, brushTarget.transform);
            createdTile.transform.position = worldPos;
            // Run logic with the RuleModel script here.
            if (createdTile.RuleModel != null)
            {
                
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
        /// <param name="parent">The transform of the object that contains the tilemap.</param>
        /// <param name="position">The position of the cell to evaluate.</param>
        /// <returns>The transform of the object within that cell.</returns>
        private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            Vector3 worldPos = GetWorldPositionCentered(grid, position);
            Bounds bounds = new Bounds(worldPos, Vector3.one * CELL_SIZE);

            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (bounds.Contains(child.position))
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the world position of a cell in a grid layout at a given position.
        /// </summary>
        /// <param name="grid">The grid that contains the cell to get the world position of.</param>
        /// <param name="position">The cell position to get the world position of.</param>
        /// <returns>The world position of the cell.</returns>
        private static Vector3 GetWorldPositionCentered(GridLayout grid, Vector3Int position)
        {
            // Create a value that lets us center our spawne game objects to line up with the grid.
            Vector3 center = new Vector3(CELL_SIZE / 2, CELL_SIZE / 2, 0f);
            //Vector3Int cellPos = new Vector3Int(position.x, yPos, position.y);
            Vector3 worldPos = grid.LocalToWorld(grid.CellToLocalInterpolated(position + center));
            return worldPos;
        }
    }
}
