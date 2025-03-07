/*****************************************************************************
// File Name : TestGridBrush.cs
// Author : Brandon Koederitz
// Creation Date : March 6, 2025
//
// Brief Description : A test script to try out making a custom grid brush.
// Basing this test off of a unity forum post: https://discussions.unity.com/t/getting-started-creating-a-custom-tile-palette-brush/816589
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Tilemaps;
using UnityEditor;
#endif

[CreateAssetMenu]
[CustomGridBrush(false, true, false, "Test Grid Brush")]
public class TestGridBrush : GridBrushBase
{
    #if UNITY_EDITOR
    public static void CreateTestBrush()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Test Brush", "New Test Brush", "asset", "Saved Test Brush");
        if (path == null) { return; }

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TestGridBrush>(), path);
    }
    #endif

    public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        base.Paint(gridLayout, brushTarget, position);
        Debug.Log(position);
        Debug.Log(brushTarget);
    }
}
