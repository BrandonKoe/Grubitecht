/*****************************************************************************
// File Name : GroundBrushEditor.cs
// Author : Brandon Koederitz
// Creation Date : March 12, 2025
//
// Brief Description : custom editor for the extend brush
*****************************************************************************/
using UnityEditor;

namespace Grubitecht.OldTilemaps.Editors
{
    [CustomEditor(typeof(Extend3DBrush))]
    public class Extend3DBrushEditor : Tile3DBrushEditor
    {
        // Add extend brush custom functionality here.
    }
}