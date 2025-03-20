/*****************************************************************************
// File Name : VoxelBrush.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Brush for painting positions on the 3D mesh tilemap.
*****************************************************************************/
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Grubitecht.Tilemaps
{
    [CreateAssetMenu]
    [CustomGridBrush(false, true, false, "Voxel Brush")]
    public class VoxelBrush : GridBrushBase
    {
        #region EditorOnly
        #if UNITY_EDITOR
        public static void CreateGridBrush3D()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Voxel Brush", "New Voxel Brush", "asset",
                "Saved Voxel Brush");
            if (path == null) { return; }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<VoxelBrush>(), path);
        }
#endif
        #endregion

        /// <summary>
        /// Paints a voxel to the target 3D voxel tilemap
        /// </summary>
        /// <param name="gridLayout">
        /// The grid layout that is being painted on.  This game object should also contain the VoxelTilemap3D 
        /// component.
        /// </param>
        /// <param name="brushTarget">The game object for the layer that is being painted on.</param>
        /// <param name="position">The position we are painting at.</param>
        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            // Gets a reference to the Voxel Tilemap we are painting on.
            VoxelTilemap3D tilemap = gridLayout.GetComponent<VoxelTilemap3D>();
            if (tilemap == null) { return; }
            // Sets the correct position based on the layer we are painting on.
            position.z = Mathf.RoundToInt(brushTarget.transform.position.y);
            if (!tilemap.CheckCell(position, TileType.Ground))
            {
                tilemap.Paint(position, TileType.Ground);
            }
            Debug.Log(position);
        }
        /// <summary>
        /// Erases a voxel from the target 3D voxel tilemap
        /// </summary>
        /// <param name="gridLayout">
        /// The grid layout that is being erased from.  This game object should also contain the VoxelTilemap3D 
        /// component.
        /// </param>
        /// <param name="brushTarget">The game object for the layer that is being erased from.</param>
        /// <param name="position">The position we are erasing at.</param>
        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            VoxelTilemap3D tilemap = gridLayout.GetComponent<VoxelTilemap3D>();
            if (tilemap == null) { return; }
            // Sets the correct position based on the layer we are painting on.
            position.z = Mathf.RoundToInt(brushTarget.transform.position.y);
            if (tilemap.CheckCell(position))
            {
                tilemap.Erase(position);
            }

            Debug.Log(position);
        }
    }
}