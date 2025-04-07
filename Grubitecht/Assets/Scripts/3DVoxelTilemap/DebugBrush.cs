/*****************************************************************************
// File Name : DebugBrush.cs
// Author : Brandon Koederitz
// Creation Date : March 21, 2025
//
// Brief Description : Brush for debugging grid positions.
*****************************************************************************/
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Grubitecht.Tilemaps
{
    [CreateAssetMenu(menuName = "Custom Brushes/Debug Brush")]
    [CustomGridBrush(false, true, false, "Debug Brush")]
    public class DebugBrush : GridBrushBase
    {
        #region EditorOnly
#if UNITY_EDITOR
        public static void CreateGridBrush3D()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Debug Brush", "New Debug Brush", "asset",
                "Saved Debug Brush");
            if (path == null) { return; }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DebugBrush>(), path);
        }
#endif
        #endregion

        /// <summary>
        /// Prints the location of the painted cell to the console.
        /// </summary>
        /// <param name="gridLayout">The layout this tilemap belongs to.</param>
        /// <param name="brushTarget">The layer we are painting on.</param>
        /// <param name="position">The position we are painting at.</param>
        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            // Sets the correct position based on the layer we are painting on.
            position.z = Mathf.RoundToInt(brushTarget.transform.position.y);
            //Debug.Log(World.Objects.Objective.NavMap.GetDistanceValue(position));
        }
    }
}