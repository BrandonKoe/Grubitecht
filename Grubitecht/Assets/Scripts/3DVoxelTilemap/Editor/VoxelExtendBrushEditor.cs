/*****************************************************************************
// File Name : VoxelExtendBrushEditor.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Custom editor for the extend brush
*****************************************************************************/
using System;
using UnityEditor;

namespace Grubitecht.Tilemaps.Editors
{
    [Obsolete("Extend brush is obsolete.  Use VoxelBrush instead.")]
    [CustomEditor(typeof(VoxelExtendBrush))]
    public class VoxelExtendBrushEditor : VoxelBrushEditor
    {
        // Extend functionality here.
    }
}
