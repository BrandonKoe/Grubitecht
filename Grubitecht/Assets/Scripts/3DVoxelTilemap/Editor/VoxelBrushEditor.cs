/*****************************************************************************
// File Name : Tile3DBrushEditor.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Custom editor for a 3D tile brush.
*****************************************************************************/
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Grubitecht.Tilemaps.Editors
{
    [CustomEditor(typeof(VoxelBrush))]
    public class VoxelBrushEditor : GridBrushEditorBase
    {
        // Need to have this or else the tilemaps wont show up as valid targets under the tile palette editor.
        public override GameObject[] validTargets
        {
            get
            {
                // Stage handle is basically an editing context.  This method makes it easier to find components.
                StageHandle currentStageHandle = StageUtility.GetCurrentStageHandle();
                return currentStageHandle.FindComponentsOfType<Tilemap>().Where(x =>
                {
                    GameObject gameObject;
                    return (gameObject = x.gameObject).scene.isLoaded
                           && gameObject.activeInHierarchy
                           && !gameObject.hideFlags.HasFlag(HideFlags.NotEditable)
                           // Only get tilemaps that are childed to a tilemap 3D.
                           && gameObject.transform.parent.TryGetComponent(out VoxelTilemap3D tilemap3D); 
                }).Select(x => x.gameObject).ToArray();
            }
        }

        // Allows us to access and modify our GroundBrush object from this editor script.
        public VoxelBrush targetBrush { get { return target as VoxelBrush; } }
    }
}