/*****************************************************************************
// File Name : GroundBrush.cs
// Author : Brandon Koederitz
// Creation Date : March 6, 2025
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
        [SerializeField] private Tile3D tile;
        [SerializeField] private Vector3 offset;

        private int yPos = 0;
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

            // Create a value that lets us center our spawne game objects to line up with the grid.
            Vector3 center = new Vector3(0.5f, 0.5f, 0f);
            //Vector3Int cellPos = new Vector3Int(position.x, yPos, position.y);
            Vector3 worldPos = gridLayout.LocalToWorld(gridLayout.CellToLocalInterpolated(position + center));
            worldPos = worldPos + offset;
            Tile3D model = Instantiate(tile, brushTarget.transform);
            model.transform.position = worldPos;
            // Run logic with the RuleModel script here.
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            
        }

        private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            int childCount = parent.childCount;
            throw new System.NotImplementedException();
        }
    }
}
